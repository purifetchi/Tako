using System.Numerics;
using Tako.Common.Logging;
using Tako.Common.Numerics;
using Tako.Definitions.Game;
using Tako.Definitions.Game.World;
using Tako.Definitions.Network;
using Tako.NBT.Serialization;
using Tako.NBT.Tags;
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
	/// The filename.
	/// </summary>
	private string? _filename;

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
	public IWorldGenerator WithFilename(string filename)
	{
		_filename = filename;
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

			case WorldType.LoadFromFile:
				LoadFromFile(baseWorld, _filename!);
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
		const float playerHeight = 2f;

		var halfY = _dimensions.Y / 2;

		for (var x = 0; x < _dimensions.X; x++)
		{
			for (var z = 0; z < _dimensions.Z; z++)
			{
				for (var y = 0; y < halfY + 1; y++)
				{
					var pos = new Vector3Int(x, y, z);
					if (y < halfY)
						world.SetBlock(pos, (byte)ClassicBlockType.Dirt);
					else
						world.SetBlock(pos, (byte)ClassicBlockType.Grass);
				}
			}
		}

		world.SpawnPoint = new Vector3(
			_dimensions.X / 2f, 
			_dimensions.Y / 2f + playerHeight, 
			_dimensions.Z / 2f);
	}

	/// <summary>
	/// Builds a classic style world.
	/// </summary>
	/// <param name="world">The world.</param>
	private void BuildClassic(IWorld world)
	{

	}

	/// <summary>
	/// Loads a world from a ClassicWorld NBT file.
	/// </summary>
	/// <param name="world">The world.</param>
	/// <param name="filename">The filename.</param>
	private void LoadFromFile(BaseWorld world, string filename)
	{
		using var reader = new NBTReader(filename);
		var tag = (CompoundTag)reader.Read();
		world.LoadDataFromNBT(tag);
	}
}
