namespace Tako.Common.Logging;

/// <summary>
/// A factory for ILogger classes.
/// </summary>
public static class LoggerFactory<T>
{
	/// <summary>
	/// The logger for this class.
	/// </summary>
	private static ILogger<T>? _logger;

	/// <summary>
	/// Gets the logger for a given class.
	/// </summary>
	/// <returns>The instance for said class.</returns>
	public static ILogger<T> Get()
	{
		_logger ??= new ConsoleLogger<T>();

		return _logger;
	}
}
