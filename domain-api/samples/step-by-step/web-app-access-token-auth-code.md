---
erp.type: sample
erp.topic: security
---

# Access token via authorization code flow in a web app

## Objective

You have an external web application that will be authorized via the @@erpnet login form on behalf of an internal user. The application MAY or MAY NOT provide an UI, but there'll be user interaction at least for the initial login.

Or, 
* Your external app is an interactive application.
* Your external app is a web application.
* It will be authenticated and authorized via the @@erpnet login form (on behalf of an @@erpnet internal user).
* Your external application will access the @@erpnet instance on behalf of the logged user.
* Your external app MAY provide UI.
* It's able to keep a secret, so it's also a condfidential application.
* There'll be user interaction (at least for the internal user to log in), so your external app will use [authorization code flow](https://auth0.com/docs/get-started/authentication-and-authorization-flow/authorization-code-flow).

## The whole process in a nutshell

After all, your final goal is to acquire an access token. The process is very similar to this example [Basic example of acquiring an access token](./basic-acquire-access-token.md), but here is added another intermediate step - the process of impersonating a user. Here's a summary of how the whole process goes:

1. Your external app will navigate to the so called [authorize endpoint](../../authentication.md#authorize-endpoint), passing your trusted app details (the trusted app, corresponding to your external app).
2. If all's OK, the browser where your app is opened will be redirected to the @@erpnet login page, where the user will enter their credentials.
3. If the user logs in successfully, the @@erpnet login page (i.e. @@erpnet Identity Server) will be redirect to a uri, back to your external web app.
4. There you'll receive an authorization code.
5. Finally you'll exchange the auth code for an access token at the token endpoint.
6. You'll obtain an access token on behalf of the logged user (2).

## Prerequisites

You have a trusted application, defined as follows:

| Attribute | Value                | Comment |
| --------- | -------------------- | ------- |
| Name      | My first trusted app | This value doesn't matter much. It's used for user-friendly identification. |
| ApplicationUri | my.trusted.app | This is your trusted app's unique identifier. It's used in the authentication process. |
| IsEnabled | true | |
| ImpersonateAsInternalUserAllowed | true | The trusted application will allow authentication from internal users. |
| ImpersonateLoginUrl | https://my.trusted.app/app.php | The url where your external app will receive the authorization code when the user logs in successfully (see [step 3 in the section above](./web-app-access-token-auth-code.md#the-whole-process-in-a-nutshell)). |
| ClientType | Confidential | Your external app "will work" with internal users only, so there'll be no "public" acccess. We can assume that it can keep a secret securely (in fact, it's a must). |
| ApplicationSecretHash | `<base64(sha256(your-secret))>` | The external app's secret. |

All other attributes can have their default values. They are not covered by this example.

## Steps

Unlike the [Basic example of acquiring an access token](./basic-acquire-access-token.md), where everything is clear enough to describe with simple HTTP requests, here the examples are shown in the context of a simple PHP application (i.e. an external app).

> [!WARNING]
> **Don't use this code in production.**
>
> Its purpose is only demonstrative. It lacks input validations, error handling and so.

### Initialization

First we'll define some constants which we'll use later.

```php
const AUTHORIZE_URI = "https://demodb.my.erp.net/id/connect/authorize";
const TOKEN_URI = "https://demodb.my.erp.net/id/connect/token";
const CALLBACK_URI = "https://my.trusted.app/app.php";
const TRUSTED_APP_URI = "my.trusted.app";
const TRUSTED_APP_SECRET = "<my_plain_app_secret>";

const DOMAIN_API_TEST_URI = "https://demodb.my.erp.net/api/domain/odata/Crm_Customers?\$top=10";
```

We won't waste time, explaining about the constants, their names are self-explanatory.

### Main code

Take a look at this snippet:

```php
if (isset($_POST) && isset($_POST["code"])) {
    
    $authCode = $_POST["code"];
    $accessToken = acquireAccessToken($authCode);

    domainApiCall($accessToken);
    exit();
}

sendAuthorizationRequest();
```

This is the main "algorithm" of our example external app. What does it do? Easy,
- If a POST request is made and its array contains `code`:
  - Assume that this `code` is the authorization code, sent back from the Identity Server.
  - Pass this code to `acquireAccessToken`, so an access code will be acquired.
  - Make a call to the DomainApi via `domainApiCall`, passing the access token.
- Else send a new authorization request.

### Authorization request (Authorize endpoint)

It's quite simple:

```php
function sendAuthorizationRequest() {
    $authorizeRequest = AUTHORIZE_URI . "?" .
      "client_id=" . TRUSTED_APP_URI . "&" .
      "redirect_uri=" . CALLBACK_URI . "&" .
      "response_type=code%20id_token&" .
      "response_mode=form_post&" .
      "scope=openid%20profile%20offline_access%20DomainApi&" .
      "nonce=abc&" .
      "state=xyz";

    header("Location: $authorizeRequest");
}
```
Just set-up the necessary arguments for the authroize endpoint and initiate a redirect. The Identity Server will take care of everything else, i.e.,
1. Will navigate to the @@erpnet login page.
2. After successful user login, will redirect to your `CALLBACK_URI`.

