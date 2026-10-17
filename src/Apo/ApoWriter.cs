namespace V9ProControl.Apo;

internal enum ApoApplyMethod
{
	/// <summary>Values are persisted but the running APO only picks them up after a replug.</summary>
	RegistryOnly,

	/// <summary>Applied through the vendor library, which is version-matched to this driver.</summary>
	OsConfLib,

	/// <summary>Applied through the audio service's policy interface, with no vendor library.</summary>
	PolicyConfig
}

/// <summary>
/// Decides how effect changes reach the running APO and performs the write.
///
/// The vendor library is preferred when present because it ships alongside the driver and is
/// matched to it. The policy interface is the fallback for machines that never installed MCHOSE
/// HUB, and because it is undocumented it is proved before being trusted: one value is written
/// back with the value it already holds, then re-read. That catches both a write that silently
/// does nothing and one that lands as REG_BINARY because the variant was not understood. A
/// failed probe is repaired and the writer falls back to plain registry writes.
/// </summary>
internal sealed class ApoWriter : IDisposable
{
	private const int ProbeTimeoutMs = 500;
	private const int ProbePollMs = 50;

	private readonly EndpointFxProperties _fx;
	private readonly OsConfLib? _library;
	private PolicyConfigApo? _policy;
	private bool _disposed;

	public ApoApplyMethod Method { get; private set; }

	/// <summary>Why the vendor library was unavailable, when it was not used.</summary>
	public OsConfLibStatus LibraryStatus { get; }

	public ApoWriter(EndpointFxProperties fx)
	{
		_fx = fx;

		_library = OsConfLib.TryOpen(out OsConfLibStatus status);
		LibraryStatus = status;
		if (_library is not null)
		{
			Method = ApoApplyMethod.OsConfLib;
			return;
		}

		_policy = PolicyConfigApo.TryOpen(fx.EndpointId);
		if (_policy is not null && ProbePolicyInterface())
		{
			Method = ApoApplyMethod.PolicyConfig;
			return;
		}

		_policy?.Dispose();
		_policy = null;
		Method = ApoApplyMethod.RegistryOnly;
	}

	/// <summary>True when changes reach the running APO rather than only being persisted.</summary>
	public bool AppliesLive => Method != ApoApplyMethod.RegistryOnly;

	/// <summary>
	/// Writes one effect value. <paramref name="vendorName"/> and <paramref name="type"/> are
	/// only used by the vendor library, which addresses properties by name rather than key.
	/// </summary>
	public void Write(Guid propertySet, int index, int value, string vendorName, ApoPropertyType type)
	{
		switch (Method)
		{
			case ApoApplyMethod.OsConfLib when _library is not null:
				if (_library.TrySet(vendorName, value, type)) { return; }
				break;

			case ApoApplyMethod.PolicyConfig when _policy is not null:
				if (_policy.TrySet(propertySet, index, value)) { return; }
				break;
		}

		_fx.WriteInt32(propertySet, index, value);
	}

	/// <summary>
	/// Writes a value back over itself and confirms the store still holds that exact DWORD.
	/// Harmless by construction: a working write is a no-op, and a broken one is repaired here.
	/// </summary>
	private bool ProbePolicyInterface()
	{
		if (_policy is null) { return false; }

		(Guid Set, int Index)? candidate = _fx.FirstDwordValue();
		if (candidate is null) { return false; }

		(Guid set, int index) = candidate.Value;
		int? original = _fx.ReadInt32(set, index);
		if (original is null) { return false; }

		if (!_policy.TrySet(set, index, original.Value)) { return false; }

		// The audio service applies the change asynchronously, so give it a moment to land.
		for (int waited = 0; waited < ProbeTimeoutMs; waited += ProbePollMs)
		{
			if (_fx.IsDwordValue(set, index) && _fx.ReadInt32(set, index) == original.Value) { return true; }
			Thread.Sleep(ProbePollMs);
		}

		RepairProbedValue(set, index, original.Value);
		return false;
	}

	private void RepairProbedValue(Guid set, int index, int original)
	{
		try
		{
			_fx.WriteInt32(set, index, original);
		}
		catch (Exception)
		{
			// Nothing further can be done here; the caller falls back to registry writes and
			// the next user change rewrites this value anyway.
		}
	}

	public void Dispose()
	{
		if (_disposed) { return; }
		_disposed = true;
		_library?.Dispose();
		_policy?.Dispose();
	}
}
