using System.Drawing.Drawing2D;
using V9ProControl.Apo;

namespace V9ProControl.Ui;

internal sealed class EqualizerBandEventArgs(int band, int gainDb) : EventArgs
{
	public int Band { get; } = band;
	public int GainDb { get; } = gainDb;
}

/// <summary>
/// Draggable curve view of the ten EQ bands, drawn rather than composed from child controls
/// so it can honour the dark palette - the stock TrackBar cannot be recoloured.
///
/// Gains are whole dB, matching what the APO stores, so dragging only raises BandChanged when
/// the rounded value actually changes. That keeps one drag from flooding osConfLib with
/// redundant writes.
/// </summary>
internal sealed class EqualizerGraph : Control
{
	private const int HandleRadius = 5;
	private const int GrabRadius = 12;
	private const int GridStepDb = 10;
	private const int PadLeft = 36;
	private const int PadRight = 30;
	private const int PadTop = 22;
	private const int PadBottom = 20;

	private static readonly Color PlotBackColor = Color.FromArgb(38, 38, 40);
	private static readonly Color GridColor = Color.FromArgb(58, 58, 62);
	private static readonly Color ZeroColor = Color.FromArgb(95, 95, 102);
	private static readonly Color AccentColor = Color.FromArgb(0, 120, 212);
	private static readonly Color DisabledColor = Color.FromArgb(105, 105, 110);

	private readonly int[] _gains = new int[Equalizer.BandCount];
	private int _selectedBand;
	private int _dragBand = -1;

	public event EventHandler<EqualizerBandEventArgs>? BandChanged;

