namespace Tako.Common.Numerics;

/// <summary>
/// Signed fixed-point, 5 fractional bits (-1024 to 1023.96875).
/// </summary>
public readonly struct FShort
{
	/// <summary>
	/// The divisor.
	/// </summary>
	private const float DIVISOR = 32f;

	/// <summary>
	/// The value.
	/// </summary>
	public short Value { get; init; }

	/// <summary>
	/// The minimum value.
	/// </summary>
	public static FShort MinValue { get; } = new(short.MinValue);

	/// <summary>
	/// The minimum value.
	/// </summary>
	public static FShort MaxValue { get; } = new(short.MinValue);

	/// <summary>
	/// Creates a new fshort from the value.
	/// </summary>
	/// <param name="value">The value.</param>
	public FShort(short value)
	{
		Value = value;
	}

	/// <summary>
	/// Converts an fshort into a float.
	/// </summary>
	/// <param name="value">The fshort value.</param>
	public static implicit operator float(FShort value) => value.Value / DIVISOR;

	/// <summary>
	/// Converts a float into an fshort.
	/// </summary>
	/// <param name="value">The float value.</param>
	public static implicit operator FShort(float value) => new FShort((short)(value * DIVISOR));
}
