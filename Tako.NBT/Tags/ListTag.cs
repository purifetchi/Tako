﻿using System.Buffers.Binary;

namespace Tako.NBT.Tags;

/// <summary>
/// A list of nameless tags, all of the same type. 
/// </summary>
public class ListTag : Tag
{
    /// <summary>
    /// The tag values.
    /// </summary>
    public IList<Tag> Values { get; }

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
    internal override Tag Parse(BinaryReader reader)
    {
        ValueTagID = (TagId)reader.ReadByte();
        var length = BinaryPrimitives.ReverseEndianness(reader.ReadInt32());

        for (var i = 0; i < length; i++)
            Values.Add(ParseSingleTag(ValueTagID, string.Empty, reader));

        return this;
    }
}
