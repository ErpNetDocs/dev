# API Access Overview

@@name provides several powerful APIs, each designed for a specific integration scenario.

All APIs share the same **authentication**, **authorization**, and **security** foundation - access is always through OAuth 2.0 tokens issued by the Identity Server.

Use this section to choose the right API for your use case and learn how to access it securely.

## Available APIs

| API | Description | Typical Use Case |
|------|--------------|------------------|
| [**Domain API**](domain-api.md) | Business-oriented API built on **OData**, exposing the @@name domain model with validation and relationships. | Integrations, automation, and custom applications that need full business logic support. |
| [**Table API**](table-api.md) | Raw, table-level API built on **OData**, optimized for analytics; **supports streaming** for large datasets. | Power BI and other reporting tools that need fast, read-only data access. |

## Common Features

- All APIs use **HTTPS** and require a valid **OAuth 2.0 access token**.  
- Permissions are enforced via **scopes** (for example, `read`, `update`).  
- Both Domain API and Table API are **built on OData**.
- **Table API** uses **HTTP streaming** to deliver large result sets efficiently.

> [!NOTE]  
> APIs **share the same session pool** on the application server. If you reuse a valid token, calls will attach to the **same active session** when possible; a new session is opened only when needed (for example, after inactivity timeout).

## Choosing the Right API

- Use **Domain API** for most application logic, workflow, and automation tasks.  
- Use **Table API** when performance and bulk data extraction are more important than business rules.  

---

## Learn More

- [**Domain API**](../../domain-api/index.md)  
  Structured, business-layer access with validation and relationships.

- [**Table API**](../../table-api/index.md)  
  High-speed, low-level access for BI and reporting.

- [**Best Practices for Calling APIs Securely**](best-practices.md)  
  Tips for safe authentication, throttling, and data handling.
