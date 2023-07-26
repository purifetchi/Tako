using System.Text;
using Tako.Definitions.Game.Players;

namespace Tako.Server.Network;

public partial class Server
{
    /// <summary>
    /// Registers chat commands.
    /// </summary>
    private void RegisterChatCommands()
    {
        Chat.RegisterChatCommand("op", OnOpCommand);
        Chat.RegisterChatCommand("move", OnMoveCommand);
        Chat.RegisterChatCommand("save", OnSaveCommand);
        Chat.RegisterChatCommand("plugins", OnPluginsCommand);
    }

    /// <summary>
    /// Handles the /op command.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="args">The args.</param>
    private void OnOpCommand(IPlayer player, string[] args)
    {
        if (!player.Op)
        {
            Chat.SendServerMessageTo(player, "&cYou are not an OP.");
            return;
        }

        if (args.Length < 2)
        {
            Chat.SendServerMessageTo(player, "&cNot enough arguments.");
            return;
        }

        var target = player.Realm
            .Players
            .Values
            .FirstOrDefault(player => player.Name == args[1]);

        if (target is null)
        {
            Chat.SendServerMessageTo(player, "&cThis player does not exist.");
            return;
        }

        target.SetOpStatus(!target.Op);
    }

    /// <summary>
    /// Handles the /move command.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="args">The args.</param>
    private void OnMoveCommand(IPlayer player, string[] args)
    {
        if (args.Length < 2)
        {
            Chat.SendServerMessageTo(player, "&cNot enough arguments.");
            return;
        }

        var realm = RealmManager.GetRealm(args[1]);
        if (realm is null)
        {
            Chat.SendServerMessageTo(player, "&cThis realm does not exist.");
            return;
        }

        realm.MovePlayer(player);
    }

    /// <summary>
    /// Handles the /save command.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="args">The args.</param>
    private void OnSaveCommand(IPlayer player, string[] args)
    {
        if (!player.Op)
        {
            Chat.SendServerMessageTo(player, "&cYou are not an OP.");
            return;
        }

        player.Realm?
            .World?
            .Save("test.cw");
    }

    /// <summary>
    /// Handles the /plugins command.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="args">The args.</param>
    private void OnPluginsCommand(IPlayer player, string[] args)
    {
        Chat.SendServerMessageTo(player, "&fLoaded plugins:");

        var sb = new StringBuilder();
        sb.Append("&a");
        foreach (var plugin in PluginManager.GetAllPlugins())
        {
            sb.Append(plugin.Name);
            sb.Append(' ');
        }

        Chat.SendServerMessageTo(player, sb.ToString());
    }
}
