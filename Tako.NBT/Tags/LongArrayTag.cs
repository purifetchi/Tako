using Tako.NBT.Serialization;

namespace Tako.NBT.Tags;

/// <summary>
/// A length-prefixed array of signed longs.
/// </summary>
public class LongArrayTag : Tag
{
    /// <summary>
    /// The values.
    /// </summary>
    public long[] Values { get; set; }

    /// <summary>
    /// Creates a new long array tag.
    /// </summary>
    /// <param name="name">The name of the tag.</param>
    public LongArrayTag(string name)
        : base(TagId.IntArray, name)
    {
        Values = Array.Empty<long>();
    }

    /// <inheritdoc/>
    public override object? GetValue()
    {
        return Values;
    }

    /// <inheritdoc/>
    internal override Tag Parse(NBTReader reader)
    {
        var length = reader.ReadInt32();
        Values = new long[length];

        for (var i = 0; i < length; i++)
            Values[i] = reader.ReadInt64();

        return this;
    }

    /// <inheritdoc/>
    internal override void Serialize(NBTWriter writer)
    {
        writer.WriteInt32(Values.Length);
        foreach (var val in Values)
            writer.WriteInt64(val);
    }
}
