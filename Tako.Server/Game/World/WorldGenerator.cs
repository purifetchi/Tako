using System.Numerics;
using Tako.Common.Logging;
using Tako.Common.Numerics;
using Tako.Definitions.Game.World;
using Tako.Server.Logging;

namespace Tako.Server.Game.World;

/// <summary>
/// A world generator.
/// </summary>
public class WorldGenerator
{
	/// <summary>
	/// The type of the world.
	/// </summary>
	public enum Type
	{
		Flat
	}

	/// <summary>
	/// The dimensions of the world.
	/// </summary>
	private Vector3Int _dimensions;

	/// <summary>
	/// The type of the world.
	/// </summary>
	private Type _type;

	/// <summary>
	/// The logger.
	/// </summary>
	private readonly ILogger<WorldGenerator> _logger = LoggerFactory<WorldGenerator>.Get();

	/// <summary>
	/// Sets the dimensions for this world.
	/// </summary>
	/// <param name="dimensions">The dimensions.</param>
	public WorldGenerator WithDimensions(Vector3Int dimensions)
	{
		_dimensions = dimensions;	
		return this;
	}

	/// <summary>
	/// Sets the type for this world.
	/// </summary>
	/// <param name="type">The type.</param>
	public WorldGenerator WithType(Type type)
	{
		_type = type;
		return this;
	}

	/// <summary>
	/// Builds the world.
	/// </summary>
	/// <returns>The built world.</returns>
	public IWorld Build()
	{
		_logger.Info($"Generating world of type {_type} and dimensions {_dimensions}.");

		var baseWorld = new BaseWorld(_dimensions);
		switch (_type)
		{
			case Type.Flat:
				BuildFlat(baseWorld);
				break;

			default:
				throw new NotImplementedException();
		}

		return baseWorld;
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
				for (var y = 0; y < _dimensions.Y / 2; y++)
				{
					world.SetBlock(new Vector3Int(x, y, z), (byte)Random.Shared.Next(1, 10));
				}
			}
		}
	}
}
