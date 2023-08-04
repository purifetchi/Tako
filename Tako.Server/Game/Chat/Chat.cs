using Tako.Common.Logging;
using Tako.Definitions.Game;
using Tako.Definitions.Game.Chat;
using Tako.Definitions.Game.Players;
using Tako.Definitions.Network;
using Tako.Definitions.Network.Packets;
using Tako.Server.Network.Packets.Server;

namespace Tako.Server.Game.Chat;

/// <summary>
/// The chat.
/// </summary>
public class Chat : IChat
{
    /// <summary>
    /// The fallback chat template.
    /// </summary>
    private const string FALLBACK_CHAT_TEMPLATE = "<{0}>: {1}";

    /// <inheritdoc/>
    public IServer Server { get; init; }

    /// <inheritdoc/>
    public string MessageTemplate { get; set; }

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<Chat> _logger = LoggerFactory<Chat>.Get();

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
        MessageTemplate = Server.Settings.Get("chat-template") ?? FALLBACK_CHAT_TEMPLATE;

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

        SplitAndSendMessage(filled, source.Realm.SendToAllWithinRealm);
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

    /// <summary>
    /// Splits the message into 64 character chunks for sending.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>A message enumerable.</returns>
    private static void SplitAndSendMessage(
        string message, 
        Action<IServerPacket> sendMethod)
    {
        const int maxMessageLength = 64;
        const char continuationCharacter = '>';

        if (message.Length <= maxMessageLength)
        {
            sendMethod(new MessagePacket
            {
                PlayerId = 0x00,
                Message = message
            });

            return;
        }

        // Split the message into partitions.
        var partitions = (int)Math.Ceiling(message.Length / (float)maxMessageLength);
        for (var i = 0; i < partitions; i++)
        {
            var index = i * maxMessageLength;
            var length = Math.Min(message.Length - index, maxMessageLength);

            var split = message.Substring(index, length);
            if (i > 0)
                split = continuationCharacter + split;

            sendMethod(new MessagePacket
            {
                PlayerId = 0x00,
                Message = split
            });
        }
    }
}
