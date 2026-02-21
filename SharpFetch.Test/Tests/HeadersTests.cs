namespace SharpFetch.Test.Tests;

public class HeadersTests
{
    [Fact]
    public void Set_StoresHeader()
    {
        var headers = new Headers();
        headers.Set("Content-Type", "application/json");
        Assert.Equal("application/json", headers.Get("Content-Type"));
    }

    [Fact]
    public void Set_IsCaseInsensitive()
    {
        var headers = new Headers();
        headers.Set("content-type", "text/plain");
        Assert.Equal("text/plain", headers.Get("Content-Type"));
    }

    [Fact]
    public void Append_JoinsMultipleValuesWithComma()
    {
        var headers = new Headers();
        headers.Append("Accept", "text/html");
        headers.Append("Accept", "application/json");
        Assert.Equal("text/html, application/json", headers.Get("Accept"));
    }

    [Fact]
    public void Delete_RemovesHeader()
    {
        var headers = new Headers();
        headers.Set("X-Token", "abc");
        headers.Delete("X-Token");
        Assert.False(headers.Has("X-Token"));
    }

    [Fact]
    public void ImplicitConversion_FromDictionary()
    {
        Headers headers = new Dictionary<string, string>
        {
            ["Authorization"] = "Bearer token",
            ["Accept"] = "application/json"
        };

        Assert.Equal("Bearer token", headers.Get("Authorization"));
        Assert.Equal("application/json", headers.Get("Accept"));
    }

    [Fact]
    public void Indexer_SetAndGet()
    {
        var headers = new Headers();
        headers["X-Custom"] = "value";
        Assert.Equal("value", headers["X-Custom"]);
    }

    [Fact]
    public void Indexer_SetNull_DeletesHeader()
    {
        var headers = new Headers();
        headers["X-Custom"] = "value";
        headers["X-Custom"] = null;
        Assert.False(headers.Has("X-Custom"));
    }

    [Fact]
    public void GetAll_ReturnsAllValues()
    {
        var headers = new Headers();
        headers.Append("Set-Cookie", "a=1");
        headers.Append("Set-Cookie", "b=2");
        Assert.Equal(["a=1", "b=2"], headers.GetAll("Set-Cookie"));
    }

    [Fact]
    public void Enumeration_YieldsAllHeaders()
    {
        var headers = new Headers();
        headers.Set("X-A", "1");
        headers.Set("X-B", "2");
        var keys = headers.Select(h => h.Key).ToHashSet();
        Assert.Contains("X-A", keys);
        Assert.Contains("X-B", keys);
    }
}
