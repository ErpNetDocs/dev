# Choosing the right API

@@name provides several powerful APIs, each designed for a specific integration scenario.

All APIs share the same **authentication**, **authorization**, and **security** foundation - access is always through OAuth 2.0 tokens issued by the @@name Identity.

Use this section to choose the right API for your use case and learn how to access it securely.

## Available APIs

| API | Description | Typical Use Case |
|------|--------------|------------------|
| [**Domain API**](#domain-api) | Business-oriented API built on **OData**, exposing the @@name domain model with validation and relationships. | Integrations, automation, and custom applications that need full business logic support. |
| [**Table API**](#table-api) | Raw, table-level API built on **OData**, optimized for analytics; **supports streaming** for large datasets. | Power BI and other reporting tools that need fast, read-only data access. |

## Common Features

- All APIs use **HTTPS** and require a valid **OAuth 2.0 access token**.  
- Permissions are enforced via **scopes** (for example, `read`, `update`).  
- Both Domain API and Table API are **built on OData**.
- **Table API** uses **HTTP streaming** to deliver large result sets efficiently.

> [!NOTE]  
> APIs **share the same session pool** on the application server. If you reuse a valid token, calls will attach to the **same active session** when possible; a new session is opened only when needed (for example, after inactivity timeout).

## Domain API

> [!NOTE]  
> Full technical documentation is available here:  
> [**Domain API Reference**](../../domain-api/index.md)

The **Domain API** is the primary interface for integrating with @@name business logic.  

It provides structured, object-oriented access to data and operations through the **OData protocol**, ensuring that all business rules, standard validation, permissions, and workflow rules are applied.

This makes it the ideal API for **applications, integrations, and automation** that need to interact with @@name in the same way as the native client does.

### When to Use the Domain API

Use the Domain API when your app needs to:

- Work with **business objects** such as documents, items, customers, or orders.  
- Automatically follow **@@name business rules**, validations, and relationships.  
- **Create**, **update**, or **delete** entities securely with full data integrity.  
- Build integrations, web apps, or backend services that extend @@name behavior.  

It is **not recommended** for large-scale analytics or raw data extraction - use the [Table API](#table-api) instead.

### Technical Summary

- **Protocol:** OData (v4 compliant)  
- **Base URL:**  
  `https://<instance>.my.erp.net/api/domain/`  
- **Authentication:** OAuth 2.0 access token (Bearer)  
- **Formats:** JSON by default  
- **Respects:** Business logic, security roles, and license sessions.  

> [!NOTE]  
> The Domain API automatically applies ERP.net's internal rules and triggers.  
> This ensures data consistency but may make it slower than raw table access - ideal for transactional integrity.

## Table API

> [!NOTE]  
> Full technical documentation is available here:  
> [**Table API Reference**](../../table-api/index.md)

The **Table API** provides fast, table-level access to @@name data, optimized for **analytics**, **data export**, and **business intelligence**.

Unlike the Domain API, the Table API exposes raw tables rather than business objects, making it ideal for **read-only**, **high-performance** data extraction - especially when feeding BI tools like Power BI.

### When to Use the Table API

Use the Table API when your app or integration needs to:

- Extract large volumes of **raw data** for reporting or analysis.  
- Access data at the **table** or **column** level, without @@name business logic.  
- Integrate with **BI platforms** or data warehouses.  
- Stream records efficiently for incremental refreshes.

Avoid using it for transactional operations or business logic - those belong in the [Domain API](#domain-api).

### Technical Summary

- **Protocol:** OData (v4 compliant)  
- **Base URL:**  
  `https://<instance>.my.erp.net/api/table/`  
- **Authentication:** OAuth 2.0 access token (Bearer)  
- **Formats:** JSON or OData-compliant response formats.  
- **Streaming:** Built-in **HTTP streaming** for handling large datasets efficiently.  
- **Scope Recommendation:** `read`  

> [!NOTE]  
> The Table API does not apply business rules or validations.  
> It's purely data-level access - faster, but requires careful handling of relationships and security.

---

## Learn More

- [**Domain API**](../../domain-api/index.md)  
  Structured, business-layer access with validation and relationships.

- [**Table API**](../../table-api/index.md)  
  High-speed, low-level access for BI and reporting.

- [**Best Practices for Calling APIs Securely**](../concepts/api-best-practices.md)  
  Tips for safe authentication, throttling, and data handling.
