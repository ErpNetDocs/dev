The Domain API is based on the [OData protocol](https://www.odata.org/).
It allows object-oriented access to the data exposed by the @@name domain objects.

Since the Domain API is an HTTP RESTfull API, it can be accessed by simple HTTP requests. 
However there are many [ODATA libraries](https://www.odata.org/libraries/) that can be used. 

## ERP.net libraries
### .NET
[ErpNet.Api.Client](https://github.com/erpnet/ErpNet.Api.Client) - Handles authentication of service clients and work with Domain API requests, both - untyped and typed.

### Javascript
[oidc-client](https://github.com/IdentityModel/oidc-client-js) - Library to provide OpenID Connect (OIDC) and OAuth2 protocol support for client-side, browser-based JavaScript client applications. Also included is support for user session and access token management.

### PHP
[OpenIdConnect client](https://github.com/ErpNetDocs/dev/blob/master/domain-api/samples/src/php/ErpNetClient.php) - A simple library that allows an application to authenticate a user through the basic OpenID Connect flow.
