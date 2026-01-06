---
erp.type: sample
erp.topic: security
---

# Access token via authorization code flow in a SPA (Single Page Application)

## Objective

You have an external SPA that will be authorized via the @@erpnet login form on behalf of an internal or external user. The application WILL provide an UI, so there'll be user interaction. Your application will be a "pure" front-end and will run entirely in the client's browser.

Or, 
* Your external app is an interactive application.
* Your external app is a web application.
* It will be authenticated and authorized via the @@erpnet login form (on behalf of an @@erpnet internal or external user).
* Your external application will access the @@erpnet instance on behalf of the logged user.
* Your external app WILL provide UI.
* Will work entirely in a client environment, so it won't be able to keep a secret. I.e., it's a public application (without a secret).
* There'll be user interaction, so your external app will use [authorization code flow](https://auth0.com/docs/get-started/authentication-and-authorization-flow/authorization-code-flow).

## The whole process in a nutshell

After all, your final goal is to acquire an access token. The process is very similar to this example [Basic example of acquiring an access token](./web-app-access-token-auth-code.md), but here is added another intermediate step - the process of impersonating a user. Here's a summary of how the whole process goes:

1. Your external app will navigate to the [authorize endpoint](../../../building-apps/concepts/how-apps-connect/identity-server.md), passing your trusted app details (the trusted app, corresponding to your external app).
2. If all's OK, the browser where your app is opened will be redirected to the @@erpnet login page, where the user will enter their credentials.
3. If the user logs in successfully, the @@erpnet login page (i.e. @@erpnet Identity) will be redirect to an uri, back to your external SPA.
4. There you'll receive an authorization code.
5. Finally you'll exchange the auth code for an access and refresh tokens at the token endpoint.
6. You'll obtain an access and refresh tokens on behalf of the logged user (2).

## Prerequisites

You have a trusted application, defined as follows:

| Attribute | Value                | Comment |
| --------- | -------------------- | ------- |
| Name      | My first trusted app | This value doesn't matter much. It's used for user-friendly identification. |
| ApplicationUri | my.trusted.app | This is your trusted app's unique identifier. It's used in the authentication process. |
| IsEnabled | true | |
| ImpersonateAsInternalUserAllowed | true | The trusted application will allow authentication from internal users. |
| ImpersonateAsCommunityUserAllowed | true | If your application allows external users. |
| ImpersonateLoginUrl | https://my.trusted.app/index.html | The url where your external app will receive the authorization code when the user logs in successfully (see [step 3 in the section above](./web-app-access-token-auth-code.md#the-whole-process-in-a-nutshell)). |
| ClientType | Public | Your external app will run entirely in a client-side uncontrolled environment, meaning it cannot keep a secret. Meaning it must be a **public** applicaiton. |

All other attributes can have their default values. They are not covered by this example.

## Steps

The application below presents a single user page using html and pure javascript.

> [!WARNING]
> **Don't use this code in production.**
>
> Its purpose is only demonstrative. It lacks input validations, error handling and so.

### Initialization

First we'll define some constants which we'll use later.

```js
var config = {
    client_id: "my.trusted.app",
    redirect_uri: "https://my.trusted.app/app.html",
    authorization_endpoint: "https://demodb.my.erp.net/id/connect/authorize",
    token_endpoint: "https://demodb.my.erp.net/id/connect/token",
    requested_scopes: "offline_access read update"
};
```

We won't waste time, explaining about the constants, their names are self-explanatory.

But there's one important detail- the `offline_access` in the `requested_scopes`. Its presence means that together with the access token you want also to receive a refresh token. It can be omitted, but then you'll only receive an access token and when it expires the user has to repeat the auth steps - i.e. to reenter its credentials.

### Body

```html
<a href="#" id="start">Click to Sign In</a>
<div id="token" class="hidden">
    <h2>Access Token</h2>
    <div id="access_token" class="code">no access token :(</div>
</div>

<div id="token" class="hidden">
    <h2>Refresh Token</h2>
    <div id="refresh_token" class="code">no refresh token :(</div>
</div>
```

That's all the UI.

- A button that initiates the auth process.
- Two "boxes" visualizing the acquired access and refresh tokens.

### Authorization request (Authorize endpoint)

The snippet below adds an event listener to the "Click to Sign In" button. When you press it, you'll trigger the authorization request - i.e. the defined function.

```js
document.getElementById("start").addEventListener("click", async function(e) {
    
    e.preventDefault();

    // Create and store a random "state" value
    var state = generateRandomString();
    localStorage.setItem("pkce_state", state);

    var url = config.authorization_endpoint 
        + "?response_type=code"
        + "&client_id=" + config.client_id
        + "&state=" + state
        + "&scope=" + config.requested_scopes
        + "&redirect_uri=" + config.redirect_uri;       

    window.location = url;
});
```
Just set-up the necessary arguments for the authroize endpoint and initiate a redirect. @@name Identity will take care of everything else, i.e.,
1. Will navigate to the @@erpnet login page.
2. After successful user login, will redirect to your `config.redirect_uri`.

