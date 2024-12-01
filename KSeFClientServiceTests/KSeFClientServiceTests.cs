using KseF.Service;
using Moq;
using Moq.Protected; // Dodano do mockowania chronionych metod
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace KSeFClientServiceTests
    {
    public class KSeFClientServiceTests
        {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly KSeFClientService _service;
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://example.com";
        private const string Nip = "1234567890";
        private const string ApiKey = "test-api-key";
        private const string PublicKeyPath = "test-public-key.pem";

        public KSeFClientServiceTests()
            {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
                {
                BaseAddress = new Uri(ApiUrl)
                };
            _service = new KSeFClientService(ApiUrl, Nip, ApiKey, PublicKeyPath);
            }


        [Fact]
        public async Task GetChallengeAndTimestampAsync_ThrowsException_WhenRequestFails()
            {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                    {
                    StatusCode = HttpStatusCode.BadRequest
                    });

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.GetChallengeAndTimestampAsync());
            }


        [Fact]
        public async Task GetKSeFSessionTokenAsync_ThrowsException_WhenRequestFails()
            {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                    {
                    StatusCode = HttpStatusCode.BadRequest
                    });

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.GetKSeFSessionTokenAsync("encrypted-token", "test-challenge"));
            }


        [Fact]
        public async Task TerminateSessionAsync_ThrowsException_WhenRequestFails()
            {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                    {
                    StatusCode = HttpStatusCode.BadRequest
                    });

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.TerminateSessionAsync());
            }
        }
    }
