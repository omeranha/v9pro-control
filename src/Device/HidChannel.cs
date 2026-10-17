using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace V9ProControl.Device;

/// <summary>
/// One request/response exchange on a vendor HID collection.
///
/// Uses raw overlapped WriteFile/ReadFile rather than FileStream: FileStream buffering over a
/// HID handle does not reliably deliver input reports. The handle is opened per exchange,
/// matching how the device behaves most predictably.
/// </summary>
internal sealed class HidChannel
{
	public const int ReportLength = 64;

	private readonly string _path;

	public ushort UsagePage { get; }

	private HidChannel(string path, ushort usagePage)
	{
		_path = path;
		UsagePage = usagePage;
	}

	/// <summary>Finds the collection with the given usage page and 64-byte in/out reports.</summary>
	public static HidChannel? Find(ushort vendorId, ushort productId, ushort usagePage)
	{
		Guid guid = NativeHid.HidInterfaceClass;
		IntPtr set = NativeHid.SetupDiGetClassDevs(ref guid, IntPtr.Zero, IntPtr.Zero, NativeHid.DigcfPresent | NativeHid.DigcfDeviceInterface);
		if (set == IntPtr.Zero || set == new IntPtr(-1)) { return null; }

		try
		{
			for (int index = 0; ; index++)
			{
				var data = new NativeHid.DeviceInterfaceData();
				data.Size = Marshal.SizeOf(data);
				if (!NativeHid.SetupDiEnumDeviceInterfaces(set, IntPtr.Zero, ref guid, index, ref data)) { break; }

				int required = 0;
				NativeHid.SetupDiGetDeviceInterfaceDetail(set, ref data, IntPtr.Zero, 0, ref required, IntPtr.Zero);
				IntPtr detail = Marshal.AllocHGlobal(required);
				try
				{
					Marshal.WriteInt32(detail, 8);
					if (!NativeHid.SetupDiGetDeviceInterfaceDetail(set, ref data, detail, required, ref required, IntPtr.Zero)) { continue; }
					string? path = Marshal.PtrToStringUni(IntPtr.Add(detail, 4));
					if (path is null) { continue; }
					if (Matches(path, vendorId, productId, usagePage)) { return new HidChannel(path, usagePage); }
				}
				finally
				{
					Marshal.FreeHGlobal(detail);
				}
			}
		}
		finally
		{
			NativeHid.SetupDiDestroyDeviceInfoList(set);
		}

		return null;
	}

	private static bool Matches(string path, ushort vendorId, ushort productId, ushort usagePage)
	{
		using SafeFileHandle probe = NativeHid.CreateFile(path, 0, NativeHid.FileShareRead | NativeHid.FileShareWrite, IntPtr.Zero, NativeHid.OpenExisting, 0, IntPtr.Zero);
		if (probe.IsInvalid) { return false; }

		var attributes = new NativeHid.HiddAttributes();
		attributes.Size = Marshal.SizeOf(attributes);
		if (!NativeHid.HidD_GetAttributes(probe, ref attributes) || attributes.VendorId != vendorId || attributes.ProductId != productId) { return false; }

		if (!NativeHid.HidD_GetPreparsedData(probe, out IntPtr preparsed)) { return false; }
		try
		{
			var caps = new NativeHid.HidpCaps { Reserved = new ushort[17] };
			NativeHid.HidP_GetCaps(preparsed, ref caps);
			return caps.UsagePage == usagePage && caps.InputReportByteLength == ReportLength && caps.OutputReportByteLength == ReportLength;
		}
		finally
		{
			NativeHid.HidD_FreePreparsedData(preparsed);
		}
	}

	/// <summary>
	/// Sends a 64-byte report and optionally waits for the reply. Returns null on timeout,
	/// which legitimately happens for fire-and-forget commands and when the headset is asleep.
	/// </summary>
	public byte[]? Exchange(byte[] request, int timeoutMs, bool expectReply = true)
	{
		using SafeFileHandle handle = NativeHid.CreateFile(_path, NativeHid.GenericRead | NativeHid.GenericWrite,
			NativeHid.FileShareRead | NativeHid.FileShareWrite, IntPtr.Zero, NativeHid.OpenExisting, NativeHid.FileFlagOverlapped, IntPtr.Zero);
		if (handle.IsInvalid) { return null; }

		IntPtr buffer = Marshal.AllocHGlobal(ReportLength);
		IntPtr overlapped = Marshal.AllocHGlobal(Marshal.SizeOf<NativeHid.Overlapped>());
		IntPtr completion = NativeHid.CreateEvent(IntPtr.Zero, true, false, IntPtr.Zero);
		try
		{
			var frame = new byte[ReportLength];
			Array.Copy(request, frame, Math.Min(request.Length, ReportLength));
			Marshal.Copy(frame, 0, buffer, ReportLength);

			if (Transfer(handle, buffer, overlapped, completion, timeoutMs, writing: true) is null) { return null; }
			if (!expectReply) { return []; }
			if (Transfer(handle, buffer, overlapped, completion, timeoutMs, writing: false) is null) { return null; }

			var reply = new byte[ReportLength];
			Marshal.Copy(buffer, reply, 0, ReportLength);
			return reply;
		}
		finally
		{
			if (completion != IntPtr.Zero) { NativeHid.CloseHandle(completion); }
			Marshal.FreeHGlobal(overlapped);
			Marshal.FreeHGlobal(buffer);
		}
	}

	private static uint? Transfer(SafeFileHandle handle, IntPtr buffer, IntPtr overlapped, IntPtr completion, int timeoutMs, bool writing)
	{
		NativeHid.ResetEvent(completion);
		Marshal.StructureToPtr(new NativeHid.Overlapped { EventHandle = completion }, overlapped, false);

		bool started = writing
			? NativeHid.WriteFile(handle, buffer, ReportLength, IntPtr.Zero, overlapped)
			: NativeHid.ReadFile(handle, buffer, ReportLength, IntPtr.Zero, overlapped);
		if (!started && Marshal.GetLastWin32Error() != NativeHid.ErrorIoPending) { return null; }

		if (NativeHid.GetOverlappedResultEx(handle, overlapped, out uint transferred, (uint)timeoutMs, false)) { return transferred; }

		// Drain the pending operation before the buffers are freed.
		NativeHid.CancelIoEx(handle, overlapped);
		NativeHid.GetOverlappedResultEx(handle, overlapped, out _, 200, false);
		return null;
	}
}
