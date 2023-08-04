using Tako.Common.Logging;
using Tako.Definitions.Game.World;

namespace Tako.Server.Game.World;

/// <summary>
/// A block manager.
/// </summary>
public partial class BlockManager : IBlockManager
{
    /// <summary>
    /// The shared block manager. It will contain default definitions for all the known classic blocks.
    /// <br />
    /// This only exists so we avoid allocating all the memory required for blocks every time we create a new realm.
    /// </summary>
    private static IBlockManager DefaultBlockManager { get; set; }

    /// <summary>
    /// The blocks dictionary.
    /// </summary>
    private readonly Dictionary<byte, IBlock> _blocks;

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<BlockManager> _logger = LoggerFactory<BlockManager>.Get();

    /// <summary>
    /// Creates a new block manager.
    /// </summary>
    public BlockManager()
    {
        _blocks = new();
    }

    /// <inheritdoc/>
    public void DefineBlock(IBlock block)
    {
        _blocks.Add(block.Id, block);
    }

    /// <inheritdoc/>
    public void DefineBlock<TBlock>(byte id)
        where TBlock : IBlock
    {
        // Get the ctor of the block
        var blockType = typeof(TBlock);
        var ctor = blockType.GetConstructor(new[] { typeof(byte) });

        // Invalid block definition.
        if (ctor is null)
        {
            _logger.Warn($"Missing constructor of type \"TBlock(byte id)\" for block type {blockType.Name}, couldn't define it.");
            return;
        }

        var block = (IBlock)ctor.Invoke(new[] { (object)id });
        _blocks.Add(id, block);

        _logger.Info($"Defined block of type {blockType.Name} for id {id}");
    }

    /// <inheritdoc/>
    public IBlock? GetBlockById(byte id)
    {
        if (_blocks.TryGetValue(id, out var block))
            return block;

        // Try to get from the shared fallback.
        if (this != DefaultBlockManager)
            return DefaultBlockManager.GetBlockById(id);

        return null;
    }

    /// <inheritdoc/>
    public void UndefineBlock(byte id)
    {
        _blocks.Remove(id);
    }

    /// <inheritdoc/>
    public void UndefineBlock(IBlock block)
    {
        _blocks.Remove(block.Id);
    }
}
