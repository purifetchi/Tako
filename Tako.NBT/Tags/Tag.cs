using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;

namespace Tako.NBT.Tags;

/// <summary>
/// An NBT tag.
/// </summary>
public abstract class Tag
{
    /// <summary>
    /// The ID of the tag.
    /// </summary>
    public TagId Id { get; init; }

    /// <summary>
    /// The name of the tag.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Parses a tag from the binary reader.
    /// </summary>
    /// <param name="reader">The binary reader.</param>
    internal abstract Tag Parse(BinaryReader reader);

    /// <summary>
    /// Creates a new NBT tag.
    /// </summary>
    /// <param name="tagId">The tag id.</param>
    /// <param name="name">The name.</param>
    public Tag(TagId tagId, string name)
    {
        Id = tagId;
        Name = name;
    }

    /// <summary>
    /// Gets the tag's value (as a C# object).
    /// </summary>
    /// <returns>The tag's value.</returns>
    public abstract object? GetValue();

    /// <summary>
    /// Reads a tag stream from a file.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns>The outermost tag.</returns>
    public static Tag FromFile(string path)
    {
        const ushort gzipMagic = 0x8B1F;
        const int magicLength = 2;

        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

        var magicBuffer = new byte[magicLength];
        fs.Read(magicBuffer, 0, magicLength);
        fs.Seek(0, SeekOrigin.Begin);

        var magic = BitConverter.ToUInt16(magicBuffer, 0);

        Stream sourceStream = fs;
        if (gzipMagic == magic)
            sourceStream = new GZipStream(fs, CompressionMode.Decompress);

        using var br = new BinaryReader(sourceStream);
        return ReadTag(br);
    }

    /// <summary>
    /// Reads a single NBT Tag from a binary reader.
    /// </summary>
    /// <returns>The NBT Tag.</returns>
    internal unsafe static Tag ReadTag(BinaryReader reader)
    {
        var tagId = (TagId)reader.ReadByte();
        var name = string.Empty;
        if (tagId != TagId.End)
        {
            var nameLength = BinaryPrimitives.ReverseEndianness(reader.ReadInt16());
            var nameBuffer = stackalloc byte[nameLength];
            var nameSpan = new Span<byte>(nameBuffer, nameLength);
            reader.Read(nameSpan);

            name = Encoding.UTF8.GetString(nameSpan);
        }

        return ParseSingleTag(tagId, name, reader);
    }

    /// <summary>
    /// Parses a single tag.
    /// </summary>
    /// <param name="tagId">The tag id.</param>
    /// <param name="name">The name.</param>
    /// <param name="reader">The reader to read from.</param>
    /// <returns>The tag.</returns>
    internal static Tag ParseSingleTag(TagId tagId, string name, BinaryReader reader)
    {
        return tagId switch
        {
            TagId.End => new EndTag(),
            TagId.Byte => new ByteTag(name).Parse(reader),
            TagId.Short => new ShortTag(name).Parse(reader),
            TagId.Int => new IntTag(name).Parse(reader),
            TagId.Long => new LongTag(name).Parse(reader),
            TagId.Float => new FloatTag(name).Parse(reader),
            TagId.Double => new DoubleTag(name).Parse(reader),
            TagId.ByteArray => new ByteArrayTag(name).Parse(reader),
            TagId.String => new StringTag(name).Parse(reader),
            TagId.List => new ListTag(name).Parse(reader),
            TagId.Compound => new CompoundTag(name).Parse(reader),
            _ => throw new NotSupportedException($"Unknown tag id detected! {tagId}"),
        };
    }
}
