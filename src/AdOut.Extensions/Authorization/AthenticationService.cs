using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AdOut.Extensions.Authorization
{
    public class AthenticationService : IAthenticationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl;

        public AthenticationService(
            IHttpClientFactory httpClientFactory,
            string baseUrl)
        {
            _httpClientFactory = httpClientFactory;
            _baseUrl = baseUrl;
        }

        public Task<AuthResponse> AuthenticateAsync(string clientId, string clientSecret)
        {
            var args = new Dictionary<string, string>()
            {
                { "grant_type", "client_credentials" },
                { "client_id", clientId },
                { "client_secret", clientSecret }
            };

            return GetResponseAsync<AuthResponse>($"{_baseUrl}connect/token", HttpMethod.Post, args);
        }

        private async Task<T> GetResponseAsync<T>(string url, HttpMethod httpMethod, Dictionary<string, string> args)
        {
            var client = _httpClientFactory.CreateClient();
            var message = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Content = new FormUrlEncodedContent(args.ToList()),
                Method = httpMethod
            };

            var httpResponse = await client.SendAsync(message);
            httpResponse.EnsureSuccessStatusCode();
            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<T>(responseContent);

            return response;
        }
    }
}
