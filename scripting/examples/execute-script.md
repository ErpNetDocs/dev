# ExecuteScript examples

These are complete requests for the freeform Domain API [`ExecuteScript` action](../../domain-api/operations/execute-script.md). The HTTP body is JavaScript source, not JSON. No `Systems.Core.Script` record, `subject`, `args`, or parameter schema is involved. The access token needs the `exec` scope, and the instance needs the X21 Advanced BPM license.

## Read and log a bounded result

```http
POST /api/domain/odata/ExecuteScript
Content-Type: text/plain
Authorization: Bearer <access-token-with-exec-scope>

const customers = Domain.Crm.Sales.CustomersRepository.query(
    { active: true }, { fetch: 10 });
console.log("Active customers fetched: " + customers.Count);
```

The response contains execution metadata and captured console output. It does not use the managed-script `{ "parameters": ..., "returnValue": ... }` envelope.

## Update and explicitly commit a standalone call

Replace the sample GUID with an existing customer ID. This example changes data, so run it only when that change is intended:

```http
POST /api/domain/odata/ExecuteScript
Content-Type: text/plain
Authorization: Bearer <access-token-with-exec-scope>

const customer = Domain.Crm.Sales.CustomersRepository.getById(
    "11111111-1111-4111-8111-111111111111");
if (customer === null)
    throw new Error("Customer not found.");

customer.Active = false;
Transaction.commit();
console.log("Customer updated.");
```

A standalone call does not implicitly commit. Do **not** call `Transaction.commit()` inside a script joined to an externally managed Domain API transaction; the external transaction owner should decide whether to commit or roll back. See [transaction control](../../domain-api/operations/execute-script.md#external-transaction-control).
