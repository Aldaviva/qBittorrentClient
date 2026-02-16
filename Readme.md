🌊 qBittorrentClient
===

[![NuGet](https://img.shields.io/nuget/v/qBittorrentApiClient?logo=nuget&color=informational)](https://www.nuget.org/packages/qBittorrentApiClient) [![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/Aldaviva/qBittorrentClient/dotnetpackage.yml?branch=master&logo=github)](https://github.com/Aldaviva/qBittorrentClient/actions/workflows/dotnetpackage.yml)

*.NET client for [qBittorrent](https://www.qbittorrent.org) HTTP [API](https://github.com/qbittorrent/qBittorrent/wiki/WebUI-API-(qBittorrent-5.0))*

<!-- MarkdownTOC autolink="true" bracket="round" autoanchor="false" levels="1,2,3" -->

- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Usage](#usage)
    - [Instantiation](#instantiation)
    - [List torrents](#list-torrents)
    - [Get torrent](#get-torrent)
    - [List files in torrent](#list-files-in-torrent)
    - [Get overall connection statistics](#get-overall-connection-statistics)
    - [Get and set preferences](#get-and-set-preferences)

<!-- /MarkdownTOC -->

## Prerequisites
- [qBittorrent](https://www.qbittorrent.org/download) ≥ 5
    - Web UI must be enabled in qBittorrent › Tools › Web UI
- [.NET](https://dotnet.microsoft.com/en-us/download) runtime that supports [.NET Standard ≥ 2](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0)
    - .NET ≥ 5
    - .NET Core ≥ 2
    - .NET Framework ≥ 4.6.1

## Installation
```ps1
dotnet add package qBittorrentApiClient
```

## Usage
### Instantiation
```cs
using qBittorrentClient qBittorrent = new qBittorrentHttpClient(new Uri("http://localhost:8080"));
```

#### Customize HTTP client
```cs
using HttpClient http = new();
using qBittorrentClient qBittorrent = new qBittorrentHttpClient(new qBittorrentHttpApiClient {
    httpClient = http 
});
```

### List torrents
```cs
IEnumerable<TorrentInfo> allTorrents = await qBittorrent.listTorrents();
```
```cs
IEnumerable<TorrentInfo> filteredTorrents = await qBittorrent.listTorrents(
    stateFilter: TorrentState.RUNNING,
    categoryFilter: "My Category",
    tagFilter: "My Tag");
```

### Get torrent
```cs
TorrentInfo? torrent = await qBittorrent.getTorrent(infoHash: "abc123");
```

### List files in torrent
```cs
IEnumerable<TorrentFile> files = await qBittorrent.listFilesInTorrent(torrent);
```

### Get overall connection statistics
```cs
TransferInfo transferInfo = await qBittorrent.getTransferInfo();
```

### Get and set preferences
```cs
Preferences preferences = await qBittorrent.getPreferences();
```
```cs
preferences.listeningPort = 12345;
await qBittorrent.setPreferences(preferences);
```