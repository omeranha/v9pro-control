using V9ProControl.Apo;
using V9ProControl.Audio;
using V9ProControl.Device;
using V9ProControl.Ui;

namespace V9ProControl;

public partial class MainForm : Form
{
	private const string DeviceFilter = "MCHOSE V9 PRO";

	private static readonly int[] FlatCurve = new int[Equalizer.BandCount];

	private AudioEndpoint? _outputEndpoint;
	private AudioEndpoint? _micEndpoint;
	private Equalizer? _equalizer;
	private PlaybackEffects? _effects;
	private OsConfLib? _apo;
	private Headset? _headset;

	private bool _suppressEvents;
	private bool _batteryBusy;
	private bool _fxAvailable;

	public MainForm()
	{
		InitializeComponent();

		DarkTheme.Apply(this);
		ConnectEndpoints();
		ConnectFxEffects();
		ConnectHeadset();
		tmrRefresh.Start();
	}

	/// <summary>
	/// The designer's Dispose only releases its own components, so unmanaged holders are
	/// released here rather than editing generated code.
	/// </summary>
	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		base.OnFormClosed(e);
		_outputEndpoint?.Dispose();
		_micEndpoint?.Dispose();
		_apo?.Dispose();
	}

	private void ConnectEndpoints()
	{
		// Prefer the headset, but stay usable on the default device when it is unplugged.
		_outputEndpoint = AudioEndpoint.FindFirst(DataFlow.Render, DeviceFilter) ?? AudioEndpoint.FindFirst(DataFlow.Render);
		_micEndpoint = AudioEndpoint.FindFirst(DataFlow.Capture, DeviceFilter) ?? AudioEndpoint.FindFirst(DataFlow.Capture);

		bool headsetFound = _outputEndpoint?.Description.Contains(DeviceFilter, StringComparison.OrdinalIgnoreCase) == true;
		SetStatus(headsetFound ? "Headset found." : "Headset not found — using the default device.");

		grpOutput.Text = $"Regular Parameters: {_outputEndpoint?.Description}";

		trkOutputVolume.Enabled = _outputEndpoint is not null;
		trkMicVolume.Enabled = chkMicMute.Enabled = _micEndpoint is not null;

		RefreshValues();
	}

	private void ConnectFxEffects()
	{
		EndpointFxProperties? fx = EndpointFxProperties.Find(DeviceFlow.Render, DeviceFilter);
		if (fx is null) {
			SetFxControlsEnabled(false);
			SetStatus("Effects unavailable: no APO settings found for the headset.");
			return;
		}

		// Without this the registry write persists but the running APO never re-reads it.
		_apo = OsConfLib.TryOpen();

		_equalizer = new Equalizer(fx, _apo);
		_effects = new PlaybackEffects(fx, _apo);
		LoadFxEffects();

		SetStatus(_apo is not null
			? "Effects and EQ apply immediately (osConfLib)."
			: "osConfLib not found — settings are saved but only apply after replugging the headset.");
	}

	private void ConnectHeadset()
	{
		_headset = Headset.Open();
		if (_headset is null) {
			lblBattery.Text = "Battery: dongle not found";
			btnBatteryRefresh.Enabled = false;
			cboSoundMode.Enabled = false;
		}
	}

	/// <summary>
	/// Device reads happen after the window is shown and strictly one after another. Firing
	/// them from the constructor in parallel made the first battery read fail.
	/// </summary>
	protected override async void OnShown(EventArgs e)
	{
		base.OnShown(e);

		EnsureAudioDriver();

		if (_headset is null) { return; }

		await RefreshBatteryAsync();
		await LoadSoundModeAsync();
	}

	/// <summary>
	/// Adding a driver package is a system-wide change, so it is offered once rather than
	/// performed silently on every launch.
	/// </summary>
	private void EnsureAudioDriver()
	{
		if (AudioDriver.FindInstalled() is not null) { return; }

		DriverPackage? package = AudioDriver.FindPackage();
		if (package is null) {
			SetStatus("Audio driver not installed, and no driver package was found to install from.");
			return;
		}

		string question = $"The C-Media audio driver this headset needs does not appear to be installed.\n\nInstall it from:\n{package.Directory}\n\nThis adds a signed driver package to Windows.";
		if (MessageBox.Show(question, "Install audio driver", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) {
			SetStatus("Audio driver not installed — volume works, but effects and EQ will not.");
			return;
		}

		(bool installed, bool rebootRequired, string message) = AudioDriver.Install(package);
		if (!installed) {
			SetStatus($"Driver installation failed: {message}");
			return;
		}

		SetStatus(rebootRequired
			? "Driver installed. Restart Windows, then start this app again."
			: "Driver installed. Restart this app so it picks up the new audio endpoint.");
	}

	/// <summary>Loaded once at startup; the periodic refresh must not fight user edits.</summary>
	private void LoadFxEffects()
	{
		_suppressEvents = true;
		try {
			if (_equalizer is not null) {
				chkEqualizerEnabled.Checked = _equalizer.IsEnabled;
				eqGraph.SetGains(_equalizer.ReadAllBands());
			}

			if (_effects is not null) {
				chkBrightening.Checked = _effects.BrighteningEnabled;
				numBrighteningLevel.Value = _effects.BrighteningLevel;

				chkVocalClarity.Checked = _effects.VocalClarityEnabled;
				numVocalClarityLevel.Value = _effects.VocalClarityLevel;
				numVocalClarityNoise.Value = _effects.VocalClarityNoiseSuppression;

				chkDynamicBass.Checked = _effects.DynamicBassEnabled;
				numDynamicBassLevel.Value = _effects.DynamicBassLevel;
				numDynamicBassCutoff.Value = _effects.DynamicBassCutoffHz;

				chkIntelligentVolume.Checked = _effects.IntelligentVolumeEnabled;
				numIntelligentVolumeLevel.Value = _effects.IntelligentVolumeLevel;
				cboIntelligentVolumeMode.SelectedIndex = Math.Clamp(_effects.IntelligentVolumeMode, 0, cboIntelligentVolumeMode.Items.Count - 1);
			}

			SetFxControlsEnabled(true);
		} catch (Exception ex) {
			SetFxControlsEnabled(false);
			SetStatus($"Failed to read the effect settings: {ex.Message}");
		} finally {
			_suppressEvents = false;
		}
	}

	private void SetFxControlsEnabled(bool enabled)
	{
		_fxAvailable = enabled;

		chkEqualizerEnabled.Enabled = enabled;

		chkBrightening.Enabled = enabled;
		chkVocalClarity.Enabled = enabled;
		chkDynamicBass.Enabled = enabled;
		chkIntelligentVolume.Enabled = enabled;

		UpdateEffectSubOptions();
	}

	/// <summary>A level only means something while its effect is on, so the inputs follow the toggles.</summary>
	private void UpdateEffectSubOptions()
	{
		bool equalizer = _fxAvailable && chkEqualizerEnabled.Checked;
		eqGraph.Enabled = equalizer;
		btnEqualizerReset.Enabled = equalizer;

		bool brightening = _fxAvailable && chkBrightening.Checked;
		lblBrighteningLevel.Enabled = brightening;
		numBrighteningLevel.Enabled = brightening;

		bool vocalClarity = _fxAvailable && chkVocalClarity.Checked;
		lblVocalClarityLevel.Enabled = vocalClarity;
		numVocalClarityLevel.Enabled = vocalClarity;
		lblVocalClarityNoise.Enabled = vocalClarity;
		numVocalClarityNoise.Enabled = vocalClarity;

		bool dynamicBass = _fxAvailable && chkDynamicBass.Checked;
		lblDynamicBassLevel.Enabled = dynamicBass;
		numDynamicBassLevel.Enabled = dynamicBass;
		lblDynamicBassCutoff.Enabled = dynamicBass;
		numDynamicBassCutoff.Enabled = dynamicBass;

		bool intelligentVolume = _fxAvailable && chkIntelligentVolume.Checked;
		lblIntelligentVolumeLevel.Enabled = intelligentVolume;
		numIntelligentVolumeLevel.Enabled = intelligentVolume;
		lblIntelligentVolumeMode.Enabled = intelligentVolume;
		cboIntelligentVolumeMode.Enabled = intelligentVolume;
	}

	/// <summary>HID exchanges block for up to a second, so they stay off the UI thread.</summary>
	private async Task RefreshBatteryAsync()
	{
		if (_headset is null || _batteryBusy) { return; }

		_batteryBusy = true;
		btnBatteryRefresh.Enabled = false;
		if (lblBattery.Text.EndsWith('—')) { lblBattery.Text = "Battery: reading…"; }
		try {
			BatteryReading? reading = await Task.Run(() => _headset.ReadBattery());
			lblBattery.Text = reading is null
				? "Battery: unavailable (headset off or out of range)"
				: $"Battery: {reading.Value.Describe()}";
		} catch (Exception ex) {
			lblBattery.Text = "Battery: read failed";
			SetStatus($"Battery read failed: {ex.Message}");
		} finally {
			_batteryBusy = false;
			btnBatteryRefresh.Enabled = true;
		}
	}

	private async Task LoadSoundModeAsync()
	{
		if (_headset is null) { return; }

		try {
			SoundMode? mode = await Task.Run(() => _headset.ReadSoundMode());
			_suppressEvents = true;
			cboSoundMode.SelectedIndex = mode is null ? -1 : (int)mode.Value;
			cboSoundMode.Enabled = mode is not null;
		} catch (Exception ex) {
			cboSoundMode.Enabled = false;
			SetStatus($"Sound mode read failed: {ex.Message}");
		} finally {
			_suppressEvents = false;
		}
	}

	private void RefreshValues()
	{
		_suppressEvents = true;
		try {
			if (_outputEndpoint is not null) {
				int volume = _outputEndpoint.Volume;
				trkOutputVolume.Value = volume;
				lblOutputVolumeValue.Text = $"{volume}%";
			}

			if (_micEndpoint is not null) {
				int volume = _micEndpoint.Volume;
				trkMicVolume.Value = volume;
				lblMicVolumeValue.Text = $"{volume}%";
				chkMicMute.Checked = _micEndpoint.IsMuted;
			}
		} catch (Exception ex) {
			SetStatus($"Failed to read the device: {ex.Message}");
		} finally {
			_suppressEvents = false;
		}
	}

	private void SetStatus(string message) => lblStatus.Text = message;

	private void OnRefreshTick(object? sender, EventArgs e) => RefreshValues();

	private async void OnBatteryRefresh(object? sender, EventArgs e)
	{
		await RefreshBatteryAsync();
		await LoadSoundModeAsync();
	}

	private void OnSoundModeChanged(object? sender, EventArgs e)
	{
		if (_suppressEvents || _headset is null || cboSoundMode.SelectedIndex < 0) { return; }

		var mode = (SoundMode)cboSoundMode.SelectedIndex;
		try {
			_headset.WriteSoundMode(mode);
			SetStatus($"Sound mode set to {cboSoundMode.SelectedItem}.");
		} catch (Exception ex) {
			SetStatus($"Failed to set the sound mode: {ex.Message}");
		}
	}

	private void OnOutputVolumeScroll(object? sender, EventArgs e)
	{
		if (_suppressEvents || _outputEndpoint is null) { return; }
		_outputEndpoint.Volume = trkOutputVolume.Value;
		lblOutputVolumeValue.Text = $"{trkOutputVolume.Value}%";
	}

	private void OnMicVolumeScroll(object? sender, EventArgs e)
	{
		if (_suppressEvents || _micEndpoint is null) { return; }
		_micEndpoint.Volume = trkMicVolume.Value;
		lblMicVolumeValue.Text = $"{trkMicVolume.Value}%";
	}

	private void OnMicMuteChanged(object? sender, EventArgs e)
	{
		if (_suppressEvents || _micEndpoint is null) { return; }
		_micEndpoint.IsMuted = chkMicMute.Checked;
	}

	private void OnEqualizerEnabledChanged(object? sender, EventArgs e)
	{
		UpdateEffectSubOptions();
		if (_suppressEvents || _equalizer is null) { return; }
		ApplyFxChange(() => _equalizer.IsEnabled = chkEqualizerEnabled.Checked);
	}

	private void OnEqualizerBandChanged(object? sender, EqualizerBandEventArgs e)
	{
		if (_suppressEvents || _equalizer is null) { return; }
		ApplyFxChange(() => _equalizer.SetBand(e.Band, e.GainDb));
	}

	private void OnEqualizerReset(object? sender, EventArgs e)
	{
		if (_equalizer is null) { return; }

		_suppressEvents = true;
		try {
			eqGraph.SetGains(FlatCurve);
		} finally {
			_suppressEvents = false;
		}

		ApplyFxChange(() => {
			for (int i = 0; i < Equalizer.BandCount; i++) { _equalizer.SetBand(i, 0); }
		});
	}

	private void OnBrighteningChanged(object? sender, EventArgs e)
	{
		UpdateEffectSubOptions();
		if (_suppressEvents || _effects is null) { return; }
		ApplyFxChange(() => {
			_effects.BrighteningEnabled = chkBrightening.Checked;
			_effects.BrighteningLevel = (int)numBrighteningLevel.Value;
		});
	}

	private void OnVocalClarityChanged(object? sender, EventArgs e)
	{
		UpdateEffectSubOptions();
		if (_suppressEvents || _effects is null) { return; }
		ApplyFxChange(() => {
			_effects.VocalClarityEnabled = chkVocalClarity.Checked;
			_effects.VocalClarityLevel = (int)numVocalClarityLevel.Value;
			_effects.VocalClarityNoiseSuppression = (int)numVocalClarityNoise.Value;
		});
	}

	private void OnDynamicBassChanged(object? sender, EventArgs e)
	{
		UpdateEffectSubOptions();
		if (_suppressEvents || _effects is null) { return; }
		ApplyFxChange(() => {
			_effects.DynamicBassEnabled = chkDynamicBass.Checked;
			_effects.DynamicBassLevel = (int)numDynamicBassLevel.Value;
			_effects.DynamicBassCutoffHz = (int)numDynamicBassCutoff.Value;
		});
	}

	private void OnIntelligentVolumeChanged(object? sender, EventArgs e)
	{
		UpdateEffectSubOptions();
		if (_suppressEvents || _effects is null) { return; }
		ApplyFxChange(() => {
			_effects.IntelligentVolumeEnabled = chkIntelligentVolume.Checked;
			_effects.IntelligentVolumeLevel = (int)numIntelligentVolumeLevel.Value;
			_effects.IntelligentVolumeMode = Math.Max(cboIntelligentVolumeMode.SelectedIndex, 0);
		});
	}

	private void ApplyFxChange(Action change)
	{
		try {
			change();
			SetStatus(_apo is not null ? "Applied." : "Saved — replug the headset to apply.");
		} catch (UnauthorizedAccessException) {
			SetStatus("Access denied — run this app as administrator.");
		} catch (Exception ex) {
			SetStatus($"Failed to write the setting: {ex.Message}");
		}
	}
}
