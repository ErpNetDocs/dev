# GetRepositoryEvents

## Overview

`GetRepositoryEvents` is an unbound Domain API function that returns the events supported by a specific repository for **user business rule** registration.

Use this function when you need to:

- discover which event types are valid for a repository
- determine whether an event requires an event parameter
- inspect the layer on which the event is available
- retrieve the event-specific attributes exposed by the platform

This is especially useful when building integrations, tools, or UI that create or validate user business rules dynamically.

For general information about Domain API operations, see [Operations (actions and functions)](https://docs.erp.net/dev/domain-api/data-manipulation/operations.html).

For business usage of these events, see [User business rules](https://docs.erp.net/tech/advanced/user-business-rules/index.html).

## Getting started

`GetRepositoryEvents` is a **function**, so it is invoked with an HTTP `GET` request.

### Request syntax

```http
GET ~/GetRepositoryEvents(repositoryName='Crm.Sales.SalesOrders')
```

### Parameters

| Parameter | Type | Required | Description |
|---|---|---:|---|
| `repositoryName` | `string` | Yes | The exact repository name for which the supported business rule events should be returned. |

### Example

#### Request

```http
GET ~/GetRepositoryEvents(repositoryName='Crm.Sales.SalesOrders')
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
| `EventAttributes` | `array` or `null` | Additional attributes exposed by the event. |

### Events with parameters

Some event types require an additional parameter.

For example:

- `STATECHANGING` and `STATECHANGED` require a document state
- `ATTRIBUTECHANGING` and `ATTRIBUTECHANGED` require an attribute name

When `RequiresEventParameter` is `true`, the `EventParameterDescription` property explains the expected value format.

### Event attributes

Some events expose additional contextual attributes through `EventAttributes`.

Each event attribute contains:

| Property | Type | Description |
|---|---|---|
| `Name` | `string` | The attribute name. |
| `Description` | `string` | Description of the attribute. |

For example, state change events may expose attributes such as:

- `$FromState`
- `$ToState`
- `$FromUserStatusId`
- `$ToUserStatusId`

These attributes provide additional context about the event being triggered.

### Repository-specific behavior

The returned events depend on the repository passed in `repositoryName`.

For example, document repositories may return document lifecycle events such as:

- `STATECHANGING`
- `STATECHANGED`
- `VOIDING`

Repositories that support editable attributes may also return attribute-related events such as:

- `ATTRIBUTECHANGING`
- `ATTRIBUTECHANGED`

## See also

- [Operations (actions and functions)](https://docs.erp.net/dev/domain-api/data-manipulation/operations.html)
- [Domain API](https://docs.erp.net/dev/domain-api/index.html)
- [User business rules](https://docs.erp.net/tech/advanced/user-business-rules/index.html)
- [Events](https://docs.erp.net/tech/advanced/user-business-rules/events/index.html)

---

A couple of quick editorial notes:

- I used **Occurs** instead of **Occurrs** in the descriptive text.
- I kept the page focused on API usage and linked to the technical docs for the business-rule side, so it does not duplicate broader user-business-rule documentation.

If you want, I can next turn this into:
1. a **shorter reference-style page**, or  
2. a **full DocFX-ready `.md` file with front matter and suggested location in the dev TOC**.