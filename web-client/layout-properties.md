# Web Client layout properties

## Overview

This topic is a reference for the main layout properties returned by the Web Client Layout API. It explains the common form and panel properties first, then the properties added by repository, data-grid, files, and main-menu panels.

Use `get-layout-schema` to discover the exact schema for a form and `get-panel-layout-schema` to inspect one panel. The names and enum values in the live schema are authoritative for the selected repository, category, and view mode.

The `get-layout-schema` and `get-panel-layout-schema` responses include this metadata as JSON Schema: available properties, descriptions, constraints, and enum values. Use the returned schema as the source of truth instead of relying only on the examples in this topic.

For panel names and registration rules, see [Web Client registered panels](registered-panels.md). For request construction and applying layouts, see [Layout API](layout-api.md).

## Layout structure

A form layout is an object whose top-level properties describe the form. The `Form` property contains the form-level settings represented by `FormLayoutInfo`. Each selected panel has its own named section containing the base `PanelLayoutInfo` and, when applicable, a panel-specific layout object.

The exact JSON property names are shown by the schema. A typical layout has this shape:

```json
{
  "Form": {
    "VisiblePanels": ["Crm.Sales.SalesOrders.Header", "Crm.Sales.SalesOrders.Lines"],
    "VisibleSidePanel": "Chatter"
  },
  "Crm.Sales.SalesOrders.Header": {
    "PanelLayoutInfo": {
      "ColumnPosition": 0,
      "Expanded": true
    },
    "RepositoryPanelLayoutInfo": {
      "VisibleDataFields": [
        { "Name": "DocumentNo" },
        { "Name": "Customer" }
      ]
    }
  }
}
```

Do not assume that every panel has every section. A panel schema contains only the properties supported by that panel.

## Form layout areas

The form layout has two distinct areas:

- **Main area** — contains the ordered panels listed in `Form.VisiblePanels`.
- **Side-panel area** — contains the panel selected by `Form.VisibleSidePanel`. It is a separate slot and is not another item in the main-panel columns.

The main area is arranged by the `PanelLayoutInfo` settings of each main panel. A panel can occupy the full row or one of two columns. The column arrangement is evaluated in panel order, so use the order in `VisiblePanels` together with each panel's `ColumnPosition` when designing the layout.

For example, two consecutive panels can be placed side by side:

```json
{
  "Form": {
    "VisiblePanels": ["Header", "Lines"]
  },
  "Header": {
    "PanelLayoutInfo": {
      "ColumnPosition": 1,
      "WidthPercent": 40
    }
  },
  "Lines": {
    "PanelLayoutInfo": {
      "ColumnPosition": 2,
      "WidthPercent": 60
    }
  }
}
```

These enum properties are represented as integers in the JSON layout. A full-row panel uses `ColumnPosition: 0` and starts a new row. Confirm the values in the live schema if the API version changes.

Panels can also be grouped into tabs. One panel starts a tab group by setting `TabsState` to a group orientation such as `Top`, `Left`, `Bottom`, or `Right`. Each following panel that uses `TabsState: "Tab"` joins the preceding group. A panel with `TabsState: "None"` is outside a tab group.

Example tab group:

```json
{
  "Form": {
    "VisiblePanels": ["Header", "Lines", "Payments"]
  },
  "Header": {
    "PanelLayoutInfo": { "TabsState": 1 }
  },
  "Lines": {
    "PanelLayoutInfo": { "TabsState": 5 }
  },
  "Payments": {
    "PanelLayoutInfo": { "TabsState": 5 }
  }
}
```

In this example, `Header` starts the tab group and `Lines` and `Payments` join it. Tab grouping and column positioning are both panel settings; changing only `Form.VisiblePanels` does not define the visual arrangement by itself.

## Form layout properties

The `Form` object contains the `FormLayoutInfo` settings that control which panels are shown and which side panel is active.

### `VisiblePanels`

`VisiblePanels` is an ordered array of main-panel names. The order controls the main form panel order. Use names returned in `Form.VisiblePanels`; do not use localized captions.

```json
{
  "Form": {
    "VisiblePanels": [
      "Crm.Sales.SalesOrders.Header",
      "Crm.Sales.SalesOrders.Lines"
    ]
  }
}
```

### `VisibleSidePanel`

`VisibleSidePanel` contains the name of the active side panel, or `null` when no side panel is selected. Use a name returned in `Form.VisibleSidePanel`.

```json
{
  "Form": {
    "VisibleSidePanel": "Chatter"
  }
}
```

## Common panel properties

Every panel schema contains `PanelLayoutInfo` unless the panel explicitly has no layout state.

| Property | Meaning |
| --- | --- |
| `EnterStop` | Includes the panel in Enter-key navigation. |
| `Expanded` | Whether the panel is expanded when displayed. |
| `ColumnPosition` | Places the panel in the full row, left column, or right column. The schema supplies the enum values. |
| `TabsState` | Starts, joins, or leaves a panel tab group. The schema supplies the tab-state enum values. |
| `HeightMode` | Controls the panel height behavior. Use the enum values from the schema. |
| `UserTitle` | Replaces the system title. `null` uses the localized system title. |
| `WidthPercent` | Width percentage when the panel is positioned in the left column. |

