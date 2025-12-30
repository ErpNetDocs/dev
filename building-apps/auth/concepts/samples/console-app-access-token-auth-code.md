---
erp.type: sample
erp.topic: security
---

# Access token via authorization code flow in a console app

## Objective

You have an external application that will be authorized via the @@erpnet login form on behalf of an internal user. The application MAY or MAY NOT provide an UI, but there'll be user interaction at least for the initial login.

Or, 
* Your external app is an interactive application.
* It will be authenticated and authorized via the @@erpnet login form (on behalf of an @@erpnet internal user).
* Your external application will access the @@erpnet instance on behalf of the logged user.
* Your external app MAY provide UI.
* It's able to keep a secret, so it's also a condfidential application.
* There'll be user interaction (at least for the internal user to log in), so your external app will use [authorization code flow](https://auth0.com/docs/get-started/authentication-and-authorization-flow/authorization-code-flow).

## The whole process in a nutshell

> [!NOTE]
> This example uses Windows console application, which is why some points further require a deeper understanding. 
> It's highly recommended, if you haven't done so, that you read the following topic first,
> [Access token via authorization code flow in a web app](./web-app-access-token-auth-code.md)

After all, your final goal is to acquire an access token. The process is very similar to this example [Basic example of acquiring an access token](./basic-acquire-access-token.md), but here is added another intermediate step - the process of impersonating a user. Here's a summary of how the whole process goes:

1. Your external app will open the so called [authorize endpoint](../../concepts/how-apps-connect/identity-server.md) with your trusted app details (the trusted app, corresponding to your external app).

> [!NOTE]
> An important detail is that the authorize endpoint must be opened in a browser (see next point and its note). 

2. If all's OK, the browser will be redirected to the @@erpnet login page, where the user will enter their credentials.

> [!NOTE]
> Because of the redirect, initially the step (1) have to be performed in a browser.
> - If your external app is web-based, you don't have to do anything (because it will work in a browser anyway).
> - If your external app is NOT web-based, you have to handle this process by yourself (the example below is just like that).

3. If the user logs in successfully, the @@erpnet login page (i.e. @@erpnet Identity) will be redirect to a uri where **your external app is listening**.
4. There you'll receive an authorization code.
5. Finally you'll exchange the auth code for an access token at the token endpoint.
6. You'll obtain an access token on behalf of the logged user (2).

## Prerequisites

You have a trusted application, defined as follows:

| Attribute | Value                | Comment |
| --------- | -------------------- | ------- |
| Name      | My first trusted app | This value doesn't matter much. It's used for user-friendly identification. |
| ApplicationUri | my.trusted.app/first | This is your trusted app's unique identifier. It's used in the authentication process. |
| IsEnabled | true | |
| ImpersonateAsInternalUserAllowed | true | The trusted application will allow authentication from internal users. |
| ImpersonateLoginUrl | http://localhost/signin-callback/ | The url where your external app is listening. Redirection to this uri will be performed after the user logs in successfully (see [step 3 in the section above](./console-app-access-token-auth-code.md#the-whole-process-in-a-nutshell)). |
| ClientType | Confidential | Your external app "will work" with internal users only, so there'll be no "public" acccess. We can assume that it can keep a secret securely (in fact, it's a must). |
| ApplicationSecretHash | `<base64(sha256(your-secret))>` | The external app's secret. |

All other attributes can have their default values. They are not covered by this example.

## Steps

Unlike the [Basic example of acquiring an access token](./basic-acquire-access-token.md), where everything is clear enough to describe with simple HTTP requests, here the examples are shown in the context of a simple C# console application (i.e. an external app).

### Initialization

First we'll declare and initialize some constants and variables which we'll use later.

```cs
const string AuthorizeUri = "https://demodb.my.erp.net/id/connect/authorize";
const string TokenUri = "https://demodb.my.erp.net/id/connect/token";
const string CallbackUri = "http://localhost/signin-callback/";
const string TrustedAppUri = "my.trusted.app/first";
const string TrustedAppSecret = "<my_plain_app_secret>";

const string DomainApiTestUri = "https://demodb.my.erp.net/api/domain/odata/Crm_Customers?$top=10";
```

We won't waste time, explaining about the constants, their names are self-explanatory. The next two variables are a little more interesting,

```cs
string authCode = string.Empty;
string authState = Guid.NewGuid().ToString();
ClientAuthData clientData;
```

The `authCode` keeps the authorization code you'll receive after the internal user logs in successfully. So, initially will be an empty string.

The `authState` is a random string, used for security purpose. You'll pass it to the authorize endpoint and @@name Identity will return it back to your `CallbackUri` uri (as you can guess, you need to compare them- if they differ, it's most likely a malicious attempt).

The last variable `clientData` will help us, holding the client (i.e. internal user) auth data such as access token, scopes, access token expiration, etc. Here's what the `ClientAuthData` type looks like:

```cs
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
```

Because we'll receive this data as a raw json string, the `JsonPropertyName` attribute will help us deserialize it with a "one-liner".

### HTTP server (a.k.a. the callback listener)

This is also part of the initialization, but it's an essential part of the whole process. That's why we condider it separately.

> [!NOTE]
> If your external application is web-based, you don't need this step, because you already "have" a web server. You just have to handle the `CallbackUri` as an additional page, route, etc.

```cs
var httpListener = new HttpListener();
httpListener.Prefixes.Add(CallbackUri);

httpListener.Start();
```

We're creating an object of type `HttpListener` (that's the [HttpListener Class](https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistener), based on [HTTP.sys](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/httpsys)).

