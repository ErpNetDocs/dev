# Authentication Flows

Authentication Flows are the process flows, which an external app can use to request access to the ERP Instance.

## Authorization Code

This authorization flow is used by interactive user applications, which access the ERP Instance on behalf of the logged user.

If we relate to Tom and Jane scenario, this would be the case when we want Toms app to create the Sales Orders using Jane account and access permissions.

Jane wants to start using Toms app to create her Sales Orders.
Toms app is generally authorized for the ERP Instance (with Trusted Application record), but Jane has not yet authorized it to work on her behalf!

If Toms app presents a login screen to get Janes credentials and use them to access the ERP Instance, this would expose the credentials to the app author.
So, instead, the *app* asks the *platform* to verify that it is Jane in front of the computer.

1. Toms app requests the ERP Instance to authenticate the user using the browser.
Jane is presented with a familiar log-in screen, which is used by ALL apps.
Since Jane trusts the platform, she securely enters her user name and password.

1. The ERP Instance validates Janes credentials and creates a special *id token*.
The token is actually just a string, which contains Janes details, like name, profile picture and email.

1. The token is passed back to the app, using the apps [Login URL](trusted-applications.md#impersonate-login-url).

Now, Toms app knows that it is Jane which is using his app, but without ever gaining access to her credentials!

## Client Credentials

Client Credentials is an authorization flow, with which an app requests service access.

The service account, which is used to create the session is configured in the Trusted Application record.

> [!note]
> The service session is not related to any specific interactive user. It can be created on service startup and renewed whenever necessary.

Lets relate this to the Tom and Jane scenario from the [Overview](index.md).
With service access, Toms app will create all Sales Orders with the same service account.
It would not use Janes or any other user account to access the ERP Instance.

The basic flow is the following:

1. Toms app provides its application URI and application secret to the ERP Instance.
The URI and secret are much like user name and password, but for application.
1. The ERP Instance looks up the Trusted Application record for Toms app.
Based on the Service Account specified in the record, the ERP Instance creates a special *access token* and passes it to Toms app.
The access token is just a string, containing the granted access permissions, which is digitally signed.
1. Toms app uses the *access token* to call the ERP APIs and create the Sales Orders.

## Basic Authentication

In basic HTTP authentication, a request contains a header field in the form of Authorization: Basic <credentials>, where credentials is the Base64 encoding of user name and password joined by a single colon :.
  
> [!note]
> Basic Authentication is not recommended because it is less secure than OAuth!
  
In ERP.net basic authentication can be used by applications, which access the ERP Instance on behalf of the logged user.
  
Some legacy external applications use the obsolete SDK API to access the ERP Instance. To authenticate they must provide username, password and application name. If the provided application name is not found among the trusted applications in the ERP Instance the authentication will fail.
  
Domain API and Table API allow basic authentication but require a trusted application with [ApplicationUri](trusted-applications.md#application-uri) = 'DomainApiBasicAuthentication' to be registered and configured to [allow basic authentication](trusted-applications.md#basic-authentication-allowed).
  
**Possible errors:**
    
Authentication faled for user 'test'.
 
Current request uses basic authentication but the provided application 'DomainApiBasicAuthentication' is not found among the trusted applications.
  
Contact your system administrator to create trusted application 'DomainApiBasicAuthentication' and configure it to allow basic authentication.

---

Authentication failed for user 'test'.
 
Current request uses basic authentication but the trusted application 'DomainApiBasicAuthentication' is not enabled.
 
Contact your system administrator to enable the trusted application 'DomainApiBasicAuthentication' and configure it to allow basic authentication.

---

Authentication failed for user 'test'.
 
Current request uses basic authentication but the provided application 'DomainApiBasicAuthentication' is not configured to allow basic authentication.

Contact your system administrator to configure the trusted application 'DomainApiBasicAuthentication' to allow basic authentication.

---

Authentication failed for user 'test'.

Current request uses basic authentication but the corresponding trusted application 'DomainApiBasicAuthentication' is configured for system user that is different from the provided user 'test'.

Use the correct application user to log in or contact your system administrator to configure the trusted application 'DomainApiBasicAuthentication' for different user.  
  
