The includes/erpnet.md file exists for the sole reason of containing the proper writing of "ERP.net".
It is intended to be included inline in the text of other topics.
Unfortunately, DocFX seems to handle the matter of inline-inclusion with a pretty heavy-weight syntax.

Example:
1. I can easily create a link to the @erpnet topic. But this is a link. We need just to include the file contents.
2. I can include the file contents, but with a pretty heavy syntax [!include[](~/includes/erpnet.md)]. This syntax is not adequeate for everyday writing.
3. Ideally, the following syntax should include the text inline:
This is the @@erpnet service!
