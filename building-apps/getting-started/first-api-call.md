# Make Your First API Call

This step validates the full integration path:

**access token → API request → successful response**

If this step succeeds, your app is correctly registered, authenticated, and authorized.

## Choose a Validation Endpoint

Use the API you selected earlier and pick an endpoint that is:

- Read-only
- Simple
- Known to return data

If you have not selected an API yet, return to  
[Choosing the Right API](./choose-right-api.md).

For the first call, avoid complex queries or write operations.

## Send the Request

You can validate your first API call in one of two ways:

- **Option A (recommended): Use your own access token** (from the previous step)
- **Option B: Use the public demo instance (`testdb`) and a demo reference token** (for a quick smoke test)

### Option A: Use your own access token

Use the access token obtained in the previous step and include it as a **Bearer token**.

A minimal request looks like this:

```http
GET {api-endpoint}
Authorization: Bearer {access_token}
Accept: application/json
```

Where:

- `{access_token}` is the token issued by the instance identity service
- `{api-endpoint}` belongs to the selected API surface (Domain API or Table API)

### Option B: Use the public demo instance (testdb) with a demo reference token

If you only want to validate the request format and basic connectivity, you can call the public demo instance using the company-provided reference token.

> [!WARNING]
> The demo token is intended only for the public sandbox instance:
>
> - It works only for `testdb.my.erp.net`.
> - It may be rotated or revoked at any time.
> - Do not use it for production integrations or real customer data.
> - Treat it as a secret (do not publish it in client-side code or logs).

Example request:

```http
GET https://testdb.my.erp.net/api/domain/odata/Crm_Sales_Customers?$top=10
Authorization: Bearer enrt_C99474338E7E587DA64126E26F138E3E0E2D0E984256073427D122B4B39AC766
Accept: application/json
```

## Verify Success

A successful first call has all of the following:

- HTTP 200 OK
- A JSON response body
- No redirects to a login page

At this point, end-to-end access is confirmed.

> [!WARNING]
> Do not log access tokens or embed secrets in client-side code.  
> See [Security Best Practices](../../auth/security-best-practices.md).  

## Next Step

Once your first API call succeeds, continue with  
[Next Steps](./next-steps.md).
