---
title: Developing for the Marketplace
description: Guidance for building and integrating applications with ERP.net Marketplace.
---

# Developing for the Marketplace

## Overview

The **@@name Marketplace** acts as a centralized hub for administrators to discover and install external applications into their @@name instances.  

When a user installs or uninstalls your application via the Marketplace, the underlying setup is handled by the @@name Instance Manager. However, unlike direct manual installations, the Marketplace flow acts as a broker and automatically triggers a secure webhook to your application.  

> [!NOTE]
> Processing this onboarding callback is completely **optional**.
> You only need to implement a webhook listener if your application requires automated deployment acknowledgement or needs to securely receive generated credentials.

Related topics:

- [Automating installation](../auth/automating-installation.md)
- [Trusted Applications (configuration)](../auth/configuration/trusted-apps-access.md)
- [App Lifecycle Events](app-lifecycle-events.md)

---

## Getting Started

### Prerequisites

To list your application in the @@name Marketplace, you need:

- A registered developer account and application entry in the @@name Marketplace portal.

### Handling App Lifecycle Events (Optional)

If your application requires automated deployment acknowledgement or needs to securely receive dynamically generated credentials (such as a `clientSecret` or `referenceToken`), you can configure an endpoint to listen for onboarding callbacks.

**[Read the detailed guide on App Lifecycle Events](app-lifecycle-events.md)** to learn about the Marketplace flow, webhook payloads, and cryptographic signature validation.
