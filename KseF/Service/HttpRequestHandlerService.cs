using System.Text;

namespace KseF.Service
    {
    public class HttpRequestHandlerService
        {
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<(string response, int httpCode)> SendRequestAsync(string url, string data, string[] headers, string method = "POST")
            {
            try
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
            catch (Exception ex)
                {
                Console.WriteLine("Błąd: " + ex.Message);
                return (null, 0);
                }
            }
        }
    }
