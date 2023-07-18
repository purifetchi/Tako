using Tako.Definitions.Logging;

namespace Tako.Server.Logging;

/// <summary>
/// A logger that logs to the console.
/// </summary>
/// <typeparam name="T">The class to log for.</typeparam>
public class ConsoleLogger<T> : ILogger<T>
{
	public void Debug(string message)
	{
		Console.WriteLine($"[DEBUG] {message}");
	}

	public void Error(string message)
	{
		Console.WriteLine($"[ERROR] {message}");
	}

	public void Fatal(string message)
	{
		Console.WriteLine($"[FATAL] {message}");
	}

	public void Info(string message)
	{
		Console.WriteLine($"[INFO] {message}");
	}

	public void Warn(string message)
	{
		Console.WriteLine($"[WARN] {message}");
	}
}