	public EqualizerGraph()
	{
		SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.Selectable | ControlStyles.ResizeRedraw, true);
		TabStop = true;
		BackColor = DarkTheme.Background;
		ForeColor = DarkTheme.Foreground;
		AccessibleRole = AccessibleRole.Slider;
		AccessibleName = "Equalizer bands";
		AccessibleDescription = "Left and right arrows select a band; up and down arrows change its gain in decibels. With the mouse, drag a band's handle.";
	}

	public int GetGain(int band) => _gains[band];

	/// <summary>Loads all bands without raising BandChanged, for syncing from the registry.</summary>
	public void SetGains(IReadOnlyList<int> gains)
	{
		for (int i = 0; i < _gains.Length && i < gains.Count; i++)
		{
			_gains[i] = Math.Clamp(gains[i], Equalizer.MinGainDb, Equalizer.MaxGainDb);
		}
		Invalidate();
	}

	public void SetGain(int band, int gainDb)
	{
		int clamped = Math.Clamp(gainDb, Equalizer.MinGainDb, Equalizer.MaxGainDb);
		if (_gains[band] == clamped) { return; }

		_gains[band] = clamped;
		Invalidate();
		BandChanged?.Invoke(this, new EqualizerBandEventArgs(band, clamped));
	}

	private Rectangle PlotRect => new(PadLeft, PadTop, Math.Max(Width - PadLeft - PadRight, 1), Math.Max(Height - PadTop - PadBottom, 1));

	private int BandX(int band)
	{
		Rectangle plot = PlotRect;
		return plot.Left + (band * (plot.Width - 1)) / (Equalizer.BandCount - 1);
	}

	private int GainY(int gainDb)
	{
		Rectangle plot = PlotRect;
		double span = Equalizer.MaxGainDb - Equalizer.MinGainDb;
		return (int)Math.Round(plot.Bottom - ((gainDb - Equalizer.MinGainDb) / span) * (plot.Height - 1));
	}

	private int GainFromY(int y)
	{
		Rectangle plot = PlotRect;
		double fraction = (double)(plot.Bottom - y) / Math.Max(plot.Height - 1, 1);
		double gain = Equalizer.MinGainDb + fraction * (Equalizer.MaxGainDb - Equalizer.MinGainDb);
		return Math.Clamp((int)Math.Round(gain), Equalizer.MinGainDb, Equalizer.MaxGainDb);
	}

	private int BandFromX(int x)
	{
		int nearest = 0;
		int bestDistance = int.MaxValue;
		for (int i = 0; i < Equalizer.BandCount; i++)
		{
			int distance = Math.Abs(BandX(i) - x);
			if (distance < bestDistance) { bestDistance = distance; nearest = i; }
		}
		return nearest;
	}

	/// <summary>
	/// A click selects a band but never moves it: the gain only changes once its handle has
	/// been grabbed. Setting the gain straight from the click position made a stray click
	/// anywhere in the plot retune the nearest band by as much as 20 dB.
	/// </summary>
	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);
		if (e.Button != MouseButtons.Left) { return; }

		Focus();
		int band = BandFromX(e.X);
		_selectedBand = band;

		int dx = BandX(band) - e.X;
		int dy = GainY(_gains[band]) - e.Y;
		if ((dx * dx) + (dy * dy) <= GrabRadius * GrabRadius) { _dragBand = band; }

		Invalidate();
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		base.OnMouseMove(e);
		if (_dragBand >= 0) { SetGain(_dragBand, GainFromY(e.Y)); return; }

		Cursor = Enabled && IsOverHandle(e.X, e.Y) ? Cursors.SizeNS : Cursors.Default;
	}

	private bool IsOverHandle(int x, int y)
	{
		int band = BandFromX(x);
		int dx = BandX(band) - x;
		int dy = GainY(_gains[band]) - y;
		return (dx * dx) + (dy * dy) <= GrabRadius * GrabRadius;
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		base.OnMouseUp(e);
		_dragBand = -1;
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		base.OnMouseLeave(e);
		Cursor = Cursors.Default;
	}

	/// <summary>Focus is required so that scrolling over the window cannot silently retune a band.</summary>
	protected override void OnMouseWheel(MouseEventArgs e)
	{
		base.OnMouseWheel(e);
		if (!Focused) { return; }
		SetGain(_selectedBand, _gains[_selectedBand] + Math.Sign(e.Delta));
	}

	/// <summary>Arrow keys would otherwise be swallowed as navigation between sibling controls.</summary>
	protected override bool IsInputKey(Keys keyData) => keyData switch
	{
		Keys.Left or Keys.Right or Keys.Up or Keys.Down => true,
		_ => base.IsInputKey(keyData)
	};

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		switch (e.KeyCode)
		{
			case Keys.Left:
				_selectedBand = Math.Max(_selectedBand - 1, 0);
				break;
			case Keys.Right:
				_selectedBand = Math.Min(_selectedBand + 1, Equalizer.BandCount - 1);
				break;
			case Keys.Up:
				SetGain(_selectedBand, _gains[_selectedBand] + 1);
				break;
			case Keys.Down:
				SetGain(_selectedBand, _gains[_selectedBand] - 1);
				break;
			case Keys.PageUp:
				SetGain(_selectedBand, _gains[_selectedBand] + GridStepDb / 2);
				break;
			case Keys.PageDown:
				SetGain(_selectedBand, _gains[_selectedBand] - GridStepDb / 2);
				break;
			case Keys.Home:
				SetGain(_selectedBand, 0);
				break;
			default:
				return;
		}

		e.Handled = true;
		Invalidate();
	}

	protected override void OnGotFocus(EventArgs e) { base.OnGotFocus(e); Invalidate(); }

	protected override void OnLostFocus(EventArgs e) { base.OnLostFocus(e); Invalidate(); }

	protected override void OnEnabledChanged(EventArgs e) { base.OnEnabledChanged(e); Invalidate(); }

	protected override void OnPaint(PaintEventArgs e)
	{
		Graphics g = e.Graphics;
		g.SmoothingMode = SmoothingMode.AntiAlias;
		g.Clear(BackColor);

		Rectangle plot = PlotRect;
		Color curveColor = Enabled ? AccentColor : DisabledColor;
		Color textColor = Enabled ? DarkTheme.Foreground : DisabledColor;

		using (var plotBrush = new SolidBrush(PlotBackColor)) { g.FillRectangle(plotBrush, plot); }

		DrawGrid(g, plot);
		DrawCurve(g, plot, curveColor);
		DrawHandles(g, curveColor);
		DrawLabels(g, plot, textColor);

		if (Focused && Enabled)
		{
			using var focusPen = new Pen(AccentColor) { DashStyle = DashStyle.Dot };
			g.DrawRectangle(focusPen, 0, 0, Width - 1, Height - 1);
		}
	}

	private void DrawGrid(Graphics g, Rectangle plot)
	{
		using var gridPen = new Pen(GridColor);
		using var zeroPen = new Pen(ZeroColor);
		using var axisBrush = new SolidBrush(DarkTheme.Muted);
		using var format = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };

		for (int db = Equalizer.MinGainDb; db <= Equalizer.MaxGainDb; db += GridStepDb)
		{
			int y = GainY(db);
			g.DrawLine(db == 0 ? zeroPen : gridPen, plot.Left, y, plot.Right, y);
			g.DrawString(Describe(db), Font, axisBrush, new RectangleF(0, y - 9, PadLeft - 6, 18), format);
		}

		for (int i = 0; i < Equalizer.BandCount; i++)
		{
			int x = BandX(i);
			g.DrawLine(gridPen, x, plot.Top, x, plot.Bottom);
		}
	}

	private void DrawCurve(Graphics g, Rectangle plot, Color curveColor)
	{
		var points = new PointF[Equalizer.BandCount];
		for (int i = 0; i < points.Length; i++) { points[i] = new PointF(BandX(i), GainY(_gains[i])); }

		using var fill = new GraphicsPath();
		fill.AddCurve(points, 0.5f);
		fill.AddLine(points[^1].X, points[^1].Y, plot.Right, plot.Bottom);
		fill.AddLine(plot.Right, plot.Bottom, plot.Left, plot.Bottom);
		fill.CloseFigure();

		using (var fillBrush = new SolidBrush(Color.FromArgb(Enabled ? 56 : 24, curveColor))) { g.FillPath(fillBrush, fill); }
		using var curvePen = new Pen(curveColor, 2f);
		g.DrawCurve(curvePen, points, 0.5f);
	}

	private void DrawHandles(Graphics g, Color curveColor)
	{
		using var handleBrush = new SolidBrush(curveColor);
		using var edgePen = new Pen(PlotBackColor, 2f);
		using var selectedPen = new Pen(Enabled ? DarkTheme.Foreground : DisabledColor, 2f);

		for (int i = 0; i < Equalizer.BandCount; i++)
		{
			int radius = i == _selectedBand ? HandleRadius + 1 : HandleRadius;
			var box = new Rectangle(BandX(i) - radius, GainY(_gains[i]) - radius, radius * 2, radius * 2);
			g.FillEllipse(handleBrush, box);
			g.DrawEllipse(i == _selectedBand ? selectedPen : edgePen, box);
		}
	}

	private void DrawLabels(Graphics g, Rectangle plot, Color textColor)
	{
		using var labelBrush = new SolidBrush(Enabled ? DarkTheme.Muted : DisabledColor);
		using var readoutBrush = new SolidBrush(textColor);
		using var centered = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
		using var leading = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

		for (int i = 0; i < Equalizer.BandCount; i++)
		{
			var box = new RectangleF(BandX(i) - 28, plot.Bottom + 2, 56, PadBottom - 2);
			g.DrawString($"{Equalizer.BandLabels[i]} Hz", Font, labelBrush, box, centered);
		}

		string readout = $"{Equalizer.BandLabels[_selectedBand]} Hz   {Describe(_gains[_selectedBand])} dB";
		g.DrawString(readout, Font, readoutBrush, new RectangleF(PadLeft, 0, plot.Width, PadTop), leading);
	}

	private static string Describe(int db) => db > 0 ? $"+{db}" : db.ToString();

	protected override AccessibleObject CreateAccessibilityInstance() => new GraphAccessibleObject(this);

	/// <summary>Reports the focused band as the control's value so screen readers can track dragging.</summary>
	private sealed class GraphAccessibleObject(EqualizerGraph owner) : ControlAccessibleObject(owner)
	{
		public override AccessibleRole Role => AccessibleRole.Slider;

		public override string Value => $"{Equalizer.BandLabels[owner._selectedBand]} Hz, {Describe(owner._gains[owner._selectedBand])} dB";
	}
}
