using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Tako.Common.Logging;
using Tako.Server.Logging;

namespace Tako.Server.Plugins.Compilation;

/// <summary>
/// The plugin compilation context.
/// </summary>
internal class CompilationContext : IDisposable
{
    /// <summary>
    /// The reference list.
    /// </summary>
    private readonly Lazy<List<MetadataReference>> _references;

    /// <summary>
    /// The runtime path.
    /// </summary>
    private readonly string _runtimePath;

    /// <summary>
    /// The compilation options.
    /// </summary>
    private readonly CSharpCompilationOptions _opts;

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<CompilationContext> _logger = LoggerFactory<CompilationContext>.Get();

    /// <summary>
    /// Static initializer for the compiler.
    /// Sets up the runtime path.
    /// </summary>
    public CompilationContext()
    {
        _runtimePath = Path.GetDirectoryName(typeof(object).Assembly.Location) +
                 Path.DirectorySeparatorChar;

        _opts = new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            optimizationLevel: OptimizationLevel.Release);

        _references = new Lazy<List<MetadataReference>>(() =>
        {
            var list = new List<MetadataReference>();

            AddNetRuntimeReference("System.Private.CoreLib.dll", list);
            AddNetRuntimeReference("System.Runtime.dll", list);
            AddNetRuntimeReference("System.Console.dll", list);
            AddNetRuntimeReference("netstandard.dll", list);
            AddNetRuntimeReference("System.Linq.dll", list);
            AddNetRuntimeReference("System.IO.dll", list);
            AddNetRuntimeReference("System.Net.Http.dll", list);
            AddNetRuntimeReference("System.Net.Primitives.dll", list);

            AddRegularReference("Tako.Definitions.dll", list);
            AddRegularReference("Tako.Common.dll", list);
            AddRegularReference("Tako.NBT.dll", list);

            return list;
        });
    }

    /// <summary>
    /// Adds a reference to a file.
    /// </summary>
    /// <param name="file">The file.</param>
    private void AddNetRuntimeReference(string file, List<MetadataReference> list)
    {
        var reference = MetadataReference.CreateFromFile(_runtimePath + file);
        list.Add(reference);
    }

    /// <summary>
    /// Adds a regular reference to the compilation.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="list">The list.</param>
    private void AddRegularReference(string file, List<MetadataReference> list)
    {
        var reference = MetadataReference.CreateFromFile(file);
        list.Add(reference);
    }

    /// <summary>
    /// Compiles a single plugin .cs file at a given path.
    /// </summary>
    /// <param name="path">The path.</param>
    public Assembly? Compile(string path, string output)
    {
        _logger.Info($"Compiling plugin {path}");

        var source = File.ReadAllText(path);
        var syntaxTree = SyntaxFactory.ParseSyntaxTree(source);

        var compilation = CSharpCompilation.Create(Path.GetFileName(path))
            .WithOptions(_opts)
            .WithReferences(_references.Value)
            .AddSyntaxTrees(syntaxTree);

        var result = compilation.Emit(output);
        if (!result.Success)
        {
            Console.WriteLine("Compilation failed.");
            foreach (var line in result.Diagnostics)
                Console.WriteLine(line);

            return null;
        }

        return Assembly.LoadFrom(output);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_references.IsValueCreated)
            _references.Value.Clear();

        GC.SuppressFinalize(this);
    }
}
