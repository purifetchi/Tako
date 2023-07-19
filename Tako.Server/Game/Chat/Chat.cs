using System.Buffers;
using Tako.Common.Logging;
using Tako.Definitions.Game.Chat;
using Tako.Definitions.Game.Players;
using Tako.Definitions.Network;
using Tako.Server.Logging;
using Tako.Server.Network.Packets.Server;

namespace Tako.Server.Game.Chat;

/// <summary>
/// The chat.
/// </summary>
public class Chat : IChat
{
	/// <inheritdoc/>
	public IServer Server { get; init; }

	/// <inheritdoc/>
	public string MessageTemplate { get; set; } = "<{0}>: {1}";

	/// <inheritdoc/>
	private ILogger<Chat> _logger = LoggerFactory<Chat>.Get();

	/// <summary>
	/// Creates a new chat.
	/// </summary>
	/// <param name="server">The server it's bound to.</param>
	public Chat(IServer server)
	{
		Server = server;
	}

	/// <inheritdoc/>
	public void RegisterChatCommand(string name, ReadOnlySpanAction<string, IPlayer> handler)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	public void SendMessage(IPlayer source, string message)
	{
		var filled = string.Format(MessageTemplate, source.Name, message);
		_logger.Info(filled);

		Server.NetworkManager.SendToAll(new MessagePacket
		{
			PlayerId = 0x00,
			Message = filled
		});
	}

	/// <inheritdoc/>
	public void SendServerMessage(string message)
	{
		Server.NetworkManager.SendToAll(new MessagePacket
		{
			PlayerId = -1,
			Message = message
		});
	}
}
