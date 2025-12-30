# Building BI and Analytics Integrations with the Table API

> [!NOTE]  
> Full technical documentation is available here:  
> [**Table API Reference**](../../../table-api/index.md)

The **Table API** provides fast, table-level access to @@name data, optimized for **analytics**, **data export**, and **business intelligence**.

Unlike the Domain API, the Table API exposes raw tables rather than business objects, making it ideal for **read-only**, **high-performance** data extraction - especially when feeding BI tools like Power BI.

## When to Use the Table API

Use the Table API when your app or integration needs to:

- Extract large volumes of **raw data** for reporting or analysis.  
- Access data at the **table** or **column** level, without @@name business logic.  
- Integrate with **BI platforms** or data warehouses.  
- Stream records efficiently for incremental refreshes.

Avoid using it for transactional operations or business logic - those belong in the [Domain API](domain-api.md).

## Technical Summary

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

- [**Table API Reference**](../../../table-api/index.md)  
  Full details on querying tables, filtering, and building BI models.

- [**Domain API**](domain-api.md)  
  For transactional or rule-based operations.  

- [**Best Practices for Calling APIs Securely**](../api-best-practices.md)  
  Guidelines for token management, HTTPS, and license usage.
