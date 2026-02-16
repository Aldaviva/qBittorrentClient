using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Unfucked;
using Unfucked.HTTP;
using Unfucked.HTTP.Config;

namespace qBittorrent.Client;

public interface qBittorrentApiClient: IDisposable {

    HttpClient httpClient { get; init; }

    /// <summary>
    /// Send an HTTP request to the qBittorrent JSON REST API and receive a response.
    /// </summary>
    /// <param name="verb">HTTP verb to send</param>
    /// <param name="apiMethodSubPath">request URL path after the <c>/api/v2/</c>, such as <c>app/setPreferences</c></param>
    /// <param name="requestBody">optional object to be serialized to JSON and passed in the JSON form field, or <c>null</c> to not send a request body</param>
    /// <param name="query">optional query parameters to send, or <c>null</c> to not send any query parameters</param>
    /// <returns>the HTTP response</returns>
    /// <exception cref="HttpRequestException">if the response status code is ≥400</exception>
    Task<HttpResponseMessage> send(HttpMethod verb, string apiMethodSubPath, object? requestBody = null, IEnumerable<KeyValuePair<string, object?>>? query = null);

    /// <inheritdoc cref="qBittorrentHttpApiClient.send"/>
    /// <returns>deserialized response body</returns>
    /// <typeparam name="T">the type to deserialize from the response JSON body</typeparam>
    Task<T> send<T>(HttpMethod verb, string apiMethodSubPath, object? requestBody = null, IEnumerable<KeyValuePair<string, object?>>? query = null);

}

/// <summary>
/// <para>HTTP REST API client for qBittorrent WebUI API</para>
/// <para>Official documentation: <see href="https://github.com/qbittorrent/qBittorrent/wiki/WebUI-API-(qBittorrent-5.0)"/></para>
/// </summary>
public class qBittorrentHttpApiClient: qBittorrentApiClient {

    private static readonly JsonSerializerOptions JSON_OPTIONS = new(JsonSerializerDefaults.Web)
        { Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) }, PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    private readonly Lazy<HttpClient> defaultHttpClient = new(() => new UnfuckedHttpClient { Timeout = TimeSpan.FromSeconds(30) });
    private readonly HttpClient?      customHttpClient;

    public HttpClient httpClient {
        get => customHttpClient ?? defaultHttpClient.Value;
        init {
            customHttpClient = value;
            api              = createApi();
            if (defaultHttpClient.IsValueCreated) {
                defaultHttpClient.Value.Dispose();
            }
        }
    }

    private readonly IWebTarget api;
    private readonly Uri?       qBittorrentWebUiBaseUrl;

    public qBittorrentHttpApiClient(Uri? qBittorrentWebUiBaseUrl = null) {
        this.qBittorrentWebUiBaseUrl = qBittorrentWebUiBaseUrl;
        api                          = createApi();
    }

    private IWebTarget createApi() => httpClient
        .Property(PropertyKey.JsonSerializerOptions, JSON_OPTIONS)
        .Target(qBittorrentWebUiBaseUrl?.Truncate(UriExtensions.Part.Origin) ?? "http://localhost:8080")
        .Path("/api/v2");

    /// <summary>
    /// Send an HTTP request to the qBittorrent JSON REST API and receive a response.
    /// </summary>
    /// <param name="verb">HTTP verb to send</param>
    /// <param name="apiMethodSubPath">request URL path after the <c>/api/v2/</c>, such as <c>app/setPreferences</c></param>
    /// <param name="requestBody">optional object to be serialized to JSON and passed in the JSON form field, or <c>null</c> to not send a request body</param>
    /// <param name="query">optional query parameters to send, or <c>null</c> to not send any query parameters</param>
    /// <returns>the HTTP response</returns>
    /// <exception cref="HttpRequestException">if the response status code is ≥400</exception>
    public async Task<HttpResponseMessage> send(HttpMethod verb, string apiMethodSubPath, object? requestBody = null, IEnumerable<KeyValuePair<string, object?>>? query = null) =>
        await target(apiMethodSubPath, query).Send(verb, createBody(verb, requestBody)).ConfigureAwait(false);

    /// <inheritdoc cref="send"/>
    /// <returns>deserialized response body</returns>
    /// <typeparam name="T">the type to deserialize from the response JSON body</typeparam>
    public async Task<T> send<T>(HttpMethod verb, string apiMethodSubPath, object? requestBody = null, IEnumerable<KeyValuePair<string, object?>>? query = null) =>
        await target(apiMethodSubPath, query).Send<T>(verb, createBody(verb, requestBody)).ConfigureAwait(false);

    private IWebTarget target(string apiMethodSubPath, IEnumerable<KeyValuePair<string, object?>>? query) => api.Path(sanitizeSubpath(apiMethodSubPath)).QueryParam(query);

    private static string sanitizeSubpath(string apiMethodSubPath) => apiMethodSubPath.TrimStart('/');

    private static FormUrlEncodedContent? createBody(HttpMethod verb, object? requestBody) =>
        (verb == HttpMethod.Post || verb == HttpMethod.Put) && requestBody != null ? new FormUrlEncodedContent([
            new KeyValuePair<string, string>("json", JsonSerializer.Serialize(requestBody, JSON_OPTIONS)) // that's right, it's JSON inside form URL-encoding
        ]) : null;

    public void Dispose() {
        if (defaultHttpClient.IsValueCreated) {
            defaultHttpClient.Value.Dispose();
        }
        GC.SuppressFinalize(this);
    }

}