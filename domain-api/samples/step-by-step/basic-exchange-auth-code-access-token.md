# Basic example- exchange an auth code for an access token

## Objective

You have an external application that requires user login. You want to acquire an access token on behalf on the logged user. As short as possible.

Or, 
* Your external app is an interactive application (at least for the user to log in).
* It will be authenticated and authorized via the @@erpnet login form (on behalf of an @@erpnet internal user).
* Your external application will access the @@erpnet instance on behalf of the logged user.
* Will work with @@erpnet internal users and it's able to keep a secret, so it's also a condfidential application.
* There'll be user interaction (because of the login), so your external app will use [authorization code flow](https://auth0.com/docs/get-started/authentication-and-authorization-flow/authorization-code-flow).

## Remarks

This example doesn't show a specific implementiation, but the required steps you need to perform. You can find more information about specific implementations in the [Resources](./basic-exchange-auth-code-access-token.md#resources) section.

The main difference with the [Basic example of acquiring an access token](./basic-acquire-access-token.md) is that there's an additional step, which is a subject of a specific technical implementation. I.e.,

1. Your external app performs an HTTP GET request to the authorize endpoint.
2. Wait for the Identity Server to:
    1. Redirect to the @@erpnet login page.
    2. "Make" an HTTP request to your external app, after the user logs in successfully.
3. Now your external app can make an HTTP POST request to the token endpoint.
4. The Identity Server creates an access token and and returns it to you.

As you can see, the extra steps, compared to the other example, are (1) and (2). I.e., 
1. You have to provide a way to visualize the @@erpnet login page (the usual way is via a web browser).
2. Also a mechanism to handle a HTTP request.

Actually, this is the essential difference between `client_credentials flow` and the `authorization code flow`. The first is bound to a specific, so-called service user while the other- to the user who will log in via the login form of the Identity Provider (i.e., the @@erpnet Identity Server).

> [!NOTE]
> It's important to clarify the following quote above, _(2.2) "Make" an HTTP request to your external app, after the user logs in successfully._, referring the @@erpnet Identity Server.
>
> The HTTP request to your external app (i.e. the redirect) is made locally, as a result from your request to the authorize endpoint. In general, this means that your external app doesn't need to be accessible outside of the network where it's avaialble. So said, if your app is a "pure" local one, external callbacks such as `localhost/signin-callback`, `192.168.10.55/signin-callback` are completely fine.

## Prerequisites

You have a trusted application, defined as follows:

| Attribute | Value                | Comment |
| --------- | -------------------- | ------- |
| Name      | My first trusted app | This value doesn't matter much. It's used for user-friendly identification. |
| ApplicationUri | my.trusted.app/first | This is your trusted app's unique identifier. It's used in the authentication process. |
| IsEnabled | true | |
| ImpersonateAsInternalUserAllowed | true | The trusted application will allow authentication from internal users. |
| ImpersonateLoginUrl | http://localhost/signin-callback | The url where your external app is listening. Redirection to this uri will be performed after the user logs in successfully. |
| ClientType | Confidential | Your external app "will work" with internal users only, so there'll be no "public" acccess. We can assume that it can keep a secret securely (in fact, it's a must). |
| ApplicationSecretHash | `<base64(sha256(your-secret))>` | The external app's secret. |

All other attributes can have their default values. They are not covered by this example.

## Steps

### Authorize endpoint

You just need to call a simple GET request.

```http
GET /id/connect/authorize?
    client_id=my.trusted.app/first&  
    redirect_uri=http://localhost/signin-callback&  
    response_type=code id_token&  
    scope=openid profile offline_access DomainApi&
    nonce=abc&  
    state=xyz HTTP/1.1
Host: demodb.my.erp.net
```

If everything is OK, the following will happen:
1. A redirect to the @@erpnet login page will be made.
2. After the user logs in successfully, a redirect back to your external app will be performed.

### Sign in callback

The previous step leads here. You'll receive a GET request such as:

```http
GET /signin-callback?
    code=g0ZGZmNjVmOWI&
    state=dkZmYxMzE2 HTTP/1.1
Host: localhost
```

