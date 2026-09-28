# Advanced Domain examples

These JavaScript fragments extend the [basic Domain examples](basic-domain.md) with object initializers and the full set of currently supported scripting query comparisons. Obtain placeholder objects such as `customerType` and `person` from the relevant [execution context](../concepts/execution-context-and-results.md) or retrieve them by ID before using them. Each `fetch` value is deliberately small; adjust it only after checking the expected result set.

The query filter names refer to Domain attributes or references, not arbitrary JavaScript properties. An unrecognized field name is currently ignored and can broaden a query. Verify names against the [Domain Model reference](https://docs.erp.net/model/entities/index.html), especially before an update or deletion.

## Object initializers

The step-by-step version creates related entities and then assigns them:

```js
const customerType = Domain.Crm.Sales.CustomerTypesRepository.createNew();
customerType.Name = "VIP";

const person = Domain.General.Contacts.PersonsRepository.createNew();
person.FirstName = "John";
person.LastName = "Doe";

const customer = Domain.Crm.Sales.CustomersRepository.createNew();
customer.Number = "C-VIP";
customer.Active = true;
customer.CustomerType = customerType;
customer.Party = person;
```

An initializer can express the same relationship in one statement:

```js
const customer = Domain.Crm.Sales.CustomersRepository.createNew({
    Number: "C-VIP",
    Active: true,
    CustomerType: Domain.Crm.Sales.CustomerTypesRepository.createNew({
        Name: "VIP"
    }),
    Party: Domain.General.Contacts.PersonsRepository.createNew({
        FirstName: "John",
        LastName: "Doe"
    })
});
```

Both forms create the related records. Do not use either to create a new type or person if an existing record should be reused. Additional fields and validation may be required at commit.

## Equality and combined filters

For a single attribute, a scalar means `equals`:

```js
const byNumber = Domain.Crm.Sales.CustomersRepository.query(
    { number: "C-001" }, { fetch: 10 });

const explicit = Domain.Crm.Sales.CustomersRepository.query(
    { number: { equals: "C-001" } }, { fetch: 10 });
```

Both queries express the same equality condition. Shorthand also works for a Domain reference:

```js
const byType = Domain.Crm.Sales.CustomersRepository.query({
    active: true,
    customerType: customerType
}, { fetch: 20 });

const explicitByType = Domain.Crm.Sales.CustomersRepository.query({
    active: { equals: true },
    customerType: { equals: customerType }
}, { fetch: 20 });
```

Conditions on different attributes are combined in the query. `customerType` above must be a retrieved Domain entity, not its display name or GUID string.

## Supported comparison forms

Comparison names are case-insensitive. The scripting filter parser currently supports the following names for suitable attributes:

| Comparison | Meaning | Example condition on a customer attribute |
| --- | --- | --- |
| `equals` | Exact value | `number: { equals: "C-001" }` |
| `in` | One of several values | `number: { in: ["C-001", "C-002"] }` |
| `greaterThanOrEqual` | Lower inclusive bound | `fromDate: { greaterThanOrEqual: new Date(2025, 6, 1) }` |
| `lessThanOrEqual` | Upper inclusive bound | `thruDate: { lessThanOrEqual: new Date(2025, 6, 31) }` |
| `like` | SQL-style pattern | `number: { like: "C-%" }` |
| `contains` | Substring | `number: { contains: "123" }` |
| `startsWith` | Prefix | `number: { startsWith: "C-" }` |
| `endsWith` | Suffix | `number: { endsWith: "-EU" }` |

The comparison must make sense for the attribute's type. Unknown comparison names throw an error; `greaterThan` and `lessThan` without `OrEqual` are not currently supported by this scripting parser.

### A list of values

Use `in` when any of several exact values is acceptable:

```js
const selected = Domain.Crm.Sales.CustomersRepository.query({
    number: { in: ["C-001", "C-002"] }
}, { fetch: 20 });
```

### Date or number range

Supply an array of comparison objects for two conditions on the same attribute:

```js
const startedInEarlyJuly = Domain.Crm.Sales.CustomersRepository.query({
    fromDate: [
        { greaterThanOrEqual: new Date(2025, 6, 1) },
        { lessThanOrEqual: new Date(2025, 6, 10) }
    ]
}, { fetch: 20 });
```

JavaScript date constructor months are zero-based: `6` is July. A scalar date condition is equality, not a range.

### Text patterns

`like` accepts SQL-style `%` wildcards. `contains`, `startsWith`, and `endsWith` express common patterns without wildcards:

```js
const anyPosition = Domain.Crm.Sales.CustomersRepository.query({
    number: { like: "%432%" }
}, { fetch: 20 });

const prefix = Domain.Crm.Sales.CustomersRepository.query({
    number: { startsWith: "C-" }
}, { fetch: 20 });

const suffix = Domain.Crm.Sales.CustomersRepository.query({
    number: { endsWith: "-EU" }
}, { fetch: 20 });

const substring = Domain.Crm.Sales.CustomersRepository.query({
    number: { contains: "VIP" }
}, { fetch: 20 });
```

For `like`, `"C-%"` matches a prefix and `"%432"` matches a suffix. Match behavior can depend on the underlying data source's collation.

## References and custom properties

Query orders related to a retrieved customer:

```js
const customer = Domain.Crm.Sales.CustomersRepository.getById(
    "11111111-1111-4111-8111-111111111111");
if (customer === null)
    throw new Error("Customer not found.");

const orders = Domain.Crm.Sales.SalesOrdersRepository.query({
    customer: { equals: customer }
}, { fetch: 20 });
```

The reference can also be written as `customer: customer`. The value is the Domain entity, not its display text.

A configured custom property can use the `CustomProperty_<Code>` filter name, where supported by the entity and data source:

```js
const matches = Domain.Crm.Sales.CustomersRepository.query({
    CustomProperty_C1: { equals: "target value" }
}, { fetch: 20 });
```

Confirm the custom property's code in your instance. If the name does not resolve to an attribute, the current parser silently ignores that condition.

## Include null values

For an attribute, `includeNulls: true` includes both the specified value and rows where that attribute has no value:

```js
const directForm = Domain.Crm.Sales.CustomersRepository.query({
    number: { equals: "C-001", includeNulls: true }
}, { fetch: 20 });

const arrayForm = Domain.Crm.Sales.CustomersRepository.query({
    number: [
        { equals: "C-001" },
        { includeNulls: true }
    ]
}, { fetch: 20 });
```

Both forms are supported. A reference filter also supports these two forms, with a Domain entity as its value:

```js
const withPartyOrUnassigned = Domain.Crm.Sales.CustomersRepository.query({
    party: { equals: person, includeNulls: true }
}, { fetch: 20 });

const sameReferenceFilter = Domain.Crm.Sales.CustomersRepository.query({
    party: [
        { equals: person },
        { includeNulls: true }
    ]
}, { fetch: 20 });
```

## Combine the forms

This query combines scalar equality, reference equality, an inclusive date range, nullable reference matching, and a result limit. Assume `customerType` and `person` were retrieved earlier:

```js
const customers = Domain.Crm.Sales.CustomersRepository.query({
    active: true,
    customerType: { equals: customerType },
    fromDate: [
        { greaterThanOrEqual: new Date(2025, 6, 1) },
        { lessThanOrEqual: new Date(2025, 6, 31) }
    ],
    party: { equals: person, includeNulls: true }
}, { fetch: 10 });
```

Without `fetch`, the current scripting query path caps results at 1,000. `fetch` is a result-size limit, not paging or ordering. A limit does not imply a stable first page.

## Flexible values

For a date attribute, the query layer accepts either a JavaScript `Date` or an ISO date string:

```js
const asDate = Domain.Crm.Sales.CustomersRepository.query({
    fromDate: { equals: new Date(2025, 6, 1) }
}, { fetch: 10 });

const asString = Domain.Crm.Sales.CustomersRepository.query({
    fromDate: { equals: "2025-07-01" }
}, { fetch: 10 });
```

For a multilingual attribute such as a person's first name, a plain string can be assigned using the current language:

```js
person.FirstName = "John";
```

See [Domain special types](../domain-model/special-types.md) when you need explicit translations rather than the current-language value.
