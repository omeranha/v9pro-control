using System.Runtime.InteropServices;

namespace V9ProControl.Audio;

/// <summary>
/// A single Windows audio endpoint (one render or one capture device), exposing
/// master volume and mute. These are standard Core Audio controls, not vendor features.
/// </summary>
internal sealed class AudioEndpoint : IDisposable
{
	private static readonly Guid AudioEndpointVolumeIid = typeof(IAudioEndpointVolume).GUID;

	private readonly IMMDevice _device;
	private readonly IAudioEndpointVolume _volume;

	public string Id { get; }
	public string Description { get; }
	public DataFlow Flow { get; }

	private AudioEndpoint(IMMDevice device, string id, string description, DataFlow flow)
	{
		_device = device;
		Id = id;
		Description = description;
		Flow = flow;

		Guid iid = AudioEndpointVolumeIid;
		_device.Activate(ref iid, 1 /* CLSCTX_INPROC_SERVER */, IntPtr.Zero, out object instance);
		_volume = (IAudioEndpointVolume)instance;
	}

	/// <summary>Master volume as a percentage (0-100).</summary>
	public int Volume
	{
		get
		{
			_volume.GetMasterVolumeLevelScalar(out float scalar);
			return (int)Math.Round(scalar * 100f);
		}
		set
		{
			float scalar = Math.Clamp(value, 0, 100) / 100f;
			_volume.SetMasterVolumeLevelScalar(scalar, IntPtr.Zero);
		}
	}

	public bool IsMuted
	{
		get
		{
			_volume.GetMute(out bool muted);
			return muted;
		}
		set => _volume.SetMute(value, IntPtr.Zero);
	}

	public void ToggleMute() => IsMuted = !IsMuted;

	/// <summary>Enumerates active endpoints whose hardware description contains <paramref name="descriptionFilter"/>.</summary>
	public static List<AudioEndpoint> Find(DataFlow flow, string? descriptionFilter = null)
	{
		var found = new List<AudioEndpoint>();
		var enumerator = (IMMDeviceEnumerator)new MMDeviceEnumeratorComObject();
		enumerator.EnumAudioEndpoints(flow, DeviceState.Active, out IMMDeviceCollection collection);
		collection.GetCount(out uint count);

		for (uint i = 0; i < count; i++)
		{
			collection.Item(i, out IMMDevice device);
			device.GetId(out string id);
			string description = ReadStringProperty(device, PropertyKey.DeviceDescription) ?? string.Empty;

			if (descriptionFilter is not null && !description.Contains(descriptionFilter, StringComparison.OrdinalIgnoreCase))
			{
				Marshal.ReleaseComObject(device);
				continue;
			}

			found.Add(new AudioEndpoint(device, id, description, flow));
		}

		return found;
	}

	public static AudioEndpoint? FindFirst(DataFlow flow, string? descriptionFilter = null) => Find(flow, descriptionFilter).FirstOrDefault();

	private static string? ReadStringProperty(IMMDevice device, PropertyKey key)
	{
		try
		{
			device.OpenPropertyStore(0 /* STGM_READ */, out IPropertyStore store);
			PropVariant value = default;
			try
			{
				store.GetValue(ref key, out value);
				return value.AsString();
			}
			finally
			{
				Ole32.PropVariantClear(ref value);
				Marshal.ReleaseComObject(store);
			}
		}
		catch (COMException)
		{
			return null;
		}
	}

	public void Dispose()
	{
		if (_volume is not null) { Marshal.ReleaseComObject(_volume); }
		if (_device is not null) { Marshal.ReleaseComObject(_device); }
	}
}
