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

> [!NOTE]
> If you only want to validate request format and basic connectivity using the public demo instance (`testdb`),
> see [Demo API Call (testdb)](./demo-api-call.md).

## Send the Request

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
