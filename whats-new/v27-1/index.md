# Version 27.1

- [**Faster saves on records with attachments and JSON payloads**](../../domain-api/data-sync/additional-data-json.md)  
  Updating metadata costs the same on a record with a 50 MB attachment as on an empty one. Delay-loaded attributes - `AdditionalDataJson`, `EmbeddedFileContents`, `EmbeddedThumbnailContents` - are skipped unless the request sets them, cutting the traffic, the memory and the database work of every such save.

- [**Domain API logout retains the session (ErpLogin)**](../../domain-api/common-tasks/login-sessions.md#logout-and-session-retention)  
  `POST /api/domain/logout` no longer immediately closes the session or frees its license slot. The session is retained and self-releases after 20 minutes of inactivity, and a later `POST /api/domain/login` with the same credentials reuses it - preventing license multiplexing by cycling logout and login.
