namespace Tako.Definitions.Game;

/// <summary>
/// Creation options for a Realm.
/// </summary>
[Flags]
public enum RealmCreationOptions
{
    None = 0,
    AutosaveEnabled = 1,
    LoadFromFileOnCreation = 2
}
