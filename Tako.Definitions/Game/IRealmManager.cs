using Tako.Definitions.Game.Players;

namespace Tako.Definitions.Game;

/// <summary>
/// A realm manager.
/// </summary>
public interface IRealmManager
{
	/// <summary>
	/// Gets or creates a realm with the given name.
	/// </summary>
	/// <param name="name">The realm name.</param>
	/// <returns>The realm.</returns>
	IRealm GetOrCreateRealm(string name);

	/// <summary>
	/// Gets a realm (but does not create one when one doesn't exist).
	/// </summary>
	/// <param name="name">The realm name.</param>
	/// <returns>The realm.</returns>
	IRealm? GetRealm(string name);

	/// <summary>
	/// Finds a player within all of the realms.
	/// </summary>
	/// <param name="playerId">The player id.</param>
	/// <returns>The player, if one exists.</returns>
	IPlayer? FindPlayerInRealms(sbyte playerId);

	/// <summary>
	/// Removes a realm with the given name.
	/// </summary>
	/// <param name="name">The realm name.</param>
	void RemoveRealm(string name);

	/// <summary>
	/// Gets the default realm.
	/// </summary>
	/// <returns>The realm.</returns>
	IRealm? GetDefaultRealm();

	/// <summary>
	/// Gets an enumerable of the realm names.
	/// </summary>
	/// <returns>The realm names.</returns>
	IEnumerable<string> GetRealmNames();

	/// <summary>
	/// Simulates all of the realms.
	/// </summary>
	void SimulateRealms();
}
