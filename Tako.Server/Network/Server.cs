using System.Diagnostics;
using Tako.Common.Allocation;
using Tako.Common.Logging;
using Tako.Definitions.Game;
using Tako.Definitions.Game.Chat;
using Tako.Definitions.Game.Players;
using Tako.Definitions.Network;
using Tako.Definitions.Network.Connections;
using Tako.Definitions.Plugins;
using Tako.Definitions.Plugins.Events;
using Tako.Definitions.Settings;
using Tako.Server.Authentication;
using Tako.Server.Game;
using Tako.Server.Game.Chat;
using Tako.Server.Game.Players;
using Tako.Server.Plugins;
using Tako.Server.Plugins.Events;
using Tako.Server.Settings;

namespace Tako.Server.Network;

/// <summary>
/// The actual Tako server.
/// </summary>
public partial class Server : IServer
{
    /// <inheritdoc/>
    public bool Active => _active;

    /// <inheritdoc/>
    public float Time => _stopwatch?.ElapsedMilliseconds / 1000f ?? 0f;

    /// <inheritdoc/>
    public IRealmManager RealmManager { get; init; } = null!;

    /// <inheritdoc/>
    public INetworkManager NetworkManager { get; private set; } = null!;

    /// <inheritdoc/>
    public IChat Chat { get; private set; } = null!;

    /// <inheritdoc/>
    public ISettings Settings { get; init; } = null!;

    /// <inheritdoc/>
    public IPluginManager PluginManager { get; init; } = null!;

    /// <inheritdoc/>
    public IEvent<float> OnServerTick { get; init; }

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<Server> _logger = LoggerFactory<Server>.Get();

    /// <summary>
    /// Is the server active?
    /// </summary>
    private bool _active;

    /// <summary>
    /// Should we authenticate players?
    /// </summary>
    private readonly bool _authenticatePlayers;

    /// <summary>
    /// The player id allocator.
    /// </summary>
    private readonly IdAllocator<sbyte> _playerIdAllocator;
    
    /// <summary>
    /// The server time stopwatch.
    /// </summary>
    private readonly Stopwatch _stopwatch;

    /// <summary>
    /// The heartbeat service.
    /// </summary>
    private HeartbeatService? _heartbeatService;

    /// <summary>
    /// Constructs a new server.
    /// </summary>
    public Server()
    {
        Settings = new FileBackedSettings(
            "server.properties",
            SetDefaultSettings);

        NetworkManager = new NetworkManager();
        NetworkManager.AddTransportProvider(
            new TcpTransportProvider(
                Settings,
                NetworkManager
                ));

        Chat = new Chat(this);
        RealmManager = new RealmManager(this);
        OnServerTick = new Event<float>();

        RegisterChatCommands();
        RegisterHandlers();

        _playerIdAllocator = new(sbyte.MaxValue);
        _authenticatePlayers = bool.Parse(Settings.Get("authenticate-players") ?? "true");
        _stopwatch = new();

        PluginManager = new PluginManager(this);
    }

    /// <summary>
    /// Runs the server.
    /// </summary>
    public void Run()
    {
        NetworkManager.StartListening();

        _logger.Info($"Server started and listening.");
        _active = true;
        _stopwatch.Start();

        _heartbeatService = new HeartbeatService(this);

        while (_active)
        {
            NetworkManager.Receive();
            RealmManager.SimulateRealms();

            OnServerTick.Send(float.Pi);

            Thread.Sleep(10);
        }

        NetworkManager.StopListening();
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
