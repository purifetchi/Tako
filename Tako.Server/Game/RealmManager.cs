using Tako.Definitions.Game;
using Tako.Definitions.Game.Players;
using Tako.Definitions.Network;

namespace Tako.Server.Game;

/// <summary>
/// Manages all of the realms.
/// </summary>
public partial class RealmManager : IRealmManager
{
	/// <summary>
	/// The realms list.
	/// </summary>
	private readonly List<Realm> _realms;

	/// <summary>
	/// The server.
	/// </summary>
	private readonly IServer _server;

	/// <summary>
	/// The save directory for realms.
	/// </summary>
	private readonly string _saveDirectory;

	/// <summary>
	/// Is autosave enabled?
	/// </summary>
	private readonly bool _autosaveEnabled;

	/// <summary>
	/// The autosave delay.
	/// </summary>
	private readonly int _autosaveDelay;

    /// <summary>
    /// Last autosave.
    /// </summary>
    private long _lastAutosave;

    /// <summary>
    /// Creates a new realm manager.
    /// </summary>
    /// <param name="server">The server instance.</param>
    public RealmManager(IServer server)
	{
		_server = server;
		_realms = new List<Realm>();

		_saveDirectory = server.Settings.Get("realms-directory") ?? "maps";
		_autosaveEnabled = bool.Parse(server.Settings.Get("autosave") ?? "true");
		_autosaveDelay = int.Parse(server.Settings.Get("autosave-delay") ?? "300");

		_lastAutosave = DateTimeOffset.Now.ToUnixTimeSeconds();

		LoadRealms();
    }

	/// <inheritdoc/>
	public IRealm GetOrCreateRealm(
		string name,
        RealmCreationOptions opts = RealmCreationOptions.AutosaveEnabled | RealmCreationOptions.LoadFromFileOnCreation)
	{
		var maybeRealm = _realms.FirstOrDefault(realm => realm.Name == name);
		if (maybeRealm is not null)
			return maybeRealm;

		var realm = new Realm(name, _realms.Count == 0, _server);
		_realms.Add(realm);

		realm.AutoSave = opts.HasFlag(RealmCreationOptions.AutosaveEnabled);

		if (opts.HasFlag(RealmCreationOptions.LoadFromFileOnCreation))
			LoadRealmWorld(realm);

        return realm;
	}

	/// <inheritdoc/>
	public IRealm? GetRealm(string name)
	{
		return _realms.FirstOrDefault(realm => realm.Name == name);
	}

	/// <inheritdoc/>
	public IPlayer? FindPlayerInRealms(sbyte playerId)
	{
		// TODO(pref): Optimize this by caching all the players in here.
		return _realms.FirstOrDefault(realm => realm.Players.ContainsKey(playerId))?
			.Players[playerId];
	}

	/// <inheritdoc/>
	public IEnumerable<string> GetRealmNames()
	{
		return _realms.Select(realm => realm.Name);
	}

	/// <inheritdoc/>
	public void RemoveRealm(string name)
	{
		var realm = _realms.Find(realm => realm.Name == name);
		if (realm is not null)
			_realms.Remove(realm);
	}

	/// <inheritdoc/>
	public IRealm? GetDefaultRealm()
	{
		return _realms.FirstOrDefault(realm => realm.IsPrimaryRealm);
	}

	/// <inheritdoc/>
	public void SimulateRealms()
	{
		foreach (var realm in _realms)
		{
			// We don't need to simulate realms without players.
			// Not now, at least.
			if (realm.Players.Count < 1)
				continue;

			realm.HeartbeatPlayers();
			realm.World?.Simulate();
		}

		if (_autosaveEnabled &&
			DateTimeOffset.Now.ToUnixTimeSeconds() - _lastAutosave >= _autosaveDelay)
		{
			AutosaveRealms();
            _lastAutosave = DateTimeOffset.Now.ToUnixTimeSeconds();
        }
	}

	/// <summary>
	/// Performs an autosave on all the realms.
	/// </summary>
	private void AutosaveRealms()
	{
		foreach (var realm in _realms)
		{
			if (!realm.AutoSave)
				continue;

			// TODO(pref): Do not save realms that haven't had any activity for a prolonged period.
			//			   In reality, we should probably put them into some sort of hibernation state
			//			   where they unload their world and stuff, but that's for a later period.
			SaveRealmWorld(realm);
        }

		_server.Chat.SendServerMessage("Realms autosave complete.");
	}
}
