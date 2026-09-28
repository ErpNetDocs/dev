# Query Domain entities

Use a repository's `query(filter, options)` method for bounded searches. A filter object names attributes or references; `fetch` limits the number of returned entities. Without `fetch`, the scripting query path currently caps results at 1,000. Prefer a specific filter and a smaller limit for interactive calls. The sample filter values below are literals; a real script can obtain them from its own [execution context](../execution-context.md).

```js
const customers = Domain.Crm.Sales.CustomersRepository.query(
    { number: { startsWith: "C-" } },
    { fetch: 10 });
```

The query returns Domain entities. For a managed script's result, select plain values rather than returning entity objects; see [Find customers by number prefix](../examples.md#find-customers-by-number-prefix).

## Filter forms

Equality can be written in shorthand or explicitly:

```js
const byNumber = Domain.Crm.Sales.CustomersRepository.query({
    number: "C-001"
}, { fetch: 10 });

const active = Domain.Crm.Sales.CustomersRepository.query({
    active: { equals: true }
}, { fetch: 10 });
```

The scripting filter parser supports these comparison names for suitable attributes: `equals`, `in`, `greaterThanOrEqual`, `lessThanOrEqual`, `like`, `contains`, `startsWith`, and `endsWith`. Comparison names are case-insensitive. For example:

```js
const selected = Domain.Crm.Sales.CustomersRepository.query({
    number: { in: ["C-001", "C-002"] }
}, { fetch: 10 });

const numbered = Domain.Crm.Sales.CustomersRepository.query({
    number: { like: "C-%" }
}, { fetch: 10 });
```

`like` uses SQL-style `%` wildcards. For a date range, supply two conditions for the same attribute:

```js
const customers = Domain.Crm.Sales.CustomersRepository.query({
    fromDate: [
        { greaterThanOrEqual: new Date(2025, 6, 1) },
        { lessThanOrEqual: new Date(2025, 6, 31) }
    ]
}, { fetch: 20 });
```

JavaScript date constructor months are zero-based, so `6` means July. ISO date strings can also be used for suitable date attributes.

## References, nulls, and custom properties

Use an actual Domain object when filtering by a reference:

```js
const customer = Domain.Crm.Sales.CustomersRepository.getById(
    "11111111-1111-4111-8111-111111111111");
if (customer === null)
    throw new Error("Customer not found.");

const orders = Domain.Crm.Sales.SalesOrdersRepository.query({
    customer: { equals: customer }
}, { fetch: 20 });
```

For an attribute, `includeNulls: true` also includes rows where it has no value:

```js
const customers = Domain.Crm.Sales.CustomersRepository.query({
    number: { equals: "C-001", includeNulls: true }
}, { fetch: 20 });
```

The `CustomProperty_<Code>` naming convention can be used for a configured custom property, but support depends on the entity and data source; verify the query in your instance before relying on it.

The scripting query API is a restricted filter language, not arbitrary JavaScript or LINQ. An unrecognized attribute or reference name is currently ignored and can unintentionally broaden a query; an unknown comparison keyword throws an error. Check names against the [Domain Model reference](https://docs.erp.net/model/entities/index.html) and verify the result set. A `fetch` limit bounds the result size, but without a defined ordering it does not select a deterministic first page.

For side-by-side examples of shorthand and explicit equality, all supported comparisons, both null-handling forms, references, custom properties, and combined filters, see [Advanced Domain examples](../examples/advanced-domain.md).
