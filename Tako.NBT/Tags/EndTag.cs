﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tako.NBT.Serialization;

namespace Tako.NBT.Tags;

/// <summary>
/// The end tag. It does pretty much nothing other than signifying the end of a compound tag.
/// </summary>
public class EndTag : Tag
{
    /// <summary>
    /// Creates a new end tag.
    /// </summary>
    public EndTag() 
        : base(TagId.End, string.Empty)
    {
    }

    /// <inheritdoc/>
    public override object? GetValue()
    {
        return null;
    }

    /// <inheritdoc/>
    internal override Tag Parse(NBTReader _)
    {
        return this;
    }

    /// <inheritdoc/>
    internal override void Serialize(NBTWriter _)
    {
    }
}
