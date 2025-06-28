using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace KseF.Service;

public class KSeFClientService : IKSeFClientService
{
    private readonly string apiUrl;
    private readonly string nip;
    private readonly string apiKey;
    private readonly string publicKeyPath;
    private string sessionToken;
    private readonly HttpClient httpClient;

    public KSeFClientService(string apiUrl, string nip, string apiKey, string publicKeyPath)
    {
        this.apiUrl = apiUrl;
        this.nip = nip;
        this.apiKey = apiKey;
        this.publicKeyPath = publicKeyPath;
        httpClient = new HttpClient();
    }

    private async Task<HttpResponseMessage> SendRequestAsync(string url, string data, HttpMethod method, Dictionary<string, string> headers = null)
    {
        using var request = new HttpRequestMessage(method, url)
        {
            Content = data != null ? new StringContent(data, Encoding.UTF8, null) : null
        };

        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return await httpClient.SendAsync(request);
    }

    public async Task<JsonDocument> GetChallengeAndTimestampAsync()
    {
        var url = $"{apiUrl}/online/Session/AuthorisationChallenge";
        var data = JsonSerializer.Serialize(new
        {
            contextIdentifier = new
            {
                type = "onip",
                identifier = nip
            }
        });

        var headers = new Dictionary<string, string>
        {
            { "Content-Type", "application/json" },
            { "Accept", "application/json" }
        };

        var response = await SendRequestAsync(url, data, HttpMethod.Post, headers);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error getting challenge: {response.StatusCode}\n{content}");

        return JsonDocument.Parse(content);
    }

    public string EncryptToken(string token, string challengeTimeMillis)
    {
        var dataToEncrypt = Encoding.UTF8.GetBytes($"{token}|{challengeTimeMillis}");
        var publicKey = File.ReadAllText(publicKeyPath);
        using var rsa = RSA.Create();
        rsa.ImportFromPem(publicKey.ToCharArray());
        var encryptedData = rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.Pkcs1);
        return Convert.ToBase64String(encryptedData);
    }

    public async Task<string> GetKSeFSessionTokenAsync(string encryptedToken, string challenge)
    {
        var doc = new XmlDocument();

        // Nagłówki namespace'ów
        var root = doc.CreateElement("ns3", "InitSessionTokenRequest", "http://ksef.mf.gov.pl/schema/gtw/svc/online/auth/request/2021/10/01/0001");
        root.SetAttribute("xmlns:ns2", "http://ksef.mf.gov.pl/schema/gtw/svc/types/2021/10/01/0001");
        root.SetAttribute("xmlns:ns3", "http://ksef.mf.gov.pl/schema/gtw/svc/online/auth/request/2021/10/01/0001");
        root.SetAttribute("xmlns:ns4", "http://ksef.mf.gov.pl/schema/gtw/svc/online/types/2021/10/01/0001");
        root.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
        doc.AppendChild(root);

        var context = doc.CreateElement("ns3", "Context", root.NamespaceURI);
        root.AppendChild(context);

        var challengeElement = doc.CreateElement("ns4", "Challenge", "http://ksef.mf.gov.pl/schema/gtw/svc/online/types/2021/10/01/0001");
        challengeElement.InnerText = challenge;
        context.AppendChild(challengeElement);

        var identifier = doc.CreateElement("ns4", "Identifier", challengeElement.NamespaceURI);
        identifier.SetAttribute("xsi:type", "ns2:SubjectIdentifierByCompanyType");
        var identifierValue = doc.CreateElement("ns2", "Identifier", "http://ksef.mf.gov.pl/schema/gtw/svc/types/2021/10/01/0001");
        identifierValue.InnerText = nip;
        identifier.AppendChild(identifierValue);
        context.AppendChild(identifier);

        var tokenElement = doc.CreateElement("ns4", "Token", challengeElement.NamespaceURI);
        tokenElement.InnerText = encryptedToken.Trim();
        context.AppendChild(tokenElement);

        var url = $"{apiUrl}/online/Session/InitToken";
        var headers = new Dictionary<string, string>
        {
            { "Content-Type", "application/octet-stream" },
            { "Accept", "application/json" }
        };

        var response = await SendRequestAsync(url, doc.OuterXml, HttpMethod.Post, headers);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error getting session token: {response.StatusCode}\n{content}");

        using var docJson = JsonDocument.Parse(content);
        sessionToken = docJson.RootElement.GetProperty("sessionToken").GetProperty("token").GetString();
        return sessionToken;
    }

    public async Task<JsonDocument> SendInvoiceAsync(string invoiceFile)
    {
        if (!File.Exists(invoiceFile))
            throw new FileNotFoundException($"Invoice file not found: {invoiceFile}");

        var invoiceData = File.ReadAllBytes(invoiceFile);
        var hashSHA = Convert.ToBase64String(SHA256.HashData(invoiceData));
        var invoiceBody = Convert.ToBase64String(invoiceData);
        var fileSize = new FileInfo(invoiceFile).Length;

        var payload = new
        {
            invoiceHash = new
            {
                fileSize,
                hashSHA = new
                {
                    algorithm = "SHA-256",
                    encoding = "Base64",
                    value = hashSHA
                }
            },
            invoicePayload = new
            {
                type = "plain",
                invoiceBody
            }
        };

        var body = JsonSerializer.Serialize(payload);

        var headers = new Dictionary<string, string>
        {
            { "Accept", "application/json" },
            { "SessionToken", sessionToken },
            { "Content-Type", "application/json" }
        };

        var response = await SendRequestAsync($"{apiUrl}/online/Invoice/Send", body, HttpMethod.Put, headers);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error sending invoice: {response.StatusCode}\n{content}");

        return JsonDocument.Parse(content);
    }

    public async Task TerminateSessionAsync()
    {
        var url = $"{apiUrl}/online/Session/Terminate";
        var headers = new Dictionary<string, string>
        {
            { "Accept", "application/json" },
            { "SessionToken", sessionToken }
        };

        var response = await SendRequestAsync(url, null, HttpMethod.Get, headers);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error terminating session: {response.StatusCode}\n{content}");

        Console.WriteLine("Session terminated successfully.");
    }
}
