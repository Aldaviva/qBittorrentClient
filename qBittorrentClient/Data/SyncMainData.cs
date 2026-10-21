namespace qBittorrent.Client.Data;

internal record SyncMainData {

    public bool fullUpdate { get; init; }

    [JsonPropertyName("rid")]
    public int responseId { get; init; }

    public ServerState? serverState { get; init; }

    internal record ServerState {

        public bool? sessionState { get; init; }

    }

}