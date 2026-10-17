using System.Runtime.InteropServices;
using V9ProControl.Audio;

namespace V9ProControl.Apo;

/// <summary>
/// Applies APO settings through the audio service's own policy interface, which is what the
/// vendor library does underneath. Removes the need for osConfLib.dll on machines that never
/// had MCHOSE HUB installed.
///
/// Established by patching the live vtable of the object osConfLib obtains and recording the
/// calls it makes: every change goes through one slot, carrying the endpoint id, an FX-store
/// flag, a PROPERTYKEY and a PROPVARIANT. A PROPERTYKEY is the same {GUID},index pair that
/// FxProperties uses, so the property set GUIDs already mapped here address it directly and
/// the vendor's property names are not needed.
///
/// The variant type matters: the vendor passes VT_UI4, and the audio service unwraps that into
/// a REG_DWORD. Passing VT_I4 instead is accepted and stored as a raw REG_BINARY blob, which
/// the APO then ignores. Gains stay two's-complement, exactly as the registry holds them.
///
/// IPolicyConfig is undocumented and its vtable layout is not guaranteed across Windows
/// builds, so a caller must verify that a write actually took effect before trusting it.
/// </summary>
internal sealed class PolicyConfigApo : IDisposable
{
	private const int SetPropertyValueSlot = 11;
	private const ushort VtUi4 = 19;
	private const int PropVariantSize = 24;
	private const int PropVariantValueOffset = 8;

	private static readonly Guid PolicyConfigClientClsid = new("294935ce-f637-4e7c-a41b-ab255460b862");

	private readonly object _policy;
	private readonly string _endpointId;
	private bool _disposed;

	private PolicyConfigApo(object policy, string endpointId)
	{
		_policy = policy;
		_endpointId = endpointId;
	}

	/// <summary>The full endpoint id, for diagnostics.</summary>
	public string EndpointId => _endpointId;

	/// <summary>
	/// Resolves the endpoint and creates the policy client, or returns null when either is
	/// unavailable. <paramref name="endpointRegistryId"/> is the MMDevices subkey name.
	/// </summary>
	public static PolicyConfigApo? TryOpen(string endpointRegistryId)
	{
		try
		{
			string? endpointId = ResolveEndpointId(endpointRegistryId);
			if (endpointId is null) { return null; }

			Type? type = Type.GetTypeFromCLSID(PolicyConfigClientClsid);
			if (type is null) { return null; }

			object? instance = Activator.CreateInstance(type);
			if (instance is null) { return null; }

			// Casting performs the QueryInterface; a build without this interface throws here.
			var policy = (IPolicyConfig)instance;
			return new PolicyConfigApo(policy, endpointId);
		}
		catch (Exception)
		{
			return null;
		}
	}

	/// <summary>
	/// Matches the registry subkey name against the device id of an active endpoint. The id has
	/// a flow-and-role prefix that cannot safely be assumed, so it is read rather than built.
	/// </summary>
	private static string? ResolveEndpointId(string endpointRegistryId)
	{
		string trimmed = endpointRegistryId.Trim('{', '}');
		var enumerator = (IMMDeviceEnumerator)new MMDeviceEnumeratorComObject();
		enumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active, out IMMDeviceCollection devices);
		devices.GetCount(out uint count);

		for (uint i = 0; i < count; i++)
		{
			devices.Item(i, out IMMDevice device);
			device.GetId(out string id);
			if (id.Contains(trimmed, StringComparison.OrdinalIgnoreCase)) { return id; }
		}

		return null;
	}

	/// <summary>Writes one property. True only means the call reported success.</summary>
	public bool TrySet(Guid propertySet, int index, int value)
	{
		if (_disposed) { return false; }

		IntPtr variant = Marshal.AllocHGlobal(PropVariantSize);
		try
		{
			for (int i = 0; i < PropVariantSize; i++) { Marshal.WriteByte(variant, i, 0); }
			Marshal.WriteInt16(variant, 0, unchecked((short)VtUi4));
			Marshal.WriteInt32(variant, PropVariantValueOffset, value);

			var key = new PropertyKey(propertySet, index);
			return ((IPolicyConfig)_policy).SetPropertyValue(_endpointId, true, ref key, variant) == 0;
		}
		catch (Exception)
		{
			return false;
		}
		finally
		{
			Marshal.FreeHGlobal(variant);
		}
	}

	public void Dispose()
	{
		if (_disposed) { return; }
		_disposed = true;
		if (Marshal.IsComObject(_policy)) { Marshal.ReleaseComObject(_policy); }
	}

	/// <summary>
	/// Only the setter is declared with real parameters. A COM vtable slot is three plus the
	/// declaration index, so the placeholders put SetPropertyValue on the observed slot; their
	/// signatures are irrelevant because they are never called.
	/// </summary>
	[ComImport]
	[Guid("568b9108-44bf-40b4-9006-86afe5b5a620")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	private interface IPolicyConfig
	{
		int Reserved03();
		int Reserved04();
		int Reserved05();
		int Reserved06();
		int Reserved07();
		int Reserved08();
		int Reserved09();
		int Reserved10();

		[PreserveSig]
		int SetPropertyValue([MarshalAs(UnmanagedType.LPWStr)] string endpointId, [MarshalAs(UnmanagedType.Bool)] bool fxStore, ref PropertyKey key, IntPtr value);
	}
}
