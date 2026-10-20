// Kept strictly declarative so the Visual Studio forms designer can parse it: no loops,
// no helper methods, no arrays, no collection expressions, and literal values only.
// Anything computed lives in MainForm.cs instead.
//
// Rows of controls sit in TableLayoutPanels rather than at hand-placed coordinates. Columns
// size themselves to their widest cell, so a longer caption or a larger UI font widens the
// column instead of running over the control beside it.
#nullable disable

namespace V9ProControl;

partial class MainForm
{
	private System.ComponentModel.IContainer components = null;

	private System.Windows.Forms.GroupBox grpOutput;
	private System.Windows.Forms.TableLayoutPanel tlpDevice;
	private System.Windows.Forms.Label lblBattery;
	private System.Windows.Forms.Button btnBatteryRefresh;
	private System.Windows.Forms.Label lblSoundMode;
	private System.Windows.Forms.ComboBox cboSoundMode;
	private System.Windows.Forms.TableLayoutPanel tlpVolume;
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
	private System.Windows.Forms.TableLayoutPanel tlpProfiles;
	private System.Windows.Forms.Label lblProfile;
	private System.Windows.Forms.ComboBox cboProfile;
	private System.Windows.Forms.Button btnProfileImport;
	private System.Windows.Forms.Button btnProfileExport;

