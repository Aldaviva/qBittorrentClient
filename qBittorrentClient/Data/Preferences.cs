namespace qBittorrent.Client.Data;

/// <summary>
/// Other preferences are excluded
/// <see href="https://github.com/qbittorrent/qBittorrent/wiki/WebUI-API-(qBittorrent-5.0)#get-application-preferences" />
/// </summary>
public record Preferences {

    /// <summary>
    /// The TCP/UDP port number on which to listen for incoming connections.
    /// </summary>
    [JsonPropertyName("listen_port")]
    public ushort listeningPort { get; set; }

}