namespace V9ProControl.Device;

internal enum ChargeState
{
	Discharging = 2,
	Charging = 3,
	Full = 4,
	Asleep = 26
}

internal readonly record struct BatteryReading(int Percent, ChargeState State)
{
	public string Describe() => State switch
	{
		ChargeState.Discharging => $"{Percent}%",
		ChargeState.Charging => $"{Percent}% (charging)",
		ChargeState.Full => $"{Percent}% (full)",
		ChargeState.Asleep => $"{Percent}% (asleep)",
		_ => $"{Percent}% (state {(int)State})"
	};
}

/// <summary>
/// Sound presets stored on the headset itself. The device accepts 0..2 only and silently
/// clamps anything higher, which was verified by writing 0..5 and reading the value back.
/// </summary>
internal enum SoundMode
{
	GameOne = 0,
	GameTwo = 1,
	Music = 2
}

/// <summary>
/// Device-side features of the MCHOSE V9 PRO, spoken over the 0xFF90 vendor HID collection
/// (report id 0x55). Distinct from the Windows-side APO effects, which live in the registry.
///
///   battery    : tx 55 65 01        -> rx 55 65 &lt;percent&gt; &lt;state&gt;
///   mode query : tx 55 11           -> rx 55 11 &lt;mode&gt;
///   mode set   : tx 55 21 01 &lt;mode&gt; -> no reply
///
/// Requires recent dongle and headset firmware; older firmware ignores 0x65 entirely.
/// </summary>
internal sealed class Headset
{
	private const ushort VendorId = 0x291D;
	private const ushort ProductId = 0x385D;
	private const ushort VendorUsagePage = 0xFF90;

	private const byte ReportId = 0x55;
	private const byte CmdBattery = 0x65;
	private const byte CmdModeQuery = 0x11;
	private const byte CmdModeSet = 0x21;

	private const int TimeoutMs = 800;

	private readonly HidChannel _channel;

	/// <summary>
	/// The device handles one exchange at a time; concurrent requests on the same collection
	/// make it drop replies, which is why the first battery read used to fail when it raced
	/// the sound-mode read at startup.
	/// </summary>
	private readonly object _gate = new();

	private Headset(HidChannel channel) => _channel = channel;

	public static Headset? Open()
	{
		HidChannel? channel = HidChannel.Find(VendorId, ProductId, VendorUsagePage);
		return channel is null ? null : new Headset(channel);
	}

	/// <summary>
	/// Returns null when the headset is off, out of range, or on older firmware. Retries once:
	/// the first exchange after an idle period is occasionally dropped by the dongle.
	/// </summary>
	public BatteryReading? ReadBattery()
	{
		lock (_gate)
		{
			for (int attempt = 0; attempt < 2; attempt++)
			{
				byte[]? reply = _channel.Exchange([ReportId, CmdBattery, 0x01], TimeoutMs);
				if (reply is not null && reply.Length >= 4 && reply[0] == ReportId && reply[1] == CmdBattery && reply[2] <= 100)
				{
					return new BatteryReading(reply[2], (ChargeState)reply[3]);
				}

				if (attempt == 0) { Thread.Sleep(250); }
			}

			return null;
		}
	}

	public SoundMode? ReadSoundMode()
	{
		lock (_gate)
		{
			for (int attempt = 0; attempt < 2; attempt++)
			{
				byte[]? reply = _channel.Exchange([ReportId, CmdModeQuery], TimeoutMs);
				if (reply is not null && reply.Length >= 3 && reply[0] == ReportId && reply[1] == CmdModeQuery && reply[2] <= (byte)SoundMode.Music)
				{
					return (SoundMode)reply[2];
				}

				if (attempt == 0) { Thread.Sleep(250); }
			}

			return null;
		}
	}

	/// <summary>The set command is fire-and-forget; the device sends no acknowledgement.</summary>
	public void WriteSoundMode(SoundMode mode)
	{
		lock (_gate)
		{
			_channel.Exchange([ReportId, CmdModeSet, 0x01, (byte)mode], TimeoutMs, expectReply: false);
		}
	}
}
