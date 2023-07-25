using Tako.Definitions.Settings;

namespace Tako.Server.Network;

/// <inheritdoc/>
public partial class Server
{
    /// <summary>
    /// Sets the default settings for the server.properties file.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    private static void SetDefaultSettings(ISettings settings)
    {
        settings.Set("server-name", "A Minecraft Classic server");
        settings.Set("motd", "Powered by Tako");
        settings.Set("port", "25565");
        settings.Set("ip", "127.0.0.1");
        settings.Set("chat-template", "<{0}>: {1}");
        settings.Set("realms-directory", "realms");
        settings.Set("autosave", "true");
        settings.Set("autosave-delay", "300");
        settings.Set("heartbeat", "true");
        settings.Set("heartbeat-url", "https://www.classicube.net/server/heartbeat/");
    }
}
