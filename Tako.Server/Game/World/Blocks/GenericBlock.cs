using Tako.Definitions.Game.Players;
using Tako.Definitions.Game.World;

namespace Tako.Server.Game.World.Blocks;

/// <summary>
/// A generic block.
/// </summary>
public class GenericBlock : IBlock
{
    /// <inheritdoc/>
    public byte Id { get; init; }

    /// <summary>
    /// Constructs a new generic block of an id.
    /// </summary>
    /// <param name="id">The id.</param>
    public GenericBlock(byte id)
    {
        Id = id;
    }

    /// <inheritdoc/>
    public bool CanBreak(IPlayer player)
    {
        return true;
    }

    /// <inheritdoc/>
    public bool CanPlace(IPlayer player)
    {
        return true;
    }
}
