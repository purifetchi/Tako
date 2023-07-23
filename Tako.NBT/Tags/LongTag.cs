using System.Buffers.Binary;
using Tako.NBT.Serialization;

namespace Tako.NBT.Tags;

/// <summary>
/// A single signed, big endian 64 bit integer
/// </summary>
public class LongTag : Tag
{
    /// <summary>
    /// The value.
    /// </summary>
    public long Value { get; set; }

    /// <summary>
    /// Creates a new long tag.
    /// </summary>
    /// <param name="name">The name.</param>
    public LongTag(string name)
        : base(TagId.Long, name)
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
        Value = BinaryPrimitives.ReverseEndianness(reader.ReadInt64());
        return this;
    }

    /// <inheritdoc/>
    internal override void Serialize(NBTWriter writer)
    {
        writer.GetBinaryWriter()
            .Write(BinaryPrimitives.ReverseEndianness(Value));
    }
}
