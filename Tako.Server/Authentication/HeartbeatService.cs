using System.Security.Cryptography;
using Tako.Common.Logging;
using Tako.Definitions.Network;
using Tako.Server.Logging;

namespace Tako.Server.Authentication;

/// <summary>
/// The service responsible for making the Classic heartbeat to the host.
/// </summary>
internal class HeartbeatService
{
    /// <summary>
    /// The time delay for a heartbeat in seconds.
    /// </summary>
    private const int TIME_DELAY_FOR_HEARTBEAT = 45;

    /// <summary>
    /// The heartbeat salt.
    /// </summary>
    public string Salt { get; init; }

    /// <summary>
    /// The server.
    /// </summary>
    private readonly IServer _server;

    /// <summary>
    /// The base heartbeat url.
    /// </summary>
    private readonly string _heartbeatBaseUrl;

    /// <summary>
    /// Is the heartbeat enabled?
    /// </summary>
    private readonly bool _heartbeatEnabled;

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<HeartbeatService> _logger = LoggerFactory<HeartbeatService>.Get();

    /// <summary>
    /// Constructs a new heartbeat service.
    /// </summary>
    /// <param name="server">The server.</param>
    public HeartbeatService(IServer server)
    {
        _server = server;

        Salt = MakeSalt();

        _heartbeatBaseUrl = _server.Settings.Get("heartbeat-url") ?? string.Empty;
        _heartbeatEnabled = bool.Parse(_server.Settings.Get("heartbeat") ?? "false");

        if (_heartbeatEnabled)
            StartHeartbeating();
    }

    /// <summary>
    /// Makes the salt.
    /// </summary>
    /// <returns>A 16 character base62 salt.</returns>
    private static string MakeSalt()
    {
        const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        const int length = 16;

        return new string(
            Enumerable.Repeat(char.MinValue, length)
            .Select(_ => alphabet[Random.Shared.Next(alphabet.Length)])
            .ToArray());
    }

    /// <summary>
    /// Starts the heartbeat service.
    /// </summary>
    private async void StartHeartbeating()
    {
        // Create the web client.
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri(_heartbeatBaseUrl)
        };

        while (_server.Active)
        {
            await Heartbeat(httpClient);
            await Task.Delay(TimeSpan.FromSeconds(TIME_DELAY_FOR_HEARTBEAT));
        }
    }

    /// <summary>
    /// Performs a single heartbeat.
    /// </summary>
    /// <returns></returns>
    private async Task Heartbeat(HttpClient client)
    {
        const string serverName = "Tako";

        var players = _server.RealmManager.GetRealmNames()
            .Select(_server.RealmManager.GetRealm)
            .Aggregate(0, (acc, realm) => acc += realm!.Players.Count);

        _logger.Info($"Sending heartbeat to: {_heartbeatBaseUrl}");
        var resp = await client.GetAsync($"?port={_server.Settings.Get("port")}&max=128&name={_server.Settings.Get("server-name")}&public=True&version=7&salt={Salt}&users={players}&software={serverName}");
        _logger.Debug(await resp.Content.ReadAsStringAsync());
    }
}
