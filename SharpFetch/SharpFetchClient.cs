using SharpFetch.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpFetch
{
    public class SharpFetchClient : IDisposable
    {
        private string? _baseUrl;

        private bool _httpClientOwned;

        private static HttpClient _client;

        public SharpFetchClient(
            string? baseUrl = null, 
            Dictionary<string, string>? defaultHeaders = null, 
            HttpClient? client = null
            ) 
        { 
            _httpClientOwned = client == null;

            _client = client ?? new HttpClient(new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2)
            });

            _baseUrl = baseUrl ?? "";

            if (defaultHeaders != null)
            {
                foreach (var header in defaultHeaders)
                {
                    _client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
        }

        public void Dispose()
        {
            if (_httpClientOwned)
            {
                _client.Dispose();
            }
        }

        public void SetBaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public void SetDefaultHeaders(Dictionary<string, string> headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    _client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
        }

        public Dictionary<string, string> GetDefaultHeaders()
        {
            var headers = new Dictionary<string, string>();
            foreach (var header in _client.DefaultRequestHeaders)
            {
                headers[header.Key] = string.Join(", ", header.Value);
            }
            return headers;
        }

        public async Task<SharpFetchResponse> Fetch(string url, Options? options = null, CancellationToken cancellationToken = default)
        {
            // Prepare Data

            options ??= new Options { Method = HttpMethod.Get };

            var uri = UrlBuilder(url);
            HttpContent? body;
            if (options.Body != null)
            {
                body = BodyBuilder(options.Body);
            } else
            {
                body = new StringContent("");
            }

            // Send request

            HttpRequestMessage request = new HttpRequestMessage(options.Method, uri);
            if (options.Headers != null)
            {
                foreach (var header in options.Headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            var response = _client.Send(request, cancellationToken);

            // Process response

            SharpFetchResponse sfResponse = new SharpFetchResponse
            {
                Status = (int)response.StatusCode,
                StatusText = response.StatusCode.ToString(),
                Body = await response.Content.ReadAsStreamAsync(),
                HttpResponseMessage = response
            };

            return sfResponse;
        }

        private HttpContent? BodyBuilder(object body)
        {
            HttpContent? result;
            if (body is string strBody)
            {
                result = new StringContent(strBody);
            }
            else if (body is byte[] byteArrayBody)
            {
                result = new ByteArrayContent(byteArrayBody);
            }
            else if (body is Stream streamBody)
            {
                result = new StreamContent(streamBody);
            }
            else if (body is object objBody)
            {
                var json = System.Text.Json.JsonSerializer.Serialize(objBody);
                result = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            }
            else
            {
                // Should be unreachable due to type checks, but just in case
                throw new NotSupportedException("Unsupported body type. Only string, byte[], objects, and streams are supported.");
            }

            return result;
        }

        private Uri UrlBuilder(string url)
        {
            var fullUrl = _baseUrl != null ? new Uri(new Uri(_baseUrl), url) : new Uri(url);
            return fullUrl;
        }
    }
}
