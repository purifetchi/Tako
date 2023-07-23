using System.Buffers.Binary;
using Tako.NBT.Serialization;

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
    internal override Tag Parse(NBTReader reader)
    {
        Value = reader.GetBinaryReader()
            .ReadDouble();
        return this;
    }

    /// <inheritdoc/>
    internal override void Serialize(NBTWriter writer)
    {
        writer.GetBinaryWriter()
            .Write(Value);
    }
}