Where the `code` in the uri query is your authorization code.

Now you're ready to exchange this authorization code for an access token.

### Token endpoint

Once you have the authorization code, obtaining the access token is pretty easy. Just make the following POST request.

```http
POST /id/connect/token HTTP/1.1
Host: demodb.my.erp.net
Content-Type: application/x-www-form-urlencoded

client_id=my.trusted.app/first&
client_secret=<my_plain_app_secret>&
grant_type=authorization_code&
code=g0ZGZmNjVmOWI&
redirect_uri=http://localhost/signin-callback
```

That's all. You'll receive something like this:

```json
{
  "id_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IkJEbGhqYjhzOEUySm1tcWg2UDlxZEEiLCJ0eXAiOiJKV1QifQ.eyJuYmYiOjE2NTUwNTc3MjQsImV4cCI6MTY1NTA1ODAyNCwiaXNzIjoiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQiLCJhdWQiOiJwayIsIm5vbmNlIjoiYWJjIiwiaWF0IjoxNjU1MDU3NzI0LCJhdF9oYXNoIjoibUhfSUZEUVppRHdZb2h5a0FZR2NJZyIsInNfaGFzaCI6IlRjMWtiNVB1U2lheEN2NXVJZHZ6ZlEiLCJzaWQiOiJ1Q3FiZkI4OHpYMXUzOW40NERwVjFRIiwic3ViIjoiYWRtaW4iLCJhdXRoX3RpbWUiOjE2NTUwMjc3MTksImlkcCI6ImxvY2FsIiwiYW1yIjpbInB3ZCJdfQ.CzTk7SXiqcgpjVXCvdgDKJ92bt2a93R76l5WmCIZ6hMG6VDYHXlkBlqmG15l8Zsc1SpLn949f-OQn4nK1LaLkOA1rrMfT6lhMdrdBkQED7mYrjTyRqUJHnkriYpLsbL4Ze5gOP1M0HlDi6ZWjhZyzJgEyqi_T44lmlyZc0ujQ0Zba-_afXV7VpmgL9dIPwSmhuP14x2UJIGziBE8m23DL4GqTMQYgX0HNGLa2Tgiztp4h9ABBWWhj_iEKJ3ZoZ3CfMVMn53fqDaf9fuIrgYrOOTKqE7UrxH2bhdLUlaqka7KeGIsRd7f6wV2XqFDfY3vtW85CzQnjuGhj-qAJoZjCw",
  "access_token": "eycccGciOiJSUzI1NiIsImtpZCI6IkJEbGhqYjhzOEUySm1tcWg2UDlxZEEiLCJ0eXAiOiJhdCtqd3QifQ.eyJuYmYiOjE2NTUwNTc3MjQsImV4cCI6MTY1NTA2MTMyNCwiaXNzIjoiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQiLCJhdWQiOiJEb21haW5BcGkiLCJjbGllbnRfaWQiOiJwayIsInN1YiI6ImFkbWluIiwiYXV0aF90aW1lIjoxNjU1MDI3NzE5LCJpZHAiOiJsb2NhbCIsImlkIjoiOWRhNjQ4MzktYThkMC00OTFkLWFlYmItNGQxOGZhNDJiMDE0IiwibmFtZSI6IlNQUyIsImVtYWlsIjoiYWRtaW5AbWFpbC5jb20iLCJ1c2VyX3R5cGUiOiJJbnRlcm5hbFVzZXIiLCJpc19hZG1pbi1ee6InRydWUiLCJlbWFpbF92ZXJpZmllZCI6ImZhb2NlIiwiZGIiOiJFMV9ERVYiLCJsb2NhbGUiOiJieyIsImp0aSI6Il9sdEJMS3djSlNLbUFhM25mbFpwNFEiLCJzaWQiOiJ1Q3FiZds2898pYMXUzOW40NqRwVjFRIiwic2NvcGUiOlsib3BlbmlkIiwicHJvZmlsZSIsIww1kRvbWFpbkFwaSIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJwd2QiXX0.AzHxj_iBM3bfcOtdaSNHNbPUHGCf0JAo7fV1fo9JT-rqCHjc0t8VEa1qO5R2jemvs7vDBf6GARxgul3pAy7YQpmqzzruswoLDkDdUMX1LXzHLgp0ppYoNa1A_M_O4UTXCe7xGBRHSyRRQLGsTTGMkv1pK0E3Xn3rAfOPvo4wfrQ8QabVcdA7mupY4qF01tIHPv_7NGS2SyPfCVdAYcxUy8HpQ-RdoXMaVWVz_JhXgMNZ9_nFTxedPGakZJMDjnvYss_GKjucbeYdZM9jSrqEmXDw6s8A3o1jKOyurzIBzug55Dxee8UBWepcO5S08GPguBFotamUvStMdDY0KkmZYvw",
  "expires_in": 3600,
  "refresh_token": "6-Cv7vQ5ouhYzs0AWg6tsG-YK7O5xP_kb5Qb8wEJMnw",
  "scope": "openid profile DomainApi offline_access"
}
```

