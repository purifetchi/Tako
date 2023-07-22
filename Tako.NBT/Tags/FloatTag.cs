using System.Buffers.Binary;

namespace Tako.NBT.Tags;

/// <summary>
/// A single, big endian IEEE-754 single-precision floating point number (NaN possible) 
/// </summary>
public class FloatTag : Tag
{
    /// <summary>
    /// The value.
    /// </summary>
    public float Value { get; set; }

    /// <summary>
    /// Creates a new float tag.
    /// </summary>
    /// <param name="name">The name.</param>
    public FloatTag(string name)
        : base(TagId.Float, name)
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
        Value = reader.ReadSingle();
        return this;
    }
}
