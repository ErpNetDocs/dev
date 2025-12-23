# Reference Access Tokens (PAT, SAT)

A **reference access token** is a long-lived, manually issued token that provides programmatic access to @@name APIs.  

Unlike short-lived OAuth access tokens, reference tokens are designed for **automation**, **integration**, and **developer access** scenarios where user interaction is not practical.

All reference tokens in @@name begin with the `enrt_` prefix, for example:

```text
enrt_1D41D4694B4F02D3D6A31FFA07E20B73F48248B26C75A0CCCB5F9DBEE41F7960
```

## Overview

Reference tokens are **opaque strings** (not JWTs) that reference an internal record stored and validated by the @@name Identity.

They can be revoked instantly and are traceable by administrators.

Two types of reference tokens exist:

| Token Type | Full Name | Represents | Issued From | Who Can Create | Requires Trusted App | Impersonates | Scopes Required |
|-------------|------------|-------------|--------------|----------------|----------------------|---------------|------------------|
| **PAT** | Personal Access Token | A specific **user** | Profile (User) Site | Any authenticated user (if allowed) | Yes | The user who created it | Yes |
| **SAT** | Service Access Token | A specific **trusted application** (via its **system user**) | Instance Manager Site | Administrators only (if allowed) | Yes | The system user of the trusted app | Yes |

## Trusted Application Policy Requirement

Reference tokens cannot be issued unless explicitly allowed in the **Trusted Application definition**.  

Each trusted application includes the following policy setting:

| **Attribute** | **Purpose** | **Values** |
|----------------|-------------|-------------|
| **AccessTokens** | Controls who may issue reference tokens (PAT/SAT). | `None`, `AuthenticatedUsers`, or `AdministratorsOnly` |

**Meaning:**

- `None` - Reference tokens cannot be issued. (Default and most secure.)  
- `AuthenticatedUsers` - Any signed-in user can create a **PAT** for this app.  
  Only administrators can issue **SATs**, since the Instance Manager site requires administrative access.  
- `AdministratorsOnly` - Only administrators can issue **PATs** or **SATs** for this app.

> [!WARNING]  
> Even if a user has permissions in the instance, they cannot create a PAT or SAT unless the trusted app explicitly allows it through this setting.

## Characteristics

- Always start with **`enrt_`**.  
- Are **issued manually** - never automatically by OAuth flows.  
- Are **bound** to a specific @@name instance.  
- Have a **defined expiration time** (for security and compliance).  
- Are **stored securely** in the @@name Identity and **revocable immediately**.  
- Can **log last used timestamp** for audit and monitoring.  
- Must be **treated as secrets** - never shared or embedded in public code or configuration.

## Usage

Reference tokens can be used in any request that accepts a standard Bearer token or API key header.

### Option 1: Authorization Header

```http
GET /api/domain/odata/Customers HTTP/1.1
Host: testdb.my.erp.net
Authorization: Bearer enrt_1D41D4694B4F02D3D6A31FFA07E20B73F48248B26C75A0CCCB5F9DBEE41F7960
```

### Option 2: API Key Header

```http
GET /api/domain/odata/Customers HTTP/1.1
Host: testdb.my.erp.net
X-Api-Key: enrt_1D41D4694B4F02D3D6A31FFA07E20B73F48248B26C75A0CCCB5F9DBEE41F7960
```

Other supported header names include:

- `Api-Key`
- `X-API-KEY`

> [!NOTE]
> Use only **one method** per request - either `Authorization` or an API key header, not both.

## Personal Access Tokens (PAT)

A **Personal Access Token** represents an individual user.  

It allows scripts, connectors, or integrations to act **on behalf of that user** without performing OAuth sign-in.

**Created from:** the user's **Profile Site**.  

**Requirements:**

- An existing **Trusted Application** association.  
- The target **Trusted Application** must have `AccessTokens` set to `AuthenticatedUsers` or higher.  
- A chosen **expiration time**.  
- One or more **scopes** (for example, `read`, `update`).

**Example use cases:**

- A user's Excel connector that reads @@name data.  
- Automated data exports running as the user.  
- Developer testing or API prototyping.

> [!WARNING]  
> PATs inherit the permissions of the issuing user.  
> Always restrict scopes and expiration to the minimum necessary.

## Service Access Tokens (SAT)

A **Service Access Token** represents an @@name **application identity**, not a user.  

It allows fully automated integrations to operate using the **System User** defined in the associated Trusted Application.

**Created from:** the **Instance Manager Site**.  
**Issued by:** administrators only.  

**Requirements:**

- The target **Trusted Application** must have:
  - `SystemUserAllowed = true`  
  - A valid **System User** assigned  
  - `AccessTokens` set to `AdministratorsOnly`  
- Explicit **scopes** (for example, `read`, `update`)  
- Defined **expiration time**

**Example use cases:**

- Continuous data synchronization or ETL services.  
- Middleware or background jobs that perform scheduled tasks.  
- Automated integration frameworks that run 24/7.

> [!NOTE]  
> SATs use the **system user** of the trusted app.  
> This ensures consistent permissions, stable identity, and centralized audit control.

## Security Guidelines

- Always store reference tokens in **secure vaults** or **encrypted configuration files**.  
- Never commit them to source control or logs.  
- Use short expiration times and rotate tokens regularly.  
- Revoke immediately when an app or user no longer needs access.  
- Use **SATs** for integrations, **PATs** for developer or user-level tools.  
- Monitor **last used timestamps** to detect unused or suspicious tokens.

## Revocation and Auditing

Reference tokens can be revoked at any time from:

- The **Profile Site** (for PATs)  
- The **Instance Manager Site** (for PATs and SATs)

Revocation is immediate - once revoked, the token becomes invalid for all future requests.

@@name also tracks:

- **Creation time**  
- **Creator identity**  
- **Scopes**  
- **Last used timestamp**

This allows administrators to review and audit all reference token activity.

## Learn More

- [**Access Tokens**](access-tokens.md)  
  Learn how @@name uses short-lived OAuth access tokens for standard app flows.

- [**Scopes and Permissions**](scopes.md)  
  Understand how scopes control what data your tokens can access.

- [**Trusted Applications and Access Control**](../how-apps-connect/trusted-apps-access.md)  
  See how trusted apps define system users, modes, and scope policies.

- [**Token Lifetime and Renewal**](token-lifetime.md)  
  Learn how expiration and renewal policies affect security.
