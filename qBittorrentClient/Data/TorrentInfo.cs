namespace qBittorrent.Client.Data;

/// <summary>
/// A torrent that contains one or more files.
/// </summary>
/// <param name="hash">Info hash V1 or V2.</param>
/// <param name="name">Name of the torrent as it appears in the torrent list, not necessarily the filename.</param>
/// <param name="contentPath">
/// <para>For single-file torrents, the absolute path of the file. Example: <c>E:\Downloads\2025-05-13-raspios-bookworm-armhf.img.xz</c></para>
/// <para>For multi-file torrents, the absolute path of the topmost folder inside the torrent (not the directory you chose in the Add Torrent dialog box). This isn't very useful, and you should use <see cref="TorrentFile.filePathAbsolute"/> instead. Example: </para>
/// </param>
/// <param name="savePath">The absolute path of the directory you chose in the Add Torrent dialog box, regardless of whether this is a single- or multi-file torrent. Example: <c>E:\Downloads</c></param>
/// <param name="size">Sum of bytes of files that should be downloaded. Files marked Do Not Download are excluded.</param>
/// <param name="tags">Comma-separated list of tags for this torrent.</param>
public record TorrentInfo(string hash, string name, string contentPath, string savePath, ulong size, string tags);

public enum TorrentState {

    ALL,
    DOWNLOADING,
    SEEDING,
    COMPLETED,
    STOPPED,
    ACTIVE,
    INACTIVE,
    RUNNING,
    STALLED,
    STALLED_UPLOADING,
    STALLED_DOWNLOADING,
    ERRORED

}