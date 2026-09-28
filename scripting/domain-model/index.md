# Domain Model in scripts

The global `Domain` object gives JavaScript scripts access to ERP.net entity repositories. Use it to retrieve, query, create, update, or delete Domain objects. The available entities and attributes are described in the [Domain Model reference](https://docs.erp.net/model/entities/index.html).

Start with [entity operations](entity-operations.md), then use [queries and filters](queries.md) for bounded searches. [Special types](special-types.md) covers custom properties, multilingual strings, amounts, and quantities.

The repository operations are shared, but the source of an input is not. A user business rule or calculated attribute may have a `subject`; a managed script receives declared values through `args`; freeform `ExecuteScript` supplies its source directly. See [Execution context](../execution-context.md).

[Basic Domain examples](../examples/basic-domain.md) walks through lookup, query, create, update, and delete operations. [Advanced Domain examples](../examples/advanced-domain.md) covers object initializers and filter variations. [Managed-script examples](../examples.md#use-the-domain-model) add complete schemas, requests, and JSON-compatible results; do not return live Domain objects from a managed script.

Domain access follows the current transaction's permissions. Creating or changing an entity does not commit the transaction; the caller decides whether it commits or rolls back.
