using SharpFetch.Models;

namespace SharpFetch
{
    public class SharpFetchClient : IDisposable
    {
        private string? _baseUrl;

        private bool _httpClientOwned;

        private readonly HttpClient _client;

        public SharpFetchClient(
            string? baseUrl = null,
            Headers? defaultHeaders = null,
            HttpClient? client = null
            )
        {
            _httpClientOwned = client == null;

            _client = client ?? new HttpClient(new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2)
            });

            _baseUrl = baseUrl ?? "";

            SetDefaultHeaders(defaultHeaders);
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

        public void SetDefaultHeaders(Headers? headers)
        {
            _client.DefaultRequestHeaders.Clear();
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    _client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
        }

        public void ClearDefaultHeaders()
        {
            _client.DefaultRequestHeaders.Clear();
        }

        public Headers GetDefaultHeaders()
        {
            var headers = new Headers();
            foreach (var header in _client.DefaultRequestHeaders)
            {
                foreach (var value in header.Value)
                {
                    headers.Append(header.Key, value);
                }
            }
            return headers;
        }

        public async Task<SharpFetchResponse> Fetch(string url, Options? options = null, CancellationToken cancellationToken = default)
        {
            // Prepare Data

            options ??= new Options();

            var uri = UrlBuilder(url);
            HttpContent? body = null;
            if (options.Body != null)
            {
                body = BodyBuilder(options.Body);
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
            request.Content = body;
            var response = await _client.SendAsync(request, cancellationToken);

            // Process response

            SharpFetchResponse sfResponse = new SharpFetchResponse
            {
                Status = (int)response.StatusCode,
                StatusText = response.StatusCode.ToString(),
                Body = await response.Content.ReadAsStreamAsync(),
                HttpResponseMessage = response,
                Url = response.RequestMessage?.RequestUri?.ToString(),
            };

            foreach (var header in response.Headers)
            {
                foreach (var value in header.Value)
                {
                    sfResponse.Headers.Append(header.Key, value);
                }
            }
            foreach (var header in response.Content.Headers)
            {
                foreach (var value in header.Value)
                {
                    sfResponse.Headers.Append(header.Key, value);
                }
            }

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
            if (!string.IsNullOrWhiteSpace(_baseUrl))
            {
                var baseUri = _baseUrl.EndsWith('/') ? _baseUrl : _baseUrl + "/";
                return new Uri(new Uri(baseUri), url);
            }
            return new Uri(url);
        }
    }
}
