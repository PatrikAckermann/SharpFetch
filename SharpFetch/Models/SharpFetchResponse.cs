using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SharpFetch.Models
{
    public class SharpFetchResponse
    {

        public int Status { get; set; }

        public string StatusText { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public Stream? Body { get; set; }

        public bool Ok { get { return Status >= 200 && Status < 300; } }

        public bool Redirected { get { throw new NotImplementedException(); } }

        public string? Url { get; set; }

        /// <summary>
        /// The original response returned by the HttpClient.
        /// </summary>
        public HttpResponseMessage HttpResponseMessage { get; set; }

        #region Methods

        public async Task<T> Json<T>()
        {
            if (Body == null)
                throw new InvalidOperationException("Response body is null.");
            var parsed = await System.Text.Json.JsonSerializer.DeserializeAsync<T>(Body);
            return parsed ?? throw new InvalidOperationException("Response body deserialized to null.");
        }

        public string Text()
        {
            using (var reader = new StreamReader(Body))
            {
                return reader.ReadToEnd();
            }
        }

        #endregion
    }
}