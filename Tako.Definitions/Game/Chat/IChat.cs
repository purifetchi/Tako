using System.Buffers;
using Tako.Definitions.Game.Players;
using Tako.Definitions.Network;

namespace Tako.Definitions.Game.Chat;

/// <summary>
/// The text chat.
/// </summary>
public interface IChat
{
	/// <summary>
	/// The server this chat is bound to.
	/// </summary>
	IServer Server { get; }

	/// <summary>
	/// The message template.
	/// </summary>
	string MessageTemplate { get; set; }

	/// <summary>
	/// Sends a message from a given player with a given message.
	/// </summary>
	/// <param name="source">The player who sent it.</param>
	/// <param name="message">The message.</param>
	void SendMessage(IPlayer source, string message);

	/// <summary>
	/// Sends a message from the server.
	/// </summary>
	/// <param name="message">The server message.</param>
	void SendServerMessage(string message);

	/// <summary>
	/// Registers a chat command.
	/// </summary>
	/// <param name="name">The name of the command.</param>
	/// <param name="handler">The handler</param>
	void RegisterChatCommand(string name, ReadOnlySpanAction<string, IPlayer> handler);
}
