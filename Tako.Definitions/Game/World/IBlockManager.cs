namespace Tako.Definitions.Game.World;

/// <summary>
/// A block manager.
/// </summary>
public interface IBlockManager
{
    /// <summary>
    /// Gets a block definition by its id.
    /// </summary>
    /// <param name="id">The block id.</param>
    /// <returns>The block definition or nothing.</returns>
    IBlock? GetBlockById(byte id);

    /// <summary>
    /// Defines a block and sets the id.
    /// </summary>
    /// <param name="block">The block.</param>
    void DefineBlock(IBlock block);

    /// <summary>
    /// Defines an id to be a block.
    /// </summary>
    /// <typeparam name="TBlock">The block type.</typeparam>
    /// <param name="id">The id.</param>
    void DefineBlock<TBlock>(byte id)
        where TBlock : IBlock;

    /// <summary>
    /// Undefines a block given its id.
    /// </summary>
    /// <param name="id">The id of the block.</param>
    void UndefineBlock(byte id);

    /// <summary>
    /// Undefines a block.
    /// </summary>
    /// <param name="block">The block.</param>
    void UndefineBlock(IBlock block);
}
