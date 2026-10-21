using System.Collections;
using Unfucked.HTTP.Exceptions;

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
        using var response = await apiClient.send(HttpMethod.Post, "app/setPreferences", newPreferences).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TransferInfo> getTransferInfo() =>
        await apiClient.send<TransferInfo>(HttpMethod.Get, "transfer/info").ConfigureAwait(false);

    private int   mostRecentResponseId;
    private bool  mostRecentSessionPausedState;
    private bool? isSessionPausingSupported;

    /// <inheritdoc />
    public async Task<bool> isSessionPaused() {
        if (isSessionPausingSupported is null) {
            Version webApiVersion = await apiClient.send<Version>(HttpMethod.Get, "app/webapiVersion").ConfigureAwait(false);
            isSessionPausingSupported = webApiVersion >= new Version(2, 16, 2);
        }

        if (isSessionPausingSupported is true) {
            SyncMainData syncMainData = await apiClient.send<SyncMainData>(HttpMethod.Get, "sync/maindata",
                query: IDictionary.Singleton<string, object?>("rid", mostRecentResponseId)).ConfigureAwait(false);

            mostRecentResponseId = syncMainData.responseId;
            return syncMainData.serverState?.sessionState is {} isPaused
                ? mostRecentSessionPausedState = isPaused
                : mostRecentSessionPausedState; // excluded from delta response, use cached value
        } else {
            throw new MissingMemberException("Session pausing was added in qBittorrent 5.3.0 (Web API 2.16.2)");
        }
    }

    /// <inheritdoc />
    public async Task setSessionPaused(bool pause) {
        if (isSessionPausingSupported != false) {
            try {
                using Stream response = await apiClient.send<Stream>(HttpMethod.Post, $"transfer/{(pause ? "pauseSession" : "resumeSession")}").ConfigureAwait(false);
                isSessionPausingSupported = true;
            } catch (NotFoundException) {
                isSessionPausingSupported = false;
            }
        }
        throw new MissingMemberException("Session pausing was added in qBittorrent 5.3.0 (Web API 2.16.2)");
    }

    /// <inheritdoc />
    public void Dispose() {
        if (disposeApiClient) {
            apiClient.Dispose();
        }
    }

}