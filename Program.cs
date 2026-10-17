namespace V9ProControl;

internal static class Program
{
	[STAThread]
	private static void Main()
	{
		ApplicationConfiguration.Initialize();

		// Dark title bar and system-drawn control parts. Still marked experimental in the
		// Windows Forms API, and it does not colour the controls themselves, so DarkTheme
		// handles those.
#pragma warning disable WFO5001
		Application.SetColorMode(SystemColorMode.Dark);
#pragma warning restore WFO5001

		Application.Run(new MainForm());
	}
}
