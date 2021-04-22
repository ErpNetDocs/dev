---
uid: erp-instance
---
# ERP Instance

Each tenant in the @@erpnet infrastructure is called an ERP Instance.

## General info

When a client creates a subscription with the @@erpnet service, they get an ERP Instance.
A single account can create and manage more than one ERP Instance.
This might be required for testing, validation or other purposes.

> [!NOTE]
> A single ERP Instance can manage multiple legal entities (companies).

Under the hood, each ERP Instance is managed as a single database.
For this reason, the terms ERP Instance and database are sometimes used interchangeably.
The proper term is ERP Instance, since this is more abstract and not related to any underlying technology.
The ERP Instances are provided as a service from the @@erpnet infrastructure and the actual storage is undistinguishable to the end user.

## UIN

Each ERP Instance has a Unique Instance Name (UIN) or simply name.
For example, the demonstration database is named "demodb".

## Root URL

The ERP Instances are accessible through their root URL address.
The root URL address is

https://<\<UIN\>>.my.erp.net

For example, the root URL of demodb is:

<https://demodb.my.erp.net>

You can go ahead and click that address.
It opens the home page of the ERP Instance.
For security reasons, it does not give access to much else than some general info and public downloads.

The root URL is usually also used as base address for the instance-related web sites. For example, demodb has several web sites active. One of the sites is the Domain API site, available at:

<https://demodb.my.erp.net/api>

> [!NOTE]
> The web sites can be hosted at custom URL addresses. Using the Root URL is just a default.