### Authorized Domain API call

Now you're authorized and we can make a legitimate call to the @@erpnet Domain Api. E.g.,

```http
GET /api/domain/odata/Crm_Customers?$top=10 HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer eycccGciOiJSUzI1NiIsImtpZCI6IkJEbGhqYjhzOEUySm1tcWg2UDlxZEEiLCJ0eXAiOiJhdCtqd3QifQ.eyJuYmYiOjE2NTUwNTc3MjQsImV4cCI6MTY1NTA2MTMyNCwiaXNzIjoiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQiLCJhdWQiOiJEb21haW5BcGkiLCJjbGllbnRfaWQiOiJwayIsInN1YiI6ImFkbWluIiwiYXV0aF90aW1lIjoxNjU1MDI3NzE5LCJpZHAiOiJsb2NhbCIsImlkIjoiOWRhNjQ4MzktYThkMC00OTFkLWFlYmItNGQxOGZhNDJiMDE0IiwibmFtZSI6IlNQUyIsImVtYWlsIjoiYWRtaW5AbWFpbC5jb20iLCJ1c2VyX3R5cGUiOiJJbnRlcm5hbFVzZXIiLCJpc19hZG1pbi1ee6InRydWUiLCJlbWFpbF92ZXJpZmllZCI6ImZhb2NlIiwiZGIiOiJFMV9ERVYiLCJsb2NhbGUiOiJieyIsImp0aSI6Il9sdEJMS3djSlNLbUFhM25mbFpwNFEiLCJzaWQiOiJ1Q3FiZds2898pYMXUzOW40NqRwVjFRIiwic2NvcGUiOlsib3BlbmlkIiwicHJvZmlsZSIsIww1kRvbWFpbkFwaSIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJwd2QiXX0.AzHxj_iBM3bfcOtdaSNHNbPUHGCf0JAo7fV1fo9JT-rqCHjc0t8VEa1qO5R2jemvs7vDBf6GARxgul3pAy7YQpmqzzruswoLDkDdUMX1LXzHLgp0ppYoNa1A_M_O4UTXCe7xGBRHSyRRQLGsTTGMkv1pK0E3Xn3rAfOPvo4wfrQ8QabVcdA7mupY4qF01tIHPv_7NGS2SyPfCVdAYcxUy8HpQ-RdoXMaVWVz_JhXgMNZ9_nFTxedPGakZJMDjnvYss_GKjucbeYdZM9jSrqEmXDw6s8A3o1jKOyurzIBzug55Dxee8UBWepcO5S08GPguBFotamUvStMdDY0KkmZYvw
```

## Resources

[Authorization code flow with a web based external application](./web-app-access-token-auth-code.md)

[Authorization code flow with a console based external application](./console-app-access-token-auth-code.md)

--

https://docs.erp.net/dev/topics/authentication/authentication-flows.html

https://docs.erp.net/dev/topics/authentication/trusted-applications.html

https://docs.erp.net/dev/domain-api/authentication.html

https://auth0.com/docs/get-started/authentication-and-authorization-flow/authorization-code-flow
