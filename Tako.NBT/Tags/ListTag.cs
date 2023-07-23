using System.Buffers.Binary;
using Tako.NBT.Serialization;

namespace Tako.NBT.Tags;

/// <summary>
/// A list of nameless tags, all of the same type. 
/// </summary>
public class ListTag : Tag
{
    /// <summary>
    /// The tag values.
    /// </summary>
    public IList<Tag> Values { get; set; }

    /// <summary>
    /// The tag id of the values.
    /// </summary>
    public TagId ValueTagID { get; private set; }

    /// <summary>
    /// Creates a new list tag.
    /// </summary>
    /// <param name="name">The name.</param>
    public ListTag(string name)
        : base(TagId.List, name)
    {
        Values = new List<Tag>();
    }

    /// <inheritdoc/>
    public override object? GetValue()
    {
        return Values;
    }

    /// <inheritdoc/>
    internal override Tag Parse(NBTReader reader)
    {
        ValueTagID = reader.ReadTagId();
        var length = BinaryPrimitives.ReverseEndianness(reader.GetBinaryReader().ReadInt32());

        for (var i = 0; i < length; i++)
            Values.Add(reader.ReadSpecificTag(ValueTagID, string.Empty));

        return this;
    }

    /// <inheritdoc/>
    internal override void Serialize(NBTWriter writer)
    {
        var bw = writer.GetBinaryWriter();
        bw.Write((byte)ValueTagID);
        bw.Write(BinaryPrimitives.ReverseEndianness(Values.Count));

        foreach (var value in Values)
            value.Serialize(writer);
    }
}
