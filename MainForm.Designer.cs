// Kept strictly declarative so the Visual Studio forms designer can parse it: no loops,
// no helper methods, no arrays, no collection expressions, and literal values only.
// Anything computed lives in MainForm.cs instead.
#nullable disable

namespace V9ProControl;

partial class MainForm
{
	private System.ComponentModel.IContainer components = null;

	private System.Windows.Forms.GroupBox grpOutput;
	private System.Windows.Forms.Label lblBattery;
	private System.Windows.Forms.Button btnBatteryRefresh;
	private System.Windows.Forms.Label lblSoundMode;
	private System.Windows.Forms.ComboBox cboSoundMode;
	private System.Windows.Forms.Label lblOutputVolume;
	private System.Windows.Forms.TrackBar trkOutputVolume;
	private System.Windows.Forms.Label lblOutputVolumeValue;
	private System.Windows.Forms.Label lblMicVolume;
	private System.Windows.Forms.TrackBar trkMicVolume;
	private System.Windows.Forms.Label lblMicVolumeValue;
	private System.Windows.Forms.CheckBox chkMicMute;

	private System.Windows.Forms.GroupBox grpEqualizer;
	private System.Windows.Forms.CheckBox chkEqualizerEnabled;
	private System.Windows.Forms.Button btnEqualizerReset;
	private V9ProControl.Ui.EqualizerGraph eqGraph;

	private System.Windows.Forms.GroupBox grpEffects;
	private System.Windows.Forms.CheckBox chkBrightening;
	private System.Windows.Forms.Label lblBrighteningLevel;
	private System.Windows.Forms.NumericUpDown numBrighteningLevel;
	private System.Windows.Forms.CheckBox chkVocalClarity;
	private System.Windows.Forms.Label lblVocalClarityLevel;
	private System.Windows.Forms.NumericUpDown numVocalClarityLevel;
	private System.Windows.Forms.Label lblVocalClarityNoise;
	private System.Windows.Forms.NumericUpDown numVocalClarityNoise;
	private System.Windows.Forms.CheckBox chkDynamicBass;
	private System.Windows.Forms.Label lblDynamicBassLevel;
	private System.Windows.Forms.NumericUpDown numDynamicBassLevel;
	private System.Windows.Forms.Label lblDynamicBassCutoff;
	private System.Windows.Forms.NumericUpDown numDynamicBassCutoff;
	private System.Windows.Forms.CheckBox chkIntelligentVolume;
	private System.Windows.Forms.Label lblIntelligentVolumeLevel;
	private System.Windows.Forms.NumericUpDown numIntelligentVolumeLevel;
	private System.Windows.Forms.Label lblIntelligentVolumeMode;
	private System.Windows.Forms.ComboBox cboIntelligentVolumeMode;

	private System.Windows.Forms.Label lblStatus;
	private System.Windows.Forms.Timer tmrRefresh;

