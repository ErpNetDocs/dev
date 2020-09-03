The includes/erpnet.md file exists for the sole reason of containing the proper writing of "ERP.net".
It is intended to be included inline in the text of other topics.
Unfortunately, DocFX seems to handle the matter of inline-inclusion with a pretty heavy-weight syntax.

Example:
1. I can easily create a link to the @erpnet topic. But this is a link. We need just to include the file contents.
For more information:
<https://dotnet.github.io/docfx/tutorial/links_and_cross_references.html#shorthand-form>

2. I can include the file contents, but with a pretty heavy syntax [!include[erpnet](~/includes/erpnet.md)]. This syntax is not adequeate for everyday writing.
For more information:
<https://dotnet.github.io/docfx/spec/docfx_flavored_markdown.html#inline>

3. Ideally, the following syntax should include the text inline:
This is the @@erpnet service!

The above line should render as:

This is the ERP.net service!
