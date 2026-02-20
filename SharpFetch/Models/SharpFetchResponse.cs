namespace SharpFetch.Models
{
    public class SharpFetchResponse
    {

        public int Status { get; set; }

        public string StatusText { get; set; } = string.Empty;

        public Headers Headers { get; set; } = new Headers();

        public Stream? Body { get; set; } = null;

        public bool Ok { get { return Status >= 200 && Status < 300; } }

        public string? Url { get; set; }

        /// <summary>
        /// The original response returned by the HttpClient.
        /// </summary>
        public HttpResponseMessage? HttpResponseMessage { get; set; }

        #region Methods

        public async Task<T> Json<T>()
        {
            if (Body == null)
                throw new InvalidOperationException("Response body is null.");
            var parsed = await System.Text.Json.JsonSerializer.DeserializeAsync<T>(Body);
            return parsed ?? throw new InvalidOperationException("Response body deserialized to null.");
        }

        public async Task<string> Text()
        {
            if (Body == null)
                throw new InvalidOperationException("Response body is null.");

            using var reader = new StreamReader(Body);
            return await reader.ReadToEndAsync();
        }

        #endregion
    }
}