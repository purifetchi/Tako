using System.Numerics;
using Tako.Common.Numerics;
using Tako.Definitions.Network.Connections;

namespace Tako.Definitions.Game.World;

/// <summary>
/// An interface for anything that's a world.
/// </summary>
public interface IWorld
{
	/// <summary>
	/// The server this world belongs to.
	/// </summary>
	IRealm Realm { get; }

    /// <summary>
    /// The spawn point for the world.
    /// </summary>
    Vector3 SpawnPoint { get; set; }

    /// <summary>
    /// Gets the block given its xyz coords.
    /// </summary>
    /// <param name="pos">The position of the block.</param>
    byte GetBlock(Vector3Int pos);

    /// <summary>
    /// Sets the block given its xyz coords and the block type.
    /// </summary>
    /// <param name="pos">The position.</param>
    /// <param name="block">The block type.</param>
    void SetBlock(Vector3Int pos, byte block);

    /// <summary>
    /// Simulates the world.
    /// </summary>
    void Simulate();

    /// <summary>
    /// Streams the world data to this connection.
    /// </summary>
    /// <param name="conn">The connection.</param>
    void StreamTo(IConnection conn);
}
