using System.Buffers.Binary;
using System.IO.Compression;
using Tako.Common.Logging;
using Tako.Common.Numerics;
using Tako.Definitions.Network.Connections;
using Tako.Server.Network.Packets.Server;

namespace Tako.Server.Game.World;

/// <summary>
/// A fluent class responsible for sending the world to a client.
/// </summary>
public class WorldStreamer
{
    /// <summary>
    /// The connection we're sending to.
    /// </summary>
    private IConnection? _conn;

    /// <summary>
    /// The world dimensions.
    /// </summary>
    private Vector3Int? _dimensions;

    /// <summary>
    /// The compressed data.
    /// </summary>
    private byte[]? _compressedData;

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<WorldStreamer> _logger = LoggerFactory<WorldStreamer>.Get();

    /// <summary>
    /// Specifies the connection.
    /// </summary>
    /// <param name="connection">The connection.</param>
    public WorldStreamer ToConnection(IConnection connection)
    {
        _conn = connection;
        _conn.Send(new LevelInitializePacket());
        return this;
    }

    /// <summary>
    /// Sets the world dimensions.
    /// </summary>
    /// <param name="dimensions">The world dimensions.</param>
    public WorldStreamer WithWorldDimensions(Vector3Int dimensions)
    {
        _dimensions = dimensions;
        return this;
    }

    /// <summary>
    /// Sets the XZY block array and compresses it.
    /// </summary>
    /// <param name="blockArray">The block array.</param>
    public WorldStreamer WithBlockArray(byte[] blockArray)
    {
        using var output = new MemoryStream();
        using var gzip = new GZipStream(output, CompressionMode.Compress);
        gzip.Write(BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(blockArray.Length)));
        gzip.Write(blockArray, 0, blockArray.Length);
        gzip.Close();

        _compressedData = output.ToArray();
        return this;
    }

    /// <summary>
    /// Streams the data to the client.
    /// </summary>
    public WorldStreamer Stream()
    {
        if (_conn is null)
        {
            _logger.Error("No connection specified for world streaming!");
            return this;
        }

        if (_dimensions is null)
        {
            _logger.Error("World has no dimensions specified!");
            return this;
        }

        if (_compressedData is null)
        {
            _logger.Error("No world block array specified!");
            return this;
        }

        _logger.Info($"Streaming the world data to {_conn.ConnectionId}.");
        var size = _compressedData.Length;
        var cursor = 0;

        do
        {
            var amount = Math.Min(1024, size - cursor);
            var percent = (byte)Math.Round(((float)(cursor + amount) / size) * 100d);

            _logger.Info($"{percent}% of the world sent...");

            // Send the packet.
            _conn.Send(new LevelDataChunkPacket
            {
                ChunkLength = (short)amount,
                ChunkData = new(_compressedData, cursor, amount),
                PercentComplete = percent
            });

            cursor += amount;
        } while (cursor < size);

        return this;
    }

    /// <summary>
    /// Finishes the world streaming.
    /// </summary>
    public void Finish()
    {
        _conn?.Send(new LevelFinalizePacket
        {
            XSize = (short)_dimensions!.Value.X,
            YSize = (short)_dimensions!.Value.Y,
            ZSize = (short)_dimensions!.Value.Z
        });

        _compressedData = null;
    }
}
