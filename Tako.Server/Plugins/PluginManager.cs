using Tako.Common.Logging;
using Tako.Definitions.Network;
using Tako.Definitions.Plugins;
using Tako.Server.Logging;

namespace Tako.Server.Plugins;

/// <summary>
/// The plugin manager.
/// </summary>
public class PluginManager : IPluginManager
{
    /// <summary>
    /// A list of all the plugins.
    /// </summary>
    private readonly List<Plugin> _plugins;

    /// <summary>
    /// The server.
    /// </summary>
    private readonly IServer _server;

    /// <summary>
    /// The constructor lookup array.
    /// </summary>
    private static readonly Type[] _ctorArray = new[] { typeof(IServer) };

    /// <summary>
    /// The constructor values array.
    /// </summary>
    private readonly object?[] _ctorValues;

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<PluginManager> _logger = LoggerFactory<PluginManager>.Get();

    /// <summary>
    /// Creates a new plugin manager.
    /// </summary>
    /// <param name="server">The server to create it for.</param>
    public PluginManager(IServer server)
    {
        _server = server;
        _ctorValues = new[] { _server };

        _plugins = new();
    }

    /// <inheritdoc/>
    public void AddPlugin<TPlugin>() 
        where TPlugin : Plugin
    {
        var ctor = typeof(TPlugin)
            .GetConstructor(_ctorArray);

        if (ctor is null)
            return;

        _logger.Info($"Loading plugin of type {typeof(TPlugin).Name}");

        var plugin = (Plugin)ctor.Invoke(_ctorValues);
        _plugins.Add(plugin);
    }

    /// <inheritdoc/>
    public IEnumerable<Plugin> GetAllPlugins()
    {
        return _plugins;
    }

    /// <inheritdoc/>
    public Plugin? GetPluginByName(string name)
    {
        return _plugins.FirstOrDefault(p => p.Name == name);
    }

    /// <inheritdoc/>
    public void LoadAllPlugins()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void UnloadPlugin(Plugin plugin)
    {
        _plugins.Remove(plugin);
    }
}
