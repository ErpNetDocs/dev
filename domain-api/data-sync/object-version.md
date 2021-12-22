# Object version

Object version is a system and API supported way for applications to track changes in @@name entities.

## Change tracking

The changes are aggregated in the [Entity aggregates](xref:aggregates).
This means, that, if for example, an external app updates a single Sales Order Line, the update creates a new version for the whole [Sales Order](xref:Crm.Sales.SalesOrders) aggregate.

Object version reflects this change by increasing its counter with 1.

>> [!note]
>> Applications should not depend on strict monotonically increasing values of Object version.
>> In some scenarios, it might skip values.
>> However, it is guaranteed, that upon update, the value is changed.

## Setup

In order for the Object version system to start working, the aggregate (e.g, its root entity) must be setup to [Track Changes](xref:track-changes) with at least level 1.

## Usage

Object version is available through the Domain API for each entity type.

In order for the API to return the object version, it must be explicitly selected in the API call.

For example:

```
Crm_Sales_SalesOrders(a727114c-3b36-e311-81cb-00155d001f00)?$select=Lines,ObjectVersion&$expand=Lines($select=ObjectVersion,default)
```

>> [!note]
>> Retrieving the object version is resource intensive operation.
>> Perform it only when strictly necessary and only for the entities, for which it is required.