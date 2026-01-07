# Demo API Call (testdb)

This topic is an optional **smoke test** that validates:

**request format → connectivity → JSON response**

It is useful if you want to quickly verify that your tooling (curl, Postman, client code) can reach the API and receive data.

> [!IMPORTANT]
> This demo call **does not** confirm that *your* application is registered, authenticated, or authorized.
>
> To validate the full integration path (**access token → API request → successful response**) using **your own instance**, follow  
> [Make Your First API Call](./first-api-call.md).

## When to Use This

Use the demo instance if you:

- Want to test HTTP request formatting
- Want to confirm outbound connectivity to the service
- Want to inspect a real JSON payload quickly

Do **not** use this demo path for production integrations or real customer data.

## Demo Instance and Reference Token

The public demo instance is:

- **Host:** `testdb.my.erp.net`

You can call it using the company-provided **demo reference token**.

> [!WARNING]
> The demo token is intended only for the public sandbox instance:
>
> - It works only for `testdb.my.erp.net`.
> - It may be rotated or revoked at any time.
> - Do not use it for production integrations or real customer data.
> - Treat it as a secret (do not publish it in client-side code or logs).

## Send the Request

Example request:

```http
GET https://testdb.my.erp.net/api/domain/odata/Crm_Sales_Customers?$top=10
Authorization: Bearer enrt_C99474338E7E587DA64126E26F138E3E0E2D0E984256073427D122B4B39AC766
Accept: application/json
```

## Verify Success

A successful demo call has all of the following:

- HTTP 200 OK
- A JSON response body
- No redirects to a login page

If this succeeds, basic connectivity and request formatting are confirmed.

## What This Does Not Validate

- This demo call does **not** validate:
- Your app registration
- Token issuance from your identity service
- Authorization and permissions for your tenant
- Access to your instance data

## Next Step

To validate a real end-to-end integration using your own instance, continue with  
[Make Your First API Call](./first-api-call.md).
