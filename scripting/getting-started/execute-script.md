# Your first ExecuteScript call

`ExecuteScript` is a freeform Domain API action. The request supplies JavaScript source directly; no script record or parameter schema is created.

Send an HTTP POST with the source as the plain-text body:

```http
POST /api/domain/odata/ExecuteScript
Content-Type: text/plain
Authorization: Bearer <access-token-with-exec-scope>

const customers = Domain.Crm.Sales.CustomersRepository.query(
    { active: true }, { fetch: 10 });
console.log("Found " + customers.Count + " customers.");
```

The script's `console.log` message appears in the response's `console` field, alongside execution metadata. This action does not return a managed-script `parameters`/`returnValue` envelope: the request contains code, not a stored definition with declared inputs. There is no automatic `subject` or `args` object. A standalone call does not implicitly commit Domain changes. The calling application needs the `exec` scope, and the instance needs the X21 Advanced BPM license.

See the [ExecuteScript operation reference](../../domain-api/operations/execute-script.md) for transaction handling and the complete response. More requests are in [ExecuteScript examples](../examples/execute-script.md).
