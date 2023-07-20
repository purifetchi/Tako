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
