# Configuration

The product feature owns its script definition and determines how it is configured. There is no common script record shared by business rules, calculated attributes, and managed scripts.

- [User business rule scripts](user-business-rules.md) are configured on a rule together with its repository, event, and activation settings.
- [Calculated attribute scripts](calculated-attributes.md) are configured on the attribute whose value they compute.
- [Managed scripts](managed-scripts.md) are configured as `Systems.Core.Script` records. Their [parameter schema](parameters.md) and [execution settings](execution-settings.md) belong to that record.

Freeform [ExecuteScript](../../domain-api/operations/execute-script.md) has no stored script configuration: source code is supplied with each request.
