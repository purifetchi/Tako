using Tako.Common.Allocation;
using Tako.Common.Logging;
using Tako.Common.Numerics;
using Tako.Definitions.Game;
using Tako.Definitions.Game.Chat;
using Tako.Definitions.Game.Players;
using Tako.Definitions.Game.World;
using Tako.Definitions.Network;
using Tako.Definitions.Network.Connections;
using Tako.Server.Game;
using Tako.Server.Game.Chat;
using Tako.Server.Game.Players;
using Tako.Server.Logging;
using Tako.Server.Network.Packets.Server;

namespace Tako.Server.Network;

/// <summary>
/// The actual Tako server.
/// </summary>
public partial class Server : IServer
{
	/// <inheritdoc/>
	public IReadOnlyList<IRealm> Realms => _realms;

	/// <inheritdoc/>
	public INetworkManager NetworkManager { get; private set; } = null!;

	/// <inheritdoc/>
	public IChat Chat { get; private set; } = null!;

	/// <summary>
	/// The logger.
	/// </summary>
	private readonly ILogger<Server> _logger = LoggerFactory<Server>.Get();

	/// <summary>
	/// Is the server active?
	/// </summary>
	private bool _active;

	/// <summary>
	/// The realms.
	/// </summary>
	private readonly List<IRealm> _realms;

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
		Chat = new Chat(this);
		RegisterChatCommands();
		RegisterHandlers();

		_realms = new();
		_playerIdAllocator = new(sbyte.MaxValue);

		CreateRealm("default", true)
			.GetWorldGenerator()
			.WithDimensions(new Vector3Int(30, 20, 30))
			.WithType(WorldType.Flat)
			.Build();

		CreateRealm("test", false)
			.GetWorldGenerator()
			.WithDimensions(new Vector3Int(30, 20, 30))
			.WithType(WorldType.Flat)
			.Build();
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

			foreach (var realm in Realms)
			{
				realm.HeartbeatPlayers();
				realm.World?.Simulate();
			}

			Thread.Sleep(10);
		}
	}

	/// <inheritdoc/>
	public IRealm CreateRealm(string name, bool primary = false)
	{
		var realm = new Realm(name, primary, this);
		_realms.Add(realm);

		return realm;
	}

	/// <inheritdoc/>
	public IPlayer AddPlayer(string name, IRealm realm, IConnection connection)
	{
		_logger.Debug($"Adding player {name} for connection {connection.ConnectionId}");
		var player = new Player(
			_playerIdAllocator.GetId(),
			name,
			false,
			connection,
			realm);
		
		return player;
	}

	/// <inheritdoc/>
	public void Shutdown()
	{
		_active = false;
	}
}
