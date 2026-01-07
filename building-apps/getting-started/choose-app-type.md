# Choose an Application Type

Before registering your app, identify the **application type** that matches how your app will authenticate users and access @@name APIs.

The application type describes **how API access is performed**:

- on behalf of a signed-in user
- on behalf of the application itself
- or using long-lived automation tokens

This choice determines which setup and authentication guidance applies later.

## Application Type Overview

Choose the row that best describes **how your app identifies users and accesses @@name APIs**:

| App behavior | User involvement | How API access is performed | Application type |
| ------------- | ------------------ | ----------------------------- | ------------------ |
| Browser-based app with no backend; users sign in | Internal users | API access is performed **on behalf of the signed-in user** | **SPA Applications** |
| Web app with backend; users sign in | Internal users | API access is performed **on behalf of the signed-in user** | **Web (Confidential) Applications** |
| Web app where users sign in for identification only | Internal and/or external users | API access is performed **on behalf of the application** | **Web Portals** |
| No user interaction; background integration or sync | None | API access is performed **on behalf of the application** | **Service Applications** |
| Scheduled or scripted jobs using long-lived tokens | None | API access is performed **on behalf of the token** | **Automation** |

**Important notes:**

- External (community) users are authenticated for **identification only** and never receive direct API access.
- When external users are involved, all API calls are performed on behalf of the application.
- If an app supports multiple user types, choose the application type based on **how API access is performed**, not who signs in.

## What This Choice Affects

The selected application type determines:

- which authentication scenario applies
- whether tokens represent a user, an application, or a reference token
- which setup and implementation guides you should follow

You do **not** configure anything at this stage.  
This step is only about choosing the correct documentation path.

## Reporting, Analytics, and Data Export

Reporting, analytics, backups, and bulk exports are **usage patterns**, not separate application types.

They are typically implemented as:

- **Service Applications** for OAuth-based, short-lived access, or
- **Automation** for controlled, long-running or scheduled tasks

API selection (Domain API vs Table API) is handled in the next step.

## Detailed Reference

For a full description of each application type and its expected behavior, see  
[Application Types](../concepts/app-types.md).

## Next Step

Once you have identified the correct application type, continue with  
[Choose the Right API](./choose-right-api.md).
