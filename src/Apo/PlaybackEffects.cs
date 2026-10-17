namespace V9ProControl.Apo;

/// <summary>
/// The playback effects the C-Media APO exposes, which all share a single property set.
///
/// Each value is written twice over, by design: through osConfLib when it is available, which
/// applies the change to the running APO, and otherwise straight to the registry, which
/// persists it but only takes effect after the audio stream restarts.
///
/// Registry index assignments were established by watching FxProperties while each control was
/// changed in MCHOSE HUB, in a known order:
///
///   0  brightening enable            Enable_AUDIOBRILLIANT_LFX
///   1  brightening level             AUDIOBRILLIANT_LEVEL            observed 1..10
///   2  vocal clarity enable          Enable_VOICECLARITY_LFX
///   3  vocal clarity level           VOICECLARITY_LEVEL              observed 3..10
///   4  vocal clarity noise suppr.    VOICECLARITY_NOISESUPP_LEVEL    observed 0..20
///   5  dynamic bass enable           Enable_VIRTUALBASS_LFX
///   6  (unidentified, defaults to 1 - probably Enable_Channel_COPY_LFX)
///   7  dynamic bass level            VIRTUALBASS_Level               observed 5..10
///   8  dynamic bass cutoff in Hz     VIRTUALBASS_CutOffFrequency     observed 200..400
///   9  intelligent volume enable     Enable_ADAPTIVEVOLUME_GFX
///  10  intelligent volume level      ADAPTIVEVOLUME_LEVEL            observed 49..100
///  11  intelligent volume mode       ADAPTIVEVOLUME_MODE             observed 0..1
///
/// Ranges are the values the Hub itself wrote. The registry stores whatever it is given - the
/// APO clamps at runtime - so these bounds come from observation, not from the store.
/// </summary>
internal sealed class PlaybackEffects
{
	public static readonly Guid PropertySet = new("5e96e4d9-ff2c-4d84-9bd6-fe10cf815009");

	private const int BrighteningEnableIndex = 0;
	private const int BrighteningLevelIndex = 1;
	private const int VocalClarityEnableIndex = 2;
	private const int VocalClarityLevelIndex = 3;
	private const int VocalClarityNoiseIndex = 4;
	private const int DynamicBassEnableIndex = 5;
	private const int DynamicBassLevelIndex = 7;
	private const int DynamicBassCutoffIndex = 8;
	private const int IntelligentVolumeEnableIndex = 9;
	private const int IntelligentVolumeLevelIndex = 10;
	private const int IntelligentVolumeModeIndex = 11;

	private const string BrighteningEnableName = "Enable_AUDIOBRILLIANT_LFX";
	private const string BrighteningLevelName = "AUDIOBRILLIANT_LEVEL";
	private const string VocalClarityEnableName = "Enable_VOICECLARITY_LFX";
	private const string VocalClarityLevelName = "VOICECLARITY_LEVEL";
	private const string VocalClarityNoiseName = "VOICECLARITY_NOISESUPP_LEVEL";
	private const string DynamicBassEnableName = "Enable_VIRTUALBASS_LFX";
	private const string DynamicBassLevelName = "VIRTUALBASS_Level";
	private const string DynamicBassCutoffName = "VIRTUALBASS_CutOffFrequency";
	private const string IntelligentVolumeEnableName = "Enable_ADAPTIVEVOLUME_GFX";
	private const string IntelligentVolumeLevelName = "ADAPTIVEVOLUME_LEVEL";
	private const string IntelligentVolumeModeName = "ADAPTIVEVOLUME_MODE";

	public const int MinLevel = 0;
	public const int MaxLevel = 10;
	public const int MaxNoiseSuppression = 20;
	public const int MinCutoffHz = 50;
	public const int MaxCutoffHz = 400;
	public const int MaxIntelligentVolumeLevel = 100;
	public const int MaxIntelligentVolumeMode = 1;

	private readonly EndpointFxProperties _fx;
	private readonly ApoWriter _writer;

