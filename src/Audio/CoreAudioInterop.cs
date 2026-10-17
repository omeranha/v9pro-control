using System.Runtime.InteropServices;

namespace V9ProControl.Audio;

internal enum DataFlow
{
	Render = 0,
	Capture = 1
}

internal enum Role
{
	Console = 0,
	Multimedia = 1,
	Communications = 2
}

internal static class DeviceState
{
	public const uint Active = 0x1;
}

[ComImport]
[Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
internal class MMDeviceEnumeratorComObject
{
}

[ComImport]
[Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IMMDeviceEnumerator
{
	void EnumAudioEndpoints(DataFlow dataFlow, uint stateMask, out IMMDeviceCollection devices);
	void GetDefaultAudioEndpoint(DataFlow dataFlow, Role role, out IMMDevice device);
	void GetDevice([MarshalAs(UnmanagedType.LPWStr)] string id, out IMMDevice device);
	void RegisterEndpointNotificationCallback(IntPtr client);
	void UnregisterEndpointNotificationCallback(IntPtr client);
}

[ComImport]
[Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IMMDeviceCollection
{
	void GetCount(out uint count);
	void Item(uint index, out IMMDevice device);
}

[ComImport]
[Guid("D666063F-1587-4E43-81F1-B948E807363F")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IMMDevice
{
	void Activate(ref Guid iid, uint clsCtx, IntPtr activationParams, [MarshalAs(UnmanagedType.IUnknown)] out object instance);
	void OpenPropertyStore(uint stgmAccess, out IPropertyStore properties);
	void GetId([MarshalAs(UnmanagedType.LPWStr)] out string id);
	void GetState(out uint state);
}

[ComImport]
[Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IPropertyStore
{
	void GetCount(out uint count);
	void GetAt(uint index, out PropertyKey key);
	void GetValue(ref PropertyKey key, out PropVariant value);
	void SetValue(ref PropertyKey key, ref PropVariant value);
	void Commit();
}

[ComImport]
[Guid("5CDF2C82-841E-4546-9722-0CF74078229A")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IAudioEndpointVolume
{
	void RegisterControlChangeNotify(IntPtr notify);
	void UnregisterControlChangeNotify(IntPtr notify);
	void GetChannelCount(out uint channelCount);
	void SetMasterVolumeLevel(float levelDb, IntPtr eventContext);
	void SetMasterVolumeLevelScalar(float level, IntPtr eventContext);
	void GetMasterVolumeLevel(out float levelDb);
	void GetMasterVolumeLevelScalar(out float level);
	void SetChannelVolumeLevel(uint channel, float levelDb, IntPtr eventContext);
	void SetChannelVolumeLevelScalar(uint channel, float level, IntPtr eventContext);
	void GetChannelVolumeLevel(uint channel, out float levelDb);
	void GetChannelVolumeLevelScalar(uint channel, out float level);
	void SetMute([MarshalAs(UnmanagedType.Bool)] bool mute, IntPtr eventContext);
	void GetMute([MarshalAs(UnmanagedType.Bool)] out bool mute);
	void GetVolumeStepInfo(out uint step, out uint stepCount);
	void VolumeStepUp(IntPtr eventContext);
	void VolumeStepDown(IntPtr eventContext);
	void QueryHardwareSupport(out uint hardwareSupportMask);
	void GetVolumeRange(out float minDb, out float maxDb, out float incrementDb);
}

[StructLayout(LayoutKind.Sequential)]
internal struct PropertyKey
{
	public Guid FormatId;
	public int PropertyId;

	public PropertyKey(Guid formatId, int propertyId)
	{
		FormatId = formatId;
		PropertyId = propertyId;
	}

	/// <summary>Friendly name including the adapter, e.g. "Speakers (MCHOSE V9 PRO)".</summary>
	public static PropertyKey DeviceFriendlyName => new(new Guid("a45c254e-df1c-4efd-8020-67d146a850e0"), 14);

	/// <summary>Hardware description only, e.g. "MCHOSE V9 PRO".</summary>
	public static PropertyKey DeviceDescription => new(new Guid("b3f8fa53-0004-438e-9003-51a46e139bfc"), 6);
}

[StructLayout(LayoutKind.Sequential)]
internal struct PropVariant
{
	private const ushort VtLpwstr = 31;

	public ushort VarType;
	public ushort Reserved1;
	public ushort Reserved2;
	public ushort Reserved3;
	public IntPtr Pointer;
	public IntPtr Pointer2;

	public string? AsString() => VarType == VtLpwstr ? Marshal.PtrToStringUni(Pointer) : null;
}

internal static class Ole32
{
	[DllImport("ole32.dll")]
	public static extern int PropVariantClear(ref PropVariant pvar);
}
