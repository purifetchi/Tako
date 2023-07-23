using System.Numerics;
using Tako.Common.Logging;
using Tako.Common.Numerics;
using Tako.Definitions.Game;
using Tako.Definitions.Game.World;
using Tako.Definitions.Network.Connections;
using Tako.NBT.Serialization;
using Tako.NBT.Tags;
using Tako.Server.Logging;
using Tako.Server.Network.Packets.Server;

namespace Tako.Server.Game.World;

/// <summary>
/// A base for any world.
/// </summary>
public class BaseWorld : IWorld
{
    /// <inheritdoc/>
    public IRealm Realm { get; init; }

    /// <inheritdoc/>
    public Vector3 SpawnPoint { get; set; } = Vector3.Zero;

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
    public BaseWorld(Vector3Int dimensions, IRealm realm)
    {
        _worldData = new byte[dimensions.X * dimensions.Y * dimensions.Z];
        _dimensions = dimensions;

        Realm = realm;
    }

    /// <inheritdoc/>
    public byte GetBlock(Vector3Int pos)
    {
        var i = pos.X + pos.Z * _dimensions.X + pos.Y * _dimensions.X * _dimensions.Z;
        return _worldData[i];
    }

    /// <inheritdoc/>
    public void SetBlock(Vector3Int pos, byte block)
    {
        var i = pos.X + pos.Z * _dimensions.X + pos.Y * _dimensions.X * _dimensions.Z;
        _worldData[i] = block;

        //_logger.Info($"Setting block at {pos} to {(ClassicBlockType)block}");

        Realm.SendToAllWithinRealm(new SetBlockPacket
        {
            X = (short)pos.X,
            Y = (short)pos.Y,
            Z = (short)pos.Z,
            BlockType = block
        });
    }

    /// <inheritdoc/>
    public void Simulate()
    {

    }

    /// <inheritdoc/>
    public void StreamTo(IConnection conn)
    {
        new WorldStreamer()
            .ToConnection(conn)
            .WithWorldDimensions(_dimensions)
            .WithBlockArray(_worldData)
            .Stream()
            .Finish();
    }

    /// <inheritdoc/>
    public void Save(string filename)
    {
        var nbt = new CompoundTag("ClassicWorld");
        nbt.Set("X", (short)_dimensions.X);
        nbt.Set("Y", (short)_dimensions.Y);
        nbt.Set("Z", (short)_dimensions.Z);

        nbt.Set("BlockArray", (sbyte[])_worldData.Clone());

        var spawn = new CompoundTag("Spawn");

        // TODO(pref): Fugly abuse of FShort implicit casts, please make it nicer.
        spawn.Set("X", (short)((FShort)SpawnPoint.X));
        spawn.Set("Y", (short)((FShort)SpawnPoint.Y));
        spawn.Set("Z", (short)((FShort)SpawnPoint.Z));

        nbt.Set("Spawn", spawn);

        using var writer = new NBTWriter(filename, NBTOptions.GZipCompressed);
        writer.Write(nbt);
    }

    /// <summary>
    /// Loads data from a ClassicWorld NBT file.
    /// </summary>
    /// <param name="tag">The outermost compound tag.</param>
    internal void LoadDataFromNBT(CompoundTag nbt)
    {
        _dimensions = new Vector3Int(
            nbt.Get<short>("X"),
            nbt.Get<short>("Y"),
            nbt.Get<short>("Z"));

        // Copy the block array.
        var blockArray = nbt.Get<sbyte[]>("BlockArray");
        _worldData = new byte[blockArray.Length];
        Buffer.BlockCopy(blockArray, 0, _worldData, 0, _worldData.Length);

        var spawn = nbt.Get<CompoundTag>("Spawn");
        SpawnPoint = new Vector3(
            (FShort)spawn.Get<short>("X"),
            (FShort)spawn.Get<short>("Y"),
            (FShort)spawn.Get<short>("Z"));
    }
}
