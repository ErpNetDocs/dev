# Service Access for Confidential Applications

This topic describes how a **confidential external application with no user interaction** integrates with @@name Instance ID to obtain an access token for calling instance APIs.

The following scenario is covered:

- The external application is a **confidential application**
- The application **has a backend component capable** of securely storing a secret
- The application does **not** present a user interface
- The application authenticates as a **service identity**
- The application uses the **Client Credentials** authorization flow
- The application obtains an access token to call instance APIs directly

The Instance ID is the identity provider of a specific @@name instance.  
Service applications use it to authenticate and obtain access tokens without user involvement, scoped to the permissions configured for the trusted application.

For example, if your @@name instance is located at:

```text
https://demodb.my.erp.net
```

then the corresponding Instance ID endpoints are located at:

```text
https://demodb.my.erp.net/id
```