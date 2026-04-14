namespace qBittorrent.Client;

/// <inheritdoc cref="qBittorrentClient" />
public sealed class qBittorrentApiClient(qBittorrentTransport apiClient): qBittorrentClient {

    private readonly bool disposeApiClient;

    /// <summary>Construct a new instance of a qBittorrent HTTP API client to communicate with a specific qBittorrent web server.</summary>
    /// <param name="baseUri"></param>
    public qBittorrentApiClient(Uri? baseUri = null): this(new qBittorrentHttpTransport(baseUri)) =>
        disposeApiClient = true;

    /// <inheritdoc />
    public async Task<IReadOnlyList<TorrentInfo>> listTorrents(TorrentStateFilter stateFilter = TorrentStateFilter.ALL, string? categoryFilter = null, string? tagFilter = null,
                                                               IEnumerable<string>? hashFilters = null, int limit = 0, int offset = 0, string? sort = null, bool descending = false) =>
        await apiClient.send<IReadOnlyList<TorrentInfo>>(HttpMethod.Get, "torrents/info", query: new Dictionary<string, object?> {
            { "filter", stateFilter },
            { "category", categoryFilter },
            { "tag", tagFilter },
            { "sort", sort },
            { "reverse", descending },
            { "limit", limit },
            { "offset", offset },
            { "hashes", hashFilters?.Join('|') }
        }).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<TorrentInfo?> getTorrent(string infoHash) =>
        (await listTorrents(hashFilters: [infoHash], limit: 1).ConfigureAwait(false)).SingleOrDefault();

    /// <inheritdoc />
    public async Task<IReadOnlyList<TorrentFile>> listFilesInTorrent(TorrentInfo torrent) =>
        (await apiClient.send<IEnumerable<TorrentFile>>(HttpMethod.Get, "torrents/files", query: ("hash", torrent.hash).KeyValues<string, object?>()).ConfigureAwait(false))
        .Select(file => file with { torrent = torrent }).ToList().AsReadOnly();

    /// <inheritdoc />
    public async Task<Preferences> getPreferences() =>
        await apiClient.send<Preferences>(HttpMethod.Get, "app/preferences").ConfigureAwait(false);

    /// <inheritdoc />
    public async Task setPreferences(Preferences newPreferences) {
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        await using
#else
        using
#endif
            Stream _ = await apiClient.send<Stream>(HttpMethod.Post, "app/setPreferences", newPreferences).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TransferInfo> getTransferInfo() =>
        await apiClient.send<TransferInfo>(HttpMethod.Get, "transfer/info").ConfigureAwait(false);

    /// <inheritdoc />
    public void Dispose() {
        if (disposeApiClient) {
            apiClient.Dispose();
        }
    }

}