using Tako.Common.Logging;
using Tako.Common.Numerics;
using Tako.Definitions.Game.World;
using Tako.Definitions.Network.Connections;
using Tako.Server.Logging;

namespace Tako.Server.Game.World;

/// <summary>
/// A base for any world.
/// </summary>
public class BaseWorld : IWorld
{
	/// <summary>
	/// The world data.
	/// </summary>
	private byte[,,] _worldData;

	/// <summary>
	/// The logger.
	/// </summary>
	private ILogger<BaseWorld> _logger = LoggerFactory<BaseWorld>.Get();

	/// <summary>
	/// The base world data.
	/// </summary>
	/// <param name="dimensions">The dimensions of the world.</param>
	public BaseWorld(Vector3Int dimensions)
	{
		_worldData = new byte[dimensions.X, dimensions.Y, dimensions.Z];
	}

	/// <inheritdoc/>
	public byte GetBlock(Vector3Int pos)
	{
		return _worldData[pos.X, pos.Y, pos.Z];
	}

	/// <inheritdoc/>
	public void SetBlock(Vector3Int pos, byte block)
	{
		_worldData[pos.X, pos.Y, pos.Z] = block;
	}

	/// <inheritdoc/>
	public void Simulate()
	{

	}

	/// <inheritdoc/>
	public void StreamTo(IConnection conn)
	{
		_logger.Info($"We should now stream the world to {conn.ConnectionId}");
	}
}
