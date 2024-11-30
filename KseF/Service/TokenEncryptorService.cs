using System.Security.Cryptography;
using System.Text;

namespace KseF.Service
    {
    public class TokenEncryptorService
        {
        private readonly string _publicKeyPath;

        public TokenEncryptor(string publicKeyPath)
            {
            _publicKeyPath = publicKeyPath;
            }

        public string EncryptToken(string token, long challengeTimeMillis)
            {
            string dataToEncrypt = $"{token}|{challengeTimeMillis}";
            string publicKeyPem = System.IO.File.ReadAllText(_publicKeyPath);

            try
                {
                byte[] encryptedData = EncryptWithPublicKey(dataToEncrypt, publicKeyPem);
                return Convert.ToBase64String(encryptedData);
                }
            catch (Exception ex)
                {
                Console.WriteLine("Nie udało się zaszyfrować tokenu: " + ex.Message);
                return null;
                }
            }

        private byte[] EncryptWithPublicKey(string data, string publicKeyPem)
            {
            // Usuwamy nagłówki i stopki z klucza PEM
            string publicKeyCleaned = publicKeyPem
                .Replace("-----BEGIN PUBLIC KEY-----", "")
                .Replace("-----END PUBLIC KEY-----", "")
                .Replace("\n", "")
                .Replace("\r", "");

            byte[] publicKeyBytes = Convert.FromBase64String(publicKeyCleaned);
            using (RSA rsa = RSA.Create())
                {
                rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                return rsa.Encrypt(dataBytes, RSAEncryptionPadding.Pkcs1);
                }
            }
        }
    }
