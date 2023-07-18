using Tako.Common.Allocation;
using Tako.Common.Logging;
using Tako.Common.Numerics;
using Tako.Definitions.Game.Players;
using Tako.Definitions.Game.World;
using Tako.Definitions.Network;
using Tako.Definitions.Network.Connections;
using Tako.Server.Game.Players;
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

	/// <inheritdoc/>
	public IReadOnlyDictionary<sbyte, IPlayer> Players => _players;

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
	/// The players dictionary.
	/// </summary>
	private readonly Dictionary<sbyte, IPlayer> _players;

	/// <summary>
	/// The player id allocator.
	/// </summary>
	private readonly IdAllocator<sbyte> _playerIdAllocator;

	/// <summary>
	/// Constructs a new server.
	/// </summary>
	public Server()
	{
		NetworkManager = new NetworkManager(System.Net.IPAddress.Any, 25565);
		World = new WorldGenerator()
			.WithDimensions(new Vector3Int(50, 20, 50))
			.WithType(WorldGenerator.Type.Flat)
			.Build();
		RegisterHandlers();

		_players = new();
		_playerIdAllocator = new(sbyte.MaxValue);

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
		var player = new Player(
			_playerIdAllocator.GetId(),
			name,
			false,
			connection,
			this);

		_players[player.PlayerId] = player;
		return player;
	}

	/// <inheritdoc/>
	public IPlayer AddNpc(string name)
	{
		_logger.Debug($"Adding NPC {name}.");
		var player = new Player(
			_playerIdAllocator.GetId(),
			name,
			false,
			null,
			this);

		_players[player.PlayerId] = player;
		return player;
	}

	/// <inheritdoc/>
	public void Shutdown()
	{
		_active = false;
	}
}
