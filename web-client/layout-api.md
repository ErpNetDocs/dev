# Manage Web Client form layouts through the Layout API

## Overview

The Web Client Layout API lets an authorized HTTP client discover form and panel layout schemas, read stored layouts, validate proposed layouts, and apply layouts for common and role-specific use.

Use this API when an integration, administration tool, or AI agent must manage Web Client layouts without opening a live form in a browser. The API creates the live form internally, so validation and application use the same form and panel rules as the Web Client.

The API is repository-oriented. Identify a repository form with `kind` and `repository`, or identify a Forms-namespace main-menu form with `kind` and `namespace`.

## Prerequisites

- An ERP.net Web Client site with the Layout API enabled.
- An authenticated Web Client API request. The controller uses the Web Client API authentication scheme; use the bearer/OIDC arrangement configured for the site.
- Access to the target repository and a valid Web Client form for it.
- Permission to manage the requested layout scope. Common layouts require the global layout manager permission. Role layouts require access to the selected role as a layout administrator, unless the caller has global layout manager permission.
- For entity forms, a repository that can create an entity form. A category must be a valid category of that repository.

The API is available relative to the Web Client site:

```text
POST /cl/layout-api/{operation}
```

Replace `/cl` with the Web Client application path used by the ERP.net instance. The interactive console is available at `/cl/layout-api/`, and the machine-readable description is available at `/cl/layout-api/openapi.json`.

### Layout API Console

The Layout API Console is a browser-based test client for the same HTTP endpoints documented in this topic. It helps you select an operation, build its request parameters, edit the layout JSON, send the request, and inspect the response. It is useful for learning and troubleshooting, but it is not required for automation: an AI agent or integration can call the endpoints directly with HTTP requests.

## Tools

Use the shared preview instance to try the API interactively or inspect its machine-readable contract:

