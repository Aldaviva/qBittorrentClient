using System.Text.Json;
using Unfucked.HTTP.Config;

namespace qBittorrent.Client;

/// <summary>
/// <para>Lower-level connection to qBittorrent over its HTTP API. Sends requests and receives responses, but all API methods and their inputs and outputs are contained in the higher-level <see cref="qBittorrentClient"/>.</para>
/// <para>To get started with this library, construct a new <see cref="qBittorrentApiClient"/>.</para>
/// <para>If you need to customize the <see cref="HttpClient"/>, you can construct a <see cref="qBittorrentHttpTransport"/>, set <see cref="qBittorrentHttpTransport.httpClient"/>, then pass it to <see cref="qBittorrentApiClient"/>.</para>
/// </summary>
public interface qBittorrentTransport: IDisposable {

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

    /// <inheritdoc cref="qBittorrentHttpTransport.send"/>
    /// <returns>deserialized response body</returns>
    /// <typeparam name="T">the type to deserialize from the response JSON body</typeparam>
    Task<T> send<T>(HttpMethod verb, string apiMethodSubPath, object? requestBody = null, IEnumerable<KeyValuePair<string, object?>>? query = null);

}

/// <inheritdoc cref="qBittorrentTransport" />
public sealed class qBittorrentHttpTransport: qBittorrentTransport {

    private static readonly JsonSerializerOptions JSON_OPTIONS = new(JsonSerializerDefaults.Web)
        { Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) }, PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    private readonly Lazy<HttpClient> defaultHttpClient = new(static () => new UnfuckedHttpClient { Timeout = TimeSpan.FromSeconds(30) });

    /// <summary>
    /// <para>Underlying <see cref="HttpClient"/> used to send requests to the qBittorrent HTTP API. A default instance is provided unless one is set.</para>
    /// <para>If you supply a custom instance, it must be either an <see cref="UnfuckedHttpClient"/>, or an <see cref="HttpClient"/> whose <see cref="HttpMessageHandler"/> is an <see cref="UnfuckedHttpHandler"/>. Otherwise, requests will throw an <see cref="InvalidOperationException"/>.</para>
    /// </summary>
    /// <exception cref="ArgumentNullException" accessor="set">Property is set to <c>null</c></exception>
    public HttpClient httpClient {
        get => field ?? defaultHttpClient.Value;
        init {
            field     = value ?? throw new ArgumentNullException(nameof(value));
            apiTarget = createApiTarget();
            if (defaultHttpClient.IsValueCreated) {
                defaultHttpClient.Value.Dispose();
            }
        }
    } = null;

    private readonly IWebTarget apiTarget;
    private readonly Uri?       webUiBaseUrl;

    /// <summary>Construct a new transport instance that points to the given qBittorrent web server base URL.</summary>
    /// <param name="qBittorrentWebUiBaseUrl">The Web UI server base URL, such as <c>http://mytorrentbox.example.com:8080/</c>, or <c>null</c> to use the default <c>http://localhost:8080</c> base URL.</param>
    public qBittorrentHttpTransport(Uri? qBittorrentWebUiBaseUrl = null) {
        webUiBaseUrl = qBittorrentWebUiBaseUrl;
        apiTarget    = createApiTarget();
    }

    private IWebTarget createApiTarget() => httpClient
        .Target(webUiBaseUrl?.Truncate(UriExtensions.Part.Origin) ?? "http://localhost:8080")
        .Property(PropertyKey.JsonSerializerOptions, JSON_OPTIONS)
        .Path("api/v2");

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

    private IWebTarget target(string apiMethodSubPath, IEnumerable<KeyValuePair<string, object?>>? query) =>
        apiTarget.Path(sanitizeSubpath(apiMethodSubPath)).QueryParam(query);

    private static string sanitizeSubpath(string apiMethodSubPath) =>
        apiMethodSubPath.TrimStart('/');

    private static FormUrlEncodedContent? createBody(HttpMethod verb, object? requestBody) =>
        (verb == HttpMethod.Post || verb == HttpMethod.Put) && requestBody is not null
            // that's right, it's JSON inside form URL-encoding
            ? new FormUrlEncodedContent([new KeyValuePair<string, string>("json", JsonSerializer.Serialize(requestBody, JSON_OPTIONS))])
            : null;

    /// <inheritdoc />
    public void Dispose() {
        if (defaultHttpClient.IsValueCreated) {
            defaultHttpClient.Value.Dispose();
        }
    }

}