### Panel enum values

`ColumnPosition` accepts:

| JSON value | Enum name | Meaning |
| --- | --- |
| `0` | `Fill` | Occupies the full row. |
| `1` | `Left` | Occupies the left column. |
| `2` | `Right` | Occupies the right column. |

`TabsState` accepts:

| JSON value | Enum name | Meaning |
| --- | --- |
| `0` | `None` | The panel is not in a tab group. |
| `1` | `Top` | Starts a horizontal tab group with tabs at the top. |
| `2` | `Left` | Starts a vertical tab group with tabs on the left. |
| `3` | `Bottom` | Starts a horizontal tab group with tabs at the bottom. |
| `4` | `Right` | Starts a vertical tab group with tabs on the right. |
| `5` | `Tab` | Joins the tab group started by the preceding panel. |

`HeightMode` accepts:

| JSON value | Enum name | Meaning |
| --- | --- |
| `0` | `Default` | Uses the normal system or layout height. |
| `1` | `FullScreen` | Stretches to the available viewport height. |
| `2` | `FullExpand` | Expands to fit its content without scrollbars. |
| `3` | `Rows1` | Uses a fixed height of one text row. |
| `4` | `Rows2` | Uses a fixed height of two text rows. |
| `5` | `Rows3` | Uses a fixed height of three text rows. |
| `6` | `Rows4` | Uses a fixed height of four text rows. |

Example:

```json
{
  "PanelLayoutInfo": {
    "ColumnPosition": 1,
    "WidthPercent": 60,
    "TabsState": 2,
    "Expanded": true,
    "UserTitle": "Order details"
  }
}
```

Panel position and tab values are layout relationships. When using `TabsState: "Tab"`, the panel joins the preceding compatible tab group; it does not create a new group by itself.

## Repository panel properties

`RepositoryPanelLayoutInfo` is added by panels that display repository fields, including entity headers, child collections, and repository list panels.

### `VisibleDataFields`

`VisibleDataFields` selects and orders the repository fields displayed by the panel. Each item is a `DataFieldLayoutInfo` object. The field `Name` must be one of the names returned by the panel schema.

Supported field settings include:

- `Name` — repository field name;
- `OrderDirection` — no sort, ascending, or descending;
- `EnterStop` — includes the field in Enter-key navigation;
- `SummaryType` — field summary behavior where supported;
- `CustomCaption` — user-defined field caption;
- `CustomDropdownWidth` and `CustomDropdownHeight` — custom selector dimensions where supported.

The integer `OrderDirection` enum values are `None` (0), `Ascending` (1), and `Descending` (2). The integer `SummaryType` enum values are `None` (0), `Sum` (1), `Avg` (2), `Min` (3), `Max` (4), `Count` (5), and `Distinct` (6).

The array is also the field order. To add one field without changing the others, start with the current `VisibleDataFields` array from `get-layout-schema` or `get-panel-layout-schema`, append the field, and submit the complete desired array.

Use the special field name `NULL` only when the intended result is an empty field list.

```json
{
  "RepositoryPanelLayoutInfo": {
    "VisibleDataFields": [
      { "Name": "DocumentNo", "OrderDirection": 1 },
      { "Name": "Customer", "CustomCaption": "Customer" },
      { "Name": "DocumentDate" }
    ]
  }
}
```

`VisibleFields` is an obsolete compatibility property. New layouts should use `VisibleDataFields`.

### `OrderBy` and `Modified`

`OrderBy` is an older repository-layout property. Prefer `DataFieldLayoutInfo.OrderDirection` for field sorting unless the live schema explicitly requires `OrderBy`.

`Modified` indicates that a stored repository layout contains explicit settings. It is normally produced by the Web Client when saving a layout; layout clients should not use it to select fields.

## Data-grid panel properties

Data-grid panels add `DataGridLayoutInfo` to the repository panel schema. These properties control grid presentation, not the repository field list.

### Column presentation

`VisibleColumns` is an optional array containing grid-specific settings for columns that were captured by the grid. Each item contains:

- `Name` — the field/column name;
- `Width` — a numeric pixel width or valid CSS width value;
- `GroupInterval` — date/time grouping interval where supported;
- `SortGroupsBy` — summary column used to order group rows;
- `FullRow` — displays the column as a full row below other cells.

`VisibleColumns` does not make a field visible. Use `RepositoryPanelLayoutInfo.VisibleDataFields` to select and order fields, and use `VisibleColumns` only for additional grid presentation.

```json
{
  "DataGridLayoutInfo": {
    "VisibleColumns": [
      { "Name": "DocumentNo", "Width": 140 },
      { "Name": "DocumentDate", "Width": 120 }
    ]
  }
}
```

The value can be `null` when no grid layout change has been reported. Do not replace `VisibleDataFields` with `VisibleColumns`.

### Sorting, grouping, and interaction

