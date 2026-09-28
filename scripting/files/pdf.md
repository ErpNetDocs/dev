# PDF documents

Use `Action.files.pdf` to create or modify an embedded `.pdf` file without importing a PDF library. A new document starts with one A4 page and must have a [file owner](index.md#create-and-edit-files). The IDs in these snippets must come from the script's own context; they are not predefined globals.

## Create a document with text and shapes

For example, create a PDF in an existing folder:

```js
const pdf = Action.files.pdf.create({ folderId, name: "Report.pdf" });
pdf.title = "Report";
const page = pdf.page(1);
page.drawText("Hello, ERP.net", 40, 60, 14);
page.drawLine(40, 70, 250, 70);
page.drawRectangle(40, 90, 180, 40);

const fileId = pdf.file.id;
pdf.save();
```

Page numbers are **one-based**. Coordinates and page dimensions are in points; the text coordinates specify the baseline. `pdf.addPage()` adds a blank A4 page. Document metadata includes `title`, `author`, and `subject`.

## Modify and combine documents

```js
const pdf = Action.files.pdf.open(targetFileId);
pdf.appendPages(sourceFileId);
pdf.movePage(pdf.pageCount, 1);
pdf.save();
```

You can also call `removePage(number)`, `page(number).drawImage(imageBytes, x, y, width, height)`, and inspect `pageCount`, `page(number).width`, and `page(number).height`. For an image already stored in ERP.net, while the PDF is still open:

```js
const pdf = Action.files.pdf.open(targetFileId);
const image = Action.files.getFileById(imageFileId);
if (image === null)
    throw new Error("Image file not found.");
pdf.page(1).drawImage(image.readBytes(), 40, 120, 100, 60);
pdf.save();
```

`save()` writes the file in the current transaction and closes the document; it does not commit the transaction. `close()` discards unsaved edits. Creating a PDF already stores a valid initial one-page file in that transaction. Opened files must be embedded `.pdf` files, and password-protected PDFs are not supported. Processed content is subject to the effective file-size limit. A managed script can tighten it through [execution settings](../configuration/execution-settings.md) and use declared IDs; see the [complete example](../examples/managed-scripts/files-and-documents.md#create-a-pdf-report) and [commit sequence](../concepts/transactions-and-persistence.md#commit-a-managed-script-call).

## Fonts and Unicode

PDF text uses a compatible font installed on the Windows or Linux host. The scripting SDK does not bundle a font or accept a font file from the script. If the host has no suitable font, a requested glyph is missing, or PDFsharp's process-wide font resolver has already been configured incompatibly, drawing text fails explicitly. Deploy a suitable host font when scripts need a particular language, including Bulgarian text.
