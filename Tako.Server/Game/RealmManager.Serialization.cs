using Tako.Common.Numerics;
using Tako.Definitions.Game;
using Tako.Definitions.Game.World;
using Tako.Definitions.Settings;
using Tako.Server.Settings;

namespace Tako.Server.Game;

/// <inheritdoc/>
public partial class RealmManager
{
    /// <summary>
	/// The realm file extension.
	/// </summary>
	private const string REALM_EXTENSION = "*.realm";

    /// <summary>
    /// The subfolder for the maps within the realms folder.
    /// </summary>
    private const string MAPS_FOLDER = "maps";

    /// <summary>
    /// Gets the world path for a realm.
    /// </summary>
    /// <param name="realm">The realm.</param>
    /// <returns>The world path.</returns>
    private string GetWorldPathFor(IRealm realm)
    {
        return $"{_saveDirectory}/{MAPS_FOLDER}/{realm.Name}.cw";
    }

    /// <summary>
    /// Saves a realm.
    /// </summary>
    /// <param name="realm">The realm.</param>
    private void SaveRealmWorld(IRealm realm)
    {
        realm.World?
            .Save(GetWorldPathFor(realm));
    }

    /// <summary>
    /// Loads a realm's world.
    /// </summary>
    /// <param name="realm">The realm.</param>
    private void LoadRealmWorld(IRealm realm)
    {
        var worldFile = GetWorldPathFor(realm);
        if (!File.Exists(worldFile))
            return;

        realm.GetWorldGenerator()
            .WithFilename(worldFile)
            .WithType(WorldType.LoadFromFile)
            .Build();
    }

    /// <summary>
	/// Loads a realm from the settings.
	/// </summary>
	/// <param name="settings">The settings.</param>
	private void LoadRealmFromSettings(ISettings settings)
    {
        var realm = GetOrCreateRealm(
            settings.Get("name")!,
            Enum.Parse<RealmCreationOptions>(settings.Get("options")!));

        // If the world doesn't exist, we need to create it.
        if (realm.World is null)
        {
            realm.GetWorldGenerator()
                .WithType(Enum.Parse<WorldType>(settings.Get("world-type")!))
                .WithDimensions(Vector3Int.Parse(settings.Get("dimensions")!))
                .Build();

            if (realm.AutoSave)
                SaveRealmWorld(realm);
        }
    }

    /// <summary>
    /// Loads the realms from the realms directory.
    /// </summary>
    private void LoadRealms()
    {
        // If the realms directory does not exist, we need to create it.
        // We also need to create a default realm.
        if (!Directory.Exists(_saveDirectory))
        {
            Directory.CreateDirectory(_saveDirectory);
            Directory.CreateDirectory(Path.Combine(_saveDirectory, MAPS_FOLDER));
        }

        var realms = Directory.GetFiles(_saveDirectory, REALM_EXTENSION);

        // If we have less than one realm, generate a default one.
        if (realms.Length < 1)
        {
            var realm = new FileBackedSettings($"{_saveDirectory}/default.realm", settings =>
            {
                settings.Set("name", "default");
                settings.Set("options", $"{RealmCreationOptions.AutosaveEnabled | RealmCreationOptions.LoadFromFileOnCreation}");
                settings.Set("world-type", $"{WorldType.Flat}");
                settings.Set("dimensions", $"{new Vector3Int(256, 64, 256)}");
            });
            realm.Save();

            LoadRealmFromSettings(realm);
            return;
        }

        foreach (var file in realms)
        {
            var settings = new FileBackedSettings(file, null!);
            LoadRealmFromSettings(settings);
        }
    }
}
