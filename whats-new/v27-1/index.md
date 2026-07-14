# Version 27.1

- [**Domain API logout retains the session (ErpLogin)**](../../domain-api/common-tasks/login-sessions.md#logout-and-session-retention)  
  `POST /api/domain/logout` no longer immediately closes the session or frees its license slot. The session is retained and self-releases after 20 minutes of inactivity, and a later `POST /api/domain/login` with the same credentials reuses it - preventing license multiplexing by cycling logout and login.
