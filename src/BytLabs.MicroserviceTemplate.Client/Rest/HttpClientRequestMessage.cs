using System.Net;
using Microsoft.OData.Client;
using Microsoft.OData;

namespace BytLabs.MicroserviceTemplate.Client.Rest;

// Routes Microsoft.OData.Client (7.x) requests through a supplied HttpClient, so the typed client works
// against an in-memory test server (WebApplicationFactory) as well as a real host. The 8.x client has a
// built-in HttpClientFactory hook; on 7.x we implement the request/response messages ourselves.
internal sealed class HttpClientRequestMessage : DataServiceClientRequestMessage
{
    private readonly HttpClient _client;
    private readonly Dictionary<string, string> _headers = new();
    private readonly MemoryStream _stream = new();

    public HttpClientRequestMessage(DataServiceClientRequestMessageArgs args, HttpClient client)
        : base(args.ActualMethod)
    {
        _client = client;
        Url = args.RequestUri;
        Method = args.Method;
        foreach (var header in args.Headers)
            SetHeader(header.Key, header.Value);
    }

    public override Uri Url { get; set; }
    public override string Method { get; set; }
    public override ICredentials? Credentials { get; set; }
    public override int Timeout { get; set; }
    public override int ReadWriteTimeout { get; set; }
    public override bool SendChunked { get; set; }

    public override IEnumerable<KeyValuePair<string, string>> Headers => _headers;
    public override string? GetHeader(string headerName) => _headers.TryGetValue(headerName, out var v) ? v : null;
    public override void SetHeader(string headerName, string headerValue) => _headers[headerName] = headerValue;

    public override Stream GetStream() => _stream;
    public override void Abort() { }

    public override IAsyncResult BeginGetRequestStream(AsyncCallback? callback, object? state)
    {
        var result = new SyncResult(state);
        callback?.Invoke(result);
        return result;
    }

    public override Stream EndGetRequestStream(IAsyncResult asyncResult) => _stream;

    public override IAsyncResult BeginGetResponse(AsyncCallback? callback, object? state)
    {
        var result = new SyncResult(state);
        callback?.Invoke(result);
        return result;
    }

    public override IODataResponseMessage EndGetResponse(IAsyncResult asyncResult) => Send();

    public override IODataResponseMessage GetResponse() => Send();

    private IODataResponseMessage Send()
    {
        var request = new HttpRequestMessage(new HttpMethod(Method), Url);
        _stream.Position = 0;
        if (_stream.Length > 0)
            request.Content = new StreamContent(new MemoryStream(_stream.ToArray()));

        foreach (var header in _headers)
        {
            if (request.Headers.TryAddWithoutValidation(header.Key, header.Value))
                continue;
            request.Content ??= new StreamContent(new MemoryStream());
            request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        var response = _client.SendAsync(request).GetAwaiter().GetResult();
        return new HttpClientResponseMessage(response);
    }

    private sealed class SyncResult : IAsyncResult
    {
        public SyncResult(object? state) => AsyncState = state;
        public object? AsyncState { get; }
        public WaitHandle AsyncWaitHandle => throw new NotSupportedException();
        public bool CompletedSynchronously => true;
        public bool IsCompleted => true;
    }
}

internal sealed class HttpClientResponseMessage : IODataResponseMessage
{
    private readonly HttpResponseMessage _response;
    private readonly Dictionary<string, string> _headers = new(StringComparer.OrdinalIgnoreCase);

    public HttpClientResponseMessage(HttpResponseMessage response)
    {
        _response = response;
        foreach (var header in response.Headers)
            _headers[header.Key] = string.Join(",", header.Value);
        if (response.Content is not null)
            foreach (var header in response.Content.Headers)
                _headers[header.Key] = string.Join(",", header.Value);
    }

    public int StatusCode { get => (int)_response.StatusCode; set => throw new NotSupportedException(); }
    public IEnumerable<KeyValuePair<string, string>> Headers => _headers;
    public string? GetHeader(string headerName) => _headers.TryGetValue(headerName, out var v) ? v : null;
    public void SetHeader(string headerName, string headerValue) => throw new NotSupportedException();
    public Stream GetStream() => _response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
}
