namespace KseF.Models
    {
    public class SessionTokenRequest
        {
        public string EncryptedToken { get; set; }
        public string Challenge { get; set; }
        }
    }
