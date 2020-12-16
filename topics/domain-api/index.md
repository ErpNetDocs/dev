# Domain API Introduction

For an overview and introduction of the Domain API, read the [home page of the developer docs](/index.md#the-domain-api).

## Purpose

The Domain API is the primary API for accessing and manipulating data in ERP.net. 
It is most useful for UI and service apps.
For BI, you should use the Table API.

## REST API
The Domain API is a HTTP REST API, which allows access to the data exposed by the ERP.net objects. 
The API allows access to the data in an object-oriented manner, through a well-known HTTP REST interface, based on the [OData v4](https://www.odata.org/) protocol.

## OData
The OData API is structured along a number of entities that represent the Domain Model of the ERP Instance. 
Each entity contains data attributes, which can be filtered, sorted, etc.
This model also provides information on how to navigate between entities.

## Data
The Domain API allows access to the ERP Instance data. 
Server Management cannot be performed through the Domain API.

All data in a database is accessible through the Domain API, restricted by the access permissions of the login account.

The Domain API is adequate for building productive e-Commerce web sites, data transfer packages and similar apps.
