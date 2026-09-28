# Entity operations

Use a repository on the global `Domain` object to access an entity. Repository names follow the [Domain Model entity reference](https://docs.erp.net/model/entities/index.html). The examples use `Crm.Sales.CustomersRepository`; replace it with the repository for your entity. These are API fragments: `customerId`, `number`, and `personId` below stand for values obtained from the script's own [execution context](../concepts/execution-context-and-results.md), not universal globals.

## Retrieve by ID

```js
const customer = Domain.Crm.Sales.CustomersRepository.getById(customerId);
if (customer === null)
    throw new Error("Customer not found.");

const customerNumber = customer.Number;
```

`getById` accepts a GUID string and returns `null` when the entity cannot be found. A managed script can declare `customerId` as an input; its [complete example](../examples/managed-scripts/domain-data.md#read-a-customer-by-id) includes a schema and API request.

To retrieve several entities, `getByIdList` accepts a comma-separated string or an array of GUID strings:

```js
const customers = Domain.Crm.Sales.CustomersRepository.getByIdList([
    "11111111-1111-4111-8111-111111111111",
    "22222222-2222-4222-8222-222222222222"
]);
```

Invalid GUID strings are ignored. Check the returned collection before using its members.

## Create

`createNew()` creates an entity in the current transaction. You can set its attributes afterward or pass an initializer:

```js
const customer = Domain.Crm.Sales.CustomersRepository.createNew({
    Number: number,
    Active: true
});
```

References can be assigned using other Domain objects. For example, a customer can be connected to an existing party:

```js
const customer = Domain.Crm.Sales.CustomersRepository.createNew();
customer.Number = number;
const person = Domain.General.Contacts.PersonsRepository.getById(personId);
if (person === null)
    throw new Error("Person not found.");
customer.Party = person;
```

These snippets demonstrate the scripting API, not all fields needed to save a customer in every configuration. Set all required fields and valid references before the outer transaction commits. Avoid creating related master data merely to satisfy an example.

## Update

An entity retrieved through a repository can be changed directly:

```js
const customer = Domain.Crm.Sales.CustomersRepository.getById(customerId);
if (customer === null)
    throw new Error("Customer not found.");

customer.Active = false;
```

In a user business rule, the triggering entity can instead be available as `subject`. That is specific to its [execution context](../concepts/execution-context-and-results.md). See [Change a customer's active state](../examples/managed-scripts/domain-data.md#change-a-customers-active-state) for a complete managed-script example.

## Delete

Call `Delete()` on an entity when deletion is the intended business operation:

```js
const customer = Domain.Crm.Sales.CustomersRepository.getById(customerId);
if (customer !== null)
    customer.Delete();
```

Deletion remains subject to Domain permissions, references, and validation. Child collections can have their own operations, such as `DeleteAll()`; check the specific entity model before using them. Creation, update, and deletion take effect only if the calling transaction commits.

For a fuller progression including both `getByIdList` forms, related records, bounded batch updates, and child-row deletion, see [Basic Domain examples](../examples/basic-domain.md).