	public PlaybackEffects(EndpointFxProperties fx, ApoWriter writer)
	{
		_fx = fx;
		_writer = writer;
	}

	/// <summary>True when changes reach the running APO instead of only being persisted.</summary>
	public bool AppliesLive => _writer.AppliesLive;

	public bool BrighteningEnabled
	{
		get => ReadFlag(BrighteningEnableIndex);
		set => Write(BrighteningEnableName, BrighteningEnableIndex, value ? 1 : 0, ApoPropertyType.Bool);
	}

	public int BrighteningLevel
	{
		get => ReadClamped(BrighteningLevelIndex, MinLevel, MaxLevel);
		set => Write(BrighteningLevelName, BrighteningLevelIndex, Math.Clamp(value, MinLevel, MaxLevel), ApoPropertyType.Int);
	}

	public bool VocalClarityEnabled
	{
		get => ReadFlag(VocalClarityEnableIndex);
		set => Write(VocalClarityEnableName, VocalClarityEnableIndex, value ? 1 : 0, ApoPropertyType.Bool);
	}

	public int VocalClarityLevel
	{
		get => ReadClamped(VocalClarityLevelIndex, MinLevel, MaxLevel);
		set => Write(VocalClarityLevelName, VocalClarityLevelIndex, Math.Clamp(value, MinLevel, MaxLevel), ApoPropertyType.Int);
	}

	public int VocalClarityNoiseSuppression
	{
		get => ReadClamped(VocalClarityNoiseIndex, MinLevel, MaxNoiseSuppression);
		set => Write(VocalClarityNoiseName, VocalClarityNoiseIndex, Math.Clamp(value, MinLevel, MaxNoiseSuppression), ApoPropertyType.Int);
	}

	public bool DynamicBassEnabled
	{
		get => ReadFlag(DynamicBassEnableIndex);
		set => Write(DynamicBassEnableName, DynamicBassEnableIndex, value ? 1 : 0, ApoPropertyType.Bool);
	}

	public int DynamicBassLevel
	{
		get => ReadClamped(DynamicBassLevelIndex, MinLevel, MaxLevel);
		set => Write(DynamicBassLevelName, DynamicBassLevelIndex, Math.Clamp(value, MinLevel, MaxLevel), ApoPropertyType.Int);
	}

	public int DynamicBassCutoffHz
	{
		get => ReadClamped(DynamicBassCutoffIndex, MinCutoffHz, MaxCutoffHz);
		set => Write(DynamicBassCutoffName, DynamicBassCutoffIndex, Math.Clamp(value, MinCutoffHz, MaxCutoffHz), ApoPropertyType.Int);
	}

	public bool IntelligentVolumeEnabled
	{
		get => ReadFlag(IntelligentVolumeEnableIndex);
		set => Write(IntelligentVolumeEnableName, IntelligentVolumeEnableIndex, value ? 1 : 0, ApoPropertyType.Bool);
	}

	public int IntelligentVolumeLevel
	{
		get => ReadClamped(IntelligentVolumeLevelIndex, MinLevel, MaxIntelligentVolumeLevel);
		set => Write(IntelligentVolumeLevelName, IntelligentVolumeLevelIndex, Math.Clamp(value, MinLevel, MaxIntelligentVolumeLevel), ApoPropertyType.Int);
	}

	public int IntelligentVolumeMode
	{
		get => ReadClamped(IntelligentVolumeModeIndex, 0, MaxIntelligentVolumeMode);
		set => Write(IntelligentVolumeModeName, IntelligentVolumeModeIndex, Math.Clamp(value, 0, MaxIntelligentVolumeMode), ApoPropertyType.Int);
	}

	/// <summary>Applies through the APO when possible, falling back to a plain registry write.</summary>
	private void Write(string property, int index, int value, ApoPropertyType type) => _writer.Write(PropertySet, index, value, property, type);

	private bool ReadFlag(int index) => (_fx.ReadInt32(PropertySet, index) ?? 0) != 0;

	private int ReadClamped(int index, int min, int max) => Math.Clamp(_fx.ReadInt32(PropertySet, index) ?? min, min, max);
}
