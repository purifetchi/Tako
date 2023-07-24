using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using Tako.NBT.Tags;

namespace Tako.NBT.Serialization;

/// <summary>
/// An NBT reader.
/// </summary>
public class NBTReader : IDisposable
{
    /// <summary>
    /// The writer we're using.
    /// </summary>
    private BinaryReader _reader = null!;

    /// <summary>
    /// The stream we're using.
    /// </summary>
    private Stream _underlyingStream = null!;

    /// <summary>
    /// Creates a new NBT reader for a stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="options"></param>
    public NBTReader(Stream stream)
    {
        CreateStream(stream);
    }

    /// <summary>
    /// Creates a new NBT reader for a filename.
    /// </summary>
    /// <param name="filename">The filename.</param>
    /// <param name="options">The options.</param>
    public NBTReader(string filename)
    {
        var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        CreateStream(stream);
    }

    /// <summary>
    /// Creates a stream for reading.
    /// </summary>
    /// <param name="underlying">The underlying stream.</param>
    private void CreateStream(Stream underlying)
    {
        const int gzipHeaderByte = 0x1F;
        const int zlibHeaderByte = 0x78;

        // Read one byte from the incoming stream.
        // This should be enough to discern between gzip and zlib...
        var magic = underlying.ReadByte();
        underlying.Seek(0, SeekOrigin.Begin);

        // NOTE(pref): Do zlib compressed NBT files exist in the wild? wiki.vg says they do...
        //             I've only seen gzipped ones, though.
        _underlyingStream = magic switch
        {
            gzipHeaderByte => new GZipStream(underlying, CompressionMode.Decompress),
            zlibHeaderByte => new ZLibStream(underlying, CompressionMode.Decompress),
            _ => underlying
        };

        _reader = new BinaryReader(_underlyingStream);
    }

    /// <summary>
    /// Reads a single tag.
    /// </summary>
    /// <returns>The read tag.</returns>
    public Tag Read()
    {
        var tagId = ReadTagId();
        var name = tagId switch
        {
            // The end tag NEVER has a string attached with it.
            TagId.End => string.Empty,
            _ => ReadString()
        };

        return ReadSpecificTag(tagId, name);
    }

    /// <summary>
    /// Reads the tag id.
    /// </summary>
    /// <returns>The tag id.</returns>
    internal TagId ReadTagId()
    {
        return (TagId)_reader.ReadByte();
    }

    /// <summary>
    /// Reads a 2-byte signed short and reverses its endianness.
    /// </summary>
    /// <returns>The 2-byte signed short.</returns>
    internal short ReadInt16()
    {
        return BinaryPrimitives.ReverseEndianness(_reader.ReadInt16());
    }

    /// <summary>
    /// Reads a 2-byte unsigned short and reverses its endianness.
    /// </summary>
    /// <returns>The 2-byte unsigned short.</returns>
    internal ushort ReadUInt16()
    {
        return BinaryPrimitives.ReverseEndianness(_reader.ReadUInt16());
    }

    /// <summary>
    /// Reads a 4-byte signed integer and reverses its endianness.
    /// </summary>
    /// <returns>The 4-byte signed integer.</returns>
    internal int ReadInt32()
    {
        return BinaryPrimitives.ReverseEndianness(_reader.ReadInt32());
    }

    /// <summary>
    /// Reads a 8-byte signed long and reverses its endianness.
    /// </summary>
    /// <returns>The 8-byte signed long.</returns>
    internal long ReadInt64()
    {
        return BinaryPrimitives.ReverseEndianness(_reader.ReadInt64());
    }

    /// <summary>
    /// Reads a specific amount of bytes into the span.
    /// </summary>
    /// <param name="bytes">The span.</param>
    internal void ReadBytes(Span<byte> bytes)
    {
        var readTotal = 0;
        do
        {
            readTotal += _reader.Read(bytes[readTotal..]);
        } while (readTotal != bytes.Length);
    }

    /// <summary>
    /// Parses a single tag.
    /// </summary>
    /// <param name="tagId">The tag id.</param>
    /// <param name="name">The name.</param>
    /// <returns>The tag.</returns>
    internal Tag ReadSpecificTag(TagId tagId, string name)
    {
        return tagId switch
        {
            TagId.End => new EndTag(),
            TagId.Byte => new ByteTag(name).Parse(this),
            TagId.Short => new ShortTag(name).Parse(this),
            TagId.Int => new IntTag(name).Parse(this),
            TagId.Long => new LongTag(name).Parse(this),
            TagId.Float => new FloatTag(name).Parse(this),
            TagId.Double => new DoubleTag(name).Parse(this),
            TagId.ByteArray => new ByteArrayTag(name).Parse(this),
            TagId.String => new StringTag(name).Parse(this),
            TagId.List => new ListTag(name).Parse(this),
            TagId.Compound => new CompoundTag(name).Parse(this),
            TagId.IntArray => new IntArrayTag(name).Parse(this),
            TagId.LongArray => new LongArrayTag(name).Parse(this),
            _ => throw new NotSupportedException($"Unknown tag id detected! {tagId}"),
        };
    }

    /// <summary>
    /// Reads a length prefixed string.
    /// </summary>
    /// <returns>The string.</returns>
    internal unsafe string ReadString()
    {
        var nameLength = ReadInt16();
        var nameBuffer = stackalloc byte[nameLength];
        var nameSpan = new Span<byte>(nameBuffer, nameLength);
        _reader.Read(nameSpan);

        return Encoding.UTF8.GetString(nameSpan);
    }

    /// <summary>
    /// Gets the binary reader.
    /// </summary>
    /// <returns>The binary reader.</returns>
    internal BinaryReader GetBinaryReader()
    {
        return _reader;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _reader.Dispose();
        _underlyingStream.Dispose();

        GC.SuppressFinalize(this);
    }
}
