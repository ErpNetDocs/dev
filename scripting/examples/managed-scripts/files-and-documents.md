# Managed scripts with files and documents

Set up an active script as described in the [managed-script examples overview](index.md). Each request below is the JSON body for its `Execute` action.

## Update an existing text file

This example reads an embedded text file, appends a default suffix, and writes the changed content back in the current transaction. Assume the file initially contains `Hello`. Set `ScriptText` to:

```js
const file = Action.files.getFileById(args.fileId);
if (file === null)
    throw new Error("File not found.");

const updated = file.readText() + args.suffix;
file.writeText(updated);
args.length = updated.length;
return file.id;
```

Set `ParametersSchema` to:

```json
{
  "parameters": [
    { "name": "fileId", "direction": "Input", "schema": { "type": "string", "format": "uuid" } },
    { "name": "suffix", "direction": "Input", "schema": { "type": "string", "default": "!" } },
    { "name": "length", "direction": "Output", "schema": { "type": "integer", "minimum": 0 } },
    { "name": "result", "direction": "Return", "schema": { "type": "string", "format": "uuid" } }
  ]
}
```

Request body (replace the ID with the existing file's ID):

```json
{ "arguments": { "fileId": "11111111-1111-4111-8111-111111111111" } }
```

Within the current transaction, the file now contains `Hello!`. The relevant response is:

```json
{
  "parameters": { "length": 6 },
  "returnValue": "11111111-1111-4111-8111-111111111111"
}
```

The suffix was omitted from the request, so its declared default was used. [Commit the transaction](../../concepts/transactions-and-persistence.md#commit-a-managed-script-call) to persist the edit. See the [Files SDK](../../files/index.md) for ownership, permissions, and embedded-file limits.

## Create an XLSX report

This example creates a workbook in an existing folder. Set `ScriptText` to:

```js
const workbook = Action.files.xlsx.create({
    folderId: args.folderId,
    name: "Orders.xlsx"
});

const sheet = workbook.sheet("Sheet1");
sheet.setCell("A1", "Orders");
sheet.setCell("B1", args.count);

args.fileId = workbook.file.id;
workbook.save();
workbook.close();
return args.fileId;
```

Set `ParametersSchema` to:

```json
{
  "parameters": [
    { "name": "folderId", "direction": "Input", "schema": { "type": "string", "format": "uuid" } },
    { "name": "count", "direction": "Input", "schema": { "type": "integer", "minimum": 0 } },
    { "name": "fileId", "direction": "Output", "schema": { "type": "string", "format": "uuid" } },
    { "name": "result", "direction": "Return", "schema": { "type": "string", "format": "uuid" } }
  ]
}
```

Request body (use the ID of a folder you can write to):

```json
{ "arguments": { "folderId": "22222222-2222-4222-8222-222222222222", "count": 21 } }
```

The response contains the new file ID in both `parameters.fileId` and `returnValue`. [Commit the transaction](../../concepts/transactions-and-persistence.md#commit-a-managed-script-call) before treating the file as persisted. The saved workbook has `Orders` in cell A1 and `21` in cell B1. See [XLSX workbooks](../../files/xlsx.md) for opening and editing an existing file.

## Create a PDF report

This example creates a one-page PDF in an existing folder. Set `ScriptText` to:

```js
const pdf = Action.files.pdf.create({
    folderId: args.folderId,
    name: "Report.pdf"
});

pdf.title = args.title;
pdf.page(1).drawText(args.title, 40, 60, 14);

args.fileId = pdf.file.id;
pdf.save();
return args.fileId;
```

Set `ParametersSchema` to:

```json
{
  "parameters": [
    { "name": "folderId", "direction": "Input", "schema": { "type": "string", "format": "uuid" } },
    { "name": "title", "direction": "Input", "schema": { "type": "string", "default": "Report" } },
    { "name": "fileId", "direction": "Output", "schema": { "type": "string", "format": "uuid" } },
    { "name": "result", "direction": "Return", "schema": { "type": "string", "format": "uuid" } }
  ]
}
```

Request body (use the ID of a folder you can write to):

```json
{ "arguments": { "folderId": "22222222-2222-4222-8222-222222222222" } }
```

The response contains the generated PDF file ID in both `parameters.fileId` and `returnValue`. [Commit the transaction](../../concepts/transactions-and-persistence.md#commit-a-managed-script-call) before treating the file as persisted. The default title is `Report`. Drawing text requires a suitable font on the host; see [PDF documents](../../files/pdf.md#fonts-and-unicode).

## Attach a text file to a customer

This example combines a Domain entity lookup with the Files SDK. The file belongs to the customer, not to a folder. Set `ScriptText` to:

```js
const customer = Domain.Crm.Sales.CustomersRepository.getById(args.customerId);
if (customer === null)
    throw new Error("Customer not found.");

const file = Action.files.createFile({
    entity: customer,
    name: "note.txt",
    mediaType: "text/plain",
    content: args.text
});

return file.id;
```

Set `ParametersSchema` to:

```json
{
  "parameters": [
    { "name": "customerId", "direction": "Input", "schema": { "type": "string", "format": "uuid" } },
    { "name": "text", "direction": "Input", "schema": { "type": "string" } },
    { "name": "result", "direction": "Return", "schema": { "type": "string", "format": "uuid" } }
  ]
}
```

Request body (replace the ID with the customer's ID):

```json
{ "arguments": { "customerId": "11111111-1111-4111-8111-111111111111", "text": "Call next week." } }
```

`returnValue` is the new file ID and `parameters` is empty. The file's `folderId` is `null`; it is attached to the customer's extensible data object. [Commit the transaction](../../concepts/transactions-and-persistence.md#commit-a-managed-script-call) to persist the attachment. See [Files SDK ownership](../../files/index.md#create-and-edit-files) for the other supported owner forms.
