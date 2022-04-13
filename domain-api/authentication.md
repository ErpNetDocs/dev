# Authentication

Authentication is needed when an application needs to know the identity of the current user. Typically these applications manage data on behalf of that user and need to make sure that this user can only access the data for which he is allowed. The most common example for that is (classic) web applications – but native and JS-based applications also have a need for authentication.

ERP.net uses OpenID Connect authentication protocol which is an extension of OAuth2.

## API Access
Applications have two fundamental ways with which they communicate with APIs – using the application identity, or delegating the user’s identity. Sometimes both methods need to be combined.

OAuth2 is a protocol that allows applications to request access tokens from a security token service and use them to communicate with APIs. This delegation reduces complexity in both the client applications as well as the APIs since authentication and authorization can be centralized.

## Identity Server
Each ERP.net instance (#erp-instance) has it's own authentication provider site which is **usually** located on https://{ERP-INSTANCE-ROOT-URL}/id. For example the identity server for _demodb_ ERP instance is located at https://demodb.my.erp.net/id.

ERP.net Identity Server is build ot top of [IdentityServer4](https://docs.identityserver.io/) which documentation can be used to understand better the authentication process.

## Terminology

* IdentityServer is an OpenID Connect provider - it implements the OpenID Connect and OAuth 2.0 protocols.
* 
Source: https://docs.identityserver.io/en/latest/intro/terminology.html

## Application Types

The most common interactions are:

 * Browsers communicate with web applications
 * Web applications communicate with web APIs (sometimes on their own, sometimes on behalf of a user)
 * Browser-based applications communicate with web APIs
 * Native applications communicate with web APIs
 * Server-based applications communicate with web APIs
 * Web APIs communicate with web APIs (sometimes on their own, sometimes on behalf of a user)

## Trusted Applications



## Endpoints

The OAuth authentication is 
