using SharpFetch.Models;

namespace SharpFetch
{
    public static class SharpFetch
    {
        private static readonly SharpFetchClient _defaultClient = new SharpFetchClient();

        public static Task<SharpFetchResponse> Fetch(string url, Options? options = null, CancellationToken cancellationToken = default)
            => _defaultClient.Fetch(url, options, cancellationToken);

        public static void ConfigureDefault(Action<SharpFetchClient> config)
            => config(_defaultClient);
    }
}
