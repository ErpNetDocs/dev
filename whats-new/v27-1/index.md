# Version 27.1

- [**One license per user and device**](../../auth/sessions/license-slot.md)  
  A user working in several windows, tabs or applications on the same device now takes a single license slot. Tokens minted for a user carry the browser they were signed in from as the `erpnet_device_id` claim, and the application server counts licenses per user and device. Sessions without a device, such as Client Credentials service sessions, keep occupying a slot each.

- [**Requests are refused with 503 when no license is available**](../../domain-api/data-manipulation/error-handling.md#no-license-available)  
  The Domain API and the Table API answer a request that cannot get a license with `503 Service Unavailable`, a `Retry-After` header and a JSON body naming the problem, instead of a bare `500`. Any other unhandled error in these sites now returns a readable JSON body as well.

- [**User-defined references**](../../domain-api/common-tasks/custom-property-references.md)  
  Stored attributes can now be configured as references to aggregate-root entities and used as normal Domain API navigation properties, including `$expand` and direct `@odata.id` filtering.

- [**Web Client panel extensions**](../../web-client/registering-extension-panels.md)  
  Register extension panels that are displayed inside entity object forms and receive the current form context.

- [**Document Layout API**](../../web-client/layout-api.md)  
  Inspect and manage Web Client form layouts programmatically, including panels, fields, columns, categories and view modes.

- [**External Identifiers**](../../domain-api/data-sync/external-identifiers.md)  
  Use stable, application-owned identifiers to address ERP.net objects across integrations.

- [**Profile Site Endpoints**](../../building-apps/concepts/profile-site-endpoints.md)  
  Use profile endpoints to retrieve information about the authenticated ERP.net user and related API context.

- [**Faster saves on records with attachments and JSON payloads**](../../domain-api/data-sync/additional-data-json.md)  
  Updating metadata costs the same on a record with a 50 MB attachment as on an empty one. Delay-loaded attributes - `AdditionalDataJson`, `EmbeddedFileContents`, `EmbeddedThumbnailContents` - are skipped unless the request sets them, cutting the traffic, the memory and the database work of every such save.

- [**Domain API logout retains the session (ErpLogin)**](../../domain-api/common-tasks/login-sessions.md#logout-and-session-retention)  
  `POST /api/domain/logout` no longer immediately closes the session or frees its license slot. The session is retained and self-releases after 20 minutes of inactivity, and a later `POST /api/domain/login` with the same credentials reuses it - preventing license multiplexing by cycling logout and login.
