using Tako.Common.Numerics;

namespace Tako.Definitions.Game.World;

/// <summary>
/// A world generator.
/// </summary>
public interface IWorldGenerator
{
	/// <summary>
	/// Sets the dimensions of the world.
	/// </summary>
	/// <param name="dimensions">The dimensions.</param>
	IWorldGenerator WithDimensions(Vector3Int dimensions);

	/// <summary>
	/// Sets the type of the generator.
	/// </summary>
	IWorldGenerator WithType(WorldType type);

	/// <summary>
	/// Builds the world.
	/// </summary>
	void Build();
}
