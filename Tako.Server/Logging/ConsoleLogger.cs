using Tako.Definitions.Logging;

namespace Tako.Server.Logging;

/// <summary>
/// A logger that logs to the console.
/// </summary>
/// <typeparam name="T">The class to log for.</typeparam>
public class ConsoleLogger<T> : ILogger<T>
{
	/// <inheritdoc/>
	public void Debug(string message)
	{
		Console.WriteLine($"[DEBUG ({DateTime.Now})] {message}");
	}

	/// <inheritdoc/>
	public void Error(string message)
	{
		Console.WriteLine($"[ERROR ({DateTime.Now})] {message}");
	}

	/// <inheritdoc/>
	public void Fatal(string message)
	{
		Console.WriteLine($"[FATAL ({DateTime.Now})] {message}");
	}

	/// <inheritdoc/>
	public void Info(string message)
	{
		Console.WriteLine($"[INFO ({DateTime.Now})] {message}");
	}

	/// <inheritdoc/>
	public void Warn(string message)
	{
		Console.WriteLine($"[WARN ({DateTime.Now})] {message}");
	}
}
