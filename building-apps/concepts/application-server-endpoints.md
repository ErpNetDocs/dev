# Application Server Endpoints

Each @@name instance is represented by an **application server**, which is the central component of the platform.  

It provides access to essential runtime information, configuration details, and basic diagnostic tools through a small set of root-level HTTP endpoints.

These endpoints are exposed directly under the instance root and are designed for discovery, monitoring, and administrative purposes.  

They operate independently of the hosted sites such as @@name Identity, Web Client, Client Center, Domain API, Table API.

The base URL of an instance looks like this:

```http
https://<my-instance>.my.erp.net
```

From this address, you can access a small set of public tools and information endpoints.  

They are primarily designed for system discovery, health monitoring, and administration utilities.

## Instance Landing Page - `/`

Displays the default landing page with basic system information.

**Returns:**

- Server version
- "What's new" link
- A list of available sites (Web Client, APIs, etc.)

**Example:**

```http
GET https://<my-instance>.my.erp.net/
```

## Site Auto-Discovery - `/tools/auto-discovery`

Returns a JSON document that describes the current instance and the publicly exposed sites.

```http
GET https://<my-instance>.my.erp.net/tools/auto-discovery
```

**Response model (example):**

```json
{
  "Host": "string",                 // Instance host name
  "DbName": "string|null",          // Database name (if available)
  "WebSites": [                     // Public sites for this instance
    {
      "Type": "DomainAPI",          // See WebsiteType enum
      "Status": "Working",          // See WebsiteStatus enum
      "StatusDescription": "string",
      "Url": "https://.../api",     // Absolute URL
      "System": false,              // true for system sites (e.g., ID)
      "AdditionalProperties": {     // Optional, site-specific extras
        "ODataServiceRoot": "https://.../api/domain/odata/" // Only for DomainAPI
      }
    }
  ]
}
```

### Properties

| Field | Type | Notes |
|-------|------|-------|
| **Host** | `string` | Instance host name. |
| **DbName** | `string \| null` | Database name, if known. |
| **WebSites** | `AutoDiscoveryWebSite[]` | List of publicly exposed sites. |

### AutoDiscoveryWebSite

| Field | Type | Notes |
|-------|------|-------|
| **Type** | `WebsiteType` | Type of site. |
| **Status** | `WebsiteStatus` | Operational state of the site:<br>• **Unknown** status not yet determined<br>• **Working** site responds normally<br>• **NotWorking** site is unreachable or unresponsive |
| **StatusDescription** | `string` | Optional human-readable status detail. |
| **Url** | `string` (absolute URL) | Root URL of the site. |
| **System** | `boolean` | `true` for system sites (e.g., Identity). |
| **AdditionalProperties** | `Dictionary<string,string>` | Optional site-specific extras. For `DomainAPI`, includes `ODataServiceRoot` pointing to the OData service root (`<domain-api-url>/domain/odata/`). |

## Server Info & Downloads - `/tools/server-info`

```http
GET https://<my-instance>.my.erp.net/tools/server-info
```

**Response (example):**

```json
{
  "version": "26.1.0",
  "downloads": {
    "whatsNew": "https://<my-instance>.my.erp.net/sys/downloads/ErpNet_Whats_New.txt",
    "winClient": "https://<my-instance>.my.erp.net/sys/downloads/ErpNet.WinClient.Setup.x86.msi",
    "posClient": "https://<my-instance>.my.erp.net/sys/downloads/EnterpriseOne.Pos.Setup.x86.msi",
    "winClient64": "https://<my-instance>.my.erp.net/sys/downloads/ErpNet.WinClient.Setup.x64.msi",
    "posClient64": "https://<my-instance>.my.erp.net/sys/downloads/EnterpriseOne.Pos.Setup.x64.msi"
  },
  "internalApi": {
    "useHttpCompression": true
  }
}
```

## Resource Monitor - `/tools/resource-monitor`

Shows live resource usage statistics such as CPU and memory consumption.

**Response:** `application/json`

**Example:**

```http
GET https://<my-instance>.my.erp.net/tools/resource-monitor
```

**Response (example):**

```json
{
  "cpuUsage": 7.12,
  "processorCount": 8,
  "availableMemoryMiB": 24567.3,
  "usedMemoryMiB": 1234.8,
  "committedMemoryMiB": 13802.6,
  "totalMemoryMiB": 25802.1
}
```

## Current instance version - `/tools/ver`

Returns the current @@name instance version.

**Example:**

```http
GET https://<my-instance>.my.erp.net/tools/ver
```

**Response:**

```plain
26.2.0.76
```

## SHA-256 Utility - `/tools/sha256`

Generates a SHA-256 hash from a given string and returns it as Base64.

**Query parameters:**

- `secret` - The input string to hash

**Example:**

```http
GET https://<my-instance>.my.erp.net/tools/sha256?secret=DEMO
```

**Response:**

```json
{ "sha256": "dZ4Y...==" }
```
