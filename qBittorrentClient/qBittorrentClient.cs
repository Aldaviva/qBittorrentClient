using qBittorrent.Client.Data;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace qBittorrent.Client;

public interface qBittorrentClient: IDisposable {

    /// <summary>
    /// <para>List torrents</para>
    /// <see href="https://github.com/qbittorrent/qBittorrent/wiki/WebUI-API-(qBittorrent-5.0)#get-torrent-list"/>
    /// </summary>
    /// <exception cref="HttpRequestException"></exception>
    Task<IEnumerable<TorrentInfo>> listTorrents(TorrentState stateFilter = TorrentState.ALL, string? categoryFilter = null, string? tagFilter = null, string? sort = null,
                                                bool descending = false, int limit = 0, int offset = 0, IEnumerable<string>? hashFilters = null);

    /// <summary>
    /// Get one torrent
    /// </summary>
    /// <param name="infoHash">The Info hash v1 (SHA-1) or v2 (truncated SHA-256) of the torrent to fetch. You can get this from qBittorrent's UI by right-clicking on a torrent and then clicking Copy › Torrent ID.</param>
    /// <returns>The torrent with the matching info hash, or <c>null</c> if no torrent was found in qBittorrent's list with a matching hash</returns>
    /// <exception cref="HttpRequestException"></exception>
    Task<TorrentInfo?> getTorrent(string infoHash);

    /// <summary>
    /// <para>List files in a torrents</para>
    /// <see href="https://github.com/qbittorrent/qBittorrent/wiki/WebUI-API-(qBittorrent-5.0)#get-torrent-contents"/>
    /// </summary>
    /// <exception cref="HttpRequestException"></exception>
    Task<IEnumerable<TorrentFile>> listFilesInTorrent(TorrentInfo torrent);

    /// <summary>
    /// <para>Get the application settings</para>
    /// <see href="https://github.com/qbittorrent/qBittorrent/wiki/WebUI-API-(qBittorrent-5.0)#get-application-preferences"/>
    /// </summary>
    /// <exception cref="HttpRequestException"></exception>
    Task<Preferences> getPreferences();

    /// <summary>
    /// <para>Get the application settings</para>
    /// <see href="https://github.com/qbittorrent/qBittorrent/wiki/WebUI-API-(qBittorrent-5.0)#set-application-preferences"/>
    /// </summary>
    /// <exception cref="HttpRequestException"></exception>
    Task setPreferences(Preferences newPreferences);

    /// <summary>
    /// <para>Get transfer statistics for entire program</para>
    /// <see href="https://github.com/qbittorrent/qBittorrent/wiki/WebUI-API-(qBittorrent-5.0)#get-global-transfer-info"/>
    /// </summary>
    /// <exception cref="HttpRequestException"></exception>
    Task<TransferInfo> getTransferInfo();

}