using System.Buffers;
using Tako.Common.Logging;
using Tako.Definitions.Game;
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

	/// <summary>
	/// The logger.
	/// </summary>
	private ILogger<Chat> _logger = LoggerFactory<Chat>.Get();

	/// <summary>
	/// The chat commands.
	/// </summary>
	private Dictionary<string, Action<IPlayer, string[]>> _commands;

	/// <summary>
	/// Creates a new chat.
	/// </summary>
	/// <param name="server">The server it's bound to.</param>
	public Chat(IServer server)
	{
		Server = server;
		_commands = new();
	}

	/// <inheritdoc/>
	public void RegisterChatCommand(string name, Action<IPlayer, string[]> handler)
	{
		_logger.Info($"Registered command {name}.");

		// TODO(pref): Maybe not making this allocate would be better but w/e.
		_commands[name] = handler;
	}

	/// <inheritdoc/>
	public void SendMessage(IPlayer source, string message)
	{
		// If this is a command.
		if (message.StartsWith('/'))
		{
			ParseCommand(source, message[1..].Split(' '));
			return;
		}

		var filled = string.Format(MessageTemplate, source.Name, message);
		_logger.Info(filled);

		source.Realm.SendToAllWithinRealm(new MessagePacket
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

	/// <inheritdoc/>
	public void SendServerMessageTo(IPlayer dest, string message)
	{
		dest.Connection?.Send(new MessagePacket 
		{ 
			PlayerId = -1,
			Message = message
		});
	}

	/// <inheritdoc/>
	public void SendServerMessageTo(IRealm realm, string message)
	{
		realm.SendToAllWithinRealm(new MessagePacket
		{
			PlayerId = -1,
			Message = message
		});
	}

	/// <summary>
	/// Parses a command.
	/// </summary>
	/// <param name="player">The player.</param>
	/// <param name="command">The command.</param>
	private void ParseCommand(IPlayer player, string[] command)
	{
		if (!_commands.TryGetValue(command[0], out var handler))
		{
			SendServerMessageTo(player, "&cThis command does not exist.");
			return;
		}

		handler(player, command);
	}
}
