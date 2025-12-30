# Building Apps with the Domain API

> [!NOTE]  
> Full technical documentation is available here:  
> [**Domain API Reference**](../../../domain-api/index.md)

The **Domain API** is the primary interface for integrating with @@name business logic.  

It provides structured, object-oriented access to data and operations through the **OData protocol**, ensuring that all business rules, standard validation, permissions, and workflow rules are applied.

This makes it the ideal API for **applications, integrations, and automation** that need to interact with @@name in the same way as the native client does.

## When to Use the Domain API

Use the Domain API when your app needs to:

- Work with **business objects** such as documents, items, customers, or orders.  
- Automatically follow **@@name business rules**, validations, and relationships.  
- **Create**, **update**, or **delete** entities securely with full data integrity.  
- Build integrations, web apps, or backend services that extend @@name behavior.  

It is **not recommended** for large-scale analytics or raw data extraction - use the [Table API](table-api.md) instead.

## Technical Summary

- **Protocol:** OData (v4 compliant)  
- **Base URL:**  
  `https://<instance>.my.erp.net/api/domain/`  
- **Authentication:** OAuth 2.0 access token (Bearer)  
- **Formats:** JSON by default  
- **Respects:** Business logic, security roles, and license sessions.  

> [!NOTE]  
> The Domain API automatically applies ERP.net's internal rules and triggers.  
> This ensures data consistency but may make it slower than raw table access - ideal for transactional integrity.

---

## Learn More

- [**Domain API Reference**](../../../domain-api/index.md)  
  Complete documentation with samples and query patterns.

- [**Table API**](table-api.md)  
  For analytics and high-volume data extraction.

- [**Best Practices for Calling APIs Securely**](../api-best-practices.md)  
  Protect tokens, optimize sessions, and handle expiration correctly.
