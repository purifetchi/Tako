using System.Reflection;
using System.Security.Cryptography;
using Tako.Common.Logging;
using Tako.Server.Logging;
using Tako.Server.Plugins.Compilation;

namespace Tako.Server.Plugins;

/// <summary>
/// A cache for compiled plugins.
/// </summary>
internal class PluginCache
{
    /// <summary>
    /// The cache subdirectory.
    /// </summary>
    private const string CACHE_SUBDIRECTORY = "/cache";

    /// <summary>
    /// The cache directory.
    /// </summary>
    private readonly string _cacheDirectory;

    /// <summary>
    /// Creates a new plugin cache.
    /// </summary>
    /// <param name="pluginDirectory">The plugin directory.</param>
    public PluginCache(string pluginDirectory)
    {
        _cacheDirectory = pluginDirectory + CACHE_SUBDIRECTORY;

        InitializeCache();
    }

    /// <summary>
    /// Initializes the cache.
    /// </summary>
    private void InitializeCache()
    {
        if (!Directory.Exists(_cacheDirectory))
            Directory.CreateDirectory(_cacheDirectory);
    }

    /// <summary>
    /// Gets the cache path for a file.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns>The cache path.</returns>
    public string GetCachePathFor(string file)
    {
        using var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        var sha = MD5.HashData(fs);

        return _cacheDirectory + 
            Path.DirectorySeparatorChar + 
            BitConverter.ToString(sha).Replace("-", "").ToLower() + 
            ".dll";
    }

    /// <summary>
    /// Gets the assembly from cache or compiles it.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="context">The compilation context.</param>
    /// <returns>The assembly.</returns>
    public Assembly? GetOrCompileAssemblyFor(
        string file, 
        CompilationContext? context)
    {
        var cachePath = GetCachePathFor(file);

        if (File.Exists(cachePath))
            return Assembly.LoadFrom(cachePath);

        var compile = context?.Compile(file, cachePath);
        return compile;
    }
}
