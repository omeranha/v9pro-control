namespace V9ProControl.Apo;

/// <summary>
/// The 10-band equaliser exposed by the C-Media APO.
///
/// Property set and layout were confirmed empirically by diffing FxProperties around a
/// single EQ toggle in MCHOSE HUB: index 0 is the enable flag, indices 1..10 are the
/// per-band gains in dB stored as two's-complement DWORDs.
///
/// Everything applies live through osConfLib. The enable flag is Enable_EQ_GFX and each band
/// is its own property, EQ_Slider1..EQ_Slider10 - names read out of the library's string
/// literals rather than guessed. Earlier attempts failed because they used the undecorated
/// name "EQ_Slider" with the band number in the type byte, and because a float payload was
/// tried: the gain is a plain int32 in dB, and a float writes its bit pattern as the gain.
/// </summary>
internal sealed class Equalizer
{
	public static readonly Guid PropertySet = new("bd341ffb-9fd1-4ed4-8159-7999d546ea8a");

	public const int BandCount = 10;
	public const int MinGainDb = -20;
	public const int MaxGainDb = 20;

	private const int EnableIndex = 0;
	private const int FirstBandIndex = 1;
	private const string EnableName = "Enable_EQ_GFX";

	/// <summary>
	/// Band centres as presented by MCHOSE HUB. These are fixed in the APO's DSP: the
	/// registry stores only the ten gain values, with no frequency data, so the centres
	/// cannot be retuned through this property set.
	/// </summary>
	public static readonly string[] BandLabels = { "20", "60", "100", "250", "500", "1k", "2k", "4k", "8k", "20k" };

	private readonly EndpointFxProperties _fx;
	private readonly ApoWriter _writer;

	public Equalizer(EndpointFxProperties fx, ApoWriter writer)
	{
		_fx = fx;
		_writer = writer;
	}

	public bool IsEnabled
	{
		get => (_fx.ReadInt32(PropertySet, EnableIndex) ?? 0) != 0;
		set => _writer.Write(PropertySet, EnableIndex, value ? 1 : 0, EnableName, ApoPropertyType.Bool);
	}

	public int GetBand(int band)
	{
		ValidateBand(band);
		return _fx.ReadInt32(PropertySet, FirstBandIndex + band) ?? 0;
	}

	/// <summary>
	/// Pushes one band to the APO. The writer applies it live when it can and falls back to a
	/// plain registry write otherwise.
	/// </summary>
	public void SetBand(int band, int gainDb)
	{
		ValidateBand(band);
		int gain = Math.Clamp(gainDb, MinGainDb, MaxGainDb);
		_writer.Write(PropertySet, FirstBandIndex + band, gain, BandName(band), ApoPropertyType.Int);
	}

	public int[] ReadAllBands()
	{
		var bands = new int[BandCount];
		for (int i = 0; i < BandCount; i++) { bands[i] = GetBand(i); }
		return bands;
	}

	private static string BandName(int band) => $"EQ_Slider{band + 1}";

	private static void ValidateBand(int band)
	{
		if (band < 0 || band >= BandCount) { throw new ArgumentOutOfRangeException(nameof(band), band, "Band index must be 0..9."); }
	}
}
