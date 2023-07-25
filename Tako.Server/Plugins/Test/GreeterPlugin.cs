using Tako.Definitions.Game.Players;
using Tako.Definitions.Network;
using Tako.Definitions.Plugins;
using Tako.Definitions.Plugins.Events;

namespace Tako.Server.Plugins.Test;

/// <summary>
/// A plugin that greets people on joining.
/// </summary>
public class GreeterPlugin : Plugin
{
    /// <inheritdoc/>
    public override string Name => "Greeter";

    /// <summary>
    /// Creates a new instance of the greeter plugin.
    /// </summary>
    /// <param name="server">The server.</param>
    public GreeterPlugin(IServer server)
        : base(server)
    {
        server.RealmManager.GetDefaultRealm()?
            .OnPlayerJoinedRealm.Subscribe(OnPlayerJoinedRealm);
    }

    /// <summary>
    /// Handles the player joined realm event.
    /// </summary>
    /// <param name="player">The player.</param>
    private EventHandlingResult OnPlayerJoinedRealm(IPlayer player)
    {
        Server.Chat.SendServerMessageTo(player, "&f- - - - -");
        Server.Chat.SendServerMessageTo(player, $"Welcome &f{player.Name}&e!");
        Server.Chat.SendServerMessageTo(player, $"This server is running &9Tako!");
        Server.Chat.SendServerMessageTo(player, $"An experimental server software written in &2C#");
        Server.Chat.SendServerMessageTo(player, $"To learn more, visit &bhttps://github.com/naomiEve/Tako");
        Server.Chat.SendServerMessageTo(player, "&f- - - - -");
        return EventHandlingResult.Continue;
    }
}
