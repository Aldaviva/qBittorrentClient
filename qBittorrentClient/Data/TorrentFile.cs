using System.IO;
using System.Text.Json.Serialization;

namespace qBittorrent.Client.Data;

/// <summary>
/// A file inside a multi-file torrent.
/// </summary>
/// <param name="filePathRelativeToSavePath">The relative path of this file to the torrent's save directory. Example: <c>my subfolder\my file.txt</c>. To get the absolute path instead, use <see cref="filePathAbsolute"/>.</param>
/// <param name="size">Number of bytes for this file</param>
/// <param name="progress">How many of the file's bytes have been downloaded, in the range [0.0, 1.0].</param>
/// <param name="priority">Urgency of downloading this file compared to other files.</param>
public record TorrentFile(
    int index,
    [property: JsonPropertyName("name")] string filePathRelativeToSavePath,
    ulong size,
    float progress,
    TorrentFile.FilePriority priority,
    [property: JsonPropertyName("is_seed")] bool isSeeding
) {

    internal TorrentInfo? torrent { get; set; }

    /// <summary>
    /// The absolute path of this file. Example: <c>E:\Downloads\my torrent\my subfolder\my file.txt</c>
    /// </summary>
    public string filePathAbsolute => torrent?.savePath is { } dir ? Path.Combine(dir, filePathRelativeToSavePath) : filePathRelativeToSavePath;

    public enum FilePriority {

        DO_NOT_DOWNLOAD = 0,
        NORMAL          = 1,
        HIGH            = 6,
        MAXIMUM         = 7

    }

}