using Tako.Definitions.Game.World;
using Tako.Server.Game.World.Blocks;

namespace Tako.Server.Game.World;

/// <inheritdoc/>
public partial class BlockManager
{
    /// <summary>
    /// Set up all of the default blocks.
    /// </summary>
    static BlockManager()
    {
        DefaultBlockManager = new BlockManager();
        for (byte i = 0; i < (byte)ClassicBlockType.LENGTH; i++)
        {
            var type = (ClassicBlockType)i;
            switch (type)
            {
                case ClassicBlockType.Obsidian:
                case ClassicBlockType.Water:
                case ClassicBlockType.WaterStill:
                case ClassicBlockType.Lava:
                case ClassicBlockType.LavaStill:
                    DefaultBlockManager.DefineBlock<OpBlock>(i);
                    continue;

                default:
                    DefaultBlockManager.DefineBlock<GenericBlock>(i);
                    continue;
            }
        }
    }
}
