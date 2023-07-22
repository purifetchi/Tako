﻿using System.Buffers.Binary;

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
    internal override Tag Parse(BinaryReader reader)
    {
        Value = BinaryPrimitives.ReverseEndianness(reader.ReadInt16());
        return this;
    }
}
