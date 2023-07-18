namespace Tako.Definitions.Logging;

/// <summary>
/// An interface for a generic logger for a given class.
/// </summary>
/// <typeparam name="T">The type of the class to log for.</typeparam>
public interface ILogger<T>
{
	/// <summary>
	/// Sends an informational message.
	/// </summary>
	/// <param name="message">The message.</param>
	void Info(string message);

	/// <summary>
	/// Sends a warning message.
	/// </summary>
	/// <param name="message">The message.</param>
	void Warn(string message);

	/// <summary>
	/// Sends an error message.
	/// </summary>
	/// <param name="message">The message.</param>
	void Error(string message);

	/// <summary>
	/// Sends a fatal message.
	/// </summary>
	/// <param name="message">The message.</param>
	void Fatal(string message);

	/// <summary>
	/// Sends a debug message.
	/// </summary>
	/// <param name="message">The message.</param>
	void Debug(string message);
}
