# Security and Best Practices

Strong authentication and authorization practices are essential for keeping @@name integrations secure.  

This page summarizes best practices when working with OAuth 2.0, @@name Identity, and trusted applications.

## Always Use HTTPS

- All authentication and token exchange endpoints require **HTTPS**.  
- Never use `http://` except in local development with valid self-signed certificates.  
- Redirect URIs must also use HTTPS for production deployments.

## Protect Client Secrets

- Only **Confidential** clients can have secrets.  
- Do not include secrets in public apps (SPAs, mobile, or native clients).  
- Store secrets securely on the server using environment variables or secret vaults.  
- Rotate secrets periodically and immediately after exposure.

## Use the Correct Flow

- Use **Authorization Code flow (with PKCE)** for interactive apps.  
- Use **Client Credentials flow** for backend or service integrations.  
- Use **Hybrid flow** only when both front-end and back-end components need separate tokens.  
- Avoid workarounds such as embedding service credentials in front-end apps.

## Apply Least Privilege

- Request only the scopes your app needs (`read`, `update`, etc.).  
- Avoid unnecessary permissions or broad access.  
- Configure Trusted Applications with minimal scopes and access modes.  
- Use separate apps for different integrations when possible.

## Handle Tokens Securely

- Treat all tokens as confidential credentials.  
- Never log, email, or store tokens in plain text.  
- Always send tokens in the `Authorization: Bearer` header.  
- Do not include tokens in URLs or query parameters.  
- Validate token signatures and expiration if implementing custom middleware.

## Refresh and Reference Tokens

- Use **refresh tokens** only in confidential clients.  
- Public clients (SPAs, mobile) must re-authenticate when access tokens expire.  
- Use **Reference Access Tokens (PAT or SAT)** only when long-lived access is required.  
- Always restrict their scopes and lifetime.

## Secure Redirect URIs

- Register all redirect URIs exactly in the Trusted Application definition.  
- Reject unknown redirect URIs to prevent redirection attacks.  
- Use HTTPS and avoid wildcards.  
- Always implement **PKCE** in public clients.

## Validate Trusted Applications Regularly

- Audit all registered Trusted Applications for correctness and minimal access.  
- Disable or delete unused apps (`IsEnabled = false`).  
- Review who can issue reference tokens via the **AccessTokens** policy (`None`, `AuthenticatedUsers`, `AdministratorsOnly`).  
- Confirm each service user is least-privilege and properly licensed.

## Avoid Token Reuse

- Do not share access tokens between users, services, or environments.  
- Each user or service identity should obtain its own token.  
- Never reuse a service token across multiple integrations.

---

## Learn More

- [**OAuth 2.0 Overview**](./how-apps-connect/oauth2-overview.md)  
  Learn how @@name implements standard OAuth flows.

- [**Trusted Applications and Access Control**](./how-apps-connect/trusted-apps-access.md)  
  How trusted app definitions control access and security modes.

- [**Access Tokens**](./tokens/access-tokens.md)  
  Token contents, signatures, and validation.

- [**Reference Access Tokens (PAT, SAT)**](./tokens/reference-access-tokens.md)  
  Managing long-lived automation credentials securely.
