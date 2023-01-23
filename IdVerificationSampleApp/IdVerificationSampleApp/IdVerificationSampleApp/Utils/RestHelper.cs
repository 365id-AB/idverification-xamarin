using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IdVerificationSampleApp.Utils
{
    public static class RestHelper
    {
        private static HttpClient _httpClient;

        private static JsonSerializerSettings _serializerSettings;

        private static JsonSerializer _jsonSerializer;

        private const string JsonMediaType = "application/json";

        public static void Init()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonMediaType));

            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };
            _serializerSettings.Converters.Add(new StringEnumConverter());

            _jsonSerializer = new JsonSerializer();
        }

        public static async Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest data)
            => await ExecuteAsync<TResult>(HttpMethod.Post, uri, data);

        private static async Task<TResult> ExecuteAsync<TResult>(HttpMethod httpMethod, string uri, object data)
        {
            var httpRequestMessage = GetHttpRequestMessage(httpMethod, uri, data);
            var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);

            using (var stream = await httpResponseMessage.Content.ReadAsStreamAsync())
            {
                using (var reader = new StreamReader(stream))
                {
                    using (var json = new JsonTextReader(reader))
                        return _jsonSerializer.Deserialize<TResult>(json);
                }
            }
        }

        private static HttpRequestMessage GetHttpRequestMessage(HttpMethod httpMethod, string uri, object data = null)
        {
            var request = new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = new Uri(uri)
            };

            if (data != null)
            {
                var serialized = JsonConvert.SerializeObject(data, _serializerSettings);
                request.Content = new StringContent(serialized, Encoding.UTF8, JsonMediaType);
            }

            return request;
        }
    }
}