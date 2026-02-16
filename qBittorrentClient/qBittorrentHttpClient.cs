using qBittorrent.Client.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Unfucked;

namespace qBittorrent.Client;

public class qBittorrentHttpClient(qBittorrentApiClient apiClient): qBittorrentClient {

    private readonly bool disposeApiClient;

    public qBittorrentHttpClient(Uri? baseUri = null): this(new qBittorrentHttpApiClient(baseUri)) {
        disposeApiClient = true;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TorrentInfo>> listTorrents(TorrentState stateFilter = TorrentState.ALL, string? categoryFilter = null, string? tagFilter = null, string? sort = null,
                                                             bool descending = false, int limit = 0, int offset = 0, IEnumerable<string>? hashFilters = null) =>
        await apiClient.send<IEnumerable<TorrentInfo>>(HttpMethod.Get, "torrents/info", query: new Dictionary<string, object?> {
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
    public async Task<TorrentInfo?> getTorrent(string infoHash) => (await listTorrents(hashFilters: [infoHash]).ConfigureAwait(false)).SingleOrDefault();

    /// <inheritdoc />
    public async Task<IEnumerable<TorrentFile>> listFilesInTorrent(TorrentInfo torrent) =>
        (await apiClient.send<IEnumerable<TorrentFile>>(HttpMethod.Get, "torrents/files", query: ("hash", torrent.hash).KeyValues<string, object?>()).ConfigureAwait(false)).Select(file =>
            file with { torrent = torrent });

    /// <inheritdoc />
    public async Task<Preferences> getPreferences() =>
        await apiClient.send<Preferences>(HttpMethod.Get, "app/preferences").ConfigureAwait(false);

    /// <inheritdoc />
    public async Task setPreferences(Preferences newPreferences) {
        using Stream _ = await apiClient.send<Stream>(HttpMethod.Post, "app/setPreferences", newPreferences).ConfigureAwait(false);
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