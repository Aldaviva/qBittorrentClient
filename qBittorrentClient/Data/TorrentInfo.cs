using System.Collections.Immutable;

namespace qBittorrent.Client.Data;

/// <summary>
/// A torrent that contains one or more files.
/// </summary>
public record TorrentInfo {

    /// <summary>Info hash V1 or V2.</summary>
    public required string hash { get; init; }

    /// <summary>Name of the torrent as it appears in the torrent list, not necessarily the filename.</summary>
    public required string name { get; init; }

    /// <summary>
    /// <para>For single-file torrents, the absolute path of the file. Example: <c>E:\Downloads\2025-05-13-raspios-bookworm-armhf.img.xz</c></para>
    /// <para>For multi-file torrents, the absolute path of the topmost folder inside the torrent (not the directory you chose in the Add Torrent dialog box). This isn't very useful, and you should use <see cref="TorrentFile.filePathAbsolute"/> instead. Example: </para>
    /// </summary>
    public required string contentPath { get; init; }

    /// <summary>The absolute path of the directory you chose in the Add Torrent dialog box, regardless of whether this is a single- or multi-file torrent. Example: <c>E:\Downloads</c></summary>
    public required string savePath { get; init; }

    /// <summary>Sum of bytes of files that should be downloaded. Files marked Do Not Download are excluded.</summary>
    public ulong size { get; init; }

    /// <summary>Comma-separated list of tags for this torrent.</summary>
    [JsonIgnore]
    public IImmutableSet<string> tags { get; private init; } = ImmutableHashSet<string>.Empty;

    [JsonInclude]
    private string rawTags {
        get => tags.Join(',');
        init => tags = value.Split(',').ToImmutableHashSet();
    }

    /// <summary>
    /// Percentage of download progress, in the range from 0 (no file contents downloaded) to 1 (all non-skipped file contents downloaded), inclusive.
    /// </summary>
    public float progress { get; init; }

    /// <summary>
    /// Download and upload state of the torrent.
    /// </summary>
    public TorrentState state { get; init; }

}

/// <summary>States of torrents to use when filtering a list of torrents</summary>
public enum TorrentStateFilter {

    /// <summary>Return all torrents without any state filtering</summary>
    ALL,

    /// <summary>Torrent is being downloaded and data is being transferred</summary>
    DOWNLOADING,

    /// <summary>Torrent is being seeded and data is being transferred</summary>
    SEEDING,

    /// <summary>Torrent is stopped and has finished downloading</summary>
    COMPLETED,

    /// <summary>Torrent is stopped and has NOT finished downloading</summary>
    STOPPED,

    /// <summary>Torrent is uploading or downloading, and is either connected to peers, or it is trying to fetch metadata</summary>
    ACTIVE,

    /// <summary>Torrent is uploading or downloading, but it is not connected to any peers</summary>
    INACTIVE,

    /// <summary>Torrent is downloading or uploading, or is stalled with no connections</summary>
    RUNNING,

    /// <summary>Torrent is being uploaded or downloaded, but is not connected to any peers</summary>
    STALLED,

    /// <summary>Torrent is being seeded, but is not connected to any peers</summary>
    STALLED_UPLOADING,

    /// <summary>Torrent is being downloaded, but is not connected to any peers</summary>
    STALLED_DOWNLOADING,

    /// <summary>Some error occurred, applies to stopped torrents</summary>
    ERRORED

}

/// <summary>
/// Download and upload state of a torrent.
/// </summary>
public enum TorrentState {

    /// <summary>Unknown status</summary>
    [JsonStringEnumMemberName("unknown")]
    UNKNOWN,

    /// <summary>Some error occurred, applies to paused torrents</summary>
    [JsonStringEnumMemberName("error")]
    ERROR,

    /// <summary>Torrent data files is missing</summary>
    [JsonStringEnumMemberName("missingFiles")]
    MISSING_FILES,

    /// <summary>Torrent is being seeded and data is being transferred</summary>
    [JsonStringEnumMemberName("uploading")]
    UPLOADING,

    /// <summary>Queuing is enabled and torrent is queued for upload</summary>
    [JsonStringEnumMemberName("queuedUP")]
    QUEUED_UPLOADING,

    /// <summary>Torrent is being seeded, but no connection were made</summary>
    [JsonStringEnumMemberName("stalledUP")]
    STALLED_UPLOADING,

    /// <summary>Torrent has finished downloading and is being checked</summary>
    [JsonStringEnumMemberName("checkingUP")]
    CHECKING_UPLOADING,

    /// <summary>Torrent is forced to upload and ignore queue limit</summary>
    [JsonStringEnumMemberName("forcedUP")]
    FORCED_UPLOADING,

    /// <summary>Torrent is allocating disk space for download</summary>
    [JsonStringEnumMemberName("allocating")]
    ALLOCATING,

    /// <summary>Torrent is being downloaded and data is being transferred</summary>
    [JsonStringEnumMemberName("downloading")]
    DOWNLOADING,

    /// <summary>Torrent has just started downloading and is fetching metadata</summary>
    [JsonStringEnumMemberName("metaDL")]
    METADATA_DOWNLOADING,

    // Undocumented: https://github.com/qbittorrent/qBittorrent/issues/21561
    /// <summary>Torrent is forced to start downloading and is fetching metadata</summary>
    [JsonStringEnumMemberName("forcedMetaDL")]
    FORCED_METADATA_DOWNLOADING,

    /// <summary>Queuing is enabled and torrent is queued for download</summary>
    [JsonStringEnumMemberName("queuedDL")]
    QUEUED_DOWNLOADING,

    /// <summary>Torrent is being downloaded, but no connection were made</summary>
    [JsonStringEnumMemberName("stalledDL")]
    STALLED_DOWNLOADING,

    /// <summary>Same as <see cref="CHECKING_UPLOADING"/>, but torrent has NOT finished downloading</summary>
    [JsonStringEnumMemberName("checkingDL")]
    CHECKING_DOWNLOADING,

    /// <summary>Torrent is forced to downloading to ignore queue limit</summary>
    [JsonStringEnumMemberName("forcedDL")]
    FORCED_DOWNLOADING,

    /// <summary>Checking resume data on qBittorrent startup</summary>
    [JsonStringEnumMemberName("checkingResumeData")]
    CHECKING_RESUMEDATA,

    /// <summary>Torrent is moving to another location</summary>
    [JsonStringEnumMemberName("moving")]
    MOVING,

    // Undocumented: https://github.com/qbittorrent/qBittorrent/issues/21561
    /// <summary>Torrent is stopped and has finished downloading</summary>
    [JsonStringEnumMemberName("stoppedUP")]
    STOPPED_UPLOADING,

    // Undocumented: https://github.com/qbittorrent/qBittorrent/issues/21561
    /// <summary>Torrent is stopped and has NOT finished downloading</summary>
    [JsonStringEnumMemberName("stoppedDL")]
    STOPPED_DOWNLOADING

}