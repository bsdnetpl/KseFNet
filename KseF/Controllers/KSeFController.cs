using KseF.Models;
using KseF.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KseF.Controllers
    {
    [Route("api/[controller]")]
    [ApiController]
    public class KSeFController : ControllerBase
        {
        private readonly IKSeFClientService _kseFClientService;

        public KSeFController(IKSeFClientService kseFClientService)
            {
            _kseFClientService = kseFClientService;
            }
        [HttpPost("initialize")]
        public async Task<IActionResult> InitializeService([FromBody] KSeFConfig config)
            {
            // Wykrywanie przesłanych danych i użycie domyślnych, jeśli brak
            string apiUrl = !string.IsNullOrWhiteSpace(config.ApiUrl)
                ? config.ApiUrl
                : "https://ksef-demo.mf.gov.pl/api";

            string nip = !string.IsNullOrWhiteSpace(config.Nip)
                ? config.Nip
                : "5771876968";

            string apiKey = !string.IsNullOrWhiteSpace(config.ApiKey)
                ? config.ApiKey
                : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";

            string publicKeyPath = !string.IsNullOrWhiteSpace(config.PublicKeyPath)
                ? config.PublicKeyPath
                : "path_to_public_key.pem";

            // Tworzenie instancji KSeFClientService
            var service = new KSeFClientService(apiUrl, nip, apiKey, publicKeyPath);

            try
                {
                // Przykładowe wywołanie usługi
                var challenge = await service.GetChallengeAndTimestampAsync();
                return Ok(challenge);
                }
            catch (Exception ex)
                {
                return BadRequest(new { error = ex.Message });
                }
            }
        [HttpGet("challenge")]
        public async Task<IActionResult> GetChallengeAndTimestamp()
            {
            try
                {
                var challengeResponse = await _kseFClientService.GetChallengeAndTimestampAsync();
                return Ok(challengeResponse);
                }
            catch (Exception ex)
                {
                return StatusCode(500, new { error = ex.Message });
                }
            }

        [HttpPost("encrypt-token")]
        public IActionResult EncryptToken([FromBody] EncryptTokenRequest request)
            {
            try
                {
                var encryptedToken = _kseFClientService.EncryptToken(request.Token, request.ChallengeTimeMillis);
                return Ok(new { encryptedToken });
                }
            catch (Exception ex)
                {
                return StatusCode(500, new { error = ex.Message });
                }
            }

        [HttpPost("session-token")]
        public async Task<IActionResult> GetSessionToken([FromBody] SessionTokenRequest request)
            {
            try
                {
                var sessionToken = await _kseFClientService.GetKSeFSessionTokenAsync(request.EncryptedToken, request.Challenge);
                return Ok(new { sessionToken });
                }
            catch (Exception ex)
                {
                return StatusCode(500, new { error = ex.Message });
                }
            }

        [HttpPost("send-invoice")]
        public async Task<IActionResult> SendInvoice([FromBody] SendInvoiceRequest request)
            {
            try
                {
                var response = await _kseFClientService.SendInvoiceAsync(request.InvoiceFilePath);
                return Ok(response);
                }
            catch (Exception ex)
                {
                return StatusCode(500, new { error = ex.Message });
                }
            }

        [HttpPost("terminate-session")]
        public async Task<IActionResult> TerminateSession()
            {
            try
                {
                await _kseFClientService.TerminateSessionAsync();
                return Ok(new { message = "Session terminated successfully." });
                }
            catch (Exception ex)
                {
                return StatusCode(500, new { error = ex.Message });
                }
            }
        }
    }
