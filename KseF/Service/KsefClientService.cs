using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace KseF.Service
    {
    public class KsefClientService(string apiUrl, string nip)
        {
        private readonly string _apiUrl = apiUrl;
        private readonly string _nip = nip;
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<Dictionary<string, object>> GetChallengeAndTimestampAsync()
            {
            string url = $"{_apiUrl}/online/Session/AuthorisationChallenge";
            var data = new
                {
                contextIdentifier = new
                    {
                    type = "onip",
                    identifier = _nip
                    }
                };

            string jsonData = JsonSerializer.Serialize(data);
            var headers = new[]
            {
            "Content-Type: application/json",
            "Accept: application/json"
        };

            var (response, httpCode) = await SendRequestAsync(url, jsonData, headers, "POST");

            if (httpCode == 201)
                {
                return JsonSerializer.Deserialize<Dictionary<string, object>>(response);
                }
            else
                {
                throw new Exception($"Błąd w uzyskiwaniu challenge: {response}");
                }
            }

        private async Task<(string response, int httpCode)> SendRequestAsync(string url, string data, string[] headers, string method = "POST")
            {
            HttpRequestMessage request = new HttpRequestMessage
                {
                Method = new HttpMethod(method),
                RequestUri = new Uri(url)
                };

            if (!string.IsNullOrEmpty(data))
                {
                request.Content = new StringContent(data, Encoding.UTF8, "application/json");
                }

            foreach (var header in headers)
                {
                var headerParts = header.Split(new[] { ':' }, 2);
                if (headerParts.Length == 2)
                    {
                    request.Headers.TryAddWithoutValidation(headerParts[0].Trim(), headerParts[1].Trim());
                }
            }

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseBody = await response.Content.ReadAsStringAsync();
            int httpCode = (int)response.StatusCode;

            return (responseBody, httpCode);
            }
        }
    }
