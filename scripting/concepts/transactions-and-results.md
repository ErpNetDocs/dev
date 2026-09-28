# Transactions and results

Domain Model operations run in the Domain transaction associated with the script invocation. Creating, updating, deleting, or writing an attached file does not by itself commit that transaction.

| Feature | Script result | Who controls persistence? |
| --- | --- | --- |
| User business rule | The rule may change data or cancel the operation. | The operation that triggered the rule controls its transaction. |
| Calculated attribute | The script's `return` value becomes the calculated value; `null` or no value produces `null`. | Do not use a value calculation as a commit mechanism. |
| ExecuteScript | The API returns execution metadata and console output. | A standalone call does not commit implicitly; an external Domain API transaction is ended by its caller. |
| Managed script | Declared output parameters and `returnValue`. | The invoking transaction controls whether Domain changes persist. |

For a standalone `ExecuteScript` call, the exposed `Transaction` object can explicitly commit or roll back. If the call joins an externally managed Domain API transaction, do not use those methods inside the script; let the transaction owner end it. See the [operation reference](../../domain-api/operations/execute-script.md#external-transaction-control).

Not every effect is part of the Domain transaction. For example, `Action.log` and `Action.error` write information messages in a separate transaction, and an external HTTP request cannot be rolled back. See [Action](../action/index.md) and [best practices](../best-practices.md).
