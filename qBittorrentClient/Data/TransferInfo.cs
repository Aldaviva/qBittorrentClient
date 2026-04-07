namespace qBittorrent.Client.Data;

/// <summary>Aggregated transfer statistics for the running qBittorrent process instance.</summary>
public record TransferInfo {

    /// <summary>Internet reachability.</summary>
    [JsonPropertyName("connection_status")]
    public ConnectionStatus connectionStatus { get; set; }

    /// <summary>How many nodes you're connected to in the Distributed Hash Table.</summary>
    [JsonPropertyName("dht_nodes")]
    public uint dhtNodes { get; set; }

    /// <summary>Quantity of data (in bytes) downloaded this session.</summary>
    [JsonPropertyName("dl_info_data")]
    public ulong dlInfoData { get; set; }

    /// <summary>Current speed (in bytes/second) being downloaded across the entire session.</summary>
    [JsonPropertyName("dl_info_speed")]
    public uint dlInfoSpeed { get; set; }

    /// <summary>Download speed limit (in bytes/second), or <c>0</c> for unlimited download speed.</summary>
    [JsonPropertyName("dl_rate_limit")]
    public uint dlRateLimit { get; set; }

    /// <summary>Quantity of data (in bytes) uploaded this session.</summary>
    [JsonPropertyName("up_info_data")]
    public ulong upInfoData { get; set; }

    /// <summary>Current speed (in bytes/second) being uploaded across the entire session.</summary>
    [JsonPropertyName("up_info_speed")]
    public uint upInfoSpeed { get; set; }

    /// <summary>Upload speed limit (in bytes/second), or <c>0</c> for unlimited upload speed.</summary>
    [JsonPropertyName("up_rate_limit")]
    public uint upRateLimit { get; set; }

    /// <summary>Internet reachability states.</summary>
    public enum ConnectionStatus {

        /// <summary>Able to make inbound and outbound connections.</summary>
        CONNECTED,

        /// <summary>Not able to accept inbound connections, possibly due to a firewall blocking traffic to qBittorrent on its listening port.</summary>
        FIREWALLED,

        /// <summary>No connections, possibly due to being disconnected from the Internet.</summary>
        DISCONNECTED

    }

}