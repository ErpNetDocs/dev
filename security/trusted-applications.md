# Trusted Applications

A trusted application record tells the system that a specific application is allowed to access the ERP Instance.

## General information

Trusted Application record can be created manually or through a marketplace activation.
Anyway, it is good to understand the information stored in such a record.

## Anatomy of a Trusted Application record

A Trusted Application record contains many fields.
To better understand them, read below.

### Name

This is the multi-language name of the application.
Used mostly for interactive display in UI clients.
Not important for the functioning of the application.

### Application URI

This is the application identifier.
It is passed as parameter by the applications, when they claim who they are in front of the ERP Instance.

The Application URI should be unique for the @erp-instance.
Preferably, it should be globally unique, so that the application can be listed in a marketplace.
Use short, concise identifier.
This will appear in logs and other files.
Avoid non-latin and special characters.

It is advised, that you incorporate this identifier as constant in your application and use the same URI for all "trusts" for your app.

> [!note]
> It is best to base your app URIs on your web site and some extension.
> For example, mywebsite.com/myapp1 is a good URI.

### Client Type

Client Type specifies the confidentiality abilities of the your application.

Client types are:

- **Confidential**
- **Public**

Confidential client applications are able to keep secrets. Public apps are generally unable to hide a secret from an advanced intruder.

For example:

- **JavaScript** apps, executed in the user browser, are public apps.
Generally they cannot hide a secret from an advanced user.
If a JavaScript app has a secret in its code, it can easily be revealed (and probably abused).

- **Server executed** apps are usually confidential.
They are executed in a trusted environment and, if properly secured, can keep a secret.

### Application Secret Hash

This is a bit of a challenge.
You have to use a tool to get the hash.
Type this in the browser (replace "mysecret" with your secret):

<https://demodb.my.erp.net/sys/tools/sha256?secret=mysecret>

### Basic Authentication Allowed

At first, this is a bit confusing.
It has effect on the whole ERP Instance.
Once we have at least (actually also at most) one application with Basic Authentication = ON, we have allowed Basic Authentication for the whole ERP Instance.

> [!note]
>When someone uses Basic Authentication, it would be considered as access from this app.

### Impersonate As Community / Internal User Allowed

Allows the app to request login from a community (external) or internal user.

When both options are OFF, the app would not be allowed to request a user to be authenticated.
This is a common scenario for service applications with no UI.

If any or both options are ON, the app is allowed to impersonate, e.g. request login.
The login would be successful only if the authenticated user is of one of the allowed types.

Common scenarios:

- **Service app** - both options OFF.
- **Internal interactive app** - only Internal users allowed.
- **Public web app** - both Community and Internal users allowed.

> [!note]
> Avoid allowing only Community and disallowing Internal users.

Usually, community accounts can be freely created by anybody.
So, allowing only community accounts could create confusion for the internal users and force them to create a separate, external (community) account.

> [!note]
> It is strongly not recommended for a user to have duplicate accounts, just for the purpose of having both community and internal accounts.

### Impersonate Login URL

This is used only for applications, which use the Authorization Code Flow.
It is called after a successful login and receives the generated tokens.

Usually, this URL is a dedicated endpoint in the app environment.

The endpoint should conform to:

<https://openid.net/specs/openid-connect-core-1_0.html#AuthResponse>

### Impersonate Logout URL

Reserved for future extensions.

### Is Enabled

Disables the access of the application.

### Scope

The scope (according to [RFC 6749](https://tools.ietf.org/html/rfc6749#section-3.3)) for which the application was trusted.
The scope is an unordered list of space-delimited case-sensitive strings.
Each string denotes a permission.

> [!note]
> Currently, this option is ignored.

The following token scopes are PLANNED for near future developments:

- **update**

Allows the user application to update data in the ERP Instance.
Without this scope, the application can only read data.

- **sec**

Allows the application to access the security infrastructure of the ERP Instance.
Generally, this scope should **NEVER** be trusted to user apps.

Scopes, reserved for future use:

- **profile**
- **general**
- **crm**
- **logistics**
- **production**
- **finance**
- **personaldata**

### System User

This is the service user, which will be used to initiate sessions, when the application requests token with [Client Credentials](authorization-flows.md#Client-Credentials).

### System User Allowed

Specifies whether the application is allowed to request service access.

### System User Login URL

Reserved.
