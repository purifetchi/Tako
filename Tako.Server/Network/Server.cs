using Tako.Common.Logging;
using Tako.Common.Numerics;
using Tako.Definitions.Game.Players;
using Tako.Definitions.Game.World;
using Tako.Definitions.Network;
using Tako.Definitions.Network.Connections;
using Tako.Server.Game.World;
using Tako.Server.Logging;

namespace Tako.Server.Network;

/// <summary>
/// The actual Tako server.
/// </summary>
public partial class Server : IServer
{
	/// <inheritdoc/>
	public IWorld World { get; private set; } = null!;

	/// <inheritdoc/>
	public INetworkManager NetworkManager { get; private set; } = null!;

	/// <summary>
	/// The server name.
	/// </summary>
	public string ServerName { get; set; }

	/// <summary>
	/// The motd.
	/// </summary>
	public string MOTD { get; set; }

	/// <summary>
	/// The logger.
	/// </summary>
	private readonly ILogger<Server> _logger = LoggerFactory<Server>.Get();

	/// <summary>
	/// Is the server active?
	/// </summary>
	private bool _active;

	/// <summary>
	/// Constructs a new server.
	/// </summary>
	public Server()
	{
		NetworkManager = new NetworkManager(System.Net.IPAddress.Any, 25565);
		World = new WorldGenerator()
			.WithDimensions(new Vector3Int(10, 20, 10))
			.WithType(WorldGenerator.Type.Flat)
			.Build();
		RegisterHandlers();

		ServerName = "Test server";
		MOTD = "Very cool :)";
	}

	/// <summary>
	/// Runs the server.
	/// </summary>
	public void Run()
	{
		_logger.Info($"Server started and listening.");
		_active = true;

		while (_active)
		{
			NetworkManager.Receive();
			World?.Simulate();

			Thread.Sleep(10);
		}
	}

	/// <inheritdoc/>
	public IPlayer AddPlayer(string name, IConnection connection)
	{
		_logger.Debug($"Adding player {name} for connection {connection.ConnectionId}");
		return null!;
	}

	/// <inheritdoc/>
	public IPlayer AddNpc(string name)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	public void Shutdown()
	{
		_active = false;
	}
}