| Property | Meaning |
| --- | --- |
| `SortColumns` | Ordered field names; append ` desc` for descending order. |
| `GroupColumns` | Field names used to group rows. |
| `Summary` | Summary items with `Column`, `SummaryType`, and optional `Name`. |
| `ShowFilterRow` | Shows the grid filter row. |
| `ShowGroupPanel` | Shows the grid grouping panel. |
| `MultiSelect` | Allows selecting multiple rows. |
| `MaxRowCount` | Maximum number of rows loaded. |
| `FullRowColumns` | Field names displayed as full-row columns. |
| `LeftFixedColumns` | Number of columns fixed on the left. |

For grid `SummaryItem.SummaryType`, the supported values are `sum`, `min`, `max`, `avg`, `count`, and `custom`. For `custom`, the optional `Name` identifies the custom summary implementation. `Column.GroupInterval` supports `a`, `y`, `ym`, `ymd`, `ymdh`, `m`, `d`, and `h` where the field type supports grouping.

Example:

```json
{
  "DataGridLayoutInfo": {
    "SortColumns": ["DocumentDate desc"],
    "GroupColumns": ["Customer"],
    "ShowFilterRow": true,
    "MultiSelect": true,
    "LeftFixedColumns": 1
  }
}
```

## Files side-panel properties

The object-files side panel adds `FolderItemsLayoutInfo`. It controls how the files list is presented:

| Property | Meaning |
| --- | --- |
| `ViewType` | File view mode, such as `SmallIcons`, `LargeIcons`, or `Details`. Use the enum values in the live schema. |
| `GroupBySection` | Groups files by section when `true`. |

Example:

```json
{
  "FolderItemsLayoutInfo": {
    "ViewType": 2,
    "GroupBySection": true
  }
}
```

The panel is commonly exposed as the files side panel. Its exact panel name must be read from the form schema.

`ViewType` values mean:

| JSON value | Enum name | Meaning |
| --- | --- | --- |
| `0` | `SmallIcons` | Displays files using small icons. |
| `1` | `LargeIcons` | Displays files using large icons. |
| `2` | `Details` | Displays files in a details/list view. |

## Main-menu form properties

The Forms-namespace main menu is a form whose panels represent repository sections. It uses the same `Form` layout section and `FormLayoutInfo` properties as other forms:

- `VisiblePanels` orders the section panels;
- `VisibleSidePanel` selects a side panel if the menu provides one.

For example:

```json
{
  "Form": {
    "VisiblePanels": [
      "RepositoryMenuSectionPanelDocuments",
      "RepositoryMenuSectionPanelDefinitions",
      "RepositoryMenuFunctionsPanel"
    ]
  }
}
```

The available section names depend on the namespace and repository set. Always use the names returned by `get-layout-schema` for the `mainMenu` request.

## Main-menu section panels and links

`RepositoryMenuSectionPanel` adds a `VisibleItems` array. Each item represents a repository tile in the section and has:

- `Name` — the repository name identifying the tile;
- `VisibleLinks` — the ordered names of links shown inside that tile.

The links are generated from the repository's available actions and related views. A layout can hide or reorder existing links, but it cannot create a new link through this property.

Common generated link-name patterns are:

- `Category<category-code>` for a create link for an entity category;
- `ChildCollection<collection-name>` for a child-collection or related-entity link.

The suffix uses the technical category code or collection name, not the localized caption.

Example:

```json
{
  "RepositoryMenuSectionPanelDocuments": {
    "PanelLayoutInfo": {
      "Expanded": true
    },
    "VisibleItems": [
      {
        "Name": "Crm.Sales.SalesOrders",
        "VisibleLinks": ["Category<category-code>", "ChildCollectionLines"]
      },
      {
        "Name": "Crm.Sales.Customers",
        "VisibleLinks": ["ChildCollectionAddresses"]
      }
    ]
  }
}
```

The actual link names are repository- and feature-dependent. Read the panel layout schema before changing `VisibleLinks`; do not infer a link name from its caption or URL.

When `VisibleItems` is supplied, the listed tiles become the visible tile set. Within a tile, `VisibleLinks` is also an explicit visible set: omitting it or setting it to `null` means that the tile has no visible links. Include the link names you want to keep.

The main-menu functions panel uses a simpler `VisibleItems` value: a comma-separated string of action names. For example:

```json
{
  "RepositoryMenuFunctionsPanel": {
    "VisibleItems": "Fulfillments,NewOrder"
  }
}
```

It is a different panel type and should not be given the section-panel object array.

## Applying focused changes

You can submit only the sections that need to change. For example, this request changes two grid properties while leaving the rest of the form layout unchanged:

```json
{
  "kind": "navigator",
  "repository": "Crm.Sales.SalesOrders",
  "viewMode": "view",
  "layout": {
    "DataGridLayoutInfo": {
      "ShowFilterRow": true,
      "LeftFixedColumns": 1
    }
  },
  "replace": false
}
```

With `replace: false`, the supplied properties are merged with the existing configuration. Use `replace: true` only when the supplied sections are intended to replace the selected layout scope.

## Related topics

- [Web Client concepts](web-client-concepts.md)
- [Web Client registered panels](registered-panels.md)
- [Layout API](layout-api.md)
