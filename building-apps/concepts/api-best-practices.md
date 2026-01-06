# Best Practices for Calling @@name APIs Securely

Building apps that interact with @@name APIs requires careful attention to **security**, **performance**, and **license usage**.  

The following guidelines apply to both the **Domain API** and **Table API** and help ensure your integrations run safely and efficiently.

## Authentication and Tokens

- Always use **OAuth 2.0** authentication via the **@@name Identity**.  
- Never hardcode tokens, client secrets, or credentials in source code.  
- Store access tokens securely - for example, in encrypted storage or a secure vault.  
- Use **short-lived access tokens** and **refresh tokens** for long-running apps.  
- For automation, use **Service Access Tokens (SATs)** instead of user tokens.  
- Rotate tokens regularly and revoke any unused or compromised credentials.

> [!CAUTION]  
> Do not use Basic Authentication unless absolutely necessary (e.g., legacy BI tools).  
> It is deprecated and significantly less secure.

## HTTPS and Data Protection

- Always use **HTTPS** when calling any @@name API.  
- Never transmit access tokens or sensitive data over unencrypted channels.  
- Validate SSL/TLS certificates to prevent man-in-the-middle attacks.  
- Sanitize user inputs to avoid injection or malformed OData queries.

## API Efficiency

Efficient API design improves performance, reduces server load, and minimizes license usage.

- Always **filter requests** to return only the necessary records.  
- Use `$select` to fetch only the fields you actually need.
- Prefer **bulk queries** instead of fetching one record per call.  
  It is faster and uses fewer sessions and network round-trips.
- **Cache static or rarely changing entities**, such as enterprise companies, user defined types or configurations.  
  Avoid re-requesting them on every operation.
- For complex reports or data aggregations, use the **Table API** instead of the Domain API.
- Handle pagination with `$skip` and `$top` for large datasets, rather than requesting all data at once.

> [!NOTE]  
> Well-structured queries and smart caching dramatically reduce latency and license overhead while improving user experience.

## Session and License Management

- Each active session consumes a **license slot**.  
- Avoid unnecessary session creation by reusing tokens where possible.  
- Close idle sessions by logging out (interactive apps) or letting them expire naturally.  
- For background integrations, use a dedicated **system user** to prevent conflicts.  
- Monitor session counts and license usage in the Instance Manager.

## Error Handling and Resilience

- Handle `401 Unauthorized` by reauthenticating or refreshing the token.  
- Handle `429 Too Many Requests` by applying **exponential backoff** or retry delays.  
- Validate all API responses before processing.  
- Log errors with sufficient context (endpoint, status code, token type).  
- Design idempotent operations - repeatable without side effects.

## Security for BI and Table API Usage

- When using the **Table API**, always restrict data access with minimal scopes and filters.  
- Apply row-level security through @@name permissions whenever possible.  
- Use **service identities** (SATs) for scheduled BI refreshes instead of user credentials.  
- Rotate BI access tokens periodically and audit their usage.

## Development and Testing

- Use **sandbox instances** for development.  
- Separate test and production trusted applications.  
- Never reuse production tokens in non-production environments.  
- Enable verbose logging during testing - but redact tokens from logs.

## Summary

Following these best practices will ensure your @@name integrations are:

- **Secure** – data and credentials are protected.  
- **Efficient** – API usage is optimized for performance and cost.  
- **Compliant** – sessions, licenses, and identities follow @@name rules.  

---

## Learn More

- [**Domain API Reference**](../../domain-api/index.md)  
  Learn how to query and manipulate @@name business data.

- [**Table API Reference**](../../table-api/index.md)  
  Retrieve high-volume analytical data with streaming support.

- [**Tokens Overview**](../../auth/tokens/tokens-overview.md)  
  Understand access tokens, reference tokens, and scopes.

- [**License Slot Usage**](../../auth/sessions/license-slot.md)  
  Learn how API sessions affect license consumption.
