# GetRepositoryEvents

## Overview

`GetRepositoryEvents` is an unbound Domain API function that returns the events supported by a specific repository for **user business rule** registration.

Use this function when you need to:

- discover which event types are valid for a repository
- determine whether an event requires an event parameter
- inspect the layer on which the event is available
- retrieve the event-specific attributes exposed by the platform

This is especially useful when building integrations, tools, UI, or documentation related to user business rules.

For general information about Domain API operations, see [Operations (actions and functions)](https://docs.erp.net/dev/domain-api/data-manipulation/operations.html).

For business usage of these events, see [User business rules](https://docs.erp.net/tech/advanced/user-business-rules/index.html).

## Getting started

`GetRepositoryEvents` is a **function**, so it is invoked with an HTTP `GET` request.

### Request syntax

```http
GET https://testdb.my.erp.net/api/domain/odata/GetRepositoryEvents(repositoryName='Crm.Sales.SalesOrders')
```

### Parameters

| Parameter | Type | Required | Description |
|---|---|---:|---|
| `repositoryName` | `string` | Yes | The exact repository name for which the supported business rule events should be returned. |

### Example

#### Request

```http
GET https://testdb.my.erp.net/api/domain/odata/GetRepositoryEvents(repositoryName='Crm.Sales.SalesOrders')
```

#### Response

```json
{
  "repositoryName": "Crm.Sales.SalesOrders",
  "events": [
    {
      "EventType": "CREATENEW",
      "Description": "Occurs when an editable domain object is created. Used to fill custom default values.",
      "Layer": "Common",
      "RequiresEventParameter": false,
      "EventParameterDescription": null,
      "EventAttributes": null
    },
    {
      "EventType": "STATECHANGING",
      "Description": "Occurs before the document state is changed. The event parameter is the new document state.",
      "Layer": "BackEnd",
      "RequiresEventParameter": true,
      "EventParameterDescription": "The new document state. Should be a document state number or name. Valid state numbers are from 10 to 50. Valid state names - 'PLANNED', 'FIRM PLANNED', 'RELEASED', 'COMPLETED' and 'CLOSED'.",
      "EventAttributes": [
        {
          "Name": "$FromState",
          "Description": "The state of the document before the change."
        },
        {
          "Name": "$ToState",
          "Description": "The new state of the document. This is a parameter of the Document.ChangeState method."
        },
        {
          "Name": "$FromUserStatusId",
          "Description": "The user status of the document before the change."
        },
        {
          "Name": "$ToUserStatusId",
          "Description": "The new user status of the document. This is a parameter of the Document.ChangeState method."
        }
      ]
    }
  ]
}
```

## Concepts

### Returned result

The function returns an object with the following properties:

| Property | Type | Description |
|---|---|---|
| `repositoryName` | `string` | The repository name passed in the request. |
| `events` | `array` | The list of supported events for the repository. |

Each item in the `events` collection contains:

| Property | Type | Description |
|---|---|---|
| `EventType` | `string` | The event code used when registering a user business rule. |
| `Description` | `string` | Human-readable explanation of when the event occurs. |
| `Layer` | `string` | The layer where the event is available, for example `Common`, `BackEnd`, or `FrontEnd`. |
| `RequiresEventParameter` | `boolean` | Indicates whether the event requires a parameter when used in a user business rule. |
| `EventParameterDescription` | `string` or `null` | Description of the required event parameter, when applicable. |
| `EventAttributes` | `array` or `null` | Additional runtime attributes exposed by the event. |

### Events with parameters

Some event types require an additional parameter.

For example:

- `STATECHANGING` and `STATECHANGED` require a document state
- `ATTRIBUTECHANGING` and `ATTRIBUTECHANGED` require an attribute name

When `RequiresEventParameter` is `true`, the `EventParameterDescription` property explains the expected value format.

This makes `GetRepositoryEvents` useful as metadata for tools and documentation that generate or validate user business rule definitions.

### Event attributes

Some events expose additional contextual attributes through `EventAttributes`.

These are **runtime-only system attributes** that are available while the event handler is being executed. They are not regular persisted repository fields and they are not part of the repository schema returned by `$metadata`.

Each event attribute contains:

| Property | Type | Description |
|---|---|---|
| `Name` | `string` | The attribute name that can be used in the event handler logic. |
| `Description` | `string` | Description of the runtime attribute. |

`EventAttributes` are useful when the event handler logic needs information that is not represented directly by the current persisted repository state.

Typical uses include:

- rule **conditions**, by setting `AttributeName` to the event attribute name
- rule logic that must inspect the target state or target user status of a transition
- rule logic that must validate a **pending new value** before it is applied

For example, the `STATECHANGING` event for `Crm.Sales.SalesOrders` may expose:

- `$FromState`
- `$ToState`
- `$FromUserStatusId`
- `$ToUserStatusId`

These attributes allow the rule logic to distinguish not only that a state change is happening, but also:

- from which system state the document is moving
- to which system state it is moving
- which user status is being left
- which user status is being entered

For attribute-related events, event attributes can expose the pending value being validated. For example, `ATTRIBUTECHANGING` can expose `$NewValue`.

Important notes:

- `EventAttributes` are **event-specific**
- `EventAttributes` are often also **repository-specific**
- if `EventAttributes` is `null`, the event does not expose additional runtime attributes
- the names returned by `GetRepositoryEvents` should be treated as the source of truth for the event handler context

### Using `EventAttributes` in user business rules

`GetRepositoryEvents` is primarily a metadata and documentation operation.

Its purpose is to describe:

- which events a repository supports
- whether an event requires a parameter
- which runtime attributes are available in the event handler context

This metadata is useful when designing integrations, generators, examples, or reusable templates for [Systems.Bpm.UserBusinessRules](https://docs.erp.net/model/entities/Systems.Bpm.UserBusinessRules.html).

In practice, once the supported events and event attributes for a repository are known, they can be used directly when authoring business rules. There is no need to call `GetRepositoryEvents` before every individual rule creation.

### Repository-specific behavior

The returned events depend on the repository passed in `repositoryName`.

For example, document repositories may return document lifecycle events such as:

- `STATECHANGING`
- `STATECHANGED`
- `VOIDING`

Repositories that support editable attributes may also return attribute-related events such as:

- `ATTRIBUTECHANGING`
- `ATTRIBUTECHANGED`

Document header repositories and document line repositories do not necessarily support the same events.

For example:

- `Crm.Sales.SalesOrders` supports document state events because it is a document header repository
- `Crm.Sales.SalesOrderLines` can support attribute-related events such as `ATTRIBUTECHANGING`, but it does not represent the document state itself

Because of this, integrations and documentation should inspect `GetRepositoryEvents` for the exact target repository instead of assuming that related repositories support the same event set.

## Examples

The examples below show how the metadata returned by `GetRepositoryEvents` can be applied when creating user business rules through Domain API.

### Example: create a `STATECHANGING` validation rule for sales orders

The example below creates a [Systems.Bpm.UserBusinessRules](https://docs.erp.net/model/entities/Systems.Bpm.UserBusinessRules.html) record for `Crm.Sales.SalesOrders`.

Business scenario:

- a sales order is being moved to a specific **user status**
- for that target user status, a custom property named `@ExportReason` must have a value
- if the custom property has no value, the state change is cancelled with an error

This is a realistic example for export-controlled or compliance-sensitive order processing.

```http
POST https://testdb.my.erp.net/api/domain/odata/Systems_Bpm_UserBusinessRules
Content-Type: application/json
```

```json
{
  "Code": "SO001",
  "Name": {
    "EN": "Sales order target user status requires export reason"
  },
  "RepositoryName": "Crm.Sales.SalesOrders",
  "Layer": "BackEnd",
  "IsActive": true,
  "Events": [
    {
      "EventType": "STATECHANGING",
      "EventParameter": "RELEASED",
      "ExecutionPriority": "Normal",
      "Layer": "BackEnd"
    }
  ],
  "Conditions": [
    {
      "ConditionNo": 10,
      "AttributeName": "$ToUserStatusId",
      "ComparisonType": "Equals",
      "Value": "afcade19-c6f7-45c3-b4f5-2bab1546a168"
    },
    {
      "ConditionNo": 20,
      "AttributeName": "@ExportReason",
      "ComparisonType": "IsNull"
    }
  ],
  "Actions": [
    {
      "ActionNo": 10,
      "ActionType": "FAIL",
      "Parameter1Type": "Constant",
      "Parameter1Value": "The sales order cannot be released to this user status because custom property @ExportReason has no value."
    }
  ]
}
```

This single request creates:

- one root record in [Systems.Bpm.UserBusinessRules](https://docs.erp.net/model/entities/Systems.Bpm.UserBusinessRules.html)
- one child record in `Systems.Bpm.UserBusinessRuleEvents`
- two child records in `Systems.Bpm.UserBusinessRuleConditions`
- one child record in `Systems.Bpm.UserBusinessRuleActions`

Notes:

- Replace `afcade19-c6f7-45c3-b4f5-2bab1546a168` with the actual target user status identifier.
- Replace `@ExportReason` with the actual custom property code used in the target ERP.net instance.
- The rule is scoped both by the `STATECHANGING` event and by the `$ToUserStatusId` event attribute.

### Example: create an `ATTRIBUTECHANGING` validation rule for sales order lines

The example below creates a [Systems.Bpm.UserBusinessRules](https://docs.erp.net/model/entities/Systems.Bpm.UserBusinessRules.html) record for `Crm.Sales.SalesOrderLines`.

Business scenario:

- a user edits `LineCustomDiscountPercent`
- a manual discount above `25%` is not allowed
- the invalid value must be rejected before the change is applied

This example uses the `$NewValue` event attribute.

```http
POST https://testdb.my.erp.net/api/domain/odata/Systems_Bpm_UserBusinessRules
Content-Type: application/json
```

```json
{
  "Code": "SO002",
  "Name": {
    "EN": "Prevent manual line discount above 25 percent"
  },
  "RepositoryName": "Crm.Sales.SalesOrderLines",
  "Layer": "FrontEnd",
  "IsActive": true,
  "Events": [
    {
      "EventType": "ATTRIBUTECHANGING",
      "EventParameter": "LineCustomDiscountPercent",
      "ExecutionPriority": "Normal",
      "Layer": "FrontEnd"
    }
  ],
  "Conditions": [
    {
      "ConditionNo": 10,
      "AttributeName": "$NewValue",
      "ComparisonType": "Greater_Than",
      "Value": "0.25"
    }
  ],
  "Actions": [
    {
      "ActionNo": 10,
      "ActionType": "FAIL",
      "Parameter1Type": "Constant",
      "Parameter1Value": "Manual line discount above 25% is not allowed."
    }
  ]
}
```

This example is important because the repository attribute may still contain the **old** value when `ATTRIBUTECHANGING` is fired. The `$NewValue` event attribute provides access to the pending value that is being validated.

## See also

- [Operations (actions and functions)](https://docs.erp.net/dev/domain-api/data-manipulation/operations.html)
- [Domain API](https://docs.erp.net/dev/domain-api/index.html)
- [User business rules](https://docs.erp.net/tech/advanced/user-business-rules/index.html)
- [Events](https://docs.erp.net/tech/advanced/user-business-rules/events/index.html)
- [Systems.Bpm.UserBusinessRules](https://docs.erp.net/model/entities/Systems.Bpm.UserBusinessRules.html)
