# Introduction to the @@name developer documentation

Explore the @@name documentation to quickly build great integrations with the platform.

## Authentication

All APIs in @@name use the same authentication mechanism, based on OAuth2.

To understand authentication, read about @@name [Identity](identity/index.md).

## Select an API

The next step is to choose the correct API:

* **[Domain API](domain-api/index.md)** - Object oriented API, based on the [Domain Model](https://docs.erp.net/model/entities/). The main API for processing data by web sites, services and other business logic apps.
* **[Table API](table-api/index.md)** - Limited purpose read-only, fast-forward API based on the raw table data model. Intended for Business Intelligence and backup apps, requiring fast dumping of big quantities of raw data.
* **Data Access API** - Legacy API, exposing table-based methods for retrieving and manipulating data. Not recommended for new developments.
