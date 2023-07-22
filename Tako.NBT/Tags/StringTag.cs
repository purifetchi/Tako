using System.Buffers.Binary;
using System.Text;

namespace Tako.NBT.Tags;

/// <summary>
/// A length-prefixed modified UTF-8 string.
/// </summary>
public class StringTag : Tag
{
	/// <summary>
	/// The string value.
	/// </summary>
	public string? Value { get; private set; }

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
	internal override Tag Parse(BinaryReader reader)
	{
		var length = BinaryPrimitives.ReverseEndianness(reader.ReadUInt16());
		var data = reader.ReadBytes(length);
		Value = Encoding.UTF8.GetString(data);
		return this;
	}
}
