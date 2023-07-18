using Tako.Definitions.Network.Connections;

namespace Tako.Definitions.Game.Players;

/// <summary>
/// A player.
/// </summary>
public interface IPlayer
{
	/// <summary>
	/// The player id.
	/// </summary>
	sbyte PlayerId { get; }

	/// <summary>
	/// This player's name.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// Is this player the op?
	/// </summary>
	bool Op { get; }

	/// <summary>
	/// The connection this player has.
	/// </summary>
	IConnection? Connection { get; }

	/// <summary>
	/// Sets the op status.
	/// </summary>
	/// <param name="op">The op status.</param>
	void SetOpStatus(bool op);
}
