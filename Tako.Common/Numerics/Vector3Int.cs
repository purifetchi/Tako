namespace Tako.Common.Numerics;

/// <summary>
/// A vector3 of integers.
/// </summary>
public struct Vector3Int
{
	public int X, Y, Z;

	/// <summary>
	/// Constructs a new vector 3 with the given values.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	public Vector3Int(int x, int y, int z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	/// <inheritdoc/>
	public override string ToString()
	{
		return $"[{X}, {Y}, {Z}]";
	}
}
