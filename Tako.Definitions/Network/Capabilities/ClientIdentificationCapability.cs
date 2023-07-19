namespace Tako.Definitions.Network.Capabilities;

/// <summary>
/// Used to identify if a client understands Classic Protocol Extensions.
/// </summary>
public enum ClientIdentificationCapability : byte
{
	Default = 0x00,
	ClassicProtocolExtensionsCompliant = 0x42
}
