using Tako.Definitions.Game.Players;

namespace Tako.Definitions.Game.Chat;

/// <summary>
/// The text chat.
/// </summary>
public interface IChat
{
	/// <summary>
	/// Sends a message from a given player with a given message.
	/// </summary>
	/// <param name="source">The player who sent it.</param>
	/// <param name="message">The message.</param>
	void SendMessage(IPlayer source, string message);
}
