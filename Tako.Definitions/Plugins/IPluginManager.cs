using System.Reflection;

namespace Tako.Definitions.Plugins;

/// <summary>
/// A plugin manager.
/// </summary>
public interface IPluginManager
{
    /// <summary>
    /// Gets all of the loaded plugins.
    /// </summary>
    /// <returns>The plugins.</returns>
    IEnumerable<Plugin> GetAllPlugins();

    /// <summary>
    /// Gets a plugin by name.
    /// </summary>
    /// <param name="name">Its name.</param>
    /// <returns>The plugin.</returns>
    Plugin? GetPluginByName(string name);

    /// <summary>
    /// Loads all the plugins from the directory.
    /// </summary>
    void LoadAllPlugins();

    /// <summary>
    /// Adds and instantiates a plugin of a type.
    /// </summary>
    /// <typeparam name="TPlugin">The type of the plugin.</typeparam>
    void AddPlugin<TPlugin>()
        where TPlugin : Plugin;

    /// <summary>
    /// Adds and instantiates a plugin of a type.
    /// </summary>
    /// <param name="plugin">The plugin type.</param>
    void AddPlugin(Type plugin);

    /// <summary>
    /// Loads all the plugins from an assembly.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    void AddAllPluginsFromAssembly(Assembly assembly);

    /// <summary>
    /// Unloads a plugin.
    /// </summary>
    /// <param name="plugin">The plugin to unload.</param>
    void UnloadPlugin(Plugin plugin);
}
