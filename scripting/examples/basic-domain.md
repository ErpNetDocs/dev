# Basic Domain examples

These examples cover retrieval, creation, updates, and deletion. They are JavaScript fragments for the scripting `Domain` API, not complete managed scripts. Replace sample IDs and values with records from your instance. The [Domain Model reference](https://docs.erp.net/model/entities/index.html) lists available repositories and attributes.

Only an entity-triggered script receives `subject`. In a managed script, read declared inputs from `args` and retrieve the entity yourself. A freeform `ExecuteScript` request has no automatic `subject` or `args`. See [execution context](../concepts/execution-context-and-results.md).

## Retrieve

### Current entity in a business rule

For a business rule on `Crm.Sales.Customers`, `subject` is the customer being processed:

```js
Action.log("Processing customer " + subject.Number);
```

Do not use `subject` in a managed script or a freeform `ExecuteScript` request.

### One entity by ID

`getById` accepts a GUID string and returns `null` if there is no matching entity:

```js
const customerId = "11111111-1111-4111-8111-111111111111";
const customer = Domain.Crm.Sales.CustomersRepository.getById(customerId);
if (customer === null)
    throw new Error("Customer not found.");

Action.log("Customer " + customer.Number);
```

### Several entities by ID

`getByIdList` accepts either a comma-separated string or an array of GUID strings:

```js
const customersFromString = Domain.Crm.Sales.CustomersRepository.getByIdList(
    "11111111-1111-4111-8111-111111111111, 22222222-2222-4222-8222-222222222222");

const customersFromArray = Domain.Crm.Sales.CustomersRepository.getByIdList([
    "11111111-1111-4111-8111-111111111111",
    "22222222-2222-4222-8222-222222222222"
]);
```

Invalid GUIDs are ignored. If none of the supplied values is a valid GUID, the method returns `null`; otherwise it returns the matching entities. Check for `null` before using the collection, and use its .NET `Count` property rather than JavaScript `length`.

```js
if (customersFromArray !== null && customersFromArray.Count > 0)
    Action.log("Found " + customersFromArray.Count + " customers.");
```

### A simple query

Use `query(filter, options)` to search by an attribute. The shorthand `active: true` means equality:

```js
const activeCustomers = Domain.Crm.Sales.CustomersRepository.query(
    { active: true }, { fetch: 20 });
```

For a customer business rule, a query can refer to the triggering customer:

```js
const orders = Domain.Crm.Sales.SalesOrdersRepository.query({
    customer: subject
}, { fetch: 20 });
```

To select orders dated today for the same customer, add an inclusive date range:

```js
const now = new Date();
const start = new Date(now.getFullYear(), now.getMonth(), now.getDate());
const end = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 23, 59, 59, 999);
const todaysOrders = Domain.Crm.Sales.SalesOrdersRepository.query({
    customer: subject,
    documentDate: [
        { greaterThanOrEqual: start },
        { lessThanOrEqual: end }
    ]
}, { fetch: 20 });
```

These examples use `subject` only because the rule is attached to customers. The `Date` values use the script's local time zone; confirm that the range suits the `DocumentDate` values in your instance. For other ranges, reference filters, and more operators, see [Advanced Domain examples](advanced-domain.md).

## Create

### Assign values after creation

`createNew()` creates an entity in the current transaction. Assign its attributes before the transaction commits:

```js
const customer = Domain.Crm.Sales.CustomersRepository.createNew();
customer.Number = "C-NEW";
customer.Active = true;
```

The same values can be supplied in an object initializer:

```js
const customer = Domain.Crm.Sales.CustomersRepository.createNew({
    Number: "C-NEW",
    Active: true
});
```

Actual customer records may require additional fields and valid references. These examples show the scripting syntax, not a guarantee that the record can be committed in every configuration.

### Create related entities

A new customer can refer to entities created in the same transaction:

```js
const customerType = Domain.Crm.Sales.CustomerTypesRepository.createNew();
customerType.Name = "Retail";

const person = Domain.General.Contacts.PersonsRepository.createNew();
person.FirstName = "John";
person.LastName = "Doe";

const customer = Domain.Crm.Sales.CustomersRepository.createNew();
customer.Number = "C-NEW";
customer.CustomerType = customerType;
customer.Party = person;
```

Creating related master data is a business decision. If the type or person already exists, retrieve that entity instead of creating a duplicate. For a nested initializer, see [Object initializers](advanced-domain.md#object-initializers).

### Create several related lines

Assume `salesOrder`, `product`, and `unit` are existing Domain objects available in this script. Create each line and connect it to the order:

```js
for (let i = 0; i < 3; i++) {
    Domain.Crm.Sales.SalesOrderLinesRepository.createNew({
        SalesOrder: salesOrder,
        LineNo: (i + 1) * 10,
        Product: product,
        Quantity: new Domain.Types.Quantity(10, unit)
    });
}
```

The order and product must be valid for this transaction; other required line attributes depend on the entity configuration.

## Update

### Current or retrieved entity

For a business rule on customers, update the current entity directly:

```js
subject.Active = false;
subject.Number = "C-UPDATED";
```

In another execution context, retrieve the entity first:

```js
const customer = Domain.Crm.Sales.CustomersRepository.getById(customerId);
if (customer === null)
    throw new Error("Customer not found.");

customer.Active = false;
customer.Number = "C-UPDATED";
```

### Bounded batch update

Check the selected rows before changing them. This example deactivates at most 20 customers whose `ThruDate` is no later than today:

```js
const expired = Domain.Crm.Sales.CustomersRepository.query({
    thruDate: { lessThanOrEqual: new Date() }
}, { fetch: 20 });

for (const customer of expired)
    customer.Active = false;
```

`fetch` bounds the returned result but does not establish a stable order. If more than 20 records qualify, this is not a complete batch-processing strategy.

## Delete

### One entity

Deletion is subject to permissions, references, business rules, and the transaction result:

```js
const customer = Domain.Crm.Sales.CustomersRepository.getById(customerId);
if (customer !== null)
    customer.Delete();
```

### Child rows

Some child collections, including sales-order lines, expose `DeleteAll()`:

```js
if (salesOrder.Lines.Count > 0)
    salesOrder.Lines.DeleteAll();
```

To remove one line rather than the whole collection, delete the line entity:

```js
const line = salesOrder.Lines[0];
line.Delete();
```

Check that the collection is nonempty and that its entities support deletion before using the last form. Do not assume the child collection exposes `Delete(line)`; its explicitly permitted scripting operation is `DeleteAll()`.
