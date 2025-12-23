# Overview

@@name provides multiple identity integration options, depending on **where your application runs**, **whether it can keep a secret**, and **which @@name instance it targets**.

This section gives a high-level overview of the available identity models and helps you choose the correct approach for your application. Each model is described in detail in its own topic.

## Identity providers in @@name

@@name exposes two distinct identity concepts:

- **ERP.net ID (Global)**  
  A global identity provider, shared across all @@name instances.  
  It is typically used by **server-side confidential applications** that integrate with @@name at a global level.

- **Instance ID**  
  An identity provider bound to a **specific @@name instance** (for example, `https://mycompany.erp.net/id`).  
  It is used by applications that authenticate users **against a concrete instance** and act on behalf of those users.

Understanding which identity provider you need is the first and most important decision when integrating.

## Application types

Applications are classified by two main characteristics:

- **Execution environment**
  - Browser-based (SPA)
  - Server-side (backend)
- **Client type**
  - **Public**: cannot securely store a secret
  - **Confidential**: can securely store a secret

These characteristics determine which authentication flow and which identity provider should be used.

## Available integration scenarios

The following topics cover the supported scenarios:

### ERP.net ID (Global) for Confidential Applications

Use this when:

- Your application has a backend
- It can securely store a client secret
- It integrates with @@name at a **global** level

### Instance ID for SPA (Public) Applications

Use this when:

- Your application is a **Single Page Application**
- It runs entirely in the browser
- It cannot keep a secret
- It authenticates users against a **specific @@name instance**
- It uses **Authorization Code flow with PKCE**

### Instance ID for Confidential Applications

Use this when:

- Your application has a backend
- It can securely store a secret
- It authenticates users against a **specific @@name instance**
- It uses **Authorization Code flow**

### SPA Embedded in Web Client (iframe)

Use this when:

- Your SPA is embedded inside the @@name Web Client
- Authentication is constrained by iframe and browser security rules
- Special handling is required to initiate sign-in flows

### Service Application (Public)

Use this when:

- Your application runs without user interaction
- It acts as a technical client or service
- It does not provide UI
- It requires limited, predefined access

## Choosing the right topic

If you are unsure where to start:

- **Browser-only app?** - *Instance ID for SPA (Public) Applications*
- **Backend app targeting one instance?** - *Instance ID for Confidential Applications*
- **Backend app targeting @@name globally?** - *ERP.net ID (Global) for Confidential Applications*
- **No UI, no users?** - *Service Application (Public)*

Each topic builds on this overview and provides concrete HTTP examples and configuration guidance.
