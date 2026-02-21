using System.Text;

namespace SharpFetch.Test.Helpers;

/// <summary>
/// A fake <see cref="HttpMessageHandler"/> for unit testing.
/// Enqueue responses in order; inspect <see cref="SentRequests"/> to assert on outgoing requests.
/// </summary>
internal sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<HttpResponseMessage> _queue = new();

    /// <summary>All requests that were sent through this handler, in order.</summary>
    public List<HttpRequestMessage> SentRequests { get; } = [];

    /// <summary>The most recent request sent, for convenience.</summary>
    public HttpRequestMessage LastRequest => SentRequests[^1];

    /// <summary>Enqueue a response with an optional string body.</summary>
    public void Enqueue(HttpStatusCode statusCode, string? body = null, string mediaType = "application/json")
    {
        var response = new HttpResponseMessage(statusCode);
        if (body != null)
            response.Content = new StringContent(body, Encoding.UTF8, mediaType);
        _queue.Enqueue(response);
    }

    /// <summary>Enqueue a fully constructed response.</summary>
    public void Enqueue(HttpResponseMessage response) => _queue.Enqueue(response);

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        SentRequests.Add(request);

        if (_queue.Count == 0)
            throw new InvalidOperationException("FakeHttpMessageHandler: no responses have been enqueued.");

        var response = _queue.Dequeue();
        response.RequestMessage = request;
        return Task.FromResult(response);
    }
}
