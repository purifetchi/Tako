using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using Tako.NBT.Tags;

namespace Tako.NBT.Serialization;

/// <summary>
/// An NBT writer.
/// </summary>
public class NBTWriter : IDisposable
{
    /// <summary>
    /// The writer we're using.
    /// </summary>
    private BinaryWriter _writer = null!;

    /// <summary>
    /// The stream we're using.
    /// </summary>
    private Stream _underlyingStream = null!;

    /// <summary>
    /// Creates a new NBT writer for a stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="options"></param>
    public NBTWriter(
        Stream stream, 
        NBTOptions options = NBTOptions.Default)
    {
        CreateStream(stream, options);
    }

    /// <summary>
    /// Creates a new NBT writer for a filename.
    /// </summary>
    /// <param name="filename">The filename.</param>
    /// <param name="options">The options.</param>
    public NBTWriter(
        string filename,
        NBTOptions options = NBTOptions.Default)
    {
        var stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        CreateStream(stream, options);
    }

    /// <summary>
    /// Creates the appropriate stream from the underlying stream.
    /// </summary>
    /// <param name="underlying">The underlying stream.</param>
    private void CreateStream(Stream underlying, NBTOptions options)
    {
        _underlyingStream = options switch
        {
            NBTOptions.Default => underlying,
            NBTOptions.GZipCompressed => new GZipStream(underlying, CompressionMode.Compress),
            NBTOptions.ZlibCompressed => throw new NotImplementedException("Zlib compression is not yet supported."),
            _ => throw new NotImplementedException("Invalid NBT option."),
        };

        _writer = new BinaryWriter(_underlyingStream);
    }

    /// <summary>
    /// Writes a nameless tag.
    /// </summary>
    /// <param name="tag">The tag.</param>
    public void WriteNameless(Tag tag)
    {
        _writer.Write((byte)tag.Id);
        tag.Serialize(this);
    }

    /// <summary>
    /// Writes a single tag.
    /// </summary>
    /// <param name="tag">The tag.</param>
    public void Write(Tag tag)
    {
        _writer.Write((byte)tag.Id);
        WriteString(tag.Name);
        tag.Serialize(this);
    }

    /// <summary>
    /// Writes an NBT length prefixed string.
    /// </summary>
    /// <param name="str">The string.</param>
    internal unsafe void WriteString(string str)
    {
        var length = Encoding.UTF8.GetByteCount(str);
        Span<byte> span = stackalloc byte[length];
        Encoding.UTF8.GetBytes(str, span);

        _writer.Write(BinaryPrimitives.ReverseEndianness((short)str.Length));
        _writer.Write(span);
    }

    /// <summary>
    /// Gets the binary writer.
    /// </summary>
    /// <returns>The binary writer.</returns>
    internal BinaryWriter GetBinaryWriter()
    {
        return _writer;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _writer.Dispose();
        _underlyingStream.Dispose();

        GC.SuppressFinalize(this);
    }
}
