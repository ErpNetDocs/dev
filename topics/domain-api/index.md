# Domain API Introduction

For an overview and introduction of the Domain API, read the thorough presentation at the [home page of the developer docs](~/index.md#the-domain-api).

## Description

The Domain API is the primary API for accessing and manipulating data in @@name.
It is most useful for UI and service apps.
For BI, you should use the Table API.

## OData

The Domain API is based on the [OData protocol](https://www.odata.org/).
It allows object-oriented access to the data exposed by the @@name domain objects.

The OData API is structured along a number of entity types (called "repositories" in @@name), that represent the Domain Model of the ERP Instance.
Each entity type contains data attributes, which can be filtered, sorted, etc.
The model also provides information on how to navigate between the repositories.

For a nice introduction to OData, click [here](https://www.odata.org/getting-started/basic-tutorial/).

## Step by step

1. [URL components](url-components.md) - OData is heavy on the URL. Read this topic to understand the structure of the URL.
1. [Query options](query-options/index.md) - read more about the supported OData query constructs like $filter, $top and similar.
1. [Query Builder](query-builder.md) - the visual query designer can help you easily build complex $select/$expand queries.
1. [Complex values](complex-values.md) - @@name utilizes several OData complex values, including quantity, amount and multi-language string.
1. [Transactions](transactions.md) - @@name supports server-side front-end transactions.
1. [Working with documents](working-with-documents.md) - working with documents is a common scenario when using the API.
1. [Batch requests](batch-requests.md) - @@name supports batch requests, saving round-trips to the server.
