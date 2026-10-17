using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace V9ProControl.Device;

internal static class NativeHid
{
	public const int DigcfPresent = 0x02;
	public const int DigcfDeviceInterface = 0x10;
	public const uint GenericRead = 0x80000000;
	public const uint GenericWrite = 0x40000000;
	public const uint FileShareRead = 1;
	public const uint FileShareWrite = 2;
	public const uint OpenExisting = 3;
	public const uint FileFlagOverlapped = 0x40000000;

	public const int ErrorIoPending = 997;

	public static readonly Guid HidInterfaceClass = new("4D1E55B2-F16F-11CF-88CB-001111000030");

	[StructLayout(LayoutKind.Sequential)]
	public struct DeviceInterfaceData
	{
		public int Size;
		public Guid InterfaceClassGuid;
		public int Flags;
		public IntPtr Reserved;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct HiddAttributes
	{
		public int Size;
		public ushort VendorId;
		public ushort ProductId;
		public ushort VersionNumber;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct HidpCaps
	{
		public ushort Usage;
		public ushort UsagePage;
		public ushort InputReportByteLength;
		public ushort OutputReportByteLength;
		public ushort FeatureReportByteLength;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)] public ushort[] Reserved;
		public ushort NumberLinkCollectionNodes;
		public ushort NumberInputButtonCaps;
		public ushort NumberInputValueCaps;
		public ushort NumberInputDataIndices;
		public ushort NumberOutputButtonCaps;
		public ushort NumberOutputValueCaps;
		public ushort NumberOutputDataIndices;
		public ushort NumberFeatureButtonCaps;
		public ushort NumberFeatureValueCaps;
		public ushort NumberFeatureDataIndices;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Overlapped
	{
		public IntPtr Internal;
		public IntPtr InternalHigh;
		public uint Offset;
		public uint OffsetHigh;
		public IntPtr EventHandle;
	}

	[DllImport("setupapi.dll", CharSet = CharSet.Unicode)]
	public static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, IntPtr enumerator, IntPtr hwndParent, int flags);

	[DllImport("setupapi.dll")]
	public static extern bool SetupDiEnumDeviceInterfaces(IntPtr set, IntPtr devInfo, ref Guid classGuid, int index, ref DeviceInterfaceData data);

	[DllImport("setupapi.dll", CharSet = CharSet.Unicode)]
	public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr set, ref DeviceInterfaceData data, IntPtr detail, int detailSize, ref int required, IntPtr devInfo);

	[DllImport("setupapi.dll")]
	public static extern bool SetupDiDestroyDeviceInfoList(IntPtr set);

	[DllImport("kernel32.dll", EntryPoint = "CreateFileW", SetLastError = true, CharSet = CharSet.Unicode)]
	public static extern SafeFileHandle CreateFile(string path, uint access, uint share, IntPtr security, uint disposition, uint flags, IntPtr template);

	[DllImport("kernel32.dll", EntryPoint = "CreateEventW", SetLastError = true)]
	public static extern IntPtr CreateEvent(IntPtr attributes, [MarshalAs(UnmanagedType.Bool)] bool manualReset, [MarshalAs(UnmanagedType.Bool)] bool initialState, IntPtr name);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ResetEvent(IntPtr handle);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool CloseHandle(IntPtr handle);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool WriteFile(SafeFileHandle handle, IntPtr buffer, uint count, IntPtr written, IntPtr overlapped);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ReadFile(SafeFileHandle handle, IntPtr buffer, uint count, IntPtr read, IntPtr overlapped);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetOverlappedResultEx(SafeFileHandle handle, IntPtr overlapped, out uint transferred, uint milliseconds, [MarshalAs(UnmanagedType.Bool)] bool alertable);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool CancelIoEx(SafeFileHandle handle, IntPtr overlapped);

	[DllImport("hid.dll")]
	public static extern bool HidD_GetAttributes(SafeFileHandle handle, ref HiddAttributes attributes);

	[DllImport("hid.dll")]
	public static extern bool HidD_GetPreparsedData(SafeFileHandle handle, out IntPtr preparsed);

	[DllImport("hid.dll")]
	public static extern bool HidD_FreePreparsedData(IntPtr preparsed);

	[DllImport("hid.dll")]
	public static extern int HidP_GetCaps(IntPtr preparsed, ref HidpCaps caps);
}
