namespace V9ProControl.Ui;

/// <summary>
/// Dark palette applied by walking the control tree.
///
/// Windows Forms does not theme every control from a single switch: TrackBar ignores custom
/// colours entirely (so it only inherits the surrounding background), and Button/ComboBox
/// need FlatStyle set before their colours are honoured.
/// </summary>
internal static class DarkTheme
{
	public static readonly Color Background = Color.FromArgb(32, 32, 32);
	public static readonly Color Surface = Color.FromArgb(45, 45, 48);
	public static readonly Color Foreground = Color.FromArgb(240, 240, 240);
	public static readonly Color Muted = Color.FromArgb(160, 160, 160);
	public static readonly Color Border = Color.FromArgb(70, 70, 74);

	public static void Apply(Form form)
	{
		form.BackColor = Background;
		form.ForeColor = Foreground;
		ApplyTo(form.Controls);
	}

	private static void ApplyTo(Control.ControlCollection controls)
	{
		foreach (Control control in controls)
		{
			switch (control)
			{
				case GroupBox group:
					group.BackColor = Background;
					group.ForeColor = Foreground;
					break;

				case Button button:
					button.FlatStyle = FlatStyle.Flat;
					button.BackColor = Surface;
					button.ForeColor = Foreground;
					button.FlatAppearance.BorderColor = Border;
					break;

				case ComboBox combo:
					combo.FlatStyle = FlatStyle.Flat;
					combo.BackColor = Surface;
					combo.ForeColor = Foreground;
					break;

				case NumericUpDown numeric:
					numeric.BackColor = Surface;
					numeric.ForeColor = Foreground;
					numeric.BorderStyle = BorderStyle.FixedSingle;
					break;

				case CheckBox check:
					check.BackColor = Background;
					check.ForeColor = Foreground;
					check.FlatStyle = FlatStyle.Flat;
					break;

				case TrackBar track:
					// TrackBar renders itself; only the surrounding fill can be influenced.
					track.BackColor = Background;
					break;

				case Label label:
					label.BackColor = Color.Transparent;
					label.ForeColor = label.ForeColor == Color.DimGray ? Muted : Foreground;
					break;

				default:
					control.BackColor = Background;
					control.ForeColor = Foreground;
					break;
			}

			if (control.HasChildren) { ApplyTo(control.Controls); }
		}
	}
}
