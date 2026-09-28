# Transactions and persistence

Domain Model operations run in the Domain transaction associated with the script invocation. Creating, updating, deleting, or writing an attached file does not by itself commit that transaction.

| Feature | Who controls persistence? |
| --- | --- |
| User business rule | The operation that triggered the rule controls its transaction. |
| Calculated attribute | Do not use a value calculation as a commit mechanism. |
| ExecuteScript | A standalone call does not commit implicitly; an external Domain API transaction is ended by its caller. |
| Managed script | The invoking transaction controls whether Domain changes persist. |

## Commit a managed-script call

`Systems.Core.Script/Execute` does not commit Domain changes. For example, to persist a file created by the [XLSX report script](../examples/managed-scripts/files-and-documents.md#create-an-xlsx-report), first start a [Domain API transaction](../../domain-api/data-manipulation/transactions.md):

```http
POST /api/domain/odata/BeginTransaction
Content-Type: application/json
Authorization: Bearer <access-token>

{ "model": "common" }
```

Save the returned transaction ID. Pass it when invoking the script:

```http
POST /api/domain/odata/Systems_Core_Scripts(<script-id>)/Execute
Content-Type: application/json
Authorization: Bearer <access-token>
TransactionId: <transaction-id>

{ "arguments": { "folderId": "22222222-2222-4222-8222-222222222222", "count": 21 } }
```

If execution succeeds, commit the transaction:

```http
POST /api/domain/odata/EndTransaction
Content-Type: application/json
Authorization: Bearer <access-token>
TransactionId: <transaction-id>

{ "commit": true }
```

If execution fails, end the transaction with `{"commit": false}` instead. A returned file ID does not prove that the file has been persisted; the commit can still fail.

For a standalone `ExecuteScript` call, the exposed `Transaction` object can explicitly commit or roll back. If the call joins an externally managed Domain API transaction, do not use those methods inside the script; let the transaction owner end it. See the [operation reference](../../domain-api/operations/execute-script.md#external-transaction-control).

Not every effect is part of the Domain transaction. For example, `Action.log` and `Action.error` write information messages in a separate transaction, and an external HTTP request cannot be rolled back. See [Action](../action/index.md) and [best practices](../best-practices.md).
