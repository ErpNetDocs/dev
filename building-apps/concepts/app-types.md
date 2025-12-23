# Application Types

@@name supports several types of applications, depending on:

- where they run
- who develops them
- and how they are distributed

All @@name Apps follow the same security and connectivity model - they communicate with an @@name instance through the built-in **@@name Identity** and **APIs**, and are represented in the system by a **Trusted Application** entity.

## Internal Applications

**Internal applications** are part of the @@name platform itself.  

They are developed, deployed, and maintained by @@name as core components of the system.

These apps are installed within the @@name instance and operate using the same security and authentication infrastructure as external apps.

Examples include:

- **[Web Client](https://docs.erp.net/webclient/)** - the main @@name web interface used by end users to access and manage data across all functional modules
- **[Client Center](https://docs.erp.net/tech/modules/crm/clientcenter/index.html)** - a portal that allows customers and business partners to view documents, orders, and invoices, and to communicate with the company directly
- **Legal** - a built-in app for managing legal entities and compliance information

Characteristics:

- Deployed as part of the @@name instance  
- Fully managed and updated with the platform  
- Use the same APIs and @@name Identity for consistency  
- **Trusted Application registration:** may be preconfigured by the platform or require manual registration/configuration, depending on the app and deployment

## External Applications

**External applications** are developed outside of the @@name platform, by customers, partners, or independent developers.

They run separately from the @@name instance but connect to it securely via @@name Identity and APIs.

External apps can take many forms - mobile clients, integrations, automation tools, or standalone web applications.

Examples include:

- A logistics dashboard that monitors deliveries and shipment statuses in real time
- A custom production planning tool that retrieves and updates manufacturing data
- A cloud integration that syncs @@name data with an accounting or shipping service
- A background service that automates invoice exports

Characteristics:

- Hosted outside of the @@name instance  
- Must be registered as **Trusted Applications** in each instance they connect to
- Authenticate and authorize through the built-in @@name Identity
- Can use interactive (user) or non-interactive (service) access modes

## Marketplace Applications

**Marketplace applications** are a special category of external apps that are published to the [@@name Marketplace](https://marketplace.erp.net/).

They are developed by @@name partners or third parties and made available to multiple tenants for installation or activation.

When a tenant administrator installs a marketplace app, the @@name instance automatically registers it as a Trusted Application.

Examples include:

- **Currency Rates Sync** – Automate your currency updates with fresh ECB rates, daily.  
- **Retail POS** – A modern, offline-capable POS system designed for retail businesses. Fast, intuitive, and built to work seamlessly with @@name.  
- **Preventive Maintenance Planner** – Transform your maintenance operations with intelligent planning.  

Characteristics:

- Developed externally but distributed through the @@name Marketplace  
- Automatically registered and authorized during installation
- Can be activated by tenant administrators
- Must comply with platform validation and security requirements

## Embedded (In-Client) Applications

**Embedded (In-Client) applications** are external web apps that can be displayed directly inside the @@name Web Client using a **WebView**.

They allow developers to create specialized web experiences that look and feel like a native part of @@name, while being hosted and maintained independently.

### Behavior and Access

Embedded apps behave differently depending on where they are hosted:

1. **Apps hosted on a trusted domain**  
   If the app is part of the @@name Marketplace or hosted under the official domain `https://*.app.erp.net`, it can securely access the @@name APIs **without performing explicit authorization**.  
   In this case, the Web Client shares the user's active session (via cookies), and the app can make API calls on behalf of the logged-in user.

2. **Apps hosted elsewhere**  
   If the app is hosted on any other domain, it cannot access @@name data directly.  
   The only information it receives is the **URL passed to the WebView** when it is opened from the Web Client.  
   To access data, such apps must perform standard authentication through @@name Identity using a registered Trusted Application.

### Typical Use Cases

- Partner-developed dashboards or widgets integrated into the @@name Web Client
- Company-specific extensions that enhance standard UI modules
- Marketplace apps that extend @@name visually within the main user experience  

### Characteristics

- Hosted externally but embedded in the Web Client through a WebView
- May use the active user session for authentication (if hosted on trusted domain)  
- Must follow strict domain and security policies to access @@name data
- Can be distributed through the @@name Marketplace or deployed privately

## Comparing Application Types

| Type | Hosted By | Registered As Trusted App | Example | Typical Use |
|------|------------|----------------------------|----------|--------------|
| **Internal** | @@name Platform | Preconfigured or manual (varies) | [Web Client](https://docs.erp.net/webclient/), [Client Center](https://docs.erp.net/tech/modules/crm/clientcenter/index.html) | Core @@name functionality |
| **External** | Customer or Partner | Manual | Custom integration, automation service | Private or custom solutions |
| **Marketplace** | Partner / Third Party | Automatic on installation | Retail POS, Currency Rates Sync, Preventive Maintenance Planner | Public or commercial apps |
| **Embedded** | @@name Platform / Partner / Third Party | Depends on hosting domain | Embedded dashboard, company widget | Embedded web experiences |

---

## Learn More

- **[What Are @@name Apps](what-are-erpnet-apps.md)**  
  Understand the core concept of @@name Apps and how they connect to an @@name instance.

- **[Trusted Applications](trusted-apps.md)**  
  Learn how each app is registered, managed, and granted access within @@name.

- **[Authentication and Authorization](../auth/overview.md)**  
  See how the built-in @@name Identity authenticates and authorizes apps and users.
