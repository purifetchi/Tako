﻿using System.Buffers.Binary;

namespace Tako.NBT.Tags;

/// <summary>
/// A single signed, big endian 32 bit integer 
/// </summary>
public class IntTag : Tag
{
    /// <summary>
    /// The value.
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Creates a new int tag.
    /// </summary>
    /// <param name="name">The name.</param>
    public IntTag(string name)
        : base(TagId.Int, name)
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
        Value = BinaryPrimitives.ReverseEndianness(reader.ReadInt32());
        return this;
    }
}
