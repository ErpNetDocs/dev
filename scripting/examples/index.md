# Examples

Start with the [Getting Started](../getting-started/index.md) walkthrough for the feature you use. The pages here serve two different purposes: **code fragments** demonstrate an operation you can incorporate into a script; **complete examples** include the feature-specific definition or HTTP request needed to run one. A fragment that mentions `subject` belongs to an entity-triggered context; one that mentions `args` belongs to a managed script.

- [Shared patterns](shared-patterns.md) covers special Domain types, `Action`, and files. The [basic Domain examples](basic-domain.md) walk through retrieval, creation, updates, and deletion; the [advanced Domain examples](advanced-domain.md) show object initializers and the supported filter forms in detail.
- [User business rules](user-business-rules.md) contains complete event-driven scripts using `subject`.
- [Calculated attributes](calculated-attributes.md) contains calculations that return an attribute value.
- [ExecuteScript](execute-script.md) contains complete freeform Domain API requests.
- Managed scripts: [inputs and results](managed-scripts/inputs-and-results.md), [Domain data](managed-scripts/domain-data.md), and [files and documents](managed-scripts/files-and-documents.md). These include `ScriptText`, parameter schemas, requests, and responses.

For example, use a [basic Domain fragment](basic-domain.md#one-entity-by-id) to learn `getById`, then use the [managed-script version](managed-scripts/domain-data.md#read-a-customer-by-id) to see where its ID comes from, how the return value is declared, and what the API caller receives. Do not copy a script body from one feature into another without checking its [execution context](../concepts/execution-context-and-results.md) and result contract.
