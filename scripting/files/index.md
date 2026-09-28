# Files SDK

`Action.files` provides transaction-bound access to ERP.net folders and attached files. It is available to scripts through the global `Action` object; there is no library to import. Use it to look up resources, query bounded pages, or create and modify embedded files.

Files are `General.Files.ObjectFile` records attached through an extensible data object. A folder is one possible owner; an aggregate-root Domain entity can also own a file. `folderId` is therefore not a universal file location.

## Look up and query resources

| Method | Purpose |
| --- | --- |
| `Action.files.getFileById(fileId)` | Return a file, or `null` when it is not found. |
| `Action.files.getFolderById(folderId)` | Return a folder, or `null` when it is not found. |
| `Action.files.list(options)` | List direct resources in a folder; root folders when `type` is `folder` and `folderId` is omitted. |
| `Action.files.find(options)` | Search files or folders by name and supported metadata, optionally within an owner. |

`list` and `find` require `type: "file"` or `type: "folder"`. Common options are `name` (substring), `offset` (zero-based), and `limit` (default 100, maximum 1,000). Files also support `mediaType`; `list` with `type: "file"` requires `folderId`. A search without an owner can include both folder files and entity attachments visible to the current transaction.

Each query returns `{ items, offset, limit, hasMore }`. For example, process a folder one page at a time:

```js
const folderId = "11111111-1111-4111-8111-111111111111";
let offset = 0;
let page;
do {
    page = Action.files.list({ type: "file", folderId, offset, limit: 50 });
    for (const file of page.items)
        Action.log(file.name);
    offset += page.items.Count;
} while (page.hasMore);
```

Replace the sample `folderId` with one obtained from the current script context. In a managed script, it could be declared in the [parameter schema](../parameters.md); in a business rule, it could come from `subject`. Query results are ordered by name and ID for stable paging; if files change while paging, repeat the search when a stable snapshot is needed.

To find files across accessible owners, or narrow a search to one entity:

```js
const allReports = Action.files.find({ type: "file", name: "report", limit: 20 });
const customerReports = Action.files.find({
    type: "file",
    entityType: "Crm.Sales.Customers",
    entityId: "11111111-1111-4111-8111-111111111111",
    name: "report", mediaType: "application/pdf", limit: 20
});
```

`entityType` and `entityId` must be supplied together. The same owner choices described below apply to file searches.

## Create and edit files

`createFile` requires a name and **exactly one** owner selector:

| Owner selector | Value |
| --- | --- |
| `folderId` | Folder identifier. |
| `folder` | Folder returned by the SDK or a Domain `Folder` object. |
| `entity` | An aggregate-root Domain object in the current transaction. |
| `entityType` and `entityId` | Entity name and identifier together. |

An entity owner does not imply a folder. When there is no folder, `file.folderId` is `null`. For example:

```js
const folder = Action.files.createFolder({ name: "Exports" });
const file = Action.files.createFile({
    folder,
    name: "result.txt",
    mediaType: "text/plain",
    content: "First result"
});
file.writeText("Updated result");
const createdFileId = file.id;
```

In a user business rule whose `subject` is an aggregate-root entity, an attachment can instead use `entity: subject`. A managed script has no automatic `subject` global; resolve its intended owner from declared inputs or use a folder.

`file.readText()` and `file.writeText(text)` use UTF-8. `readBytes()` and `writeBytes(bytes)` work with binary content; JavaScript byte arrays such as `[0, 1, 255]` are accepted on write. Files also support `rename(name)`, `moveTo(folderId)`, and `delete()`. Folders support `rename(name)`, `moveTo(parentFolderId)`, and `delete()`. `createFolder({ name, parentFolderId })` creates a root folder when the parent is omitted.

Content operations support **embedded files**. A linked file may appear in metadata results, but reading or writing its content through this SDK is not supported. Reads and writes obey the effective file-size limit; managed scripts may tighten it through [execution settings](../execution-settings.md). Changes belong to the current Domain transaction; a script does not commit them by calling `writeText()` or `createFile()`.

For native document processing, continue with [XLSX workbooks](xlsx.md) and [PDF documents](pdf.md).
