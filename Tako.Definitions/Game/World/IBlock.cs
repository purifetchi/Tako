using Tako.Definitions.Game.Players;

namespace Tako.Definitions.Game.World;

/// <summary>
/// A block definition.
/// </summary>
public interface IBlock
{
    /// <summary>
    /// The id of this block.
    /// </summary>
    byte Id { get; }

    /// <summary>
    /// Can a player break this block?
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>Whether the player can break this block.</returns>
    bool CanBreak(IPlayer player);

    /// <summary>
    /// Can a player place this block?
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The block.</returns>
    bool CanPlace(IPlayer player);
}
