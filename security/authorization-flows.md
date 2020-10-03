# Authorization Flows

Authorization Flows are the process flows, which an external app can use to request access permission to the ERP Instance.

## Client Credentials

Client Credentials is an authorization flow, with which an app requests service access.

The service account, which is used to create the session is configured in the Trusted Application record.

> [!note]
> The service session is not related to any specific interactive user. It can be created on service startup and renewed whenever necessary.

Lets relate this to the Tom and Jane scenario from the [Overview](overview.md).
With service access, Toms app will create all Sales Orders with the same service account.
It would not use Janes or any other user account to access the ERP Instance.

The basic flow is the following:

1. Toms app provides its application URI and application secret to the ERP Instance.
The URI and secret are much like user name and password, but for application.
1. The ERP Instance looks up the Trusted Application record for Toms app.
Based on the Service Account specified in the record, the ERP Instance creates a special *access token* and passes it to Toms app.
The access token is just a string, containing the granted access permissions, which is digitally signed.
1. Toms app uses the *access token* to call the ERP APIs and create the Sales Orders.

## Authorization Code

This authorization flow is used by interactive user applications, which access the ERP Instance on behalf of the logged user.

If we relate to Tom and Jane scenario, this would be the case when we want Toms app to create the Sales Orders using Jane account and access permissions.

Jane wants to start using Toms app to create her Sales Orders.
Toms app is generally authorized for the ERP Instance (with Trusted Application record), but Jane has not yet authorized it to work on her behalf!

If Toms app presents a login screen to get Janes credentials and use them to access the ERP Instance, this would expose the credentials to the app author.
So, instead, the *app* asks the *platform* to verify that it is Jane in front of the computer.

Jane is presented with a familiar log-in screen, which is used by ALL apps.
After successfully validating her credentials,  the system creates a special *id token* and passes it to the app.
The token is actually just a string, which contains Janes details, like name, profile picture and email.

Now, Toms app knows that it is Jane which is using his app, but without ever gaining access to her credentials!