# Web Client registered panels

## Overview

Web Client forms are composed of main panels and side panels. A panel is a named area that displays a list, a record, a related collection, or a supporting feature. The available panels depend on the form, repository, view mode, permissions, and enabled features.

This topic helps external developers and layout managers identify panel names for Layout API requests. Captions are localized; panel names are the stable identifiers. Use the names returned by `get-layout-schema`.

## Main and side panels

The form schema exposes the panels available for the current request:

- `Form.VisiblePanels` contains main-panel names.
- `Form.VisibleSidePanel` contains side-panel names.

The schema is authoritative for the selected repository, category, and view mode. A panel can be available in the schema even when it is not currently open in the user interface.

## Navigator form panels

Navigator forms display a repository list and navigation panels for filters, child collections, and hierarchies. Typical panel types and names are listed below. A repository can expose only a subset of these names.

### Repository list panels

The standard list/grid panel is:

```text
NavigatorPanel
```

Other list-oriented panels include:

```text
PivotPanel
KanbanPanel
```

Availability depends on the selected view mode and repository capabilities.

### Filter side panels

The standard filter panel is:

```text
Filter
```

It is a side panel used to edit the repository filter.

### Hierarchy panels

Hierarchy panels use the hierarchy reference name:

```text
Hierarchy.Parent
Hierarchy.Category
```

Only applicable hierarchy references are registered for a repository.

### Child-collection side panels

Aggregate child collections use the `CC.` prefix and the model collection name:

```text
CC.Lines
CC.DocumentPayments
```

The collection name is not the localized caption.

## Entity form panels

Entity forms commonly contain a record header, child collections, related records, and system features. Their available panel list can be much larger than the set initially visible to the user.

### Entity header panels

The main object panel uses the repository-qualified `Header` name:

```text
Crm.Sales.SalesOrders.Header
General.Products.Products.Header
```

### Child-collection panels

Child collections use the owner repository followed by the collection name:

```text
Crm.Sales.SalesOrders.Lines
Crm.Sales.SalesOrders.DocumentPayments
Projects.Agile.Cases.Tasks
```

Nested collections extend the path:

```text
Projects.Agile.Cases.Tasks.Comments
```

### Referenced-object panels

Some entity forms include panels for important references, such as a customer, company, or payment term. The form creates a panel for the referenced object itself and can also expose that object's child collections.

The reference path determines which object is displayed, but it is not appended to the panel name. The name is formed from the referenced repository:

```text
<referenced-repository>.Header
<referenced-repository>.<child-collection>
```

For example, an important `Customer` reference on a sales order can produce names such as:

```text
Crm.Sales.Customers.Header
General.Contacts.Parties.Header
Logistics.Inventory.Products.Header
```

If the referenced repository has a child collection, its panel uses that repository as the prefix, for example:

```text
Crm.Sales.Customers.Addresses
General.Contacts.Parties.ContactMechanisms
```

The exact set of important references is defined by the repository and can vary by form. Use the schema to identify the available names. A referenced-object panel displays the referenced object; a related-data panel displays a list of objects that point to the current object.

### Related-data navigator panels

Entity forms can contain navigator panels for records in other repositories that point to the current entity. These panels are registered from reverse references, including multi-step paths through filterable references. The panel is opened with a filter that selects records related to the current entity.

The name is formed as:

```text
<related-repository>.<reference-name>[.<reference-name>...]
```

```text
Crm.Sales.SalesOrders.Customer
Crm.Sales.SalesOrders.EnterpriseCompany
Projects.Agile.Cases.AssignedTo
```

For example, read `Crm.Sales.SalesOrders.Customer` as:

1. `Crm.Sales.SalesOrders` — the repository displayed by the panel;
2. `Customer` — the reference on that repository that points to the current object.

If the relation is reached through two references, the name contains both data-member names in order, such as `<repository>.<first-reference>.<second-reference>`. The repository and reference names are technical names, not localized captions. These panels are list panels; they do not display the referenced object itself.

### Pivot panels

Related pivot panels are registered from the same reverse-reference paths as entity-form related-data navigator panels. They use a `pivot.` prefix followed by the related repository and reference path:

```text
pivot.Crm.Sales.SalesOrders.Customer
pivot.General.Documents.Documents.EnterpriseCompany
```

For example, `pivot.Crm.Sales.SalesOrders.Customer` is the analytical counterpart of the related sales-orders list for a customer. Pivot availability depends on the repository and view mode.

### System panels

System panels provide supporting features when the repository and site support them. Examples include:

```text
Crm.Sales.SalesOrders.ExtensibleDataObject
Crm.Sales.SalesOrders.SystemInfo
Chatter
```

Other names you may encounter are:

```text
Notifications
Todo
PrintPreview
```

Availability depends on permissions, repository capabilities, and view mode.

## Panel names and captions

Panel names are technical identifiers. Captions are localized display text. For example, `Crm.Sales.SalesOrders.Lines` can have a different caption in each language, while its layout name remains unchanged.

Do not build a layout request from a caption. Use the panel name returned by the schema.

## Find a panel for a layout task

1. Call `get-layout-schema` for the exact form kind, repository or namespace, category, and view mode.
2. Find the target name in `Form.VisiblePanels` or `Form.VisibleSidePanel`.
3. Call `get-panel-layout-schema` with that exact name.
4. Use the returned panel section in `validate-layout` or `set-layout`.

Example request for a sales-order child collection:

```json
{
  "kind": "entity",
  "repository": "Crm.Sales.SalesOrders",
  "viewMode": "view",
  "categoryId": "<category-code>",
  "panelName": "Crm.Sales.SalesOrders.Lines"
}
```

Replace the category placeholder with a value returned by the live schema. A repository may not expose every example in this topic.

## Related topics

- [Web Client concepts](web-client-concepts.md)
- [Layout API](layout-api.md)
- [Web Client layout properties](layout-properties.md)
