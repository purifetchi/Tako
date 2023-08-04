using System.Reflection;
using Tako.Common.Logging;
using Tako.Definitions.Network;
using Tako.Definitions.Plugins;
using Tako.Server.Plugins.Compilation;

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
    /// The plugin path.
    /// </summary>
    private readonly string _pluginPath;

    /// <summary>
    /// The plugin cache.
    /// </summary>
    private readonly PluginCache _cache;

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

        _pluginPath = _server.Settings.Get("plugin-path") ?? "plugins";
        if (!Directory.Exists(_pluginPath))
            Directory.CreateDirectory(_pluginPath);

        _plugins = new();
        _cache = new(_pluginPath);

        LoadAllPlugins();
    }

    /// <inheritdoc/>
    public void AddPlugin<TPlugin>() 
        where TPlugin : Plugin
    {
        AddPlugin(typeof(TPlugin));
    }

    /// <inheritdoc/>
    public void AddPlugin(Type pluginType)
    {
        var ctor = pluginType.GetConstructor(_ctorArray);

        if (ctor is null)
            return;

        _logger.Info($"Loading plugin of type {pluginType.Name}");

        var plugin = (Plugin)ctor.Invoke(_ctorValues);
        _plugins.Add(plugin);
    }

    /// <inheritdoc/>
    public void AddAllPluginsFromAssembly(Assembly assembly)
    {
        var plugins = assembly.GetTypes()
            .Where(type => type.IsAssignableTo(typeof(Plugin)));

        foreach (var plugin in plugins)
            AddPlugin(plugin);
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
        const string pattern = "*.cs";

        using var context = new CompilationContext();

        foreach (var plugin in Directory.GetFiles(_pluginPath, pattern))
        {
            var assembly = _cache.GetOrCompileAssemblyFor(
                plugin,
                context);

            if (assembly is not null)
                AddAllPluginsFromAssembly(assembly);
        }
    }

    /// <inheritdoc/>
    public void UnloadPlugin(Plugin plugin)
    {
        _plugins.Remove(plugin);
    }
}
