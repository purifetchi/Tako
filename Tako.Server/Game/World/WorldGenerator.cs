using Tako.Common.Logging;
using Tako.Common.Numerics;
using Tako.Definitions.Game;
using Tako.Definitions.Game.World;
using Tako.Definitions.Network;
using Tako.Server.Logging;

namespace Tako.Server.Game.World;

/// <summary>
/// A world generator.
/// </summary>
public class WorldGenerator : IWorldGenerator
{
	/// <summary>
	/// The realm we're building for.
	/// </summary>
	private readonly IRealm _worldRealm;

	/// <summary>
	/// The dimensions of the world.
	/// </summary>
	private Vector3Int _dimensions;

	/// <summary>
	/// The type of the world.
	/// </summary>
	private WorldType _type;

	/// <summary>
	/// The logger.
	/// </summary>
	private readonly ILogger<WorldGenerator> _logger = LoggerFactory<WorldGenerator>.Get();

	/// <summary>
	/// Constructs a new world generator for a realm.
	/// </summary>
	/// <param name="realm">The realm.</param>
	public WorldGenerator(IRealm realm)
	{
		_worldRealm = realm;
	}

	/// <inheritdoc/>
	public IWorldGenerator WithDimensions(Vector3Int dimensions)
	{
		_dimensions = dimensions;	
		return this;
	}

	/// <inheritdoc/>
	public IWorldGenerator WithType(WorldType type)
	{
		_type = type;
		return this;
	}

	/// <inheritdoc/>
	public void Build()
	{
		_logger.Info($"Generating world of type {_type} and dimensions {_dimensions}.");

		var baseWorld = new BaseWorld(_dimensions, _worldRealm);
		switch (_type)
		{
			case WorldType.Flat:
				BuildFlat(baseWorld);
				break;

			case WorldType.Classic:
				BuildClassic(baseWorld);
				break;

			default:
				throw new NotImplementedException();
		}

		_worldRealm.SetWorld(baseWorld);
	}

	/// <summary>
	/// Builds a flat world.
	/// </summary>
	/// <param name="world">The world.</param>
	private void BuildFlat(IWorld world)
	{
		for (var x = 0; x < _dimensions.X; x++)
		{
			for (var z = 0; z < _dimensions.Z; z++)
			{
				for (var y = 0; y < 11; y++)
				{
					var pos = new Vector3Int(x, y, z);
					if (y < 10)
						world.SetBlock(pos, (byte)ClassicBlockType.Dirt);
					else
						world.SetBlock(pos, (byte)ClassicBlockType.Grass);
				}
			}
		}
	}

	/// <summary>
	/// Builds a classic style world.
	/// </summary>
	/// <param name="world">The world.</param>
	private void BuildClassic(IWorld world)
	{

	}
}
