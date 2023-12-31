﻿using System.Buffers.Binary;
using System.Text;
using Tako.NBT.Serialization;

namespace Tako.NBT.Tags;

/// <summary>
/// A length-prefixed modified UTF-8 string.
/// </summary>
public class StringTag : Tag
{
	/// <summary>
	/// The string value.
	/// </summary>
	public string? Value { get; set; }

	/// <summary>
	/// Creates a new string tag.
	/// </summary>
	/// <param name="name">The name.</param>
	public StringTag(string name)
		: base(TagId.String, name)
	{

	}

	/// <inheritdocr/>
	public override object? GetValue()
	{
		return Value;
	}

	/// <inheritdoc/>
	internal override Tag Parse(NBTReader reader)
	{
		Value = reader.ReadString();
		return this;
	}

	/// <inheritdoc/>
    internal override void Serialize(NBTWriter writer)
    {
		writer.WriteString(Value!);
    }
}
