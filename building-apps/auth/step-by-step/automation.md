# Automation with Reference Access Tokens

This topic describes how automated and integration scenarios authenticate to @@name APIs using **reference access tokens** (`enrt_...`), without interacting with the Instance ID or performing OAuth flows.

Reference access tokens are **manually issued, long-lived, opaque strings**. intended for machine-driven access, background jobs, and developer automation where interactive sign-in is not practical or required. They are validated directly by @@name.

## Prerequisites

- A reference access token (`enrt_445659C00F83CCA2D427BF113EFB355319346865C0B4FE5A3C76C16271B06AFF`) must be issued in advance
- The token must include the required scopes for the target APIs
- The token must be **valid and not expired**

## Implementation

This section demonstrates how to call @@name instance APIs using a **reference access token**. No OAuth flow is performed.

### 1. Provide the reference token

Store the token securely and load it at runtime:

```text
REFERENCE_TOKEN = enrt_445659C00F83CCA2D427BF113EFB355319346865C0B4FE5A3C76C16271B06AFF
```

### 2. Call an API with the token

> [!NOTE]
> The following example uses the **Domain API** to demonstrate how a reference access token is sent with an API request.

Send the token in **one** of the supported headers.

**Option A: Authorization header**

```http
GET /api/domain/odata/Customers?$top=10 HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer enrt_445659C00F83CCA2D427BF113EFB355319346865C0B4FE5A3C76C16271B06AFF
```

**Option B: API key header**

```http
GET /api/domain/odata/Customers?$top=10 HTTP/1.1
Host: demodb.my.erp.net
X-Api-Key: enrt_445659C00F83CCA2D427BF113EFB355319346865C0B4FE5A3C76C16271B06AFF
```

### Result

If the token is valid and includes the required scopes, the API returns the requested data.

### Notes

- Use only one method per request (either `Authorization` or an API key header).
- Reference tokens are opaque and validated by @@name; they are not exchanged for other tokens.