	private System.Windows.Forms.GroupBox grpEffects;
	private System.Windows.Forms.TableLayoutPanel tlpEffects;
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
		tlpDevice = new TableLayoutPanel();
		lblBattery = new Label();
		btnBatteryRefresh = new Button();
		lblSoundMode = new Label();
		cboSoundMode = new ComboBox();
		tlpVolume = new TableLayoutPanel();
		lblOutputVolume = new Label();
		trkOutputVolume = new TrackBar();
		lblOutputVolumeValue = new Label();
		lblMicVolume = new Label();
		trkMicVolume = new TrackBar();
		lblMicVolumeValue = new Label();
		chkMicMute = new CheckBox();
		grpEffects = new GroupBox();
		tlpEffects = new TableLayoutPanel();
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
		tlpProfiles = new TableLayoutPanel();
		lblProfile = new Label();
		cboProfile = new ComboBox();
		btnProfileImport = new Button();
		btnProfileExport = new Button();
		lblStatus = new Label();
		tmrRefresh = new System.Windows.Forms.Timer(components);
		grpOutput.SuspendLayout();
		tlpDevice.SuspendLayout();
		tlpVolume.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)trkOutputVolume).BeginInit();
		((System.ComponentModel.ISupportInitialize)trkMicVolume).BeginInit();
		grpEffects.SuspendLayout();
		tlpEffects.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)numBrighteningLevel).BeginInit();
		((System.ComponentModel.ISupportInitialize)numVocalClarityLevel).BeginInit();
		((System.ComponentModel.ISupportInitialize)numVocalClarityNoise).BeginInit();
		((System.ComponentModel.ISupportInitialize)numDynamicBassLevel).BeginInit();
		((System.ComponentModel.ISupportInitialize)numDynamicBassCutoff).BeginInit();
		((System.ComponentModel.ISupportInitialize)numIntelligentVolumeLevel).BeginInit();
		grpEqualizer.SuspendLayout();
		tlpProfiles.SuspendLayout();
		SuspendLayout();
		// 
		// grpOutput
		// 
		grpOutput.Controls.Add(tlpDevice);
		grpOutput.Controls.Add(tlpVolume);
		grpOutput.Location = new Point(12, 12);
		grpOutput.Name = "grpOutput";
		grpOutput.Size = new Size(600, 158);
		grpOutput.TabIndex = 0;
		grpOutput.TabStop = false;
		grpOutput.Text = "Regular Parameters";
		// 
		// tlpDevice
		// 
		tlpDevice.AutoSize = true;
		tlpDevice.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		tlpDevice.ColumnCount = 4;
		tlpDevice.ColumnStyles.Add(new ColumnStyle());
		tlpDevice.ColumnStyles.Add(new ColumnStyle());
		tlpDevice.ColumnStyles.Add(new ColumnStyle());
		tlpDevice.ColumnStyles.Add(new ColumnStyle());
		tlpDevice.Controls.Add(lblBattery, 0, 0);
		tlpDevice.Controls.Add(btnBatteryRefresh, 1, 0);
		tlpDevice.Controls.Add(lblSoundMode, 2, 0);
		tlpDevice.Controls.Add(cboSoundMode, 3, 0);
		tlpDevice.Location = new Point(14, 20);
		tlpDevice.Name = "tlpDevice";
		tlpDevice.RowCount = 1;
		tlpDevice.RowStyles.Add(new RowStyle());
		tlpDevice.Size = new Size(430, 31);
		tlpDevice.TabIndex = 0;
		// 
		// lblBattery
		// 
		lblBattery.Anchor = AnchorStyles.Left;
		lblBattery.AutoEllipsis = true;
		lblBattery.AutoSize = true;
		lblBattery.Location = new Point(3, 8);
		lblBattery.Margin = new Padding(3, 3, 12, 3);
		lblBattery.Name = "lblBattery";
		lblBattery.Size = new Size(62, 15);
		lblBattery.TabIndex = 0;
		lblBattery.Text = "Battery: —";
		// 
		// btnBatteryRefresh
		// 
		btnBatteryRefresh.Anchor = AnchorStyles.Left;
		btnBatteryRefresh.FlatStyle = FlatStyle.Flat;
		btnBatteryRefresh.Location = new Point(80, 3);
		btnBatteryRefresh.Margin = new Padding(3, 3, 20, 3);
		btnBatteryRefresh.Name = "btnBatteryRefresh";
		btnBatteryRefresh.Size = new Size(75, 25);
		btnBatteryRefresh.TabIndex = 1;
		btnBatteryRefresh.Text = "Refresh";
		btnBatteryRefresh.UseVisualStyleBackColor = true;
		btnBatteryRefresh.Click += OnBatteryRefresh;
		// 
		// lblSoundMode
		// 
		lblSoundMode.Anchor = AnchorStyles.Left;
		lblSoundMode.AutoSize = true;
		lblSoundMode.Location = new Point(178, 8);
		lblSoundMode.Margin = new Padding(3, 3, 8, 3);
		lblSoundMode.Name = "lblSoundMode";
		lblSoundMode.Size = new Size(78, 15);
		lblSoundMode.TabIndex = 2;
		lblSoundMode.Text = "Sound mode:";
		// 
		// cboSoundMode
		// 
		cboSoundMode.Anchor = AnchorStyles.Left;
		cboSoundMode.DropDownStyle = ComboBoxStyle.DropDownList;
		cboSoundMode.FlatStyle = FlatStyle.Flat;
		cboSoundMode.Items.AddRange(new object[] { "Game mode 1", "Game mode 2", "Music mode" });
		cboSoundMode.Location = new Point(267, 4);
		cboSoundMode.Name = "cboSoundMode";
		cboSoundMode.Size = new Size(160, 23);
		cboSoundMode.TabIndex = 3;
		cboSoundMode.SelectedIndexChanged += OnSoundModeChanged;
		// 
		// tlpVolume
		// 
		tlpVolume.AutoSize = true;
		tlpVolume.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		tlpVolume.ColumnCount = 4;
		tlpVolume.ColumnStyles.Add(new ColumnStyle());
		tlpVolume.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 340F));
		tlpVolume.ColumnStyles.Add(new ColumnStyle());
		tlpVolume.ColumnStyles.Add(new ColumnStyle());
		tlpVolume.Controls.Add(lblOutputVolume, 0, 0);
		tlpVolume.Controls.Add(trkOutputVolume, 1, 0);
		tlpVolume.Controls.Add(lblOutputVolumeValue, 2, 0);
		tlpVolume.Controls.Add(lblMicVolume, 0, 1);
		tlpVolume.Controls.Add(trkMicVolume, 1, 1);
		tlpVolume.Controls.Add(lblMicVolumeValue, 2, 1);
		tlpVolume.Controls.Add(chkMicMute, 3, 1);
		tlpVolume.Location = new Point(14, 55);
		tlpVolume.Name = "tlpVolume";
		tlpVolume.RowCount = 2;
		tlpVolume.RowStyles.Add(new RowStyle());
		tlpVolume.RowStyles.Add(new RowStyle());
		tlpVolume.Size = new Size(559, 90);
		tlpVolume.TabIndex = 1;
		// 
		// lblOutputVolume
		// 
		lblOutputVolume.Anchor = AnchorStyles.Left;
		lblOutputVolume.AutoSize = true;
		lblOutputVolume.Location = new Point(3, 15);
		lblOutputVolume.Margin = new Padding(3, 3, 12, 3);
		lblOutputVolume.Name = "lblOutputVolume";
		lblOutputVolume.Size = new Size(88, 15);
		lblOutputVolume.TabIndex = 0;
		lblOutputVolume.Text = "Output volume";
		// 
		// trkOutputVolume
		// 
		trkOutputVolume.Anchor = AnchorStyles.Left;
		trkOutputVolume.Location = new Point(106, 0);
		trkOutputVolume.Margin = new Padding(3, 0, 8, 0);
		trkOutputVolume.Maximum = 100;
		trkOutputVolume.Name = "trkOutputVolume";
		trkOutputVolume.Size = new Size(329, 45);
		trkOutputVolume.TabIndex = 1;
		trkOutputVolume.TickFrequency = 10;
		trkOutputVolume.Scroll += OnOutputVolumeScroll;
		// 
		// lblOutputVolumeValue
		// 
		lblOutputVolumeValue.Anchor = AnchorStyles.Left;
		lblOutputVolumeValue.Location = new Point(446, 15);
		lblOutputVolumeValue.Margin = new Padding(3, 3, 12, 3);
		lblOutputVolumeValue.Name = "lblOutputVolumeValue";
		lblOutputVolumeValue.Size = new Size(44, 15);
		lblOutputVolumeValue.TabIndex = 2;
		lblOutputVolumeValue.Text = "—";
		lblOutputVolumeValue.TextAlign = ContentAlignment.MiddleRight;
		// 
		// lblMicVolume
		// 
		lblMicVolume.Anchor = AnchorStyles.Left;
		lblMicVolume.AutoSize = true;
		lblMicVolume.Location = new Point(3, 60);
		lblMicVolume.Margin = new Padding(3, 3, 12, 3);
		lblMicVolume.Name = "lblMicVolume";
		lblMicVolume.Size = new Size(72, 15);
		lblMicVolume.TabIndex = 3;
		lblMicVolume.Text = "Microphone";
		// 
		// trkMicVolume
		// 
		trkMicVolume.Anchor = AnchorStyles.Left;
		trkMicVolume.Location = new Point(106, 45);
		trkMicVolume.Margin = new Padding(3, 0, 8, 0);
		trkMicVolume.Maximum = 100;
		trkMicVolume.Name = "trkMicVolume";
		trkMicVolume.Size = new Size(329, 45);
		trkMicVolume.TabIndex = 4;
		trkMicVolume.TickFrequency = 10;
		trkMicVolume.Scroll += OnMicVolumeScroll;
		// 
		// lblMicVolumeValue
		// 
		lblMicVolumeValue.Anchor = AnchorStyles.Left;
		lblMicVolumeValue.Location = new Point(446, 60);
		lblMicVolumeValue.Margin = new Padding(3, 3, 12, 3);
		lblMicVolumeValue.Name = "lblMicVolumeValue";
		lblMicVolumeValue.Size = new Size(44, 15);
		lblMicVolumeValue.TabIndex = 5;
		lblMicVolumeValue.Text = "—";
		lblMicVolumeValue.TextAlign = ContentAlignment.MiddleRight;
		// 
		// chkMicMute
		// 
		chkMicMute.Anchor = AnchorStyles.Left;
		chkMicMute.AutoSize = true;
		chkMicMute.FlatStyle = FlatStyle.Flat;
		chkMicMute.Location = new Point(505, 58);
		chkMicMute.Name = "chkMicMute";
		chkMicMute.Size = new Size(51, 19);
		chkMicMute.TabIndex = 6;
		chkMicMute.Text = "Mute";
		chkMicMute.CheckedChanged += OnMicMuteChanged;
		// 
		// grpEffects
		// 
		grpEffects.Controls.Add(tlpEffects);
		grpEffects.Location = new Point(12, 182);
		grpEffects.Name = "grpEffects";
		grpEffects.Size = new Size(600, 152);
		grpEffects.TabIndex = 1;
		grpEffects.TabStop = false;
		grpEffects.Text = "Effects";
		// 
		// tlpEffects
		// 
		tlpEffects.AutoSize = true;
		tlpEffects.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		tlpEffects.ColumnCount = 5;
		tlpEffects.ColumnStyles.Add(new ColumnStyle());
		tlpEffects.ColumnStyles.Add(new ColumnStyle());
		tlpEffects.ColumnStyles.Add(new ColumnStyle());
		tlpEffects.ColumnStyles.Add(new ColumnStyle());
		tlpEffects.ColumnStyles.Add(new ColumnStyle());
		tlpEffects.Controls.Add(chkBrightening, 0, 0);
		tlpEffects.Controls.Add(lblBrighteningLevel, 1, 0);
		tlpEffects.Controls.Add(numBrighteningLevel, 2, 0);
		tlpEffects.Controls.Add(chkVocalClarity, 0, 1);
		tlpEffects.Controls.Add(lblVocalClarityLevel, 1, 1);
		tlpEffects.Controls.Add(numVocalClarityLevel, 2, 1);
		tlpEffects.Controls.Add(lblVocalClarityNoise, 3, 1);
		tlpEffects.Controls.Add(numVocalClarityNoise, 4, 1);
		tlpEffects.Controls.Add(chkDynamicBass, 0, 2);
		tlpEffects.Controls.Add(lblDynamicBassLevel, 1, 2);
		tlpEffects.Controls.Add(numDynamicBassLevel, 2, 2);
		tlpEffects.Controls.Add(lblDynamicBassCutoff, 3, 2);
		tlpEffects.Controls.Add(numDynamicBassCutoff, 4, 2);
		tlpEffects.Controls.Add(chkIntelligentVolume, 0, 3);
		tlpEffects.Controls.Add(lblIntelligentVolumeLevel, 1, 3);
		tlpEffects.Controls.Add(numIntelligentVolumeLevel, 2, 3);
		tlpEffects.Controls.Add(lblIntelligentVolumeMode, 3, 3);
		tlpEffects.Controls.Add(cboIntelligentVolumeMode, 4, 3);
		tlpEffects.Location = new Point(14, 22);
		tlpEffects.Name = "tlpEffects";
		tlpEffects.RowCount = 4;
		tlpEffects.RowStyles.Add(new RowStyle());
		tlpEffects.RowStyles.Add(new RowStyle());
		tlpEffects.RowStyles.Add(new RowStyle());
		tlpEffects.RowStyles.Add(new RowStyle());
		tlpEffects.Size = new Size(583, 116);
		tlpEffects.TabIndex = 0;
		// 
		// chkBrightening
		// 
		chkBrightening.Anchor = AnchorStyles.Left;
		chkBrightening.AutoSize = true;
		chkBrightening.FlatStyle = FlatStyle.Flat;
		chkBrightening.Location = new Point(3, 5);
		chkBrightening.Margin = new Padding(3, 3, 14, 3);
		chkBrightening.Name = "chkBrightening";
		chkBrightening.Size = new Size(120, 19);
		chkBrightening.TabIndex = 0;
		chkBrightening.Text = "Audio Brightening";
		chkBrightening.CheckedChanged += OnBrighteningChanged;
		// 
		// lblBrighteningLevel
		// 
		lblBrighteningLevel.Anchor = AnchorStyles.Left;
		lblBrighteningLevel.AutoSize = true;
		lblBrighteningLevel.Location = new Point(173, 7);
		lblBrighteningLevel.Margin = new Padding(3, 3, 6, 3);
		lblBrighteningLevel.Name = "lblBrighteningLevel";
		lblBrighteningLevel.Size = new Size(52, 15);
		lblBrighteningLevel.TabIndex = 1;
		lblBrighteningLevel.Text = "Intensity";
		// 
		// numBrighteningLevel
		// 
		numBrighteningLevel.Anchor = AnchorStyles.Left;
		numBrighteningLevel.Location = new Point(234, 3);
		numBrighteningLevel.Margin = new Padding(3, 3, 14, 3);
		numBrighteningLevel.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
		numBrighteningLevel.Name = "numBrighteningLevel";
		numBrighteningLevel.Size = new Size(55, 23);
		numBrighteningLevel.TabIndex = 2;
		numBrighteningLevel.TextAlign = HorizontalAlignment.Center;
		numBrighteningLevel.ValueChanged += OnBrighteningChanged;
		// 
		// chkVocalClarity
		// 
		chkVocalClarity.Anchor = AnchorStyles.Left;
		chkVocalClarity.AutoSize = true;
		chkVocalClarity.FlatStyle = FlatStyle.Flat;
		chkVocalClarity.Location = new Point(3, 34);
		chkVocalClarity.Margin = new Padding(3, 3, 14, 3);
		chkVocalClarity.Name = "chkVocalClarity";
		chkVocalClarity.Size = new Size(88, 19);
		chkVocalClarity.TabIndex = 3;
		chkVocalClarity.Text = "Vocal Clarity";
		chkVocalClarity.CheckedChanged += OnVocalClarityChanged;
		// 
		// lblVocalClarityLevel
		// 
		lblVocalClarityLevel.Anchor = AnchorStyles.Left;
		lblVocalClarityLevel.AutoSize = true;
		lblVocalClarityLevel.Location = new Point(173, 36);
		lblVocalClarityLevel.Margin = new Padding(3, 3, 6, 3);
		lblVocalClarityLevel.Name = "lblVocalClarityLevel";
		lblVocalClarityLevel.Size = new Size(52, 15);
		lblVocalClarityLevel.TabIndex = 4;
		lblVocalClarityLevel.Text = "Intensity";
		// 
		// numVocalClarityLevel
		// 
		numVocalClarityLevel.Anchor = AnchorStyles.Left;
		numVocalClarityLevel.Location = new Point(234, 32);
		numVocalClarityLevel.Margin = new Padding(3, 3, 14, 3);
		numVocalClarityLevel.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
		numVocalClarityLevel.Name = "numVocalClarityLevel";
		numVocalClarityLevel.Size = new Size(55, 23);
		numVocalClarityLevel.TabIndex = 5;
		numVocalClarityLevel.TextAlign = HorizontalAlignment.Center;
		numVocalClarityLevel.ValueChanged += OnVocalClarityChanged;
		// 
		// lblVocalClarityNoise
		// 
		lblVocalClarityNoise.Anchor = AnchorStyles.Left;
		lblVocalClarityNoise.AutoSize = true;
		lblVocalClarityNoise.Location = new Point(306, 36);
		lblVocalClarityNoise.Margin = new Padding(3, 3, 6, 3);
		lblVocalClarityNoise.Name = "lblVocalClarityNoise";
		lblVocalClarityNoise.Size = new Size(103, 15);
		lblVocalClarityNoise.TabIndex = 6;
		lblVocalClarityNoise.Text = "Noise suppression";
		// 
		// numVocalClarityNoise
		// 
		numVocalClarityNoise.Anchor = AnchorStyles.Left;
		numVocalClarityNoise.Location = new Point(450, 32);
		numVocalClarityNoise.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
		numVocalClarityNoise.Name = "numVocalClarityNoise";
		numVocalClarityNoise.Size = new Size(65, 23);
		numVocalClarityNoise.TabIndex = 7;
		numVocalClarityNoise.TextAlign = HorizontalAlignment.Center;
		numVocalClarityNoise.ValueChanged += OnVocalClarityChanged;
		// 
		// chkDynamicBass
		// 
		chkDynamicBass.Anchor = AnchorStyles.Left;
		chkDynamicBass.AutoSize = true;
		chkDynamicBass.FlatStyle = FlatStyle.Flat;
		chkDynamicBass.Location = new Point(3, 63);
		chkDynamicBass.Margin = new Padding(3, 3, 14, 3);
		chkDynamicBass.Name = "chkDynamicBass";
		chkDynamicBass.Size = new Size(153, 19);
		chkDynamicBass.TabIndex = 8;
		chkDynamicBass.Text = "Dynamic Low Frequency";
		chkDynamicBass.CheckedChanged += OnDynamicBassChanged;
		// 
		// lblDynamicBassLevel
		// 
		lblDynamicBassLevel.Anchor = AnchorStyles.Left;
		lblDynamicBassLevel.AutoSize = true;
		lblDynamicBassLevel.Location = new Point(173, 65);
		lblDynamicBassLevel.Margin = new Padding(3, 3, 6, 3);
		lblDynamicBassLevel.Name = "lblDynamicBassLevel";
		lblDynamicBassLevel.Size = new Size(52, 15);
		lblDynamicBassLevel.TabIndex = 9;
		lblDynamicBassLevel.Text = "Intensity";
		// 
		// numDynamicBassLevel
		// 
		numDynamicBassLevel.Anchor = AnchorStyles.Left;
		numDynamicBassLevel.Location = new Point(234, 61);
		numDynamicBassLevel.Margin = new Padding(3, 3, 14, 3);
		numDynamicBassLevel.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
		numDynamicBassLevel.Name = "numDynamicBassLevel";
		numDynamicBassLevel.Size = new Size(55, 23);
		numDynamicBassLevel.TabIndex = 10;
		numDynamicBassLevel.TextAlign = HorizontalAlignment.Center;
		numDynamicBassLevel.ValueChanged += OnDynamicBassChanged;
		// 
		// lblDynamicBassCutoff
		// 
		lblDynamicBassCutoff.Anchor = AnchorStyles.Left;
		lblDynamicBassCutoff.AutoSize = true;
		lblDynamicBassCutoff.Location = new Point(306, 65);
		lblDynamicBassCutoff.Margin = new Padding(3, 3, 6, 3);
		lblDynamicBassCutoff.Name = "lblDynamicBassCutoff";
		lblDynamicBassCutoff.Size = new Size(135, 15);
		lblDynamicBassCutoff.TabIndex = 11;
		lblDynamicBassCutoff.Text = "Band segmentation (Hz)";
		// 
		// numDynamicBassCutoff
		// 
		numDynamicBassCutoff.Anchor = AnchorStyles.Left;
		numDynamicBassCutoff.Location = new Point(450, 61);
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
		chkIntelligentVolume.Anchor = AnchorStyles.Left;
		chkIntelligentVolume.AutoSize = true;
		chkIntelligentVolume.FlatStyle = FlatStyle.Flat;
		chkIntelligentVolume.Location = new Point(3, 92);
		chkIntelligentVolume.Margin = new Padding(3, 3, 14, 3);
		chkIntelligentVolume.Name = "chkIntelligentVolume";
		chkIntelligentVolume.Size = new Size(119, 19);
		chkIntelligentVolume.TabIndex = 13;
		chkIntelligentVolume.Text = "Intelligent Volume";
		chkIntelligentVolume.CheckedChanged += OnIntelligentVolumeChanged;
		// 
		// lblIntelligentVolumeLevel
		// 
		lblIntelligentVolumeLevel.Anchor = AnchorStyles.Left;
		lblIntelligentVolumeLevel.AutoSize = true;
		lblIntelligentVolumeLevel.Location = new Point(173, 94);
		lblIntelligentVolumeLevel.Margin = new Padding(3, 3, 6, 3);
		lblIntelligentVolumeLevel.Name = "lblIntelligentVolumeLevel";
		lblIntelligentVolumeLevel.Size = new Size(52, 15);
		lblIntelligentVolumeLevel.TabIndex = 14;
		lblIntelligentVolumeLevel.Text = "Intensity";
		// 
		// numIntelligentVolumeLevel
		// 
		numIntelligentVolumeLevel.Anchor = AnchorStyles.Left;
		numIntelligentVolumeLevel.Location = new Point(234, 90);
		numIntelligentVolumeLevel.Margin = new Padding(3, 3, 14, 3);
		numIntelligentVolumeLevel.Name = "numIntelligentVolumeLevel";
		numIntelligentVolumeLevel.Size = new Size(55, 23);
		numIntelligentVolumeLevel.TabIndex = 15;
		numIntelligentVolumeLevel.TextAlign = HorizontalAlignment.Center;
		numIntelligentVolumeLevel.ValueChanged += OnIntelligentVolumeChanged;
		// 
		// lblIntelligentVolumeMode
		// 
		lblIntelligentVolumeMode.Anchor = AnchorStyles.Left;
		lblIntelligentVolumeMode.AutoSize = true;
		lblIntelligentVolumeMode.Location = new Point(306, 94);
		lblIntelligentVolumeMode.Margin = new Padding(3, 3, 6, 3);
		lblIntelligentVolumeMode.Name = "lblIntelligentVolumeMode";
		lblIntelligentVolumeMode.Size = new Size(38, 15);
		lblIntelligentVolumeMode.TabIndex = 16;
		lblIntelligentVolumeMode.Text = "Mode";
		// 
		// cboIntelligentVolumeMode
		// 
		cboIntelligentVolumeMode.Anchor = AnchorStyles.Left;
		cboIntelligentVolumeMode.DropDownStyle = ComboBoxStyle.DropDownList;
		cboIntelligentVolumeMode.FlatStyle = FlatStyle.Flat;
		cboIntelligentVolumeMode.Items.AddRange(new object[] { "Music: High Dyn.", "Speech: Low Dyn." });
		cboIntelligentVolumeMode.Location = new Point(450, 90);
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
		grpEqualizer.Controls.Add(tlpProfiles);
		grpEqualizer.Location = new Point(12, 346);
		grpEqualizer.Name = "grpEqualizer";
		grpEqualizer.Size = new Size(600, 268);
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
		btnEqualizerReset.Anchor = AnchorStyles.Top | AnchorStyles.Right;
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
		eqGraph.AccessibleDescription = "Left and right arrows select a band; up and down arrows change its gain in decibels. With the mouse, drag a band's handle.";
		eqGraph.AccessibleName = "Equalizer bands";
		eqGraph.AccessibleRole = AccessibleRole.Slider;
		eqGraph.BackColor = Color.FromArgb(32, 32, 32);
		eqGraph.ForeColor = Color.FromArgb(240, 240, 240);
		eqGraph.Location = new Point(16, 56);
		eqGraph.Name = "eqGraph";
		eqGraph.Size = new Size(568, 160);
		eqGraph.TabIndex = 2;
		eqGraph.BandChanged += OnEqualizerBandChanged;
		// 
		// tlpProfiles
		// 
		tlpProfiles.AutoSize = true;
		tlpProfiles.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		tlpProfiles.ColumnCount = 4;
		tlpProfiles.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
		tlpProfiles.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
		tlpProfiles.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
		tlpProfiles.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
		tlpProfiles.Controls.Add(lblProfile, 0, 0);
		tlpProfiles.Controls.Add(cboProfile, 1, 0);
		tlpProfiles.Controls.Add(btnProfileImport, 2, 0);
		tlpProfiles.Controls.Add(btnProfileExport, 3, 0);
		tlpProfiles.Location = new Point(14, 224);
		tlpProfiles.Name = "tlpProfiles";
		tlpProfiles.RowCount = 1;
		tlpProfiles.RowStyles.Add(new RowStyle(SizeType.AutoSize));
		tlpProfiles.Size = new Size(406, 31);
		tlpProfiles.TabIndex = 3;
		// 
		// lblProfile
		// 
		lblProfile.Anchor = AnchorStyles.Left;
		lblProfile.AutoSize = true;
		lblProfile.Margin = new Padding(3, 3, 8, 3);
		lblProfile.Name = "lblProfile";
		lblProfile.Size = new Size(44, 15);
		lblProfile.TabIndex = 0;
		lblProfile.Text = "Profile";
		// 
		// cboProfile
		// 
		cboProfile.Anchor = AnchorStyles.Left;
		cboProfile.DropDownStyle = ComboBoxStyle.DropDownList;
		cboProfile.FlatStyle = FlatStyle.Flat;
		cboProfile.Margin = new Padding(3, 3, 20, 3);
		cboProfile.Name = "cboProfile";
		cboProfile.Size = new Size(180, 23);
		cboProfile.TabIndex = 1;
		cboProfile.SelectedIndexChanged += OnProfileSelected;
		// 
		// btnProfileImport
		// 
		btnProfileImport.Anchor = AnchorStyles.Left;
		btnProfileImport.FlatStyle = FlatStyle.Flat;
		btnProfileImport.Margin = new Padding(3, 3, 8, 3);
		btnProfileImport.Name = "btnProfileImport";
		btnProfileImport.Size = new Size(75, 25);
		btnProfileImport.TabIndex = 2;
		btnProfileImport.Text = "Import";
		btnProfileImport.UseVisualStyleBackColor = true;
		btnProfileImport.Click += OnProfileImport;
		// 
		// btnProfileExport
		// 
		btnProfileExport.Anchor = AnchorStyles.Left;
		btnProfileExport.FlatStyle = FlatStyle.Flat;
		btnProfileExport.Name = "btnProfileExport";
		btnProfileExport.Size = new Size(75, 25);
		btnProfileExport.TabIndex = 3;
		btnProfileExport.Text = "Export";
		btnProfileExport.UseVisualStyleBackColor = true;
		btnProfileExport.Click += OnProfileExport;
		// 
		// lblStatus
		// 
		lblStatus.ForeColor = Color.DimGray;
		lblStatus.Location = new Point(14, 624);
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
		ClientSize = new Size(624, 651);
		Controls.Add(grpOutput);
		Controls.Add(grpEffects);
		Controls.Add(grpEqualizer);
		Controls.Add(lblStatus);
		FormBorderStyle = FormBorderStyle.FixedSingle;
		MaximizeBox = false;
		Name = "MainForm";
		StartPosition = FormStartPosition.CenterScreen;
		Text = "V9 PRO Control";
		grpOutput.ResumeLayout(false);
		grpOutput.PerformLayout();
		tlpDevice.ResumeLayout(false);
		tlpDevice.PerformLayout();
		tlpVolume.ResumeLayout(false);
		tlpVolume.PerformLayout();
		((System.ComponentModel.ISupportInitialize)trkOutputVolume).EndInit();
		((System.ComponentModel.ISupportInitialize)trkMicVolume).EndInit();
		grpEffects.ResumeLayout(false);
		grpEffects.PerformLayout();
		tlpEffects.ResumeLayout(false);
		tlpEffects.PerformLayout();
		((System.ComponentModel.ISupportInitialize)numBrighteningLevel).EndInit();
		((System.ComponentModel.ISupportInitialize)numVocalClarityLevel).EndInit();
		((System.ComponentModel.ISupportInitialize)numVocalClarityNoise).EndInit();
		((System.ComponentModel.ISupportInitialize)numDynamicBassLevel).EndInit();
		((System.ComponentModel.ISupportInitialize)numDynamicBassCutoff).EndInit();
		((System.ComponentModel.ISupportInitialize)numIntelligentVolumeLevel).EndInit();
		grpEqualizer.ResumeLayout(false);
		grpEqualizer.PerformLayout();
		tlpProfiles.ResumeLayout(false);
		tlpProfiles.PerformLayout();
		ResumeLayout(false);
	}
}
