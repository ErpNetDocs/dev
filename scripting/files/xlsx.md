# XLSX workbooks

Use `Action.files.xlsx` to create or edit an embedded `.xlsx` file. It is part of the Files SDK; scripts do not import an Excel library. A new workbook contains a `Sheet1` worksheet and must have a [file owner](index.md#create-and-edit-files). The `folderId` and `fileId` values below are obtained from the script's own context; neither is a predefined global.

## Create a workbook

For example, create a workbook in an existing folder:

```js
const workbook = Action.files.xlsx.create({
    folderId,
    name: "SalesReport.xlsx"
});
const sheet = workbook.sheet("Sheet1");
sheet.setCell("A1", "Sales report");
sheet.setCell("A2", "Orders");
sheet.setCell("B2", 21);
workbook.addSheet("Summary").setCell("A1", true);

const fileId = workbook.file.id;
workbook.save();
workbook.close();
```

`xlsx.create` also accepts `folder`, `entity`, or `entityType` plus `entityId` instead of `folderId`. An optional `sheetName` sets the initial worksheet name. The file receives the XLSX media type automatically.

## Read or modify an existing workbook

```js
const workbook = Action.files.xlsx.open(fileId);
const sheet = workbook.sheet("Sheet1");
const previous = sheet.getCell("B2");
sheet.setCell("B2", previous + 1);
workbook.save();
workbook.close();
```

Cells use A1 addresses. `getCell` returns a string, number, Boolean, or `null` for an empty cell. `setCell` accepts a string, finite number, Boolean, or `null` to clear a cell. A cached formula result can be read, but this API does **not** calculate formulas; date-formatted numeric values are returned as Excel serial numbers.

`save()` writes the workbook back to its attached file in the current transaction. `close()` releases it; closing an edited workbook without `save()` discards those edits. Creating a workbook already creates a valid initial file, but later cell changes still need `save()`. The file must be embedded, have an `.xlsx` name, and fit the effective file-size limit. A managed script may declare `folderId` or `fileId` in its [parameter schema](../parameters.md), tighten the limit through [execution settings](../execution-settings.md), and return the saved file ID; see the [complete example](../examples.md#create-an-xlsx-report).
