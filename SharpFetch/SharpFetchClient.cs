using SharpFetch.Enums;
using SharpFetch.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpFetch
{
    public class SharpFetchClient
    {
        private string? _baseUrl;

        private Dictionary<string, string> _defaultHeaders;

        private HttpClient _client;

        public SharpFetchClient(
            string? BaseUrl = null, 
            Dictionary<string, string>? DefaultHeaders = null, 
            HttpClient? Client = null
            ) 
        { 
            _client = Client ?? new HttpClient();
            _baseUrl = BaseUrl ?? "";
            _defaultHeaders = DefaultHeaders ?? [];
        }

        public void SetBaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public void SetDefaultHeaders(Dictionary<string, string> headers)
        {
            _defaultHeaders = headers;
        }

        public void SetHttpClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<SharpFetchResponse> Fetch(string url, Options? options = null)
        {
            options ??= new Options { Method = MethodEnum.GET };

            var uri = UriBuilder(url);
            HttpContent? body;
            if (options.Body != null)
            {
                body = BodyBuilder(options.Body);
            }
            body = new StringContent("");

            HttpResponseMessage response;

            switch (options.Method)
            {
                case MethodEnum.GET:
                    response = await _client.GetAsync(uri);
                    break;
                case MethodEnum.POST:
                    response = await _client.PostAsync(uri, body);  
                    break;
                case MethodEnum.PUT:
                    response = await _client.PutAsync(uri, body);
                    break;
                case MethodEnum.PATCH:
                    response = await _client.PatchAsync(uri, body);
                    break;
                default:
                    throw new NotImplementedException("Only GET method is implemented.");
            }

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
                throw new NotSupportedException("Unsupported body type. Only string, byte[], objects, and streams are supported.");
            }

            return result;
        }

        private Uri UriBuilder(string url)
        {
            var fullUrl = _baseUrl != null ? new Uri(new Uri(_baseUrl), url) : new Uri(url);
            return fullUrl;
        }
    }
}
