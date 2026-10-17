using System.Diagnostics;

namespace V9ProControl.Device;

internal sealed record DriverPackage(string Directory, string BaseInf, string? ExtensionInf, string? ApoInf);

/// <summary>
/// Detects and installs the C-Media CM2025 audio driver the headset needs.
///
/// The headset ships under more than one driver package: a machine may be running
/// mchose_v9_pro.inf or the generic C-Media blk2025ghs.inf, and both drive the same hardware.
/// Checking only for the MCHOSE-branded package would report the driver as missing on a
/// perfectly working machine and then replace it, so any known variant counts as installed.
///
/// Presence is read from the driver store's directory names, which are the INF file name plus
/// an architecture and hash suffix. That is locale independent, unlike the labelled output of
/// pnputil, whose field names are translated.
/// </summary>
internal static class AudioDriver
{
	private const string DriverStore = @"C:\Windows\System32\DriverStore\FileRepository";

	private static readonly string[] KnownBaseInfs = { "mchose_v9_pro.inf", "blk2025ghs.inf", "mchose_s9_pro.inf" };

	private static readonly string[] PackageSearchPaths =
	{
		@"drivers\V9_PRO",
		@"..\..\..\drivers\V9_PRO",
		@"C:\Program Files\MCHOSE HUB\libs\V9_PRO"
	};

	/// <summary>Name of the installed driver package, or null when none of the variants is present.</summary>
	public static string? FindInstalled()
	{
		if (!System.IO.Directory.Exists(DriverStore)) { return null; }

		foreach (string inf in KnownBaseInfs)
		{
			try
			{
				string? match = System.IO.Directory.EnumerateDirectories(DriverStore, inf + "_*").FirstOrDefault();
				if (match is not null) { return System.IO.Path.GetFileName(match); }
			}
			catch (UnauthorizedAccessException)
			{
				return null;
			}
		}

		return null;
	}

	/// <summary>
	/// Locates a driver package to install from. drivers/ is not distributed with this project,
	/// so an installed MCHOSE HUB is used as the fallback source.
	/// </summary>
	public static DriverPackage? FindPackage()
	{
		foreach (string candidate in PackageSearchPaths)
		{
			string directory = System.IO.Path.IsPathRooted(candidate) ? candidate : System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory, candidate));
			if (!System.IO.Directory.Exists(directory)) { continue; }

			string[] infs = System.IO.Directory.GetFiles(directory, "*.inf", SearchOption.AllDirectories);
			string? extension = infs.FirstOrDefault(p => p.Contains("01_EXTENSION", StringComparison.OrdinalIgnoreCase));
			string? apo = infs.FirstOrDefault(p => p.Contains("02_APO", StringComparison.OrdinalIgnoreCase));
			string? baseInf = infs.FirstOrDefault(p => p != extension && p != apo);

			if (baseInf is not null) { return new DriverPackage(directory, baseInf, extension, apo); }
		}

		return null;
	}

	/// <summary>
	/// Installs the package with pnputil, base driver first so the extension and APO have
	/// something to attach to. Requires elevation, which this app already runs with.
	/// </summary>
	public static (bool Installed, bool RebootRequired, string Message) Install(DriverPackage package)
	{
		bool rebootRequired = false;

		foreach (string? inf in new string?[] { package.BaseInf, package.ExtensionInf, package.ApoInf })
		{
			if (inf is null) { continue; }

			(int exitCode, string output) = RunPnpUtil(inf);
			if (exitCode == 3010) { rebootRequired = true; continue; }
			if (exitCode != 0) { return (false, false, $"{System.IO.Path.GetFileName(inf)} failed (pnputil {exitCode}): {output}"); }
		}

		return (true, rebootRequired, "Driver installed.");
	}

	private static (int ExitCode, string Output) RunPnpUtil(string infPath)
	{
		var info = new ProcessStartInfo("pnputil.exe")
		{
			UseShellExecute = false,
			CreateNoWindow = true,
			RedirectStandardOutput = true,
			RedirectStandardError = true
		};
		info.ArgumentList.Add("/add-driver");
		info.ArgumentList.Add(infPath);
		info.ArgumentList.Add("/install");

		using Process? process = Process.Start(info);
		if (process is null) { return (-1, "could not start pnputil"); }

		string output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
		process.WaitForExit(120_000);
		return (process.ExitCode, output.Trim().Replace(Environment.NewLine, " "));
	}
}
