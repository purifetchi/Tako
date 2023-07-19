namespace Tako.Definitions.Game.World;

/// <summary>
/// The block change mode when sent in SetBlock.
/// </summary>
public enum BlockChangeMode : byte
{
	Destroyed = 0x00,
	Created = 0x01
}