	protected override void Dispose(bool disposing)
	{
		if (disposing && (components != null))
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		components = new System.ComponentModel.Container();
		grpOutput = new GroupBox();
		lblBattery = new Label();
		btnBatteryRefresh = new Button();
		lblSoundMode = new Label();
		cboSoundMode = new ComboBox();
		lblOutputVolume = new Label();
		trkOutputVolume = new TrackBar();
		lblOutputVolumeValue = new Label();
		lblMicVolume = new Label();
		trkMicVolume = new TrackBar();
		lblMicVolumeValue = new Label();
		chkMicMute = new CheckBox();
		grpEffects = new GroupBox();
		chkBrightening = new CheckBox();
		lblBrighteningLevel = new Label();
		numBrighteningLevel = new NumericUpDown();
		chkVocalClarity = new CheckBox();
		lblVocalClarityLevel = new Label();
		numVocalClarityLevel = new NumericUpDown();
		lblVocalClarityNoise = new Label();
		numVocalClarityNoise = new NumericUpDown();
		chkDynamicBass = new CheckBox();
		lblDynamicBassLevel = new Label();
		numDynamicBassLevel = new NumericUpDown();
		lblDynamicBassCutoff = new Label();
		numDynamicBassCutoff = new NumericUpDown();
		chkIntelligentVolume = new CheckBox();
		lblIntelligentVolumeLevel = new Label();
		numIntelligentVolumeLevel = new NumericUpDown();
		lblIntelligentVolumeMode = new Label();
		cboIntelligentVolumeMode = new ComboBox();
		grpEqualizer = new GroupBox();
		chkEqualizerEnabled = new CheckBox();
		btnEqualizerReset = new Button();
		eqGraph = new V9ProControl.Ui.EqualizerGraph();
		lblStatus = new Label();
		tmrRefresh = new System.Windows.Forms.Timer(components);
		grpOutput.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)trkOutputVolume).BeginInit();
		((System.ComponentModel.ISupportInitialize)trkMicVolume).BeginInit();
		grpEffects.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)numBrighteningLevel).BeginInit();
		((System.ComponentModel.ISupportInitialize)numVocalClarityLevel).BeginInit();
		((System.ComponentModel.ISupportInitialize)numVocalClarityNoise).BeginInit();
		((System.ComponentModel.ISupportInitialize)numDynamicBassLevel).BeginInit();
		((System.ComponentModel.ISupportInitialize)numDynamicBassCutoff).BeginInit();
		((System.ComponentModel.ISupportInitialize)numIntelligentVolumeLevel).BeginInit();
		grpEqualizer.SuspendLayout();
		SuspendLayout();
		// 
		// grpOutput
		// 
		grpOutput.Controls.Add(lblBattery);
		grpOutput.Controls.Add(btnBatteryRefresh);
		grpOutput.Controls.Add(lblSoundMode);
		grpOutput.Controls.Add(cboSoundMode);
		grpOutput.Controls.Add(lblOutputVolume);
		grpOutput.Controls.Add(trkOutputVolume);
		grpOutput.Controls.Add(lblOutputVolumeValue);
		grpOutput.Controls.Add(lblMicVolume);
		grpOutput.Controls.Add(trkMicVolume);
		grpOutput.Controls.Add(lblMicVolumeValue);
		grpOutput.Controls.Add(chkMicMute);
		grpOutput.Location = new Point(12, 12);
		grpOutput.Name = "grpOutput";
		grpOutput.Size = new Size(600, 155);
		grpOutput.TabIndex = 0;
		grpOutput.TabStop = false;
		grpOutput.Text = "Regular Parameters";
		// 
		// lblBattery
		// 
		// Fixed width, not AutoSize: the longest states ("dongle not found" at 143px,
		// "100% (charging)" at 136px) would otherwise grow the label under the Refresh button.
		// AutoEllipsis keeps anything unexpected inside its own bounds.
		lblBattery.AutoEllipsis = true;
		lblBattery.Location = new Point(16, 27);
		lblBattery.Name = "lblBattery";
		lblBattery.Size = new Size(150, 15);
		lblBattery.TabIndex = 0;
		lblBattery.Text = "Battery: —";
		// 
		// btnBatteryRefresh
		// 
		btnBatteryRefresh.FlatStyle = FlatStyle.Flat;
		btnBatteryRefresh.Location = new Point(174, 22);
		btnBatteryRefresh.Name = "btnBatteryRefresh";
		btnBatteryRefresh.Size = new Size(75, 25);
		btnBatteryRefresh.TabIndex = 1;
		btnBatteryRefresh.Text = "Refresh";
		btnBatteryRefresh.UseVisualStyleBackColor = true;
		btnBatteryRefresh.Click += OnBatteryRefresh;
		// 
		// lblSoundMode
		// 
		lblSoundMode.AutoSize = true;
		lblSoundMode.Location = new Point(265, 27);
		lblSoundMode.Name = "lblSoundMode";
		lblSoundMode.Size = new Size(78, 15);
		lblSoundMode.TabIndex = 2;
		lblSoundMode.Text = "Sound mode:";
		// 
		// cboSoundMode
		// 
		cboSoundMode.DropDownStyle = ComboBoxStyle.DropDownList;
		cboSoundMode.FlatStyle = FlatStyle.Flat;
		cboSoundMode.Items.AddRange(new object[] { "Game mode 1", "Game mode 2", "Music mode" });
		cboSoundMode.Location = new Point(351, 23);
		cboSoundMode.Name = "cboSoundMode";
		cboSoundMode.Size = new Size(160, 23);
		cboSoundMode.TabIndex = 3;
		cboSoundMode.SelectedIndexChanged += OnSoundModeChanged;
		// 
		// lblOutputVolume
		// 
		lblOutputVolume.Location = new Point(16, 68);
		lblOutputVolume.Name = "lblOutputVolume";
		lblOutputVolume.Size = new Size(96, 15);
		lblOutputVolume.TabIndex = 4;
		lblOutputVolume.Text = "Output volume";
		// 
		// trkOutputVolume
		// 
		trkOutputVolume.Location = new Point(116, 62);
		trkOutputVolume.Maximum = 100;
		trkOutputVolume.Name = "trkOutputVolume";
		trkOutputVolume.Size = new Size(330, 45);
		trkOutputVolume.TabIndex = 5;
		trkOutputVolume.TickFrequency = 10;
		trkOutputVolume.Scroll += OnOutputVolumeScroll;
		// 
		// lblOutputVolumeValue
		// 
		lblOutputVolumeValue.Location = new Point(456, 68);
		lblOutputVolumeValue.Name = "lblOutputVolumeValue";
		lblOutputVolumeValue.Size = new Size(44, 15);
		lblOutputVolumeValue.TabIndex = 6;
		lblOutputVolumeValue.Text = "—";
		lblOutputVolumeValue.TextAlign = ContentAlignment.MiddleRight;
		// 
		// lblMicVolume
		// 
		lblMicVolume.Location = new Point(16, 113);
		lblMicVolume.Name = "lblMicVolume";
		lblMicVolume.Size = new Size(96, 15);
		lblMicVolume.TabIndex = 7;
		lblMicVolume.Text = "Microphone";
		// 
		// trkMicVolume
		// 
		trkMicVolume.Location = new Point(116, 107);
		trkMicVolume.Maximum = 100;
		trkMicVolume.Name = "trkMicVolume";
		trkMicVolume.Size = new Size(330, 45);
		trkMicVolume.TabIndex = 8;
		trkMicVolume.TickFrequency = 10;
		trkMicVolume.Scroll += OnMicVolumeScroll;
		// 
		// lblMicVolumeValue
		// 
		lblMicVolumeValue.Location = new Point(456, 113);
		lblMicVolumeValue.Name = "lblMicVolumeValue";
		lblMicVolumeValue.Size = new Size(44, 15);
		lblMicVolumeValue.TabIndex = 9;
		lblMicVolumeValue.Text = "—";
		lblMicVolumeValue.TextAlign = ContentAlignment.MiddleRight;
		// 
		// chkMicMute
		// 
		chkMicMute.AutoSize = true;
		chkMicMute.FlatStyle = FlatStyle.Flat;
		chkMicMute.Location = new Point(516, 111);
		chkMicMute.Name = "chkMicMute";
		chkMicMute.Size = new Size(51, 19);
		chkMicMute.TabIndex = 10;
		chkMicMute.Text = "Mute";
		chkMicMute.CheckedChanged += OnMicMuteChanged;
		// 
		// grpEffects
		// 
		grpEffects.Controls.Add(chkBrightening);
		grpEffects.Controls.Add(lblBrighteningLevel);
		grpEffects.Controls.Add(numBrighteningLevel);
		grpEffects.Controls.Add(chkVocalClarity);
		grpEffects.Controls.Add(lblVocalClarityLevel);
		grpEffects.Controls.Add(numVocalClarityLevel);
		grpEffects.Controls.Add(lblVocalClarityNoise);
		grpEffects.Controls.Add(numVocalClarityNoise);
		grpEffects.Controls.Add(chkDynamicBass);
		grpEffects.Controls.Add(lblDynamicBassLevel);
		grpEffects.Controls.Add(numDynamicBassLevel);
		grpEffects.Controls.Add(lblDynamicBassCutoff);
		grpEffects.Controls.Add(numDynamicBassCutoff);
		grpEffects.Controls.Add(chkIntelligentVolume);
		grpEffects.Controls.Add(lblIntelligentVolumeLevel);
		grpEffects.Controls.Add(numIntelligentVolumeLevel);
		grpEffects.Controls.Add(lblIntelligentVolumeMode);
		grpEffects.Controls.Add(cboIntelligentVolumeMode);
		grpEffects.Location = new Point(12, 179);
		grpEffects.Name = "grpEffects";
		grpEffects.Size = new Size(600, 152);
		grpEffects.TabIndex = 1;
		grpEffects.TabStop = false;
		grpEffects.Text = "Effects";
		// 
		// chkBrightening
		// 
		chkBrightening.AutoSize = true;
		chkBrightening.FlatStyle = FlatStyle.Flat;
		chkBrightening.Location = new Point(16, 28);
		chkBrightening.Name = "chkBrightening";
		chkBrightening.Size = new Size(120, 19);
		chkBrightening.TabIndex = 0;
		chkBrightening.Text = "Audio Brightening";
		chkBrightening.CheckedChanged += OnBrighteningChanged;
		// 
		// lblBrighteningLevel
		// 
		lblBrighteningLevel.Location = new Point(180, 30);
		lblBrighteningLevel.Name = "lblBrighteningLevel";
		lblBrighteningLevel.Size = new Size(64, 15);
		lblBrighteningLevel.TabIndex = 1;
		lblBrighteningLevel.Text = "Intensity";
		// 
		// numBrighteningLevel
		// 
		numBrighteningLevel.Location = new Point(250, 26);
		numBrighteningLevel.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
		numBrighteningLevel.Name = "numBrighteningLevel";
		numBrighteningLevel.Size = new Size(55, 23);
		numBrighteningLevel.TabIndex = 2;
		numBrighteningLevel.TextAlign = HorizontalAlignment.Center;
		numBrighteningLevel.ValueChanged += OnBrighteningChanged;
		// 
		// chkVocalClarity
		// 
		chkVocalClarity.AutoSize = true;
		chkVocalClarity.FlatStyle = FlatStyle.Flat;
		chkVocalClarity.Location = new Point(16, 59);
		chkVocalClarity.Name = "chkVocalClarity";
		chkVocalClarity.Size = new Size(88, 19);
		chkVocalClarity.TabIndex = 3;
		chkVocalClarity.Text = "Vocal Clarity";
		chkVocalClarity.CheckedChanged += OnVocalClarityChanged;
		// 
		// lblVocalClarityLevel
		// 
		lblVocalClarityLevel.Location = new Point(180, 61);
		lblVocalClarityLevel.Name = "lblVocalClarityLevel";
		lblVocalClarityLevel.Size = new Size(64, 15);
		lblVocalClarityLevel.TabIndex = 4;
		lblVocalClarityLevel.Text = "Intensity";
		// 
		// numVocalClarityLevel
		// 
		numVocalClarityLevel.Location = new Point(250, 57);
		numVocalClarityLevel.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
		numVocalClarityLevel.Name = "numVocalClarityLevel";
		numVocalClarityLevel.Size = new Size(55, 23);
		numVocalClarityLevel.TabIndex = 5;
		numVocalClarityLevel.TextAlign = HorizontalAlignment.Center;
		numVocalClarityLevel.ValueChanged += OnVocalClarityChanged;
		// 
		// lblVocalClarityNoise
		// 
		lblVocalClarityNoise.Location = new Point(330, 61);
		lblVocalClarityNoise.Name = "lblVocalClarityNoise";
		lblVocalClarityNoise.Size = new Size(176, 15);
		lblVocalClarityNoise.TabIndex = 6;
		lblVocalClarityNoise.Text = "Noise suppression";
		// 
		// numVocalClarityNoise
		// 
		numVocalClarityNoise.Location = new Point(520, 57);
		numVocalClarityNoise.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
		numVocalClarityNoise.Name = "numVocalClarityNoise";
		numVocalClarityNoise.Size = new Size(65, 23);
		numVocalClarityNoise.TabIndex = 7;
		numVocalClarityNoise.TextAlign = HorizontalAlignment.Center;
		numVocalClarityNoise.ValueChanged += OnVocalClarityChanged;
		// 
		// chkDynamicBass
		// 
		chkDynamicBass.AutoSize = true;
		chkDynamicBass.FlatStyle = FlatStyle.Flat;
		chkDynamicBass.Location = new Point(16, 90);
		chkDynamicBass.Name = "chkDynamicBass";
		chkDynamicBass.Size = new Size(153, 19);
		chkDynamicBass.TabIndex = 8;
		chkDynamicBass.Text = "Dynamic Low Frequency";
		chkDynamicBass.CheckedChanged += OnDynamicBassChanged;
		// 
		// lblDynamicBassLevel
		// 
		lblDynamicBassLevel.Location = new Point(180, 92);
		lblDynamicBassLevel.Name = "lblDynamicBassLevel";
		lblDynamicBassLevel.Size = new Size(64, 15);
		lblDynamicBassLevel.TabIndex = 9;
		lblDynamicBassLevel.Text = "Intensity";
		// 
		// numDynamicBassLevel
		// 
		numDynamicBassLevel.Location = new Point(250, 88);
		numDynamicBassLevel.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
		numDynamicBassLevel.Name = "numDynamicBassLevel";
		numDynamicBassLevel.Size = new Size(55, 23);
		numDynamicBassLevel.TabIndex = 10;
		numDynamicBassLevel.TextAlign = HorizontalAlignment.Center;
		numDynamicBassLevel.ValueChanged += OnDynamicBassChanged;
		// 
		// lblDynamicBassCutoff
		// 
		lblDynamicBassCutoff.Location = new Point(330, 92);
		lblDynamicBassCutoff.Name = "lblDynamicBassCutoff";
		lblDynamicBassCutoff.Size = new Size(176, 15);
		lblDynamicBassCutoff.TabIndex = 11;
		lblDynamicBassCutoff.Text = "Band segmentation (Hz)";
		// 
		// numDynamicBassCutoff
		// 
		numDynamicBassCutoff.Location = new Point(520, 88);
		numDynamicBassCutoff.Maximum = new decimal(new int[] { 400, 0, 0, 0 });
		numDynamicBassCutoff.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
		numDynamicBassCutoff.Name = "numDynamicBassCutoff";
		numDynamicBassCutoff.Size = new Size(65, 23);
		numDynamicBassCutoff.TabIndex = 12;
		numDynamicBassCutoff.TextAlign = HorizontalAlignment.Center;
		numDynamicBassCutoff.Value = new decimal(new int[] { 50, 0, 0, 0 });
		numDynamicBassCutoff.ValueChanged += OnDynamicBassChanged;
		// 
		// chkIntelligentVolume
		// 
		chkIntelligentVolume.AutoSize = true;
		chkIntelligentVolume.FlatStyle = FlatStyle.Flat;
		chkIntelligentVolume.Location = new Point(16, 121);
		chkIntelligentVolume.Name = "chkIntelligentVolume";
		chkIntelligentVolume.Size = new Size(119, 19);
		chkIntelligentVolume.TabIndex = 13;
		chkIntelligentVolume.Text = "Intelligent Volume";
		chkIntelligentVolume.CheckedChanged += OnIntelligentVolumeChanged;
		// 
		// lblIntelligentVolumeLevel
		// 
		lblIntelligentVolumeLevel.Location = new Point(180, 123);
		lblIntelligentVolumeLevel.Name = "lblIntelligentVolumeLevel";
		lblIntelligentVolumeLevel.Size = new Size(64, 15);
		lblIntelligentVolumeLevel.TabIndex = 14;
		lblIntelligentVolumeLevel.Text = "Intensity";
		// 
		// numIntelligentVolumeLevel
		// 
		numIntelligentVolumeLevel.Location = new Point(250, 119);
		numIntelligentVolumeLevel.Name = "numIntelligentVolumeLevel";
		numIntelligentVolumeLevel.Size = new Size(55, 23);
		numIntelligentVolumeLevel.TabIndex = 15;
		numIntelligentVolumeLevel.TextAlign = HorizontalAlignment.Center;
		numIntelligentVolumeLevel.ValueChanged += OnIntelligentVolumeChanged;
		// 
		// lblIntelligentVolumeMode
		// 
		lblIntelligentVolumeMode.Location = new Point(330, 123);
		lblIntelligentVolumeMode.Name = "lblIntelligentVolumeMode";
		lblIntelligentVolumeMode.Size = new Size(120, 15);
		lblIntelligentVolumeMode.TabIndex = 16;
		lblIntelligentVolumeMode.Text = "Mode";
		// 
		// cboIntelligentVolumeMode
		// 
		cboIntelligentVolumeMode.DropDownStyle = ComboBoxStyle.DropDownList;
		cboIntelligentVolumeMode.FlatStyle = FlatStyle.Flat;
		cboIntelligentVolumeMode.Items.AddRange(new object[] { "Music: High Dyn.", "Speech: Low Dyn." });
		cboIntelligentVolumeMode.Location = new Point(455, 119);
		cboIntelligentVolumeMode.Name = "cboIntelligentVolumeMode";
		cboIntelligentVolumeMode.Size = new Size(130, 23);
		cboIntelligentVolumeMode.TabIndex = 17;
		cboIntelligentVolumeMode.SelectedIndexChanged += OnIntelligentVolumeChanged;
		// 
		// grpEqualizer
		// 
		grpEqualizer.Controls.Add(chkEqualizerEnabled);
		grpEqualizer.Controls.Add(btnEqualizerReset);
		grpEqualizer.Controls.Add(eqGraph);
		grpEqualizer.Location = new Point(12, 343);
		grpEqualizer.Name = "grpEqualizer";
		grpEqualizer.Size = new Size(600, 228);
		grpEqualizer.TabIndex = 2;
		grpEqualizer.TabStop = false;
		grpEqualizer.Text = "Equalizer";
		// 
		// chkEqualizerEnabled
		// 
		chkEqualizerEnabled.AutoSize = true;
		chkEqualizerEnabled.FlatStyle = FlatStyle.Flat;
		chkEqualizerEnabled.Location = new Point(16, 26);
		chkEqualizerEnabled.Name = "chkEqualizerEnabled";
		chkEqualizerEnabled.Size = new Size(65, 19);
		chkEqualizerEnabled.TabIndex = 0;
		chkEqualizerEnabled.Text = "Enabled";
		chkEqualizerEnabled.CheckedChanged += OnEqualizerEnabledChanged;
		// 
		// btnEqualizerReset
		// 
		btnEqualizerReset.FlatStyle = FlatStyle.Flat;
		btnEqualizerReset.Location = new Point(510, 22);
		btnEqualizerReset.Name = "btnEqualizerReset";
		btnEqualizerReset.Size = new Size(75, 25);
		btnEqualizerReset.TabIndex = 1;
		btnEqualizerReset.Text = "Flat";
		btnEqualizerReset.UseVisualStyleBackColor = true;
		btnEqualizerReset.Click += OnEqualizerReset;
		// 
		// eqGraph
		// 
		eqGraph.Location = new Point(16, 56);
		eqGraph.Name = "eqGraph";
		eqGraph.Size = new Size(568, 160);
		eqGraph.TabIndex = 2;
		eqGraph.BandChanged += OnEqualizerBandChanged;
		// 
		// lblStatus
		// 
		lblStatus.ForeColor = Color.DimGray;
		lblStatus.Location = new Point(14, 581);
		lblStatus.Name = "lblStatus";
		lblStatus.Size = new Size(596, 15);
		lblStatus.TabIndex = 3;
		// 
		// tmrRefresh
		// 
		tmrRefresh.Interval = 1000;
		tmrRefresh.Tick += OnRefreshTick;
		// 
		// MainForm
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(624, 607);
		Controls.Add(grpOutput);
		Controls.Add(grpEffects);
		Controls.Add(grpEqualizer);
		Controls.Add(lblStatus);
		FormBorderStyle = FormBorderStyle.FixedSingle;
		MaximizeBox = false;
		Name = "MainForm";
		Text = "V9 PRO Control";
		grpOutput.ResumeLayout(false);
		grpOutput.PerformLayout();
		((System.ComponentModel.ISupportInitialize)trkOutputVolume).EndInit();
		((System.ComponentModel.ISupportInitialize)trkMicVolume).EndInit();
		grpEffects.ResumeLayout(false);
		grpEffects.PerformLayout();
		((System.ComponentModel.ISupportInitialize)numBrighteningLevel).EndInit();
		((System.ComponentModel.ISupportInitialize)numVocalClarityLevel).EndInit();
		((System.ComponentModel.ISupportInitialize)numVocalClarityNoise).EndInit();
		((System.ComponentModel.ISupportInitialize)numDynamicBassLevel).EndInit();
		((System.ComponentModel.ISupportInitialize)numDynamicBassCutoff).EndInit();
		((System.ComponentModel.ISupportInitialize)numIntelligentVolumeLevel).EndInit();
		grpEqualizer.ResumeLayout(false);
		grpEqualizer.PerformLayout();
		ResumeLayout(false);
	}
}
