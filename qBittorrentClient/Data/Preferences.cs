using System.Text.Json.Serialization;

namespace qBittorrent.Client.Data;

/// <summary>
/// Other preferences are excluded
/// <see href="https://github.com/qbittorrent/qBittorrent/wiki/WebUI-API-(qBittorrent-5.0)#get-application-preferences" />
/// </summary>
public record Preferences {

    [JsonPropertyName("listen_port")]
    public ushort listeningPort { get; set; }

}