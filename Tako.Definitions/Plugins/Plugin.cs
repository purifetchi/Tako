using Tako.Definitions.Network;

namespace Tako.Definitions.Plugins;

/// <summary>
/// A base plugin class.
/// </summary>
public abstract class Plugin
{
    /// <summary>
    /// The server.
    /// </summary>
    public IServer Server { get; private set; }

    /// <summary>
    /// The name of the plugin.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Creates a new instance of this plugin with the given server.
    /// </summary>
    /// <param name="server">The server.</param>
    public Plugin(IServer server)
    {
        Server = server;
    }
}
