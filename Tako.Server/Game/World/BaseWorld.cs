using System.Buffers.Binary;
using System.IO.Compression;
using Tako.Common.Logging;
using Tako.Common.Numerics;
using Tako.Definitions.Game.World;
using Tako.Definitions.Network.Connections;
using Tako.Server.Logging;
using Tako.Server.Network.Packets.Server;

namespace Tako.Server.Game.World;

/// <summary>
/// A base for any world.
/// </summary>
public class BaseWorld : IWorld
{
	/// <summary>
	/// The world data.
	/// </summary>
	private byte[] _worldData;

	/// <summary>
	/// The dimensions of the world.
	/// </summary>
	private Vector3Int _dimensions;

	/// <summary>
	/// The logger.
	/// </summary>
	private ILogger<BaseWorld> _logger = LoggerFactory<BaseWorld>.Get();

	/// <summary>
	/// The base world data.
	/// </summary>
	/// <param name="dimensions">The dimensions of the world.</param>
	public BaseWorld(Vector3Int dimensions)
	{
		_worldData = new byte[dimensions.X * dimensions.Y * dimensions.Z];
		_dimensions = dimensions;
	}

	/// <inheritdoc/>
	public byte GetBlock(Vector3Int pos)
	{
		var i = pos.X + pos.Z * _dimensions.X + pos.Y * _dimensions.X * _dimensions.Y;
		return _worldData[i];
	}

	/// <inheritdoc/>
	public void SetBlock(Vector3Int pos, byte block)
	{
		var i = pos.X + pos.Z * _dimensions.X + pos.Y * _dimensions.X * _dimensions.Y;
		_worldData[i] = block;
	}

	/// <inheritdoc/>
	public void Simulate()
	{

	}

	/// <inheritdoc/>
	public void StreamTo(IConnection conn)
	{
		_logger.Info($"We should now stream the world to {conn.ConnectionId}");
		conn.Send(new LevelInitializePacket());
		
		using var output = new MemoryStream();
		using var gzip = new GZipStream(output, CompressionMode.Compress);
		gzip.Write(BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(_worldData.Length)));
		gzip.Write(_worldData, 0, _worldData.Length);
		gzip.Close();

		var world = output.ToArray();

		var size = world.Length;
		var cursor = 0;
		
		_logger.Info($"Gzipped world data weighs: {size}.");
		do
		{
			var amount = Math.Min(1024, size);

			_logger.Info($"{(byte)Math.Round(((cursor + amount) / size) * 100d)}% of the world sent...");

			Console.Write("Chunk: ");
			for (var i = 0; i < amount; i++)
				Console.Write(world[cursor + i].ToString("X2"));

			Console.Write('\n');

			// Send the packet.
			conn.Send(new LevelDataChunkPacket
			{
				ChunkLength = (short)amount,
				ChunkData = new ArraySegment<byte>(world, cursor, amount),
				PercentComplete = (byte)Math.Round(((cursor + amount) / size) * 100d)
			});

			cursor += amount;
		} while (cursor < size);

		conn.Send(new LevelFinalizePacket
		{
			XSize = (short)_dimensions.X,
			YSize = (short)_dimensions.Y,
			ZSize = (short)_dimensions.Z,
		});
	}
}
