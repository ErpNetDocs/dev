# Introduction to security in ERP.net

In the good old days of lesser security, there was just a user and a password.
If we need to access an app, we enter the user name and the password and voila - we get access!

So, what is the problem to continue like this?

## World of platforms and apps

Suppose Tom has created a good little app, which automates the Sales Order creation.
You just select from one of three predefined templates and your sales order is created.

The problem is: How should Toms app access your ERP Instance?

Option 1 would be to give him your user name and password.
OK, but then your user name and password are no longer so secure, aren't they?
Even if you don't give them directly to Tom, but "just" enter them in a web form, Tom still receives your precious security secret!

Option 2 would be ... Can't we can just give Toms app access to the ERP Instance, but without sharing our credentials?

This is where OAuth 2 comes. A group of very clever people have come up with a standard, which allows users to *authorize* apps to do things on their behalf.

Enter the age of *platforms* and *apps*.

When you install an app on your phone, you authorize this app to do certain things with your data. 
The Android or iOS *platform* allows the *app* to access only the data you have authorized it to access.
You fully trust the platform, but you don't fully trust any app out there.
The platform should stop malicious attempts to steal and manipulate your data.

## OAuth in the ERP world

Similar, but still a bit more complicated paradigm can be applied to ERP Instances.
The administrator of the ERP Instance plays the role of the phone owner.
The ERP Instance is the *platform* and the *app* ... is the app.

### Install the application

Before using an app, we need to install it.
With ERP Instances, we don't actually install the app, because the app is already hosted somewhere on the web.
So, this step is skipped.

We just have to tell the system, that we trust this app.

### Manually trust an application

To manually trust an application, you create a Trusted Application record, which informs the ERP Instance, that you trust this application.

For more information, see [Trusted Applications](trusted-applications.md).

### Install from marketplace

Another way to trust an application is to select it from a marketplace.
In this case, the marketplace redirects the browser to a "Register Trusted Application" endpoint in your ERP Instance.
The registration process verifies that you are an administrator of the ERP Instance and then simply asks you if you want to trust this application.

> [!NOTE]
> This is very similar to the way your phone asks you if you trust a new application.

## Interactive and service applications

Once an application is trusted, it can use two ways to access ERP data - through service and through user accounts.

### Service applications

Service applications have no UI and use only service accounts. They can use only the *Client Credentials* flow to request service access.

### Interactive applications

Interactive applications have UI and can impersonate as both service accounts and user accounts. They use *Client Credentials* flow to request service access and *Authorization Code* flow to request user access.

For more information about Client Credentials and Authorization Code flows, read [Authorization Flows](authorization-flows.md).