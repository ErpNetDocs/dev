# Building Apps for @@name

The **Building Apps** section is your complete guide to developing secure, modern, and scalable applications for @@name.

@@name provides a unified platform where apps can **connect**, **authenticate**, and **extend** the system safely - whether they are internal extensions, web apps, background services, or external integrations.

## What You Can Build

You can build:

- **Interactive web or desktop apps** – for users who sign in and work with @@name data.  
- **Service or background integrations** – automated jobs that sync or process data.  
- **Hybrid apps** – combined solutions with both user-facing and background components.  
- **BI dashboards and connectors** – systems that analyze @@name data through APIs.

Every app connects securely through the @@name **Identity Server** and uses **tokens** to access the **Domain API** or **Table API**.

## Sections Overview

[**Concepts**](./concepts/overview.md)  
Understand what @@name apps are, how they are classified, and how trusted applications manage access and lifecycle.

[**Authentication and Authorization**](./auth/overview.md)  
Learn how apps sign in, request tokens, and interact securely with the Identity Server using OAuth 2.0.

[**API Access**](./api-access/overview.md)  
Discover how to access and manipulate @@name data through the Domain API and Table API, including security and performance best practices.

[**Sessions and Licensing**](./auth/sessions/overview.md)  
See how sessions are created, how they consume licenses, and how to manage compliance.

[**Samples**](./auth/samples/index.md)  
Practical examples showing how to authenticate, get tokens, and call APIs from different app types.

[**Security Best Practices**](./auth/security-best-practices.md)  
Essential recommendations for keeping your integrations secure, compliant, and maintainable.

## Before You Start

1. Make sure your @@name instance is accessible.  
2. Register your app as a **Trusted Application** in that instance.  
3. Choose the proper **authentication flow** for your app type.  
4. Start testing API calls using tools like **Postman** or **curl**.

> [!NOTE]  
> If you're new to @@name development, begin with the  
> [**Concepts - Overview**](concepts/overview.md) page to understand how apps fit into the @@name ecosystem.

---

## Learn More

- [**Domain API**](../domain-api/index.md)  
  Learn how to query and manipulate business data programmatically.

- [**Table API**](../table-api/index.md)  
  Build fast BI connectors with streaming data access.
