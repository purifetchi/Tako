using System.Buffers.Binary;
using System.Runtime.InteropServices;
using Tako.NBT.Serialization;

namespace Tako.NBT.Tags;

/// <summary>
/// A length-prefixed array of signed bytes. 
/// </summary>
public class ByteArrayTag : Tag
{
    /// <summary>
    /// The values.
    /// </summary>
    public sbyte[] Values { get; set; }

    /// <summary>
    /// Creates a new byte array tag.
    /// </summary>
    /// <param name="name">The name.</param>
    public ByteArrayTag(string name)
        : base(TagId.ByteArray, name)
    {
        Values = Array.Empty<sbyte>();
    }

    /// <inheritdoc/>
    public override object? GetValue()
    {
        return Values;
    }

    /// <inheritdoc/>
    internal override Tag Parse(NBTReader reader)
    {
        var length = BinaryPrimitives.ReverseEndianness(reader.GetBinaryReader().ReadInt32());
        Values = new sbyte[length];

        reader.ReadBytes(MemoryMarshal.Cast<sbyte, byte>(Values));
        return this;
    }

    /// <inheritdoc/>
    internal override void Serialize(NBTWriter writer)
    {
        var bw = writer.GetBinaryWriter();
        bw.Write(BinaryPrimitives.ReverseEndianness(Values.Length));

        var asBytes = MemoryMarshal.Cast<sbyte, byte>(Values);
        bw.Write(asBytes);
    }
}
