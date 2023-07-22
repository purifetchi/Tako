using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace Tako.NBT.Tags;
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
    internal override Tag Parse(BinaryReader reader)
    {
        var length = BinaryPrimitives.ReverseEndianness(reader.ReadInt32());
        Values = new sbyte[length];

        var asBytes = MemoryMarshal.Cast<sbyte, byte>(Values);
        var readTotal = 0;
        do
        {
            readTotal += reader.Read(asBytes[readTotal..]);
        } while (readTotal != length);
        
        return this;
    }
}
