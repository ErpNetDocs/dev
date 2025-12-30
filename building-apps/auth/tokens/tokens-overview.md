# Overview

In @@name, **tokens** are used to authenticate and authorize applications and users when interacting with the system's APIs.  

They are the foundation of secure API access and the preferred way for external applications to communicate with @@name APIs.

## What Are Tokens

Tokens are cryptographically signed credentials that represent an authenticated user or application.  

Instead of passing around usernames and passwords, tokens allow applications to securely prove their identity and permissions when calling APIs.

@@name follows the **OAuth 2.0** standard for token management, and all tokens are issued by the instance's **@@name Identity**.

## Types of Tokens

1. **Access Tokens**  
   Used by applications to call @@name APIs. They contain authorization data defining who or what the app represents and what it can do.

2. **Refresh Tokens**  
   Used by apps to obtain a new access token without forcing the user to sign in again. Available only to interactive clients.

3. **Reference Access Tokens (PAT, SAT)**  
   Manually issued, long-lived tokens used for integrations, scripts, or automation.  
   - **PAT (Personal Access Token)** – acts on behalf of a specific user.  
   - **SAT (Service Access Token)** – acts as a defined service identity (SystemUser).

## How Tokens Work in @@name

Tokens are issued by the @@name **Identity** after successful authentication.  

The app then includes the token in API requests using the `Authorization: Bearer <token>` header.

Tokens contain:

- **Who** is calling (user or service)
- **What** they can access (scopes)
- **When** the access expires
- **Where** the token was issued

All API calls are validated against these claims before data access is granted.

## Why Use Tokens

- **Security** – avoids exposing user credentials in requests.  
- **Auditing** – all API calls are traceable to a specific app or user identity.  
- **Scalability** – authentication and authorization are decoupled from API logic.  
- **Flexibility** – different tokens for different flows (interactive, service, or hybrid).  

Tokens also allow seamless integration with external systems and automation tools without compromising security or requiring direct user credentials.

## Token Flow in @@name

When an external application connects to @@name, the flow generally involves:

1. **Authentication** – The application authenticates via an OAuth 2.0 flow (Authorization Code, Client Credentials, or Hybrid).  
2. **Token Issuance** – @@name Identity issues the token representing the app or user identity.  
3. **Accessing APIs** – The app uses the token to authorize API requests.  
4. **Token Expiration** – Tokens expire after a defined period.  
5. **Token Renewal** – The app either uses a refresh token or requests a new access token as needed.

---

## Learn More

- [**Access Tokens**](access-tokens.md)  
  Learn about how access tokens are issued and used in API requests.

- [**Reference Access Tokens (PAT, SAT)**](reference-access-tokens.md)  
  Understand how long-lived reference tokens enable service or personal access.

- [**Scopes**](../configuration/scopes.md)  
  Learn how scopes define which @@name resources a token can access.

- [**Token Lifetime and Renewal**](token-lifetime.md)  
  See how tokens expire, and how refresh tokens extend access securely.
