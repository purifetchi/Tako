using System.Buffers.Binary;

namespace Tako.NBT.Tags;

/// <summary>
/// A single, big endian IEEE-754 double-precision floating point number (NaN possible) 
/// </summary>
public class DoubleTag : Tag
{
    /// <summary>
    /// The value.
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Creates a new double tag.
    /// </summary>
    /// <param name="name">The name.</param>
    public DoubleTag(string name)
        : base(TagId.Double, name)
    {

    }

    /// <inheritdoc/>
    public override object? GetValue()
    {
        return Value;
    }

    /// <inheritdoc/>
    internal override Tag Parse(BinaryReader reader)
    {
        Value = reader.ReadDouble();
        return this;
    }
}
