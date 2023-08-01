namespace Tako.Server.Game.Players;

/// <summary>
/// Various strings sent to the client when we disconnect them
/// </summary>
internal static class DisconnectMessages
{
    /// <summary>
    /// A client has sent us a corrupted packet.
    /// </summary>
    public const string CORRUPTED_PACKET_SENT = "Corrupted packet sent.";

    /// <summary>
    /// A client is trying to log in with an unknown protocol to us.
    /// </summary>
    public const string PROTOCOL_MISMATCH = "Protocol mismatch.";

    /// <summary>
    /// A client sent us a bad authentication key.
    /// </summary>
    public const string INVALID_AUTHENTICATION_KEY = "Invalid authentication key.";

    /// <summary>
    /// A client tried to set an invalid block.
    /// </summary>
    public const string INVALID_BLOCK_ID = "Invalid block id {0}.";
}
