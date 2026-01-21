---
erp.type: sample
erp.topic: security
---

# Basic example of acquiring an access token

## Objective

Your external application just wants to acquire an access token so can access the Domain API. As short as possible.

Or, 
* Your external app is a service application.
* It will be authenticated and authorized via an internal user.
* Your external app won't provide UI and/or interaction with external users, so it's also a condfidential application.
* There will be no interaction, so your external app will use [client credentials authorization flow](https://auth0.com/docs/get-started/authentication-and-authorization-flow/client-credentials-flow).

## Prerequisites

You have a trusted application, defined as follows:

| Attribute | Value                | Comment |
| --------- | -------------------- | ------- |
| Name      | My first trusted app | Frankly, this value doesn't matter much. It's used for user-friendly identification. |
| ApplicationUri | my.trusted.app/first | This is your trusted app's unique identifier. It's used in the authentication process. |
| IsEnabled | true | |
| SystemUserAllowed | true | You need this, because your external app will be logged as a service. |
| SystemUser | `<an-internal-erp-user>` | The user, which will be used when the application logins as a service. |
| ImpersonateAsInternalUserAllowed | true | The trusted application will allow authentication from internal users. |
| ClientType | Confidential | Your external app is a service. I.e., it can keep a secret securely. |
| ApplicationSecretHash | `<base64(sha256(your-secret))>` | The external app's secret. It's used in the process of authentication. |

All other attributes can have their default values. They are not covered by this example.

## Steps

You just need to call a pretty simple HTTP request.

```http
POST /id/connect/token HTTP/1.1
Host: demodb.my.erp.net
Content-Type: application/x-www-form-urlencoded

client_id=my.trusted.app/first&
client_secret=<my_plain_app_secret>&
grant_type=client_credentials&
scope=read
```

Yes, it's that simple. Here are some clarifications:
* The POST request is sent to the @@name Identity token endpoint directly.
* The body of the request:
  - client_id - your trusted app's application uri.
  - client_secret - the plain secret phrase of the trusted app.
  - grant_type - the way your app will get an access token.
  - scope - what's needed to access.

> [!NOTE]
> With the client_credentials flow (i.e., @@erpnet service application), the scope argument can be omitted. Then your access token will contain all scopes, defined in the trusted application.

If everything is correct, you'll receive a similar response:

```json
{
  "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IkJEbGhqYjhzOEUySm1tcWg2UDlxZEEiLCJ0eXAiOiJhdCtqd3QifQ.eyJuYmYiOjE2NTQwNzU1MjksImV4cCI6MTY1NDA3OTEyOSwiaXNzIjoiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQiLCJhdWQiOiJEb21haW5BcGkiLCJjbGllbnRfaWQiOiJwayIsImNsaWVudF9zeXN0ZW1fdXNlciI6InAua29zdG92QGVycC5iZyIsImNsaWVudF9kYiI6IkUxX0RFViIsImp0aSI6IlNob3JjNVJ2MTM2ak5POHRCMF9yRHciLCJzY29wZSI6WyJEb21haW5BcGkiLCJzZWMiLCJ1cGRhdGUiXX0.RPzYKl9xPvFcLa0O8yqzJCJtmZUS88iDeWBFa9pyvYdzfQ18E4W8w6CLJPf9whFFiJWhgAsOASVuz98-MIgj9VwTjNtXMdMAPvZC0HYPnMusYUxxYRNejjqtPG7n4V0LVzyWYHu99-YUipFBmzXxCywR8TtaBv374CKfLdS4M1vaMMYShzD22L_R3kKc3uZhQ5Ygpci1tuNC8gC6CoXIv0a9gjthwgshCzmbEmiNhjvJ7WDZ98gnzkvl5_wLANRrDYUcLPvq04OfVRn2uS-dF-NLIeO5dr7Mn905YodY4Mngr4S5WbBvrWAt0hRLO6Oy_X2KCcQdmh0Nq73ELruoBw",
  "expires_in": 3600,
  "token_type": "Bearer",
  "scope": "read"
}
```

As you can see, you now have an access token (`access_token`), expiring in an hour (`expires_in`).

Now this token could be used for your Domain API requests. E.g, 

```http
GET /api/domain/odata/Crm_Sales_Customers?$top=10 HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IkJEbGhqYjhzOEUySm1tcWg2UDlxZEEiLCJ0eXAiOiJhdCtqd3QifQ.eyJuYmYiOjE2NTQwNzU1MjksImV4cCI6MTY1NDA3OTEyOSwiaXNzIjoiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQiLCJhdWQiOiJEb21haW5BcGkiLCJjbGllbnRfaWQiOiJwayIsImNsaWVudF9zeXN0ZW1fdXNlciI6InAua29zdG92QGVycC5iZyIsImNsaWVudF9kYiI6IkUxX0RFViIsImp0aSI6IlNob3JjNVJ2MTM2ak5POHRCMF9yRHciLCJzY29wZSI6WyJEb21haW5BcGkiLCJzZWMiLCJ1cGRhdGUiXX0.RPzYKl9xPvFcLa0O8yqzJCJtmZUS88iDeWBFa9pyvYdzfQ18E4W8w6CLJPf9whFFiJWhgAsOASVuz98-MIgj9VwTjNtXMdMAPvZC0HYPnMusYUxxYRNejjqtPG7n4V0LVzyWYHu99-YUipFBmzXxCywR8TtaBv374CKfLdS4M1vaMMYShzD22L_R3kKc3uZhQ5Ygpci1tuNC8gC6CoXIv0a9gjthwgshCzmbEmiNhjvJ7WDZ98gnzkvl5_wLANRrDYUcLPvq04OfVRn2uS-dF-NLIeO5dr7Mn905YodY4Mngr4S5WbBvrWAt0hRLO6Oy_X2KCcQdmh0Nq73ELruoBw
```