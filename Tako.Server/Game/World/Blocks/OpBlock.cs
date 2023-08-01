using Tako.Definitions.Game.Players;
using Tako.Definitions.Game.World;

namespace Tako.Server.Game.World.Blocks;

/// <summary>
/// A block that's only editable by an OP.
/// </summary>
public class OpBlock : IBlock
{
    /// <inheritdoc/>
    public byte Id { get; init; }

    /// <summary>
    /// Constructs a new OP block of an id.
    /// </summary>
    /// <param name="id">The id.</param>
    public OpBlock(byte id)
    {
        Id = id;
    }

    /// <inheritdoc/>
    public bool CanBreak(IPlayer player)
    {
        return player.Op;
    }

    /// <inheritdoc/>
    public bool CanPlace(IPlayer player)
    {
        return player.Op;
    }
}
