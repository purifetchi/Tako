using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Tako.Definitions.Game.Players;
using Tako.Definitions.Network;
using Tako.Definitions.Network.Connections;
using Tako.Server.Network.Packets.Server;

namespace Tako.Server.Game.Players;

/// <summary>
/// A player.
/// </summary>
public class Player : IPlayer
{
	/// <summary>
	/// The marker used for players that are ourselves.
	/// </summary>
	public const sbyte SELF_PLAYER_MARKER = -1;

	/// <inheritdoc/>
	public sbyte PlayerId { get; init; }

	/// <inheritdoc/>
	public string Name { get; init; } = null!;

	/// <inheritdoc/>
	public bool Op { get; set; }

	/// <inheritdoc/>
	public Vector3 Position { get; private set; } = Vector3.Zero;

	/// <inheritdoc/>
	public IConnection? Connection { get; init; }

	/// <inheritdoc/>
	public IServer Server { get; init; }

	/// <summary>
	/// Constructs a new player.
	/// </summary>
	/// <param name="playerId">The player id of said player.</param>
	/// <param name="name">Their name.</param>
	/// <param name="op">Their op status.</param>
	/// <param name="connection">Their connection, if any.</param>
	public Player(
		sbyte playerId, 
		[NotNull] string name, 
		bool op, 
		IConnection? connection,
		IServer server)
	{
		PlayerId = playerId;
		Name = name;
		Op = op;
		Connection = connection;
		Server = server;
	}

	/// <inheritdoc/>
	public void SetOpStatus(bool op)
	{
		Op = op;
	}

	/// <inheritdoc/>
	public void SetPosition(Vector3 position)
	{
		// If the positions are roughly similar, don't do anything.
		if (Vector3.DistanceSquared(Position, position) < float.Epsilon)
			return;

		Position = position;
		Server.NetworkManager.SendToAllThatMatch(new SetPositionAndOrientationPacket
		{
			PlayerId = PlayerId,
			X = position.X,
			Y = position.Y,
			Z = position.Z,
			Yaw = 0,
			Pitch = 0
		}, conn => conn != Connection);
	}

	/// <inheritdoc/>
	public void Spawn(Vector3 position)
	{
		// First send the packet to the player that just joined.
		Connection?.Send(new SpawnPlayerPacket
		{
			PlayerId = SELF_PLAYER_MARKER,
			PlayerName = Name,
			X = position.X,
			Y = position.Y,
			Z = position.Z,
			Pitch = 0,
			Yaw = 0
		});

		// Then to everyone else.
		Server.NetworkManager.SendToAllThatMatch(new SpawnPlayerPacket
		{
			PlayerId = PlayerId,
			PlayerName = Name,
			X = position.X,
			Y = position.Y,
			Z = position.Z,
			Pitch = 0,
			Yaw = 0
		}, conn => conn != Connection);
	}
}
