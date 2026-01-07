# Prerequisites

This page lists what you must have in place before starting the **Getting Started** flow.

If any of these are missing, stop here and resolve them first.

## Target @@name Instance

You need the base URL of the @@name instance you will connect to, for example:

- `https://{instance}.my.erp.net`

All API calls and authentication requests are scoped to a specific instance.

## App Scenario

You should know whether your app operates:

- **Interactively** (users sign in)
- **Non-interactively** (service, automation, or background integration)

This determines how the app authenticates and which setup steps apply later.

## Required Access

To proceed, you must be able to:

- Register (or request registration of) a **Trusted Application** in the target instance
- Use (or request) the required **scopes** for your integration

If you do not have this access, coordinate with the instance administrator before continuing.

## Tools

You need a way to send HTTP requests to validate authentication and API access. Any of the following are sufficient:

- curl
- Postman
- Your application code

## Common Pitfalls

Avoid the following:

- Mixing instance-specific and global identity authorities
- Guessing API endpoints instead of using documented ones
- Requesting overly broad permissions instead of the minimum required scopes
