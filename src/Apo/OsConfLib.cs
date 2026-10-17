using System.Runtime.InteropServices;

namespace V9ProControl.Apo;

/// <summary>Why opening the library failed, so the UI can say something true about it.</summary>
internal enum OsConfLibStatus
{
	Ready,

	/// <summary>osConfLib.dll is not beside the executable nor in an installed MCHOSE HUB.</summary>
	LibraryMissing,

	/// <summary>The library loaded but would not initialise.</summary>
	InitFailed,

	/// <summary>The library initialised but enumerated no devices, which is what happens
	/// while the headset has no active audio endpoint.</summary>
	NoDevice
}

/// <summary>Property type expected in PropertyControl's trailing byte. Zero is invalid and makes the library fail fast.</summary>
internal enum ApoPropertyType : byte
{
	Bool = 1,
	Int = 2,
	Float = 3
}

/// <summary>
/// The vendor's APO control library (osConfLib.dll), which is how MCHOSE HUB applies effect
/// changes. Writing FxProperties in the registry persists a setting but the APO does not
/// re-read it, so changes only take effect after the audio stream restarts. PropertyControl
/// both applies the change live and updates the registry itself.
///
/// Confirmed by setting Enable_EQ_GFX and AUDIOBRILLIANT_LEVEL and watching the registry
/// follow. Properties are addressed by wide-string name:
///
///   Enable_EQ_GFX, EQ_Slider1 .. EQ_Slider10,
///   Enable_AUDIOBRILLIANT_LFX, AUDIOBRILLIANT_LEVEL,
///   Enable_VOICECLARITY_LFX, VOICECLARITY_LEVEL, VOICECLARITY_NOISESUPP_LEVEL,
///   Enable_VIRTUALBASS_LFX, VIRTUALBASS_Level, VIRTUALBASS_CutOffFrequency,
///   Enable_ADAPTIVEVOLUME_GFX, ADAPTIVEVOLUME_LEVEL, ADAPTIVEVOLUME_MODE
///
/// Only names from that list are ever passed: unknown names corrupt the library's heap.
/// Values are int32; a float payload is accepted but stored as its bit pattern.
/// </summary>
internal sealed class OsConfLib : IDisposable
{
	/// <summary>Device interface the C-Media driver registers; the other candidate enumerates nothing.</summary>
	private static readonly Guid DriverInterface = new("19a9eee1-2ae3-44c2-98a1-019324bc0608");

	private const string LibraryName = "osConfLib.dll";
	private const int RenderDeviceType = 0;
	private const int DeviceInfoSize = 64;   // the struct is 16 bytes; over-allocated deliberately
	private const int BufferSize = 64;

	private readonly IntPtr _deviceInfo;
	private bool _disposed;

	private OsConfLib(IntPtr deviceInfo) => _deviceInfo = deviceInfo;

	/// <summary>
	/// Loads the library and selects the render device. Reports why it failed rather than just
	/// returning null: a missing DLL and a headset with no audio endpoint are different
	/// problems, and conflating them sends you looking for the wrong thing.
	/// Looks beside the executable first so the DLL can be kept locally and MCHOSE HUB
	/// uninstalled.
	/// </summary>
	public static OsConfLib? TryOpen(out OsConfLibStatus status)
	{
		string? directory = ResolveLibraryDirectory();
		if (directory is null)
		{
			status = OsConfLibStatus.LibraryMissing;
			return null;
		}

		try
		{
			if (!SetDllDirectory(directory) || ConfLibInitWithGUID(DriverInterface) != 0)
			{
				status = OsConfLibStatus.InitFailed;
				return null;
			}

			// From here the library holds state, so every failure path has to unwind it. The
			// caller retries on a timer while the headset is absent, and leaving initialised
			// instances behind would accumulate.
			if (CreateDeviceList() != 0)
			{
				ConfLibUnInit();
				status = OsConfLibStatus.InitFailed;
				return null;
			}

			IntPtr info = Marshal.AllocHGlobal(DeviceInfoSize);
			for (int i = 0; i < DeviceInfoSize; i++) { Marshal.WriteByte(info, i, 0); }
			if (GetDeviceById(RenderDeviceType, 0, info) != 0)
			{
				Marshal.FreeHGlobal(info);
				ConfLibUnInit();
				status = OsConfLibStatus.NoDevice;
				return null;
			}

			status = OsConfLibStatus.Ready;
			return new OsConfLib(info);
		}
		catch (DllNotFoundException)
		{
			status = OsConfLibStatus.LibraryMissing;
			return null;
		}
		catch (EntryPointNotFoundException)
		{
			status = OsConfLibStatus.InitFailed;
			return null;
		}
	}

	private static string? ResolveLibraryDirectory()
	{
		string local = AppContext.BaseDirectory;
		if (File.Exists(Path.Combine(local, LibraryName))) { return local; }

		foreach (string candidate in new[]
		{
			@"C:\Program Files\MCHOSE HUB\libs",
			@"C:\Program Files (x86)\MCHOSE HUB\libs"
		})
		{
			if (File.Exists(Path.Combine(candidate, LibraryName))) { return candidate; }
		}

		return null;
	}

	/// <summary>
	/// Applies a named property. The input is a vector whose first slot points at the value;
	/// both vectors must be non-null or the library dereferences null.
	///
	/// The trailing type byte must be non-zero. Passing 0 does not return an error - it trips
	/// a fast-fail inside the library (0xC0000409) and takes the process down, which is what
	/// crashed the app before this was understood. Buffers are deliberately not freed: the
	/// library may retain them, and freeing risks a double free.
	/// </summary>
	public bool TrySet(string property, int value, ApoPropertyType type)
	{
		if (_disposed) { return false; }

		try
		{
			IntPtr valueBuffer = Alloc(BufferSize);
			IntPtr outBuffer = Alloc(BufferSize);
			IntPtr inVector = Alloc(BufferSize);
			IntPtr outVector = Alloc(BufferSize);

			Marshal.WriteInt32(valueBuffer, 0, value);
			Marshal.WriteIntPtr(inVector, 0, valueBuffer);
			Marshal.WriteIntPtr(outVector, 0, outBuffer);

			return PropertyControl(_deviceInfo, property, inVector, outVector, (byte)type) == 0;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private static IntPtr Alloc(int size)
	{
		IntPtr p = Marshal.AllocHGlobal(size);
		for (int i = 0; i < size; i++) { Marshal.WriteByte(p, i, 0); }
		return p;
	}

	public void Dispose()
	{
		if (_disposed) { return; }
		_disposed = true;
		try { ConfLibUnInit(); } catch (Exception) { /* unloading must not throw on exit */ }
		if (_deviceInfo != IntPtr.Zero) { Marshal.FreeHGlobal(_deviceInfo); }
	}

	[DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern bool SetDllDirectory(string path);

	[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int ConfLibInitWithGUID(Guid driverInterface);

	[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int ConfLibUnInit();

	[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int CreateDeviceList();

	[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int GetDeviceById(int type, int id, IntPtr deviceInfo);

	[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
	private static extern int PropertyControl(IntPtr deviceInfo, string property, IntPtr pIn, IntPtr pOut, byte sub);
}
