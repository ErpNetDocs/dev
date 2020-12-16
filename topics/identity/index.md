# Introduction to Identity in @@name

@@name APIs use [OAuth 2](https://oauth.net/2/) for authentication and authorization.
In OAuth 2, the authentication and authorization is performed by an identity provider.
@@name has built-in identity provider, which conforms to the [OpenID Connect](https://openid.net/connect/) specification.

## Why do we need OAuth 2?

In the good old days of lesser security, there was just a user and a password.
If we need to access an app, we enter the user name and the password and voila - we get access!

So, what is the problem to continue like this?

Suppose Tom has created a good little app, which automates the Sales Order creation.
You just select from one of three predefined templates and your sales order is created.

The problem is: How should Toms app access your ERP Instance?

Option 1 would be to give him your user name and password.
OK, but then your user name and password are no longer so secure, aren't they?
Even if you don't give them directly to Tom, but "just" enter them in a web form, Tom still receives your precious security secret!

Option 2 would be ... Can't we can just give Toms app access to the ERP Instance, but without sharing our credentials?

This is where OAuth 2 comes in. A group of very clever people have come up with a standard, which allows users to *authorize* apps to do things on their behalf.

Enter the age of *platforms* and *apps*.

> [!note]
> To be more precise, it should be called age of Identity providers and apps.
> But in the current case, the platform is also the Identity provider.

## Trust an application

When you install an app on your phone, you authorize this app to do certain things with your data. 
The Android or iOS *platform* allows the *app* to access only the data you have authorized it to access.
You fully trust the platform, but you don't fully trust any app out there.
The platform should stop malicious attempts to steal and manipulate your data.

Similar, but still a bit more complicated paradigm can be applied to ERP Instances.
The administrator of the ERP Instance plays the role of the phone owner.
The ERP Instance is the *platform* and the *app* is, well, the app.

In our phones, before using an app, we need to install it.
With ERP Instances, we don't actually install the app, because the app is already hosted somewhere.
So, this step is skipped.
We just have to tell the system, that we trust this app.

### Manually trust an application

To manually trust an application, you create a Trusted Application record, which informs the ERP Instance, that you trust this application.

For more information, see [Trusted Applications](trusted-applications.md).

### Install from a marketplace

Another way to trust an application is to activate it from a marketplace.
In this case, the marketplace redirects the browser to a "Register Trusted Application" endpoint in your ERP Instance.
The registration process verifies that you are an administrator of the ERP Instance and then simply asks you if you want to trust this application.

> [!NOTE]
> This is very similar to the way your phone asks you if you trust a new application.

## Access data in the ERP Instance

Once an application is trusted, it can use two ways to access ERP data - through user account or through service access.

### Interactive applications

Interactive applications have UI and use *Authorization Code* flow to request user access.

### Service applications

Service applications have no UI and use only service accounts.
They use the *Client Credentials* flow to request service access.

> [!note]
> A single application can use both interactive user login and service access to perform different functions.

Read more about [Authentication Flows](authentication-flows.md).
