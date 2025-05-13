using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

const string AuthorizeUri = "https://demodb.my.erp.net/id/connect/authorize";
const string TokenUri = "https://demodb.my.erp.net/id/connect/token";
const string CallbackUri = "http://localhost/signin-callback/";
const string DomainApiTestUri = "https://demodb.my.erp.net/api/domain/odata/Crm_Customers?$top=10";
const string TrustedAppUri = "my.trusted.app/first";
const string TrustedAppSecret = "<my_plain_app_secret>";

string authCode = string.Empty;
string authState = Guid.NewGuid().ToString();
ClientAuthData clientData;

var httpListener = new HttpListener();
httpListener.Prefixes.Add(CallbackUri);
httpListener.Start();

var signCallbackTask = httpListener.GetContextAsync().ContinueWith(HandleSignInCallback);

var authorizeUriArgs =
    $"client_id={TrustedAppUri}&" +
    $"redirect_uri={CallbackUri}&" +
    "response_type=code%20id_token&" +
    "response_mode=form_post&" +
    "scope=openid%20profile%20offline_access%20DomainApi&" +
    "nonce=abc&" +
    $"state={authState}";

// Start the authorize endpoint with your default browser.
// This way you'll be able to enter your credentials.
var processStartInfo = new ProcessStartInfo("cmd", $"/c start {AuthorizeUri}?{authorizeUriArgs.Replace("&", "^&").Replace(" ", "%20")}")
{
    CreateNoWindow = true
};
Process.Start(processStartInfo);

// Wait until the redirect is made.
signCallbackTask.Wait();

try
{
    #region Create an http client and request/response message objects.

    var handler = new HttpClientHandler
    {
        ClientCertificateOptions = ClientCertificateOption.Manual,
        ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
        {
            return true;
        }
    };

    var httpClient = new HttpClient(handler);
    HttpRequestMessage? httpRequest = null;
    HttpResponseMessage? httpResponse = null;

    #endregion

    #region Acquire an access token.

    var tokenUriBody =
        $"client_id={TrustedAppUri}&" +
        $"client_secret={TrustedAppSecret}&" +
        "grant_type=authorization_code&" +
        $"code={authCode}&" +
        $"redirect_uri={CallbackUri}";

    httpRequest = new HttpRequestMessage()
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri(TokenUri),
        Content = new StringContent(
            tokenUriBody,
            Encoding.UTF8,
            "application/x-www-form-urlencoded")
    };

    httpResponse = httpClient.Send(httpRequest);
    // This will throw if the returned status code is not 2xx.
    httpResponse.EnsureSuccessStatusCode();

    // Deserialize the JSON response as a ClientAuthData struct.
    clientData = JsonSerializer.Deserialize<ClientAuthData>(httpResponse.Content.ReadAsStream());

    Console.WriteLine($"Access token: {clientData.AccessToken}");
    Console.WriteLine($"Refresh token: {clientData.RefreshToken}");

    #endregion

    #region Domain Api call - select top 10 Crm_Customers

    httpRequest = new HttpRequestMessage()
    {
        Method = HttpMethod.Get,
        RequestUri = new Uri(DomainApiTestUri)
    };
    httpRequest.Headers.Add("Authorization", $"Bearer {clientData.AccessToken}");

    httpResponse = httpClient.Send(httpRequest);
    httpResponse.EnsureSuccessStatusCode();

    Console.WriteLine("=================");
    var domainApiResponse = await httpResponse.Content.ReadAsStringAsync();
    Console.WriteLine(domainApiResponse);

    #endregion
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

void HandleSignInCallback(Task<HttpListenerContext> httpListenerContextTask)
{
    var httpContext = httpListenerContextTask.Result;
    var logMessage = "Well done, everything is OK.\r\nNow you have an authorization code.";

    try
    {
        #region Request an authorization code.

        var body = new Dictionary<string, string>();
        using (var streamReader = new StreamReader(httpContext.Request.InputStream))
        {
            body = streamReader.ReadToEnd()
                .Split('&')
                .Select(v => v.Split('='))
                .ToDictionary(pair => pair[0], pair => pair[1]);
        }

        if (!body.Any())
            throw new Exception("empty body :(");

        if (body["state"] != authState)
            throw new Exception("The returned state differs from the one we've passed.");

        authCode = body["code"];

        if (string.IsNullOrEmpty(authCode))
            throw new Exception("No or invalid authorization code.");

        #endregion
    }
    catch (Exception ex)
    {
        logMessage = ex.Message;
    }

    byte[] buffer = Encoding.UTF8.GetBytes(logMessage);
    httpContext.Response.ContentLength64 = buffer.Length;
    using var output = httpContext.Response.OutputStream;
    output.Write(buffer, 0, buffer.Length);
}

public struct ClientAuthData
{
    [JsonPropertyName("id_token")]
    public string IdToken { get; set; }

    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonPropertyName("scope")]
    public string Scope { get; set; }
}