- [Layout API console](https://testdb.my.erp.net/cl/layout-api/)
- [Layout API OpenAPI description](https://testdb.my.erp.net/cl/layout-api/openapi.json)

## Form and panel concepts

Before selecting a panel or a panel property, read [Web Client registered panels](registered-panels.md). It explains how navigator and entity-form panel names are registered and how related-data and referenced-object panel names are formed.

For the properties available inside form and panel layout sections, see [Web Client layout properties](layout-properties.md).

### Form kinds

The `kind` property selects the form family:

| `kind` | Purpose | Required identity |
| --- | --- | --- |
| `navigator` | A repository collection form | `repository` |
| `entity` | A form for one new entity object | `repository`, and optionally `categoryId` |
| `mainMenu` | The Forms-namespace repository menu | `namespace` |

The API creates a new, unsaved entity object for `entity` requests. It does not require an `objectId` and never saves that object. For a category-specific entity request, the created object must resolve to the requested category.

### Repository names

Use the domain repository name, such as `Crm.Sales.SalesOrders`. Repository names written in OData form with underscores are also normalized by the API.

The repository application is selected automatically from the repository namespace. You do not send the internal application name.

### View modes

`viewMode` selects a form view mode. The standard value is `view`; other values are form-specific, such as `pivot`, `kanban`, `timeline`, `printpreview`, or `changehistory`. `edit` is not a general Web Client form view mode. Omit `viewMode` to use the form default. Use `get-layout-schema` first to see the view modes offered by the selected form.

Panel availability can change with the view mode. A registered panel is not necessarily available in every mode.

### Categories

`categoryId` identifies the category used when the API creates an entity form for schema discovery, panel discovery, or layout reading. `categoryIds` is used inside a layout assignment when applying or validating layouts.

Categories are supported only for entity forms. The category must exist in the selected repository and must match the category of the created entity object.

### Common entity-category examples

Many ERP.net repositories use a related type repository as their entity-category source. The category field name is a useful clue when choosing a category, but the value sent as `categoryId` is the actual category identifier returned by the repository, not the literal name of the category type.

| Repository example | Category source | Meaning of one category |
| --- | --- | --- |
| `Crm.Sales.SalesOrders` | `DocumentType` | One document type, such as a sales-order document type. |
| `Crm.Sales.Customers` | `CustomerType` | One customer type. |
| `General.Products.Products` | `ProductType` | One product type. |
| `Logistics.Procurement.Suppliers` | `SupplierType` | One supplier type. |
| `Projects.Agile.Cases` | `CaseCategory` | One active case category. |
| `General.Contacts.Parties` | `PartyType` | One party type, such as `Company`. |

For the first four examples, the category identifier is normally the identifier of the related type record. For case categories, it is the identifier of the `CaseCategory` record. Some repositories use enum values instead of related records, so always use the category values reported by the live repository or form schema.

`General.Contacts.Parties` is an important exception: its categories come from the `PartyType` enum. Its `categoryId` is a string enum value such as `Company`, not a GUID.

For example, a category-specific entity-form request has this shape:

```json
{
  "kind": "entity",
  "repository": "Crm.Sales.Customers",
  "categoryId": "<CustomerType-id>",
  "viewMode": "view"
}
```

The placeholder must be replaced with an actual category identifier from the target ERP.net instance. Category identifiers and available categories can vary by database, company context, and permissions; do not copy a category identifier from another instance.

### Panels and side panels

A form has registered panels and may expose visible main panels and a visible side panel. The form schema reports compact panel registration information and the panel names available in the selected view mode.

Use `get-panel-layout-schema` to inspect one panel in detail. The API creates that panel on demand and returns its complete instance-aware schema, including available fields and constraints. This keeps the form-level response small even when a form has hundreds of registered panels.

## Layout structure

A layout is a JSON object with a `Form` section and, when configured, one section for each panel. The exact properties are defined by the schemas returned by the API.

Typical structure:

```json
{
  "Form": {
    "VisiblePanels": ["NavigatorPanel"],
    "VisibleSidePanel": "Details"
  },
  "NavigatorPanel": {
    "RepositoryPanelLayoutInfo": {
      "VisibleDataFields": [
        { "Name": "DocumentNo" },
        { "Name": "DocumentDate" }
      ]
    },
    "DataGridLayoutInfo": {
      "VisibleColumns": [
        { "Name": "DocumentNo", "Width": "110px" },
        { "Name": "DocumentDate", "Width": "120px" }
      ]
    }
  }
}
```

### Form layout properties

The `Form` section contains form-level properties, such as visible panels and the visible side panel. The accepted panel names and enum values are returned by `get-layout-schema`.

Do not invent panel names. Use the values returned in the form schema for the requested form and view mode.

### Panel-specific layout properties

Each panel section is owned by the panel implementation. Request the panel schema before writing panel properties.

For repository navigator panels, two related sections can appear:

- `RepositoryPanelLayoutInfo` describes repository-oriented field presentation, including `VisibleDataFields`.
- `DataGridLayoutInfo` describes the grid presentation, including `VisibleColumns` and column widths.

They are not interchangeable. If the goal is to control the columns rendered by a data grid, configure `DataGridLayoutInfo.VisibleColumns`. If the goal is to control repository fields used by the panel, configure `RepositoryPanelLayoutInfo.VisibleDataFields` as well.

Column width values are layout strings. Use the format returned by the panel schema and existing layouts, such as `110px`.

### How layout sections are built

The layout JSON follows the form and panel type hierarchy:

```text
form layout
├── Form
└── <panel name>
    ├── PanelLayoutInfo                 common properties for every panel
    ├── <base panel type section>       properties added by a panel base class
    └── <derived panel type sections>   properties added by panel subclasses
```

`Form` is the form-level section. Every other top-level section normally has a panel name returned by `get-layout-schema`. The value of that section is the schema returned by `get-panel-layout-schema` for that panel.

The panel schema is cumulative. The base `Panel` contributes `PanelLayoutInfo`. A panel inheritor calls the base schema builder and adds its own typed section. Further inheritors can call their base implementation and add more sections. Consequently, a repository data-grid panel can expose all of the following in one panel section:

```text
NavigatorPanel
├── PanelLayoutInfo
├── RepositoryPanelLayoutInfo
└── DataGridLayoutInfo
```

This is an inheritance relationship, not three separate panels. `NavigatorPanel` remains the single top-level layout section; the nested sections describe the capabilities contributed by its panel class hierarchy.

For example, the `DataGridRepositoryPanel` implementation builds on repository-panel behavior. Its schema contains the common `PanelLayoutInfo`, repository properties such as `RepositoryPanelLayoutInfo`, and grid properties such as `DataGridLayoutInfo`. Other panel classes add different sections, such as filter, data-object, pivot, or menu-specific layout information.

Do not infer nested section names from the panel name. Read the complete panel schema and use the property names it returns. A panel can add or omit sections depending on its concrete implementation and runtime repository.

## API operations

All operations use `POST` and a JSON request body.

### `get-layout-schema`

Returns the form-level JSON Schema. It includes form properties, available view modes, compact panel registration metadata, visible-panel values, and entity layout categories when applicable. Detailed panel properties are intentionally omitted.

### `get-panel-layout-schema`

Returns the complete schema for one panel. The panel must be registered and available for the selected form context.

### `get-layout`

Returns a stored layout. Omit `roleId` to address the common role layout. `key` selects a configuration scope when a form supports named layout sections; omit it to return the complete effective form layout.

### `validate-layout`

Applies each requested assignment to a temporary live form, flushes the current layout, validates the effective result against the form schema and the required panel schemas, and returns path-based errors. It never saves the layout.

### `set-layout`

Applies and saves every requested assignment. It returns the normalized saved layout for each expanded target. Always validate the identical request before calling this operation.

### `roles`

Returns the roles that the authenticated API user is allowed to manage for layouts. Use this endpoint when a prompt names a role but provides no role identifier, or when an agent must confirm that a requested role is manageable before building a batch assignment.

```http
GET /cl/layout-api/roles
Authorization: Bearer <access-token>
```

The response is an array containing each role's identifier and display name:

```json
[
  {
    "id": "11111111-1111-1111-1111-111111111111",
    "name": "Sales manager"
  }
]
```

Role names are display text and can be localized or non-unique. Use `id` in `roleIds`; use `name` only for matching a user's prompt and confirming the selection.

### `categories`

Returns the entity categories available for a repository. Use this endpoint when an agent needs to translate a category name from a prompt into the category code required by `categoryId` or `categoryIds`.

```http
GET /cl/layout-api/categories?repository=Crm.Sales.Customers
Authorization: Bearer <access-token>
```

The repository query parameter uses the domain repository name. The response contains category codes and display names:

```json
[
  {
    "id": "Retail",
    "name": "Retail customer"
  },
  {
    "id": "Corporate",
    "name": "Corporate customer"
  }
]
```

Use the returned `id` as the category value. Categories are meaningful only for repositories that support entity categories; do not call this endpoint for navigator-only or main-menu operations.

## Request schema reference

The following sections describe every request property used by `get-layout` and `set-layout`. Property names are case-insensitive when ASP.NET Core binds the JSON request, but the examples use the documented casing.

### Common form-selector properties

These properties identify the form for both operations:

| Property | Required | Description |
| --- | --- | --- |
| `kind` | Yes | `navigator`, `entity`, or `mainMenu`. Selects the form family. |
| `repository` | For `navigator` and `entity` | Domain repository name, for example `Crm.Sales.SalesOrders`. Do not send it for `mainMenu`. |
| `namespace` | For `mainMenu` | Forms namespace used to create the main-menu form. It is the first two repository-namespace segments, or the only segment for a one-segment namespace. Do not send it for repository forms. |
| `viewMode` | No | Form view mode, such as `view`, `pivot`, `kanban`, `timeline`, `printpreview`, or `changehistory`, when offered by that form. `edit` is not a general form view mode. If omitted, the form default is used. The selected mode controls panel availability. `mainMenu` supports only `view`. |
| `categoryId` | No | Entity category used when creating an entity form for a read operation. It is allowed only for `entity` and must be a valid category code for `repository`. For batch writes, put categories in each assignment's `categoryIds` instead. |

The request must not mix `repository` with `namespace`, or use `categoryId` with `navigator` or `mainMenu`.

### `get-layout` properties

`get-layout` accepts the common form-selector properties plus:

| Property | Required | Description |
| --- | --- | --- |
| `roleId` | No | The target role layout as a role GUID. Omit it, or set it to `null`, to read the common layout. The caller must be authorized to manage the selected role. |
| `key` | No | A colon-delimited layout configuration scope key. Omit it, or set it to `null`, to return the complete effective form layout. Use a key only when the form exposes that named scope. For example, `NavigatorPanel:RepositoryPanelLayoutInfo:VisibleDataFields`. This is a configuration key, not a JSONPath. |

Example: read a role-specific entity layout for a selected category:

```json
{
  "kind": "entity",
  "repository": "Crm.Sales.Customers",
  "categoryId": "<CustomerType-id>",
  "viewMode": "view",
  "roleId": "<role-guid>",
  "key": null
}
```

### `set-layout` properties

`set-layout` accepts the common form-selector properties and a required `layouts` array. The root `categoryId` must be omitted for this operation; categories belong to the assignments.

| Property | Required | Description |
| --- | --- | --- |
| `layouts` | Yes | One or more layout assignments. The API expands every assignment into its role/category targets, applies each target, and saves it. |

Each item in `layouts` has these properties:

| Property | Required | Description |
| --- | --- | --- |
| `roleIds` | No | Array of role GUIDs. Empty or omitted means the common role layout. With multiple values, the assignment is expanded once per role. |
| `categoryIds` | No | Array of category codes. Empty or omitted means the common category layout. Supported only for entity forms. With multiple values, the assignment is expanded once per category. |
| `key` | No | Colon-delimited configuration scope key to update. Omit it to update the complete form layout. For example, `NavigatorPanel:RepositoryPanelLayoutInfo:VisibleDataFields`. |
| `replace` | No | `true` replaces the selected scope; `false` merges the supplied properties into the selected scope. The default is `true`. |
| `layout` | Yes | JSON layout object. Use properties accepted by `get-layout-schema` and the panel schemas requested with `get-panel-layout-schema`. |

When `roleIds` and `categoryIds` both contain values, the API updates their Cartesian product. For example, two roles and three categories create six targets. A target may occur only once in one request.

The `layout` object is scoped by its top-level sections. `Form` contains form properties, and a panel name contains that panel's properties. With `replace: false`, send only the sections and properties that should change. With `replace: true`, construct a complete replacement for the selected scope from the discovered schemas.

### Scoped layout keys

`key` addresses a nested configuration scope using colon-separated names. It is useful when changing only one panel section or one panel property. The key below selects the repository field list inside the navigator panel:

```text
NavigatorPanel:RepositoryPanelLayoutInfo:VisibleDataFields
```

When this key is supplied, `layout` is the value of that scope—in this case an array of `DataFieldLayoutInfo` objects—not a complete form-layout object.

Example for a sales-orders navigator:

```json
{
  "kind": "navigator",
  "repository": "Crm.Sales.SalesOrders",
  "viewMode": "view",
  "layouts": [
    {
      "roleIds": [],
      "categoryIds": [],
      "key": "NavigatorPanel:RepositoryPanelLayoutInfo:VisibleDataFields",
      "replace": true,
      "layout": [
        { "Name": "DocumentNo" },
        { "Name": "DocumentDate" },
        { "Name": "Customer" },
        { "Name": "State" },
        { "Name": "RequiredDeliveryDate" },
        { "Name": "DocumentCurrency" },
        { "Name": "AmountToPay" }
      ]
    }
  ]
}
```

The field names must belong to the repository returned by the selected panel schema. For a different repository, first call `get-panel-layout-schema` and use its `VisibleDataFields` item schema.

### Assignment scope examples

| Goal | `roleIds` | `categoryIds` | Result |
| --- | --- | --- | --- |
| Common layout for every category | `[]` | `[]` | Common role and common category. |
| One role, all categories | `[role-guid]` | `[]` | Role-specific common-category layout. It does not automatically overwrite every category-specific layout. |
| Common role, two categories | `[]` | `[category-a, category-b]` | Two category-specific common-role layouts. |
| Two roles and two categories | `[role-a, role-b]` | `[category-a, category-b]` | Four role/category layouts. |

## Example: change only visible fields and columns

Use `replace: false` when you want to retain every existing layout setting except the properties in the request. This example changes only the repository fields and grid columns of the `NavigatorPanel`; it does not replace the form layout or other panel settings.

```http
POST /cl/layout-api/set-layout
Content-Type: application/json
Authorization: Bearer <access-token>
```

```json
{
  "kind": "navigator",
  "repository": "Crm.Sales.SalesOrders",
  "viewMode": "view",
  "layouts": [
    {
      "roleIds": [],
      "categoryIds": [],
      "replace": false,
      "layout": {
        "NavigatorPanel": {
          "RepositoryPanelLayoutInfo": {
            "VisibleDataFields": [
              { "Name": "DocumentNo" },
              { "Name": "DocumentDate" },
              { "Name": "Customer" },
              { "Name": "State" }
            ]
          },
          "DataGridLayoutInfo": {
            "VisibleColumns": [
              { "Name": "DocumentNo", "Width": "110px" },
              { "Name": "DocumentDate", "Width": "120px" },
              { "Name": "Customer", "Width": "240px" },
              { "Name": "State", "Width": "120px" }
            ]
          }
        }
      }
    }
  ]
}
```

`VisibleDataFields` and `VisibleColumns` are different properties. The first controls repository-field presentation; the second controls the data-grid columns. Use the exact field and column names returned by `get-panel-layout-schema`.

### Adding one field or column requires read-modify-write

The API does not have an append operation for layout arrays. `replace: false` merges object sections and leaves omitted properties unchanged, but an array supplied for a property is written as the value of that property.

For example, this request does not append `AmountToPay` to the existing grid columns:

```json
{
  "replace": false,
  "layout": {
    "NavigatorPanel": {
      "DataGridLayoutInfo": {
        "VisibleColumns": [
          { "Name": "AmountToPay", "Width": "120px" }
        ]
      }
    }
  }
}
```

It replaces the `VisibleColumns` array with an array containing only `AmountToPay`. The same rule applies to `VisibleDataFields`, `SortColumns`, `GroupColumns`, and other array-valued layout properties.

To add one field or column while preserving the existing values:

1. Call `get-layout` for the exact form, view mode, role, category, and key that will be changed.
2. Read the existing array from the response.
3. Add, remove, or reorder the item in your client or AI-agent memory.
4. Send the complete updated array in a `validate-layout` request. Keep unrelated properties omitted and use `replace: false` when the surrounding scope must be preserved.
5. If validation succeeds, send the identical request to `set-layout`.

If the current array is:

```json
[
  { "Name": "DocumentNo", "Width": "110px" },
  { "Name": "DocumentDate", "Width": "120px" },
  { "Name": "Customer", "Width": "240px" }
]
```

the request that adds `AmountToPay` must contain the complete new array:

```json
{
  "roleIds": [],
  "categoryIds": [],
  "replace": false,
  "layout": {
    "NavigatorPanel": {
      "DataGridLayoutInfo": {
        "VisibleColumns": [
          { "Name": "DocumentNo", "Width": "110px" },
          { "Name": "DocumentDate", "Width": "120px" },
          { "Name": "Customer", "Width": "240px" },
          { "Name": "AmountToPay", "Width": "120px" }
        ]
      }
    }
  }
}
```

This is intentionally a read-modify-write operation. It prevents the API from guessing whether an omitted array item should be retained, removed, reordered, or reset to a form default.

## Example: change one other panel property

The base `PanelLayoutInfo` section contains common panel presentation properties. This example changes only whether the `NavigatorPanel` is expanded. Existing field, column, sizing, and ordering settings remain unchanged because `replace` is `false`.

```json
{
  "kind": "navigator",
  "repository": "Crm.Sales.SalesOrders",
  "viewMode": "view",
  "layouts": [
    {
      "roleIds": [],
      "categoryIds": [],
      "replace": false,
      "layout": {
        "NavigatorPanel": {
          "PanelLayoutInfo": {
            "Expanded": false
          }
        }
      }
    }
  ]
}
```

Other common `PanelLayoutInfo` properties include `EnterStop`, `ColumnPosition`, `TabsState`, `HeightMode`, `UserTitle`, and `WidthPercent`. Their accepted values and constraints are panel-schema data; request `get-panel-layout-schema` before using them.

## Procedure: discover a form schema

1. Select the form kind and repository.
2. Call `get-layout-schema`.
3. Read the available view modes and panel registration metadata.
4. Select only the panels that the layout must change.
5. Call `get-panel-layout-schema` for each selected panel.

Example for the Sales Orders navigator:

```http
POST /cl/layout-api/get-layout-schema
Content-Type: application/json
Authorization: Bearer <access-token>
```

```json
{
  "kind": "navigator",
  "repository": "Crm.Sales.SalesOrders",
  "viewMode": "view"
}
```

Then request the complete navigator panel schema:

```http
POST /cl/layout-api/get-panel-layout-schema
Content-Type: application/json
Authorization: Bearer <access-token>
```

```json
{
  "kind": "navigator",
  "repository": "Crm.Sales.SalesOrders",
  "viewMode": "view",
  "panelName": "NavigatorPanel"
}
```

## Procedure: read the common layout

1. Send the same form identity used for schema discovery.
2. Omit `roleId` to select the common layout.
3. Omit `key` to receive the complete effective layout.

```http
POST /cl/layout-api/get-layout
Content-Type: application/json
Authorization: Bearer <access-token>
```

```json
{
  "kind": "navigator",
  "repository": "Crm.Sales.SalesOrders",
  "viewMode": "view"
}
```

## Procedure: validate a layout without saving

1. Discover the form and panel schemas.
2. Construct a layout using only fields, panels, and values allowed by those schemas.
3. Put the layout in one or more `layouts` assignments.
4. Use empty `roleIds` and `categoryIds` for the common layout.
5. Call `validate-layout`.
6. Fix every returned error before saving.

This example shows a compact, realistic Sales Orders grid layout:

```http
POST /cl/layout-api/validate-layout
Content-Type: application/json
Authorization: Bearer <access-token>
```

```json
{
  "kind": "navigator",
  "repository": "Crm.Sales.SalesOrders",
  "viewMode": "view",
  "layouts": [
    {
      "roleIds": [],
      "categoryIds": [],
      "replace": true,
      "layout": {
        "Form": {
          "VisiblePanels": ["NavigatorPanel"],
          "VisibleSidePanel": "Details"
        },
        "NavigatorPanel": {
          "RepositoryPanelLayoutInfo": {
            "VisibleDataFields": [
              { "Name": "DocumentNo" },
              { "Name": "DocumentDate" },
              { "Name": "Customer" },
              { "Name": "State" },
              { "Name": "AmountToPay" }
            ]
          },
          "DataGridLayoutInfo": {
            "VisibleColumns": [
              { "Name": "DocumentNo", "Width": "110px" },
              { "Name": "DocumentDate", "Width": "120px" },
              { "Name": "Customer", "Width": "240px" },
              { "Name": "State", "Width": "120px" },
              { "Name": "AmountToPay", "Width": "120px" }
            ]
          }
        }
      }
    }
  ]
}
```

The response contains `Valid`, one result for each expanded target, the effective layout, and an `Errors` array. Each error includes a JSON path such as `$.NavigatorPanel.DataGridLayoutInfo.VisibleColumns[2].Name`.

## Procedure: save a common layout

After validation succeeds, send the identical request to `set-layout`:

```http
POST /cl/layout-api/set-layout
Content-Type: application/json
Authorization: Bearer <access-token>
```

Use the same JSON body as the validation example. The response contains:

```json
{
  "updatedLayouts": 1,
  "results": [
    {
      "roleId": null,
      "categoryId": null,
      "key": null,
      "layout": {}
    }
  ]
}
```

The returned layout is the normalized layout after the live form has applied it and the configuration has been saved.

### Errors and warnings

The API separates problems in the submitted request from incompatible properties that already exist in the stored layout.

- `Errors` describe the request being validated or applied. An invalid field name, panel name, enum value, layout key, or property value is an error. A result with one or more errors has `Valid: false`; do not send that request to `set-layout`.
- `Warnings` describe legacy properties found in the layout before the new request is applied. They do not make the result invalid. The live form ignores those incompatible stored properties while building the effective layout, so the requested valid changes can still be inspected or saved.
- Warnings are compatibility information, not an automatic migration. The API does not rewrite every old invalid property merely because it reported a warning. Review warnings when deciding whether the stored layout should later be replaced with a clean, complete layout.

An invalid submitted property is returned as an error:

```json
{
  "valid": false,
  "results": [
    {
      "roleId": null,
      "categoryId": null,
      "layout": null,
      "errors": [
        {
          "path": "$.NavigatorPanel.DataGridLayoutInfo.VisibleColumns[0].Name",
          "message": "The value does not match any allowed schema. Invalid value: \"NotAField\"."
        }
      ],
      "warnings": []
    }
  ]
}
```

If the existing stored layout contains an old field or property that is no longer in the current schema, the request can still be valid, but the result includes a warning:

```json
{
  "valid": true,
  "results": [
    {
      "roleId": null,
      "categoryId": null,
      "layout": {
        "NavigatorPanel": {
          "DataGridLayoutInfo": {
            "ShowFilterRow": true
          }
        }
      },
      "errors": [],
      "warnings": [
        {
          "path": "$.NavigatorPanel.DataGridLayoutInfo.VisibleColumns[2].Name",
          "message": "Stored layout property was ignored: The value does not match any allowed schema."
        }
      ]
    }
  ]
}
```

Treat a warning as advisory: inspect it, but do not retry the same request unless the requested layout itself has an error. `validate-layout` reports the effective layout produced by a temporary form and never saves it. `set-layout` reports the normalized layout after applying and saving it. When an assignment has a `key`, `set-layout` returns the saved value for that scoped key; without a key it returns the complete form layout.

## Procedure: apply one layout to several roles

Put the role identifiers in one assignment. The API expands the assignment once for each role.

```json
{
  "kind": "navigator",
  "repository": "Crm.Sales.SalesOrders",
  "viewMode": "view",
  "layouts": [
    {
      "roleIds": [
        "11111111-1111-1111-1111-111111111111",
        "22222222-2222-2222-2222-222222222222"
      ],
      "categoryIds": [],
      "replace": false,
      "layout": {
        "Form": {
          "VisibleSidePanel": "Details"
        }
      }
    }
  ]
}
```

Use `replace: false` to merge the supplied properties into each selected scope. Use `replace: true` to replace the selected scope with the supplied layout.

## Procedure: apply category layouts in a batch

For entity forms, `categoryIds` can contain several categories. The API updates the Cartesian product of `roleIds` and `categoryIds`.

```json
{
  "kind": "entity",
  "repository": "Crm.Sales.Customers",
  "viewMode": "view",
  "layouts": [
    {
      "roleIds": [],
      "categoryIds": ["Retail", "Corporate"],
      "replace": true,
      "layout": {
        "Form": {
          "VisiblePanels": ["Header", "Details"]
        }
      }
    }
  ]
}
```

Replace the category values with category codes returned by the repository. Empty `roleIds` means the common role layout. Empty `categoryIds` means the common category layout.

Categories are not allowed for navigator or main-menu forms.

## Procedure: configure a main-menu layout

The main menu is addressed as a Forms namespace rather than a repository. `namespace` contains the first two segments of the repository namespace, or the single segment when the namespace has only one segment. For example, repositories in `Crm.Sales` use `Crm.Sales`.

```http
POST /cl/layout-api/get-layout-schema
Content-Type: application/json
Authorization: Bearer <access-token>
```

```json
{
  "kind": "mainMenu",
  "namespace": "Crm.Sales"
}
```

Use the returned schema to discover the available menu panels and then apply a layout with `set-layout` using the same `kind` and `namespace`.

## Batch rules

- `layouts` must contain at least one assignment for `set-layout` and `validate-layout`.
- Empty or omitted `roleIds` selects the common role layout.
- Empty or omitted `categoryIds` selects the common category layout.
- When both arrays contain values, every role/category combination is updated.
- The same role/category target cannot occur twice in one request.
- `categoryIds` is supported only for `entity` forms.
- The root `categoryId` is used for form creation and schema/read operations; categories for batch writes belong inside each assignment.
- `replace: true` replaces the selected scope; `replace: false` merges the supplied properties.
- An omitted assignment `key` addresses the complete form layout. A supplied key addresses a named configuration scope when supported by the form.

## Working with an AI agent

Give an agent this sequence:

1. Call `roles` when the prompt refers to roles by name or requests role-specific layouts.
2. Call `categories` for entity repositories when the prompt refers to categories by name.
3. Call `get-layout-schema` for the target form.
4. Select the panels and properties that must change.
5. Call `get-panel-layout-schema` for each selected panel.
6. Build the smallest layout containing the requested changes.
7. Call `validate-layout` with the complete intended batch.
8. Inspect every result, error, and warning path.
9. Call `set-layout` with the identical validated request.
10. Read the returned layouts and verify the normalized result.

### Cache and index schemas

Schema responses can be large, especially for forms with many registered panels. An agent should cache schemas during a layout task instead of requesting the same schema repeatedly. A useful cache key includes:

```text
site + kind + repository/namespace + categoryId + viewMode
```

Cache the form schema separately from panel schemas. Index the cached data for fast lookup by:

- panel name;
- panel type and side/main-panel location;
- layout property name;
- field name and field description;
- enum property and allowed values.

This lets an agent search the schema for a field or property without placing the entire schema in every reasoning step. Invalidate or refresh the cache when the site, repository, category, view mode, or panel schema request changes. Treat the live response as authoritative when a cached entry is missing or an operation returns a schema-validation error.

### Cost-effective agent plan

Use the following plan to minimize HTTP calls and prompt/context size while keeping layout changes safe:

1. **Normalize the request.** Extract the form kind, repository or namespace, view mode, category names, role names, target panels, and requested changes from the prompt. Do not call an endpoint for information already present in the verified cache.
2. **Resolve only named roles and categories.** Call `roles` only for role-specific work and `categories` only for entity forms whose category codes are not already known. Cache both results for the current site and user.
3. **Load one form schema.** Call `get-layout-schema` once for each unique form context (`kind`, repository or namespace, category, and view mode). Index its panel names, view modes, categories, and main/side locations.
4. **Request panel schemas on demand.** Call `get-panel-layout-schema` only for panels that will be changed. Do not request every registered panel merely because it is listed in the form schema.
5. **Read existing values only when necessary.** Call `get-layout` when the task depends on the current layout, uses `replace: false`, changes one item in an array, or must preserve unrelated settings. If the task is a complete replacement and all values are already known, skip this call.
6. **Build a minimal assignment.** Prefer a scoped `key` when changing one panel property or one panel section. A scoped request sends only that value and avoids loading or rewriting the complete form layout. Use `replace: false` for a larger form-level change when preserving surrounding configuration. Put identical role/category changes in one batch assignment.
7. **Validate once.** Send one `validate-layout` request containing the complete batch. Do not validate each role or category separately unless they use different form contexts or different layouts.
8. **Repair the request, not the stored data.** Fix errors from the submitted layout. Treat warnings about old stored properties as compatibility information unless they affect the requested result.
9. **Apply once.** After validation succeeds, send the identical JSON body to `set-layout`. Do not regenerate the layout between validation and application.
10. **Verify selectively.** Use the normalized `set-layout` response as the result. Call `get-layout` again only to verify a separate scope, confirm inheritance, or continue with a dependent change.

For repeated workloads, maintain two caches:

- a **metadata cache** for roles, categories, repositories, view modes, and form schemas;
- a **layout cache** for the last validated or saved layout per role/category/key scope.

Do not place the complete schema or complete layout in every model prompt. Retrieve indexed slices such as the matching panel schema, field enum, or property description. This reduces token usage while preserving the live schema as the authority for validation.

### Prefer scoped requests for local changes

Use a scoped request when the task affects one independent configuration value, such as:

- the visible fields of one repository panel;
- the grid columns of one data-grid panel;
- the files side-panel view type;
- the visible links of one main-menu tile.

For example, changing only repository fields uses:

```text
NavigatorPanel:RepositoryPanelLayoutInfo:VisibleDataFields
```

The `layout` value is then only the array for that property. This reduces request size, response size, merge work, and the chance of accidentally changing unrelated settings.

Use the complete form layout instead when a change coordinates multiple panels, changes panel order or visibility together with panel settings, or depends on relationships such as tab groups and two-column positioning. Always request the schema for the selected scope and use `replace: true` only when the scope value is complete.

The agent should never guess a panel name, field name, enum value, category code, or view mode when it can discover that value from the schemas.

## Notes and limitations

- The API creates a live form internally; a browser preview is not required.
- `get-layout-schema` is intentionally compact. Request panel schemas separately when needed.
- A panel can be registered but unavailable in the selected view mode.
- A layout property can be accepted by JSON configuration but normalized by the live form before it is returned.
- `validate-layout` applies layouts to temporary forms and does not call `SaveLayout`.
- `set-layout` writes to the database and should be protected by the same review and authorization process as any other configuration change.
- Invalid properties in the submitted request are errors. Invalid properties already present in a stored layout are reported as `Warnings` and ignored while the effective layout is applied.
- The API returns errors in JSON form. The response body should be retained when diagnosing an invalid layout.

## Related topics

- [Web Client concepts](web-client-concepts.md)
- [Web Client registered panels](registered-panels.md)
- [Web Client layout properties](layout-properties.md)
