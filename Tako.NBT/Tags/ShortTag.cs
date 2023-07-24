using System.Buffers.Binary;
using Tako.NBT.Serialization;

namespace Tako.NBT.Tags;

/// <summary>
/// A single signed, big endian 16 bit integer.
/// </summary>
public class ShortTag : Tag
{
    /// <summary>
    /// The value.
    /// </summary>
    public short Value { get; set; }

    /// <summary>
    /// Creates a new short tag.
    /// </summary>
    /// <param name="name">The name.</param>
    public ShortTag(string name)
        : base(TagId.Short, name)
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
        Value = reader.ReadInt16();
        return this;
    }

    /// <inheritdoc/>
    internal override void Serialize(NBTWriter writer)
    {
        writer.WriteInt16(Value);
    }
}