As you see, you're passing the following:
- The uri (i.e. the client id) of your trusted app `config.client_id`.
- The `config.redirect_uri` - the uri where you're waiting for the callback, when the user signs in.
- The scopes your external app needs.
- `response_type=code`. This "instructs" @@name Identity to send you a **code** (i.e. authorization code).

### Access token request (Token endpoint)

The process of acquiring an access token is very simple also. When you already have the authorization code, you just have to send (i.e. make a POST request) it to the token endpoint. After, you will receive your access and refresh tokens as a response to your callback uri.

First, we'll define a helper function, that will send a POST request.

```js
function sendPostRequest(url, params, success, error) {
    var request = new XMLHttpRequest();
    request.open('POST', url, true);
    request.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
    request.onload = function() {

        var body = JSON.parse(request.response);

        if(request.status == 200) {
            success(request, body);
        } else {
            error(request, body);
        }
    }

    var body = Object.keys(params).map(key => key + '=' + params[key]).join('&');
    request.send(body);
}
```

Now, the following is the actual request to the token endpoint:

```js
var args = window.location.search.substring(1);
if (args != "") {

    // OAUTH REDIRECT HANDLING
    var idServerResponse = JSON.parse(
      '{"' + args.replace(/&/g, '","').replace(/=/g,'":"') + '"}', 
      function(key, value) { return key===""?value:decodeURIComponent(value) });

    if (idServerResponse.code) {
        
        // Exchange the authorization code for an access token
        sendPostRequest(config.token_endpoint, {
            grant_type: "authorization_code",
            code: idServerResponse.code,
            client_id: config.client_id,
            redirect_uri: config.redirect_uri
        }, function(request, body) {
            // Here you have your access and refresh tokens.
            document.getElementById("access_token").innerText = body.access_token;
            document.getElementById("refresh_token").innerText = body.refresh_token;
        });
    };
}
```

Or here's what it does broken into steps:
- Get the arguments from the current URI. That's because we're expecting the `redirect_uri` callback. I.e. @@name Identity sends our authorization code.
- If there're arguments (i.e. we **are** in the redirect scenario)- proceed.
- Parse the arguments and extract the authorization code - `idServerResponse.code`.
- Send a POST request, including the authorization code, client id and the redirect uri. The redirect uri is passed again, just as a security measure.
- Response is received- parse the access and refresh tokens and update the corresponding UI elements.

### Everything together

```html
<html>
<title>ERP.net - Pure JS access token acquisition</title>

<script>
  var config = {
    client_id: "my.trusted.app",
    redirect_uri: "https://my.trusted.app/app.html",
    authorization_endpoint: "https:///demodb.my.erp.net/id/connect/authorize",
    token_endpoint: "https:///demodb.my.erp.net/id/connect/token",
    requested_scopes: "offline_access read update"
  };
</script>

<a href="#" id="start">Click to Sign In</a>
<div id="token" class="hidden">
    <h2>Access Token</h2>
    <div id="access_token" class="code">:(</div>
</div>
<div id="token" class="hidden">
    <h2>Refresh Token</h2>
    <div id="refresh_token" class="code">:(</div>
</div>

<script>
  document.getElementById("start").addEventListener("click", async function(e) {
      
      e.preventDefault();

      // Create and store a random "state" value
      var state = generateRandomString();

      var url = config.authorization_endpoint 
          + "?response_type=code"
          + "&client_id=" + config.client_id
          + "&state=" + state
          + "&scope=" + config.requested_scopes
          + "&redirect_uri=" + config.redirect_uri;

      window.location = url;
  });

  var args = window.location.search.substring(1);
  if (args != "") {

      // OAUTH REDIRECT HANDLING
      var idServerResponse = JSON.parse(
        '{"' + args.replace(/&/g, '","').replace(/=/g,'":"') + '"}',
        function(key, value) { return key===""?value:decodeURIComponent(value) });

      if (idServerResponse.code) {
          
          // Exchange the authorization code for an access token
          sendPostRequest(config.token_endpoint, {
              grant_type: "authorization_code",
              code: idServerResponse.code,
              client_id: config.client_id,
              redirect_uri: config.redirect_uri,
              code_verifier: localStorage.getItem("pkce_code_verifier")
          }, function(request, body) {
              // Here you have your access token.
              document.getElementById("access_token").innerText = body.access_token;
          });
      };
  }

  function sendPostRequest(url, params, success, error) {
      var request = new XMLHttpRequest();
      request.open('POST', url, true);
      request.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
      request.onload = function() {

          var body = JSON.parse(request.response);

          if(request.status == 200) {
              success(request, body);
          } else {
              error(request, body);
          }
      }

      var body = Object.keys(params).map(key => key + '=' + params[key]).join('&');
      request.send(body);
  }
</script>

</html>
```

## Resources

The sample project in this example can be found here:

https://github.com/ErpNetDocs/dev/blob/master/domain-api/samples/src/step-by-step/AccessTokenCodeSPA

--

https://docs.erp.net/dev/topics/authentication/authentication-flows.html

https://docs.erp.net/dev/topics/authentication/trusted-applications.html

https://docs.erp.net/dev/domain-api/authentication.html

https://auth0.com/docs/get-started/authentication-and-authorization-flow/authorization-code-flow
