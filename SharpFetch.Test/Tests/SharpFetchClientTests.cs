using SharpFetch.Test.Helpers;

namespace SharpFetch.Test.Tests;

public class SharpFetchClientTests
{
    /// <summary>
    /// Creates a <see cref="SharpFetchClient"/> backed by a <see cref="FakeHttpMessageHandler"/>.
    /// Enqueue responses on <paramref name="handler"/> before calling Fetch.
    /// </summary>
    private static (SharpFetchClient client, FakeHttpMessageHandler handler) CreateClient(string? baseUrl = null)
    {
        var handler = new FakeHttpMessageHandler();
        var client = new SharpFetchClient(baseUrl: baseUrl, client: new HttpClient(handler));
        return (client, handler);
    }

    [Fact]
    public async Task Fetch_ReturnsOkResponse()
    {
        var (client, handler) = CreateClient();
        handler.Enqueue(HttpStatusCode.OK, """{"id":1}""");

        var response = await client.Fetch("https://example.com/api/item");

        Assert.True(response.Ok);
        Assert.Equal(200, response.Status);
    }

    [Fact]
    public async Task Fetch_ParsesJsonBody()
    {
        var (client, handler) = CreateClient();
        handler.Enqueue(HttpStatusCode.OK, """{"name":"test"}""");

        var response = await client.Fetch("https://example.com/api/item");
        var json = await response.Json<Dictionary<string, string>>();

        Assert.Equal("test", json["name"]);
    }

    [Fact]
    public async Task Fetch_ParsesTextBody()
    {
        var (client, handler) = CreateClient();
        handler.Enqueue(HttpStatusCode.OK, "hello world", "text/plain");

        var response = await client.Fetch("https://example.com/api/text");
        var text = await response.Text();

        Assert.Equal("hello world", text);
    }

    [Fact]
    public async Task Fetch_SendsCorrectMethod()
    {
        var (client, handler) = CreateClient();
        handler.Enqueue(HttpStatusCode.Created);

        await client.Fetch("https://example.com/api/item", new Options { Method = HttpMethod.Post });

        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
    }

    [Fact]
    public async Task Fetch_SendsRequestHeaders()
    {
        var (client, handler) = CreateClient();
        handler.Enqueue(HttpStatusCode.OK);

        await client.Fetch("https://example.com/api/item", new Options
        {
            Headers = new Dictionary<string, string> { ["X-Custom"] = "hello" }
        });

        Assert.True(handler.LastRequest.Headers.Contains("X-Custom"));
    }

    [Fact]
    public async Task Fetch_WithBaseUrl_BuildsCorrectUri()
    {
        var (client, handler) = CreateClient(baseUrl: "https://example.com/api/");
        handler.Enqueue(HttpStatusCode.OK);

        await client.Fetch("items");

        Assert.Equal("https://example.com/api/items", handler.LastRequest.RequestUri?.ToString());
    }

    [Fact]
    public async Task Fetch_ReturnsNotOk_OnErrorStatusCode()
    {
        var (client, handler) = CreateClient();
        handler.Enqueue(HttpStatusCode.NotFound);

        var response = await client.Fetch("https://example.com/missing");

        Assert.False(response.Ok);
        Assert.Equal(404, response.Status);
    }

    [Fact]
    public async Task Fetch_ResponseHeadersAreMapped()
    {
        var (client, handler) = CreateClient();
        var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK);
        fakeResponse.Headers.Add("X-Request-Id", "abc-123");
        handler.Enqueue(fakeResponse);

        var response = await client.Fetch("https://example.com/api/item");

        Assert.Equal("abc-123", response.Headers.Get("X-Request-Id"));
    }

    [Theory]
    [InlineData(null, "https://example.com/test/", "https://example.com/test/")]
    [InlineData("", "https://example.com/test/", "https://example.com/test/")]
    [InlineData(" ", "https://example.com/test/", "https://example.com/test/")]
    public async Task UrlBuilder_NoBaseUrl(string? baseUrl, string requestUrl, string expectedUrl)
    {
        var responseUrl = await GetResponseUrl(baseUrl, requestUrl);
        Assert.Equal(expectedUrl, responseUrl);
    }

    [Theory]
    [InlineData("https://example.com/api/", "items/1", "https://example.com/api/items/1")]
    [InlineData("https://example.com/api", "items/1", "https://example.com/api/items/1")]
    [InlineData("https://example.com/api/", "", "https://example.com/api/")]
    public async Task UrlBuilder_WithBaseUrl(string? baseUrl, string requestUrl, string expectedUrl)
    {
        var responseUrl = await GetResponseUrl(baseUrl, requestUrl);
        Assert.Equal(expectedUrl, responseUrl);
    }

    private async Task<string> GetResponseUrl(string? baseUrl, string requestUrl)
    {
        var (client, handler) = CreateClient(baseUrl);
        handler.Enqueue(HttpStatusCode.OK);
        var response = await client.Fetch(requestUrl);
        return response.Url ?? "";
    }
}