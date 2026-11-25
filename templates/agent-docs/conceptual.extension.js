exports.postTransform = function (model) {
  var markdownFile = model._path.split('/').pop().replace(/\.html$/, '.md');
  var markdownHref = encodeURIComponent(markdownFile);
  var indexHref = model._rel + 'llms.txt';

  var links = '<p><a rel="alternate" type="text/markdown" href="' + markdownHref +
    '">View as Markdown</a> | <a rel="describedby" href="' + indexHref +
    '">Documentation index</a></p>';

  // DocFX includes article text in the search index.
  var footer = model._appFooter || '<span>Made with <a href="https://dotnet.github.io/docfx">docfx</a></span>';
  model._appFooter = links + footer;

  return model;
};
