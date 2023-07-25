using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Tako.Common.Numerics;
using Tako.Definitions.Game;
using Tako.Definitions.Game.Players;
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
    public Orientation Orientation { get; private set; }

    /// <inheritdoc/>
    public IConnection? Connection { get; init; }

    /// <inheritdoc/>
    public IRealm Realm { get; private set; }

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
        IRealm realm)
    {
        PlayerId = playerId;
        Name = name;
        Op = op;
        Connection = connection;
        Realm = realm;
    }

    /// <inheritdoc/>
    public void SetOpStatus(bool op)
    {
        Op = op;

        Connection?.Send(new UpdatePlayerTypePacket
        {
            Type = Op ? PlayerType.Op : PlayerType.Regular
        });
    }

    /// <inheritdoc/>
    public void SetPositionAndOrientation(Vector3 position, Orientation orientation)
    {
        Position = position;
        Orientation = orientation;

        Realm.SendToAllWithinRealmThatMatch(new SetPositionAndOrientationPacket
        {
            PlayerId = PlayerId,
            X = position.X,
            Y = position.Y,
            Z = position.Z,
            Yaw = Orientation.Yaw,
            Pitch = Orientation.Pitch
        }, conn => conn != Connection);
    }

    /// <inheritdoc/>
    public void Spawn(Vector3 position)
    {
        Position = position;

        // First send the packet to the player that just joined.
        Connection?.Send(new SpawnPlayerPacket
        {
            PlayerId = SELF_PLAYER_MARKER,
            PlayerName = Name,
            X = Position.X,
            Y = Position.Y,
            Z = Position.Z,
            Pitch = Orientation.Pitch,
            Yaw = Orientation.Yaw
        });

        // Then to everyone else.
        Realm.SendToAllWithinRealmThatMatch(new SpawnPlayerPacket
        {
            PlayerId = PlayerId,
            PlayerName = Name,
            X = Position.X,
            Y = Position.Y,
            Z = Position.Z,
            Pitch = Orientation.Pitch,
            Yaw = Orientation.Yaw
        }, conn => conn != Connection);

        Realm.Server.Chat.SendServerMessageTo(Realm, $"{Name} has joined realm {Realm.Name}!");
    }

    /// <inheritdoc/>
    public void Disconnect(string? reason = null)
    {
        if (reason is null)
            Realm.Server.Chat.SendServerMessageTo(Realm, $"{Name} has left!");
        else
            Realm.Server.Chat.SendServerMessageTo(Realm, $"{Name} has left because {reason}");

        Realm.SendToAllWithinRealmThatMatch(new DespawnPlayerPacket
        {
            PlayerId = PlayerId
        }, conn => conn != Connection);

        Connection?.Send(new DisconnectPlayerPacket
        {
            DisconnectReason = reason ?? ""
        });
        Connection?.Disconnect();
    }

    /// <inheritdoc/>
    public void Ping()
    {
        Connection?.Send(new PingMessage());
    }

    /// <inheritdoc/>
    public void SetRealm(IRealm realm)
    {
        Realm = realm;
    }
}
