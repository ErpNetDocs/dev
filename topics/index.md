# Introduction

Explore the @@name developer documentation to quickly learn how to build great integrations with the platform.

## ERP Instances

Each ERP database is an instance.
To learn more, see [ERP Instances](erp-instances.md).

## Choose application type

When designing a new app, the first step is to choose which authentication type and API should be used.
To simplify this, we have compiled a list of common [application types](application-types.md).

## Authentication

All APIs in @@name use the same authentication mechanism, based on OAuth2.

To understand authentication, see [Authentication](authentication/index.md).

## Select API

Choose the correct API:

* **[Domain API](~/domain-api/index.md)** - Object oriented API, based on the [Domain Model](https://docs.erp.net/model/entities/). The main API for processing data by web sites, services and other business logic apps.
* **[Table API](table-api/index.md)** - Limited purpose read-only, fast-forward API based on the raw table data model. Intended for Business Intelligence and backup apps, requiring fast dumping of big quantities of raw data.
* **Data Access API** - Legacy API, exposing table-based methods for retrieving and manipulating data. 
Not documented and not recommended for new developments.
