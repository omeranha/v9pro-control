using Microsoft.Win32;
using System.Security.AccessControl;

namespace V9ProControl.Apo;

/// <summary>
/// Access to an audio endpoint's APO effect settings, which live in the registry under
/// MMDevices\...\FxProperties. The C-Media APO reads these, which is why settings survive
/// with no vendor software running.
/// </summary>
internal sealed class EndpointFxProperties
{
	private const string MmDevicesRoot = @"SOFTWARE\Microsoft\Windows\CurrentVersion\MMDevices\Audio";
	private const string DeviceDescriptionValue = "{b3f8fa53-0004-438e-9003-51a46e139bfc},6";
	private const string DeviceStateValue = "DeviceState";
	private const int DeviceStateActive = 0x1;

	private readonly string _fxPropertiesPath;

	public string EndpointId { get; }
	public string Description { get; }

	private EndpointFxProperties(string fxPropertiesPath, string endpointId, string description)
	{
		_fxPropertiesPath = fxPropertiesPath;
		EndpointId = endpointId;
		Description = description;
	}

	/// <summary>
	/// Locates the endpoint's effect settings, preferring the active one.
	///
	/// Reinstalling the driver leaves the previous endpoints registered but not present, each
	/// keeping its own FxProperties. Returning the first name match therefore read a dead
	/// endpoint holding stale defaults while osConfLib applied changes to the live device, so
	/// settings appeared to reset every time the app started. Inactive endpoints are only used
	/// as a fallback, for when the headset is registered but currently disconnected.
	/// </summary>
	public static EndpointFxProperties? Find(DeviceFlow flow, string descriptionFilter)
	{
		string flowKey = flow == DeviceFlow.Render ? "Render" : "Capture";
		using RegistryKey? root = Registry.LocalMachine.OpenSubKey($@"{MmDevicesRoot}\{flowKey}");
		if (root is null) { return null; }

		EndpointFxProperties? inactive = null;

		foreach (string endpointId in root.GetSubKeyNames())
		{
			using RegistryKey? endpoint = root.OpenSubKey(endpointId);
			if (endpoint is null) { continue; }

			using RegistryKey? properties = endpoint.OpenSubKey("Properties");
			string? description = properties?.GetValue(DeviceDescriptionValue) as string;
			if (description is null || !description.Contains(descriptionFilter, StringComparison.OrdinalIgnoreCase)) { continue; }

			string fxPath = $@"{MmDevicesRoot}\{flowKey}\{endpointId}\FxProperties";
			using RegistryKey? fx = Registry.LocalMachine.OpenSubKey(fxPath);
			if (fx is null) { continue; }

			var candidate = new EndpointFxProperties(fxPath, endpointId, description);
			if (IsActive(endpoint)) { return candidate; }
			inactive ??= candidate;
		}

		return inactive;
	}

	private static bool IsActive(RegistryKey endpoint) => endpoint.GetValue(DeviceStateValue) is int state && (state & DeviceStateActive) == DeviceStateActive;

	/// <summary>
	/// Reads a DWORD effect value. Gains are stored as two's-complement DWORDs, so -2 comes
	/// back as either Int32 -2 or 4294967294 depending on the value's high bit; both are
	/// reinterpreted to the same signed result.
	/// </summary>
	public int? ReadInt32(Guid propertySet, int index)
	{
		using RegistryKey? fx = Registry.LocalMachine.OpenSubKey(_fxPropertiesPath);
		object? raw = fx?.GetValue(ValueName(propertySet, index));
		return raw switch
		{
			null => null,
			int value => value,
			uint value => unchecked((int)value),
			long value => unchecked((int)value),
			_ => unchecked((int)Convert.ToInt64(raw, System.Globalization.CultureInfo.InvariantCulture))
		};
	}

	/// <summary>
	/// True when the value exists and is stored as a DWORD. A write that reports success but
	/// lands as REG_BINARY has gone through an interface that did not understand it, and the
	/// APO ignores the result, so the distinction matters.
	/// </summary>
	public bool IsDwordValue(Guid propertySet, int index)
	{
		using RegistryKey? fx = Registry.LocalMachine.OpenSubKey(_fxPropertiesPath);
		return fx?.GetValueKind(ValueName(propertySet, index)) == RegistryValueKind.DWord;
	}

	/// <summary>Property set and index of the first DWORD value present, for probing writes.</summary>
	public (Guid Set, int Index)? FirstDwordValue()
	{
		using RegistryKey? fx = Registry.LocalMachine.OpenSubKey(_fxPropertiesPath);
		if (fx is null) { return null; }

		foreach (string name in fx.GetValueNames())
		{
			if (fx.GetValueKind(name) != RegistryValueKind.DWord) { continue; }

			int comma = name.LastIndexOf(',');
			if (comma <= 2 || !int.TryParse(name.AsSpan(comma + 1), out int index)) { continue; }
			if (!Guid.TryParse(name.Substring(0, comma).Trim('{', '}'), out Guid set)) { continue; }

			return (set, index);
		}

		return null;
	}

	/// <summary>
	/// Writes a DWORD effect value. Requires administrator rights.</summary>
	///
	/// The key must be opened asking for only SetValue + QueryValues: Administrators are
	/// granted SetValue and ReadKey but NOT CreateSubKey, so the usual writable:true open
	/// (which requests KEY_WRITE, including CreateSubKey) is refused outright.
	/// </summary>
	public void WriteInt32(Guid propertySet, int index, int value)
	{
		using RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
		using RegistryKey? fx = baseKey.OpenSubKey(_fxPropertiesPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.SetValue | RegistryRights.QueryValues);
		if (fx is null) { throw new InvalidOperationException($"Cannot open {_fxPropertiesPath} for writing."); }
		fx.SetValue(ValueName(propertySet, index), value, RegistryValueKind.DWord);
	}

	private static string ValueName(Guid propertySet, int index) => $"{{{propertySet}}},{index}";
}

internal enum DeviceFlow
{
	Render,
	Capture
}