As you see, you're passing the following:
- The uri of your trusted app `TRUSTED_APP_URI`.
- The `CALLBACK_URI` - the uri where you're waiting for the callback, when the user signs in.
- The scopes your external app needs.

In addition, there're two other important arguments:
- `response_type=code id_token`. This "instructs" the Identity Server to send you a **code** (i.e. authorization code) and an identity token (not discussed in this topic).
- `response_mode=form_post`. This "tells" the Identity Server that you're expecting the callback request as an HTTP POST request.

### Access token request (Token endpoint)

The process of acquiring an access token is very simple also. When you already have the authorization code, you just have to send (i.e. make a POST request) it to the token endpoint. After, you will receive your access token as a response.

```php
function acquireAccessToken($authCode) {
    $tokenRequestBody = array(
        "client_id" => TRUSTED_APP_URI,
        "client_secret" => TRUSTED_APP_SECRET,
        "grant_type" => "authorization_code",
        "code" => $authCode,
        "redirect_uri" => CALLBACK_URI
    );

    $opt = array(
        'http' => array(
            'header' => "Content-type: application/x-www-form-urlencoded\r\n",
            'method' => 'POST',
            'content' => http_build_query($tokenRequestBody)
        )
    );

    $context = stream_context_create($opt);
    $result = file_get_contents(TOKEN_URI, false, $context);
    if ($result == FALSE) {
        return false;
    }

    $clientAuthData = json_decode($result, true);
    return $clientAuthData["access_token"];
}
```

### Authorized Domain API call

Now we're authorized and we can make a legitimate call to the @@erpnet Domain Api. E.g.,

```php
function domainApiCall($accessToken) {
    $opt = array(
        'http' => array(
            'header' => 'Authorization: Bearer ' . $accessToken)
    );

    $context  = stream_context_create($opt);
    $response = file_get_contents(DOMAIN_API_TEST_URI, false, $context);

    print_r($response);
}
```

The response will contain the result of the query.

### Everything together

```php
<?php 

const AUTHORIZE_URI = 'https://demodb.my.erp.net/id/connect/authorize';
const TOKEN_URI = 'https://demodb.my.erp.net/id/connect/token';
const CALLBACK_URI = 'https://my.trusted.app/app.php';
const TRUSTED_APP_URI = 'my.trusted.app';
const TRUSTED_APP_SECRET = '<my_plain_app_secret>';

const DOMAIN_API_TEST_URI = 'https://demodb.my.erp.net/api/domain/odata/Crm_Customers?$top=10';

if (isset($_POST) && isset($_POST['code'])) {
    
  $authCode = $_POST['code'];
  $accessToken = acquireAccessToken($authCode);

  domainApiCall($accessToken);

  exit();
}

sendAuthorizationRequest();

function sendAuthorizationRequest() {
  $authorizeRequest = AUTHORIZE_URI . '?' .
    'client_id=' . TRUSTED_APP_URI . '&' .
    'redirect_uri=' . CALLBACK_URI . '&' .
    'response_type=code%20id_token&' .
    'response_mode=form_post&' .
    'scope=openid%20profile%20offline_access%20DomainApi&' .
    'nonce=abc&' .
    'state=xyz';
  
  header("Location: $authorizeRequest");
}

function acquireAccessToken($authCode) {
  $tokenRequestBody = array(
    'client_id' => TRUSTED_APP_URI,
    'client_secret' => TRUSTED_APP_SECRET,
    'grant_type' => 'authorization_code',
    'code' => $authCode,
    'redirect_uri' => CALLBACK_URI
  );
  
  $opt = array(
    'http' => array(
      'header' => 'Content-type: application/x-www-form-urlencoded',
      'method' => 'POST',
      'content' => http_build_query($tokenRequestBody)
    )
  );

  $context = stream_context_create($opt);
  $result = file_get_contents(TOKEN_URI, false, $context);
  if ($result == FALSE) {
    return false;
  }
  
  $clientAuthData = json_decode($result, true);
  
  return $clientAuthData['access_token'];
}

function domainApiCall($accessToken) {
  $opt = array(
    'http' => array(
      'header' => 'Authorization: Bearer ' . $accessToken
    )
  );

  $context  = stream_context_create($opt);
  $response = file_get_contents(DOMAIN_API_TEST_URI, false, $context);

  print_r($response);
}

?>
```

## Resources

The sample project in this example can be found here:

https://github.com/ErpNetDocs/dev/blob/master/domain-api/samples/src/step-by-step/AccessTokenCodeWeb

--

https://docs.erp.net/dev/topics/authentication/authentication-flows.html

https://docs.erp.net/dev/topics/authentication/trusted-applications.html

https://docs.erp.net/dev/domain-api/authentication.html

https://auth0.com/docs/get-started/authentication-and-authorization-flow/authorization-code-flow
