namespace KseF.Service
{
    public interface IKSeFClientService
    {
        string EncryptToken(string token, string challengeTimeMillis);
        Task<JsonDocument> GetChallengeAndTimestampAsync();
        Task<string> GetKSeFSessionTokenAsync(string encryptedToken, string challenge);
        Task<JsonDocument> SendInvoiceAsync(string invoiceFile);
        Task TerminateSessionAsync();
    }
}
