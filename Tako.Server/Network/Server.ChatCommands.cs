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
	}

	/// <summary>
	/// Handles the /op command.
	/// </summary>
	/// <param name="player">The player.</param>
	/// <param name="args">The args.</param>
	private void OnOpCommand(IPlayer player, string[] args)
	{
		var target = Players.Values.FirstOrDefault(player => player.Name == args[1]);

		if (target is null)
		{
			Chat.SendServerMessageTo(player, "&cThis player does not exist.");
			return;
		}

		target.SetOpStatus(!target.Op);
	}
}
