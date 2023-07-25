using Tako.Common.Network.Serialization;
using Tako.Common.Numerics;
using Tako.Definitions.Network.Packets;

namespace Tako.Server.Network.Packets.Client;

/// <summary>
/// Sent frequently (even while not moving) by the player with the player's current location and orientation. 
/// 
/// Player ID is always -1 (255), referring to itself. 
/// </summary>
public struct PositionAndOrientationPacket : IClientPacket
{
    /// <summary>
    /// The player id.
    /// </summary>
    public sbyte PlayerId { get; set; }

    /// <summary>
    /// The X coordinate.
    /// </summary>
    public FShort X { get; set; }

    /// <summary>
    /// The Y coordinate.
    /// </summary>
    public FShort Y { get; set; }

    /// <summary>
    /// The Z coordinate.
    /// </summary>
    public FShort Z { get; set; }

    /// <summary>
    /// The yaw.
    /// </summary>
    public byte Yaw { get; set; }

    /// <summary>
    /// The pitch.
    /// </summary>
    public byte Pitch { get; set; }

    /// <inheritdoc/>
    public void Deserialize(ref NetworkReader reader)
    {
        PlayerId = reader.Read<sbyte>();
        X = reader.ReadFShort();
        Y = reader.ReadFShort();
        Z = reader.ReadFShort();
        Yaw = reader.Read<byte>();
        Pitch = reader.Read<byte>();
    }
}
