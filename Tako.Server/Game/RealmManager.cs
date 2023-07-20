using Tako.Definitions.Game;
using Tako.Definitions.Game.Players;
using Tako.Definitions.Network;

namespace Tako.Server.Game;

/// <summary>
/// Manages all of the realms.
/// </summary>
public class RealmManager : IRealmManager
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
	/// Creates a new realm manager.
	/// </summary>
	/// <param name="server">The server instance.</param>
	public RealmManager(IServer server)
	{
		_server = server;
		_realms = new List<Realm>();
	}

	/// <inheritdoc/>
	public IRealm GetOrCreateRealm(string name)
	{
		var maybeRealm = _realms.FirstOrDefault(realm => realm.Name == name);
		if (maybeRealm is not null)
			return maybeRealm;

		var realm = new Realm(name, _realms.Count == 0, _server);
		_realms.Add(realm);

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
	}
}
