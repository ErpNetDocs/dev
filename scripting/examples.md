# Managed script examples

These are complete managed-script examples. For each one, create an active `Systems.Core.Script` record, set `ScriptLanguage` to `JavaScript`, copy the JavaScript into `ScriptText`, and copy the JSON into `ParametersSchema`. Give each record its own unique `Code` and `Name`.

Call a script with its record ID:

```http
POST /api/domain/odata/Systems_Core_Scripts(<script-id>)/Execute
Content-Type: application/json
Authorization: Bearer <access-token>

{ "arguments": { ... } }
```

The access token needs the [`exec` scope](security.md), and execution requires the X21 Advanced BPM license. The responses below show only `parameters` and `returnValue`; the actual OData response includes metadata. If you have not used managed scripts before, see [Managed scripts](managed-scripts.md) for the record fields and execution contract.

## Calculate a value and update an argument

This example demonstrates an input, an input/output parameter, an output parameter, and a return value. Set `ScriptText` to:

```js
args.doubled = args.inputValue * 2;
args.counter += 1;
return { success: args.inputValue === 21 };
```

Set `ParametersSchema` to:

```json
{
  "parameters": [
    { "name": "inputValue", "direction": "Input", "schema": { "type": "integer" } },
    { "name": "counter", "direction": "InputOutput", "schema": { "type": "integer" } },
    { "name": "doubled", "direction": "Output", "schema": { "type": "integer" } },
    { "name": "result", "direction": "Return", "schema": { "type": "object" } }
  ]
}
```

Request body:

```json
{ "arguments": { "inputValue": 21, "counter": 4 } }
```

Relevant response:

```json
{
  "parameters": { "counter": 5, "doubled": 42 },
  "returnValue": { "success": true }
}
```

The `Input` value is not repeated in the result. Only `Output` and `InputOutput` values appear under `parameters`.

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

The file now contains `Hello!`. The relevant response is:

```json
{
  "parameters": { "length": 6 },
  "returnValue": "11111111-1111-4111-8111-111111111111"
}
```

The suffix was omitted from the request, so its declared default was used. See the [Files SDK](files/index.md) for ownership, permissions, and embedded-file limits.

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

The response contains the newly created file ID in both `parameters.fileId` and `returnValue`. The workbook has `Orders` in cell A1 and `21` in cell B1. See [XLSX workbooks](files/xlsx.md) for opening and editing an existing file.

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

The response contains the generated PDF file ID in both `parameters.fileId` and `returnValue`. The default title is `Report`. Drawing text requires a suitable font on the host; see [PDF documents](files/pdf.md#fonts-and-unicode).

## Use the Domain Model

Managed scripts can access the Domain Model through the global `Domain` object. Unlike a user business rule, a managed script has no current-entity `subject`; pass the entity ID through `args` and retrieve it from its repository. Return plain values rather than Domain objects. The examples below use customers, but the repository and attributes can be changed for other entities. Normal Domain permissions and transaction rules still apply.

### Read a customer by ID

Assume the customer has number `C-001` and is active. Set `ScriptText` to:

```js
const customer = Domain.Crm.Sales.CustomersRepository.getById(args.customerId);
if (customer === null)
    throw new Error("Customer not found.");

return { number: customer.Number, active: customer.Active };
```

Set `ParametersSchema` to:

```json
{
  "parameters": [
    { "name": "customerId", "direction": "Input", "schema": { "type": "string", "format": "uuid" } },
    { "name": "result", "direction": "Return", "schema": { "type": "object" } }
  ]
}
```

Request body (replace the ID with the customer's ID):

```json
{ "arguments": { "customerId": "11111111-1111-4111-8111-111111111111" } }
```

Relevant response for the assumed customer:

```json
{
  "parameters": {},
  "returnValue": { "number": "C-001", "active": true }
}
```

### Find customers by number prefix

This query fetches at most ten customers. Assume the matching numbers are `C-001` and `C-002`. Set `ScriptText` to:

```js
const customers = Domain.Crm.Sales.CustomersRepository.query(
    { number: { startsWith: args.prefix } },
    { fetch: 10 });

const numbers = [];
for (let i = 0; i < customers.Count; i++)
    numbers.push(customers[i].Number);

return numbers;
```

Set `ParametersSchema` to:

```json
{
  "parameters": [
    { "name": "prefix", "direction": "Input", "schema": { "type": "string", "minLength": 1 } },
    { "name": "result", "direction": "Return", "schema": { "type": "array", "maxItems": 10 } }
  ]
}
```

Request body:

```json
{ "arguments": { "prefix": "C-" } }
```

Relevant response for the assumed matches:

```json
{
  "parameters": {},
  "returnValue": ["C-001", "C-002"]
}
```

The `fetch` option bounds the query; the return array contains only customer numbers, not live Domain objects. Do not assume the sample order unless your query defines an ordering.

### Change a customer's active state

Assume the customer is currently active. Set `ScriptText` to:

```js
const customer = Domain.Crm.Sales.CustomersRepository.getById(args.customerId);
if (customer === null)
    throw new Error("Customer not found.");

const wasActive = customer.Active;
customer.Active = args.active;
return { wasActive, isActive: customer.Active };
```

Set `ParametersSchema` to:

```json
{
  "parameters": [
    { "name": "customerId", "direction": "Input", "schema": { "type": "string", "format": "uuid" } },
    { "name": "active", "direction": "Input", "schema": { "type": "boolean" } },
    { "name": "result", "direction": "Return", "schema": { "type": "object" } }
  ]
}
```

Request body (replace the ID with the customer's ID):

```json
{ "arguments": { "customerId": "11111111-1111-4111-8111-111111111111", "active": false } }
```

Relevant response for an initially active customer:

```json
{
  "parameters": {},
  "returnValue": { "wasActive": true, "isActive": false }
}
```

This changes the customer in the current Domain transaction. The change persists only when the calling transaction commits; the script does not commit it itself.

### Attach a text file to a customer

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

`returnValue` is the new file ID and `parameters` is empty. The file's `folderId` is `null`; it is attached to the customer's extensible data object. See [Files SDK ownership](files/index.md#create-and-edit-files) for the other supported owner forms.
