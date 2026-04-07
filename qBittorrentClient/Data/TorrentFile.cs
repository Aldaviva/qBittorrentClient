namespace qBittorrent.Client.Data;

/// <summary>
/// A file inside a multi-file torrent.
/// </summary>
public record TorrentFile {

    internal TorrentInfo? torrent { get; init; }

    /// <summary>
    /// The absolute path of this file. Example: <c>E:\Downloads\my torrent\my subfolder\my file.txt</c>
    /// </summary>
    public string filePathAbsolute => torrent?.savePath is {} dir ? Path.Combine(dir, filePathRelativeToSavePath) : filePathRelativeToSavePath;

    /// <summary>Index (ordered position) of where the file was declared in the torrent</summary>
    public int index { get; init; }

    /// <summary>The relative path of this file to the torrent's save directory. Example: <c>my subfolder\my file.txt</c>. To get the absolute path instead, use <see cref="filePathAbsolute"/>.</summary>
    [JsonPropertyName("name")]
    public required string filePathRelativeToSavePath { get; init; }

    /// <summary>Number of bytes for this file</summary>
    public ulong size { get; init; }

    /// <summary>How many of the file's bytes have been downloaded, in the range [0.0, 1.0].</summary>
    public float progress { get; init; }

    /// <summary>Urgency of downloading this file compared to other files.</summary>
    public FilePriority priority {
        get;
        // https://github.com/qbittorrent/qBittorrent/issues/11450
        init => field = value == (FilePriority) 4 ? FilePriority.NORMAL : value;
    }

    /// <summary>
    /// The priority at which this file should be downloaded, compared to other files in the torrent.
    /// </summary>
    public enum FilePriority {

        /// <summary>
        /// This file should not be downloaded at all. Part or all of it may still be downloaded if it is contained in a chunk that overlaps another file which has a higher priority.
        /// </summary>
        DO_NOT_DOWNLOAD = 0,

        /// <summary>
        /// Download with default priority.
        /// </summary>
        NORMAL = 1,

        /// <summary>
        /// Download with priority one degree higher than <see cref="NORMAL"/>.
        /// </summary>
        HIGH = 6,

        /// <summary>
        /// Download with highest priority.
        /// </summary>
        MAXIMUM = 7

    }

}