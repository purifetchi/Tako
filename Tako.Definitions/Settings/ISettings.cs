namespace Tako.Definitions.Settings;

/// <summary>
/// An interface for server settings (usually stored in server.properties).
/// </summary>
public interface ISettings
{
	/// <summary>
	/// Gets a value by its key.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <returns>The value.</returns>
	string? Get(string key);

	/// <summary>
	/// Gets a value by its key casting it to a specific type, with the fallback returned otherwise.
	/// </summary>
	/// <typeparam name="T">The type to cast to.</typeparam>
	/// <param name="key">The key.</param>
	/// <param name="fallback">The fallback.</param>
	/// <returns>The value.</returns>
	T Get<T>(string key, T fallback);

	/// <summary>
	/// Sets the value for a given key.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="value">The value</param>
	void Set(string key, string value);

	/// <summary>
	/// Saves the settings.
	/// </summary>
	void Save();
}