After, we are adding our `CallbackUri` (as a place where we'll "listen") and we just start the server.

The server is started, but we have to handle when the `CallbackUri` is requested. Because we're listening on just one uri, the following code is enough:

```cs
var signCallbackTask = httpListener
    .GetContextAsync()
    .ContinueWith(HandleSignInCallback);
```

`GetContextAsync()` waits for an incoming request, but as an asynchronous operation.

After, `ContinueWith()` will proceed when the task is completed (i.e. an incoming request is made). There our delegate `HandleSignInCallback` will be passed the completed task. The `HandleSignInCallback` implementation is below.

> [!NOTE]
> It doesn't matter what the http server is. Here we're using the HTTP.sys implementation only because it's super simple to initialize.

> [!WARNING]
> HTTP.sys is Windows based web server. This code won't work on non-win OS. Also the process of "adding a prefix", requires elevated permissions to work, so you have to start this example as an administrator.

### Authorize endpoint

Now this is the first step towards the essentials- you'll request an authorize code. This is done by submitting a GET request to the authorize endpoint. Here're the needed request parameters:

```cs
var authorizeUriArgs =
    $"client_id={TrustedAppUri}&" +
    $"redirect_uri={CallbackUri}&" +
    "response_type=code id_token&" +
    "response_mode=form_post&" +
    "scope=openid profile offline_access DomainApi&" +
    "nonce=abc&" +
    $"state={authState}";
```

As you see, you're passing the following:
- The uri of your trusted app `TrustedAppUri`.
- The `CallbackUri` - the uri where you're waiting for the callback, when the user signs in.
- The scopes your external app needs.
- The `authState`, described at the beginning.

In addition, there are two other important arguments:
- `response_type=code id_token`. This "instructs" @@name Identity to send you a **code** (i.e. authorization code) and an identity token (not discussed in this topic).
- `response_mode=form_post`. This "tells" @@name Identity that you are expecting the callback request as an HTTP POST request.

Our request args are prepared, so we just have to execute the GET request (i.e. the authorize endpoint).

```cs
// Start the authorize endpoint with your default browser.
// This way you'll be able to enter your credentials.
var processStartInfo = new ProcessStartInfo("cmd", $"/c start {AuthorizeUri}?{authorizeUriArgs.Replace("&", "^&").Replace(" ", "%20")}")
{
    CreateNoWindow = true
};

Process.Start(processStartInfo);
```

This is Windows based code that will open the GET request via the windows command shell.

This is necessary, because as described above, if the request is successful, a redirect to the @@erpnet login page will be made. 

Because our example external app is console application (i.e. we can't easily handle such a redirect, nor can we visualize the login page itself), so we'll just make the request with our default browser. In the meantime we'll wait the signin callback to trigger.

```cs
// Wait until the redirect is made.
signCallbackTask.Wait();
```

In other words, we're blocking our external app here, until the sign in callback is called. More precisely our `HandleSignInCallback` function completes after the the callback is called.

### Sign in callback

And here's how we handle the callback:

```cs
void HandleSignInCallback(Task<HttpListenerContext> httpListenerContextTask)
{
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
}
```

Only the essential part of the function's body is shown (the full code is available [below](./console-app-access-token-auth-code.md#everything-together))

In short, you process a simple POST request, in which you're interested in only two parameters, part of its body.

1. The `state` - it must be equal to our `authState`, passed to the authorize endpoint.
2. The `code` - this is our authorization code.

### Token endpoint

Now when you have an authorization code, you can easily acquire an access token. For this to happen, you need to make a POST request to the token endpoint (i.e. `TokenUri`).

But first you need to prepare the body of the POST request. Here's how:

```cs
var tokenUriBody =
    $"client_id={TrustedAppUri}&" +
    $"client_secret={TrustedAppSecret}&" +
    "grant_type=authorization_code&" +
    $"code={authCode}&" +
    $"redirect_uri={CallbackUri}";
```
As you can see, it's quite simple. You're passing the following:
* Your trusted app uri and its secret `TrustedAppUri`, `TrustedAppSecret`.
* The authorization code you received in the previous step `authCode`.
* The `CallbackUri`.
* And the very important one `grant_type=authroization_code` - this is the moment when you "tell" @@name Identity that you'll use the authorization code flow.

Then just send it and make sure that the returned http status code is a successful one:

```cs
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
```

Finally, you have our access token. It's in the response:

```cs
// Deserialize the JSON response as a ClientAuthData struct.
clientData = JsonSerializer.Deserialize<ClientAuthData>(httpResponse.Content.ReadAsStream());

Console.WriteLine($"Access token: {clientData.AccessToken}");
Console.WriteLine($"Refresh token: {clientData.RefreshToken}");
```

### Authorized Domain API call

Now you're authorized and you can make a legitimate call to the @@erpnet Domain Api. E.g.

```cs
httpRequest = new HttpRequestMessage()
{
    Method = HttpMethod.Get,
    RequestUri = new Uri(DomainApiTestUri)
};
httpRequest.Headers.Add("Authorization", $"Bearer {clientData.AccessToken}");

httpResponse = httpClient.Send(httpRequest);
```

The response will contain the result of the query.

### Everything together

```cs
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
```

## Resources

The sample project in this example can be found here:

https://github.com/ErpNetDocs/dev/tree/master/domain-api/samples/src/step-by-step/AccessTokenCodeConsole

--

https://docs.erp.net/dev/topics/authentication/authentication-flows.html

https://docs.erp.net/dev/topics/authentication/trusted-applications.html

https://docs.erp.net/dev/domain-api/authentication.html

https://auth0.com/docs/get-started/authentication-and-authorization-flow/authorization-code-flow