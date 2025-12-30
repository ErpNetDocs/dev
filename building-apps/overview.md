# Overview

@@name Apps allow developers to extend, integrate, and automate @@name systems using the @@name APIs.  

They make it possible to build new user experiences, connect @@name with other software, and streamline business processes.

Every app - whether internal, external, or marketplace - connects to @@name securely through its built-in **@@name Identity** and is represented in the system by a **Trusted Application** entity.  

Together, these components ensure that every app is properly identified, authenticated, and authorized with appropriate access.

## What Are @@name Apps

An *@@name App* is any application, service, or integration that communicates with an @@name instance through its public APIs.  

Apps can range from small automation scripts to complete web or mobile solutions built around @@name data and business logic.

Examples of @@name Apps include:

- A web store that allows customers to browse products and place orders online
- A background integration that synchronizes stock levels with an online store
- A web portal that allows suppliers to update product information
- An analytics service that extracts and processes ERP data for reporting

Apps can be developed by in-house teams, partners, or independent developers and can operate in private environments or be distributed through the [@@name Marketplace](https://marketplace.erp.net/).

## Building Apps for @@name

The **Building Apps** section is your complete guide to developing secure, modern, and scalable applications for @@name.

@@name provides a unified platform where apps can **connect**, **authenticate**, and **extend** the system safely - whether they are internal extensions, web apps, background services, or external integrations.

## What You Can Build

You can build:

- **Interactive web or desktop apps** – for users who sign in and work with @@name data.  
- **Service or background integrations** – automated jobs that sync or process data.  
- **Hybrid apps** – combined solutions with both user-facing and background components.  
- **BI dashboards and connectors** – systems that analyze @@name data through APIs.

Every app connects securely through the @@name **Identity** and uses **tokens** to access the **Domain API** or **Table API**.

## Why Build Apps

Apps extend the value of @@name beyond its core modules.

They allow organizations to:

- Tailor @@name to their unique business workflows
- Integrate with external platforms and internal systems
- Automate routine operations and data flows
- Deliver focused experiences for specific users or roles

By connecting securely to an @@name instance, apps help organizations innovate without altering the base system.

## How Apps Work with an @@name Instance

Each app communicates with an @@name instance through standard APIs that are part of the instance itself.

Before it can do so, the app must be recognized and granted the right level of access.

This is handled by two core components of the platform:

- The **@@name Identity**, which authenticates users and services and enforces authorization decisions based on instance policies
- The **Trusted Application**, which represents the app's registration and configuration inside the instance

When an app needs to access data, it initiates authentication with the @@name Identity (either interactively through a user or non-interactively as a service).

After successful authentication and authorization, the app calls the APIs within the instance to read or update @@name data in line with its allowed permissions.

```mermaid
flowchart LR
  user([User or Service]) --- app([ERP.net App])

  subgraph "ERP.net Instance"
    direction TB
    idp([ERP.net Identity])
    api([ERP.net APIs])
    data([ERP.net Data])
    api --> data
  end

  app -->|Auth| idp
  app -->|API Calls| api
```

## Apps as Part of the @@name Platform

@@name treats every app as a managed, auditable part of the system.

Administrators can see which apps are trusted, what they are authorized to access, and how they interact with the instance.

They can also revoke or adjust access at any time, maintaining full control over integrations.

This model allows @@name Apps to be open, extensible, and secure - forming a flexible ecosystem where developers can build new solutions without compromising platform integrity.

## Sections Overview

[**Authentication and Authorization**](./auth/overview.md)  
Learn how apps sign in, request tokens, and interact securely with the @@name Identity using OAuth 2.0.

[**API Access**](./concepts/api-access/overview.md)  
Discover how to access and manipulate @@name data through the Domain API and Table API, including security and performance best practices.

[**Sessions and Licensing**](./auth/sessions/overview.md)  
See how sessions are created, how they consume licenses, and how to manage compliance.

[**Samples**](./auth/concepts/samples/index.md)  
Practical examples showing how to authenticate, get tokens, and call APIs from different app types.

[**Security Best Practices**](./auth/concepts/security-best-practices.md)  
Essential recommendations for keeping your integrations secure, compliant, and maintainable.

## Before You Start

1. Make sure your @@name instance is accessible.  
2. Register your app as a **Trusted Application** in that instance.  
3. Choose the proper **authentication flow** for your app type.  
4. Start testing API calls using tools like **Postman** or **curl**.

---

## Learn More

- **[What Are @@name Apps](./concepts/what-are-erpnet-apps.md)**  
  Understand what @@name Apps are and how they connect to an ERP.net instance.

- **[Application Types](./concepts/app-types.md)**  
  Explore the different kinds of @@name Apps - internal, external, and marketplace.

- **[Authentication and Authorization](./auth/overview.md)**  
  See how the built-in @@name Identity authenticates and authorizes apps and users.
  