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
	}

	/// <summary>
	/// Handles the /op command.
	/// </summary>
	/// <param name="player">The player.</param>
	/// <param name="args">The args.</param>
	private void OnOpCommand(IPlayer player, string[] args)
	{
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
		player.Realm?
			.World?
			.Save("test.cw");
	}
}
