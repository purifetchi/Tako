namespace Tako.Common.Numerics;

/// <summary>
/// A player rotation.
/// </summary>
public readonly struct Orientation
{
	/// <summary>
	/// The yaw.
	/// </summary>
	public byte Yaw { get; init; }

	/// <summary>
	/// The pitch.
	/// </summary>
	public byte Pitch { get; init; }

	/// <summary>
	/// A basic forward zero orientation.
	/// </summary>
	public static Orientation Zero { get; } = new Orientation(0, 0);

	/// <summary>
	/// Creates a new orientation from the given yaw and pitch.
	/// </summary>
	/// <param name="yaw">The yaw.</param>
	/// <param name="pitch">The pitch.</param>
	public Orientation(byte yaw, byte pitch)
	{
		Yaw = yaw;
		Pitch = pitch;
	}
}
