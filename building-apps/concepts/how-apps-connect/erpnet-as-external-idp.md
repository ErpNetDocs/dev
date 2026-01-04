# Using @@name Identity for SSO

@@name Identity can act as a fully compliant **OpenID Connect (OIDC)** identity provider for any external application - web, desktop, mobile, or service-based.  

Integrating with it allows your application to authenticate users using their @@name accounts and receive industry-standard identity and access tokens (ID token, access token, refresh token).

This guide explains how an application can connect to @@name Identity and use it as an external identity provider.

> [!NOTE]
> This page describes the **@@name global identity provider** (`https://id.erp.net/id`) used when @@name acts as an **external Identity Provider** (SSO/sign-in) for your application.  
> It is **not** the mechanism for per-instance API access. To call the APIs of a specific ERP.net instance, use the **Instance Identity Service** (`https://{instance}.my.erp.net/id`) and configure a **Trusted Application** in that instance.  
> If you’re unsure which authority applies, see [Choose an identity authority (instance vs global)](./identity-authorities.md).

## When to Use @@name as an External IdP

Use @@name as the Identity Provider when:

- Your application needs to authenticate users with their @@name accounts  
- You want **single sign-on (SSO)** between @@name and other systems  
- You do not want to manage user passwords yourself  
- You want standardized identity data via OIDC claims  

@@name supports **standard OAuth 2.0 + OIDC flows**, so any OIDC-compatible framework or library can integrate with it.

## Quick Start (Platform-Agnostic)

To integrate your application with @@name Identity, configure an OIDC client with the following parameters:

### Required settings

- **Authority**  
  `https://id.erp.net/id`  
  *(or your tenant-specific @@name Identity URL)*

- **Client ID** (provided by @@name)
- **Client Secret** (provided by @@name; confidential clients only)

- **Scopes**  
  - `openid`  
  - `profile`  
  - *(optional)* `offline_access` (refresh tokens)  

- **Response type**  
  - `code id_token`

- **Login callback (redirect URI)**  
  URL where @@name redirects after successful login.

- **Logout callback**  
  URL where @@name redirects after logout.

## Example: ASP.NET Core (Confidential Web App)

```csharp
.AddOpenIdConnect("ErpNet", options =>
{
    options.Authority = "https://id.erp.net/id";
    options.ClientId = "<your-client-id>";
    options.ClientSecret = "<your-client-secret>";
    options.ResponseType = "code id_token";

    options.CallbackPath = "/signin-erpnet";
    options.SignedOutCallbackPath = "/signout-erpnet";

    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("offline_access");

    options.GetClaimsFromUserInfoEndpoint = true;
    options.SaveTokens = true;
});
```

## Redirect / Callback URLs

Your integration must define:

### **Login Callback URL**

@@name Identity redirects here after user authentication.

### **Logout Callback URL**

Used for federated sign-out and user session cleanup.

Both URLs **must be registered** inside @@name Identity for your specific client application.

## Claims

After authentication, your application receives standard OIDC user claims, such as:

- `sub`  
- `name`  
- `preferred_username`  
- `email`  

Additional custom @@name claims may also be available depending on configuration.

## Required @@name Client Application (Important)

To authenticate against @@name Identity, your application must be **registered as an OIDC client** inside @@name.

The @@name team must configure:

- **Client ID**  
- **Client Secret** (for confidential clients)  
- **Allowed redirect URLs**  
- **Allowed logout URLs**  
- **Allowed scopes**  
- **PKCE / confidential client settings**  
- **Token lifetimes**  

These settings are **specific to your app** and are **not public**.

## How to Obtain a Client ID and Secret

Contact **@@name Support** or your @@name system administrator and request a new OIDC client application.

Provide:

- Your **login callback URL(s)**  
- Your **logout callback URL**  
- Required **scopes** (API access, offline access, etc.)  
- Whether your application is:  
  - a **public client** (SPA, mobile, desktop - *no client secret*), or  
  - a **confidential client** (server-side - *requires secret*)

Once the @@name team creates the client, you will receive the credentials needed for integration.

## Summary

@@name Identity is a standards-based OIDC provider.

To authenticate users with @@name:

1. Register your application as an OIDC client inside @@name  
2. Configure your application with the provided **Client ID** and (if applicable) **Client Secret**  
3. Use `https://id.erp.net/id` as the authority  
4. Register correct **login** and **logout** callback URLs  
5. Use OIDC claims to identify the user  

This allows your application to authenticate users securely through @@name using standard OpenID Connect protocols.
