using System.Text.Json;
using System.Text.Json.Serialization;

namespace V9ProControl.Apo;

/// <summary>A named set of band gains in dB, ordered low frequency to high.</summary>
internal sealed record EqualizerProfile(string Name, int[] Gains)
{
	public override string ToString() => Name;
}

/// <summary>
/// Reads and writes equalizer profiles as JSON in a profiles folder beside the executable,
/// using the same shape MCHOSE HUB exports so files can be exchanged with it:
///
///   { "name": "xxx", "data": [ -4, -3, -2, -1, 0, 1, 2 ] }
///
/// Nothing about a file on disk is trusted. The band count is normalised to this equalizer's
/// ten, since a Hub export may carry a different number, values are clamped to the supported
/// range, and gains are read as numbers rather than integers because an export may spell them
/// with a decimal point.
/// </summary>
internal static class EqualizerProfiles
{
	public const string FileExtension = ".json";
	private const string DefaultProfileName = "Default";

	private static readonly JsonSerializerOptions WriteOptions = new() { WriteIndented = true };

	/// <summary>The profiles folder beside the executable, created on demand.</summary>
	public static string Directory => Path.Combine(AppContext.BaseDirectory, "profiles");

	/// <summary>
	/// Loads every readable profile, sorted by name. Unreadable files are skipped rather than
	/// failing the whole list, so one bad file cannot hide the rest.
	/// </summary>
	public static List<EqualizerProfile> LoadAll()
	{
		var profiles = new List<EqualizerProfile>();
		if (!System.IO.Directory.Exists(Directory)) { return profiles; }

		foreach (string path in System.IO.Directory.GetFiles(Directory, "*" + FileExtension))
		{
			EqualizerProfile? profile = TryLoad(path, out _);
			if (profile is not null) { profiles.Add(profile); }
		}

		profiles.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCultureIgnoreCase));
		return profiles;
	}

	/// <summary>Reads one profile, reporting why it could not be read rather than throwing.</summary>
	public static EqualizerProfile? TryLoad(string path, out string? error)
	{
		error = null;
		try
		{
			ProfileDocument? document = JsonSerializer.Deserialize<ProfileDocument>(File.ReadAllText(path));
			if (document?.Data is null)
			{
				error = "no \"data\" array in the file";
				return null;
			}

			string name = string.IsNullOrWhiteSpace(document.Name) ? Path.GetFileNameWithoutExtension(path) : document.Name.Trim();
			return new EqualizerProfile(name, Normalise(document.Data));
		}
		catch (Exception ex)
		{
			error = ex.Message;
			return null;
		}
	}

	public static void Save(EqualizerProfile profile, string path)
	{
		System.IO.Directory.CreateDirectory(Path.GetDirectoryName(path)!);
		var document = new ProfileDocument { Name = profile.Name, Data = Array.ConvertAll(profile.Gains, g => (double)g) };
		File.WriteAllText(path, JsonSerializer.Serialize(document, WriteOptions));
	}

	/// <summary>Path inside the profiles folder for a profile name, with unusable characters replaced.</summary>
	public static string PathFor(string name)
	{
		string safe = name;
		foreach (char invalid in Path.GetInvalidFileNameChars()) { safe = safe.Replace(invalid, '_'); }
		if (string.IsNullOrWhiteSpace(safe)) { safe = DefaultProfileName; }
		return Path.Combine(Directory, safe + FileExtension);
	}

	/// <summary>
	/// Creates a starting profile from the current gains when the folder holds none, so there
	/// is always something selectable on a first run.
	/// </summary>
	public static void EnsureDefault(int[] currentGains)
	{
		try
		{
			System.IO.Directory.CreateDirectory(Directory);
			if (System.IO.Directory.GetFiles(Directory, "*" + FileExtension).Length > 0) { return; }

			Save(new EqualizerProfile(DefaultProfileName, Normalise(Array.ConvertAll(currentGains, g => (double)g))), PathFor(DefaultProfileName));
		}
		catch (Exception)
		{
			// A read-only or inaccessible folder only costs the profile feature, so the rest of
			// the app carries on without it.
		}
	}

	/// <summary>Pads or truncates to the band count and clamps each gain to the supported range.</summary>
	private static int[] Normalise(double[] data)
	{
		var gains = new int[Equalizer.BandCount];
		for (int i = 0; i < gains.Length; i++)
		{
			if (i >= data.Length) { break; }
			gains[i] = Math.Clamp((int)Math.Round(data[i]), Equalizer.MinGainDb, Equalizer.MaxGainDb);
		}
		return gains;
	}

	private sealed class ProfileDocument
	{
		[JsonPropertyName("name")]
		public string? Name { get; set; }

		[JsonPropertyName("data")]
		public double[]? Data { get; set; }
	}
}
