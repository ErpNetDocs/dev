# Choosing the Right API

@@name provides multiple APIs, each intended for a specific integration scenario.

In this step, you select the API that best matches how your app will interact with an @@name instance.

## Available APIs

| API | Description | Typical Use Case |
| ------ | -------------- | ------------------ |
| **Domain API** | Business-oriented API built on **OData**, exposing the @@name domain model with validation and relationships. | Integrations, automation, and custom applications that need full business logic support. |
| **Table API** | Raw, table-level API built on **OData**, optimized for analytics; **supports streaming** for large datasets. | Power BI and other reporting tools that need fast, read-only data access. |

## How to Choose

Use the **Domain API** if your app needs to:

- Work with business objects (documents, items, customers, orders)
- Create, update, or delete data
- Rely on @@name business rules and validations

Use the **Table API** if your app needs to:

- Read large volumes of data efficiently
- Feed BI or reporting tools
- Perform analytics or backups without business logic

If your app primarily **changes data**, choose the Domain API.  

If it primarily **reads data in bulk**, choose the Table API.

## Reference Documentation

Detailed technical documentation for each API is available here:

- [Domain API Reference](../../domain-api/index.md)
- [Table API Reference](../../table-api/index.md)

## Next Step

After selecting the appropriate API, continue with  
[Create a Trusted Application](./create-trusted-app.md).
