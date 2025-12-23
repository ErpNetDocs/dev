# Glossary and Key Terms

This glossary explains common terms and concepts used throughout the @@name developer documentation.

---

### Access Token

A short-lived token issued by @@name Identity that authorizes API requests.  
Used to prove that an app or user has permission to access @@name resources.

---

### Refresh Token

A long-lived token used to obtain new access tokens without re-authenticating.  
Typically valid for 30 days.

---

### Reference Access Token

Also known as a **Personal Access Token (PAT)** or **Service Access Token (SAT)**.  
A manually generated, long-lived token stored on the server and represented by an opaque value (e.g. `enrt_...`).

---

### Trusted Application

A registered app in @@name that defines how it authenticates, which users can issue tokens for it, and what **System User** it runs as.

---

### System User

A dedicated internal user account used by background or service applications.  

---

### Scope

A permission boundary embedded in tokens.  
Determines what operations (e.g. read, update) the token holder can perform.

---

### Session

A live connection between a token and an @@name instance.  
Sessions consume licenses and are automatically closed after inactivity.

---

### License Slot

Represents one concurrent session in @@name.  
Each active session consumes a license slot until it ends or times out.

---

### ERP.net Identity

The built-in @@name authentication authority that handles sign-in, issues tokens, and validates access for all apps.

---

### OAuth 2.0

The industry-standard protocol used for authentication and authorization in @@name.  
Defines flows like **Authorization Code** and **Client Credentials**.

---

### Authorization Code Flow

Used by interactive applications (web or desktop) where a user signs in and authorizes the app to act on their behalf.

---

### Client Credentials Flow

Used by service or background applications that act as themselves rather than a user.  
Authenticates via client ID and secret.

---

### Hybrid App

An application that combines both user-facing and service components.  
Uses both Authorization Code and Client Credentials flows.

---

### Instance Manager

The administrative interface for managing trusted apps, users, and licenses for an @@name instance.

---

### Profile Site

A self-service web interface where users can manage their profile, passwords, and personal tokens (PATs).

---

### Domain API

The main OData-based API for @@name business objects - ideal for apps and services that need structured, relational data access.

---

### Table API

An OData-based, table-oriented API optimized for BI tools and large data extractions.  
Supports **streaming** for efficient large dataset transfers.

---

### Reference

For a deeper understanding of these terms, explore:

- [Authentication and Authorization](./auth/overview.md)
- [Tokens](./auth/concepts/tokens/tokens-overview.md)
- [Sessions](./auth/concepts/sessions/overview.md)
- [API Access](./api-access/overview.md)
