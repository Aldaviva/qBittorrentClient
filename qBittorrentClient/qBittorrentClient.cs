namespace qBittorrent.Client;

/// <summary>
/// <para>Library entry point and high-level client for qBittorrent's HTTP API.</para>
/// <para>To get started, construct a new instance of <see cref="qBittorrentApiClient"/>.</para>
/// </summary>
public interface qBittorrentClient: IDisposable {

    /// <summary>
    /// <para>List torrents.</para>
    /// <see href="https://github.com/qbittorrent/qBittorrent/wiki/WebUI-API-(qBittorrent-5.0)#get-torrent-list"/>
    /// </summary>
    /// <exception cref="HttpRequestException"></exception>
    Task<IReadOnlyList<TorrentInfo>> listTorrents(TorrentStateFilter stateFilter = TorrentStateFilter.ALL, string? categoryFilter = null, string? tagFilter = null,
                                                  IEnumerable<string>? hashFilters = null, int limit = 0, int offset = 0, string? sort = null, bool descending = false);

    /// <summary>
    /// <para>Get one torrent.</para>
    /// <para>Uses the view of a torrent from the list response, not the properties response.</para>
    /// <see href="https://github.com/qbittorrent/qBittorrent/wiki/WebUI-API-(qBittorrent-5.0)#get-torrent-list"/>
    /// </summary>
    /// <param name="infoHash">The Info hash v1 (SHA-1) or v2 (truncated SHA-256) of the torrent to fetch. You can get this from qBittorrent's UI by right-clicking on a torrent and then clicking Copy › Torrent ID.</param>
    /// <returns>The torrent with the matching info hash, or <c>null</c> if no torrent was found in qBittorrent's list with a matching hash</returns>
    /// <exception cref="HttpRequestException"></exception>
    Task<TorrentInfo?> getTorrent(string infoHash);

    /// <summary>
    /// <para>List files in a torrent.</para>
    /// <see href="https://github.com/qbittorrent/qBittorrent/wiki/WebUI-API-(qBittorrent-5.0)#get-torrent-contents"/>
    /// </summary>
    /// <exception cref="HttpRequestException"></exception>
    Task<IReadOnlyList<TorrentFile>> listFilesInTorrent(TorrentInfo torrent);

    /// <summary>
    /// <para>Get the application settings.</para>
    /// <see href="https://github.com/qbittorrent/qBittorrent/wiki/WebUI-API-(qBittorrent-5.0)#get-application-preferences"/>
    /// </summary>
    /// <exception cref="HttpRequestException"></exception>
    Task<Preferences> getPreferences();

    /// <summary>
    /// <para>Get the application settings.</para>
    /// <see href="https://github.com/qbittorrent/qBittorrent/wiki/WebUI-API-(qBittorrent-5.0)#set-application-preferences"/>
    /// </summary>
    /// <exception cref="HttpRequestException"></exception>
    Task setPreferences(Preferences newPreferences);

    /// <summary>
    /// <para>Get transfer statistics for entire program.</para>
    /// <see href="https://github.com/qbittorrent/qBittorrent/wiki/WebUI-API-(qBittorrent-5.0)#get-global-transfer-info"/>
    /// </summary>
    /// <exception cref="HttpRequestException"></exception>
    Task<TransferInfo> getTransferInfo();

}