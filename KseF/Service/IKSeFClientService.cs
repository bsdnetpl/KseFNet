
namespace KseF.Service
    {
    public interface IKSeFClientService
        {
        string EncryptToken(string token, string challengeTimeMillis);
        Task<dynamic> GetChallengeAndTimestampAsync();
        Task<string> GetKSeFSessionTokenAsync(string encryptedToken, string challenge);
        Task<dynamic> SendInvoiceAsync(string invoiceFile);
        Task TerminateSessionAsync();
        }
    }