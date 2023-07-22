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
	/// Sets the filename for this world. (Or the filename for the world to load)
	/// </summary>
	/// <param name="filename">The filename</param>
	IWorldGenerator WithFilename(string filename);

	/// <summary>
	/// Builds the world.
	/// </summary>
	void Build();
}
