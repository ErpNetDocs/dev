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

* **IdentityServer** - IdentityServer is an OpenID Connect provider - it implements the OpenID Connect and OAuth 2.0 protocols.
* **User** - A user is a human that is using a registered client to access resources.
* **Client** - A client is a piece of software that requests tokens from IdentityServer - either for authenticating a user (requesting an identity token) or for accessing a resource (requesting an access token). A client must be first registered with IdentityServer before it can request tokens. In ERP.net registered clients are adressed as trusted applications.
* **Resources** - Resources are something you want to protect with IdentityServer - either identity data of your users, or APIs. In our case this is the Domain API.
* **Identity Token** - An identity token represents the outcome of an authentication process. It contains at a bare minimum an identifier for the user (called the sub aka subject claim) and information about how and when the user authenticated. It can contain additional identity data.
* **Access Token** - An access token allows access to an API resource. Clients request access tokens and forward them to the API. Access tokens contain information about the client and the user (if present). APIs use that information to authorize access to their data.

Source: https://docs.identityserver.io/en/latest/intro/terminology.html

## Application Types

ERP.net supports two client application types
* **Interactive applications**  
  This type of applications are used by end users to access the ERP.net resources and database. This applications must use a web browser to show the ERP.net login screen to the end user. After successfull login the application can request identity_token or/and access_token. These applications must use the [Authorization Code Flow](https://auth0.com/docs/get-started/authentication-and-authorization-flow/authorization-code-flow) 

* **Service applications**  
  These are machine to machine applications that do not require user login. They use the[ Client Credentials Flow](https://auth0.com/docs/get-started/authentication-and-authorization-flow/client-credentials-flow) 

## Trusted Applications

The registered clients in one ERP.net instance are called trusted applications. They are stored in the database and have all required properties that Identity Server needs in order to manage the client application authorization. 

For more information of trusted applications visit the [Trusted Applications Topic](https://docs.erp.net/dev/topics/authentication/trusted-applications.html)

## Endpoints

**Discovery Endpoint**
The discovery endpoint can be used to retrieve metadata about your IdentityServer - it returns information like the issuer name, key material, supported scopes etc. See the [spec](https://openid.net/specs/openid-connect-discovery-1_0.html) for more details.

The discovery endpoint is available via /.well-known/openid-configuration relative to the base address, e.g.:

https://demodb.my.erp.net/id/.well-known/openid-configuration

**Authorize Endpoint**
The authorize endpoint can be used to request tokens or authorization codes via the browser. This process typically involves authentication of the end-user and optionally consent. 
For full list of available parameters visit this [link](https://docs.identityserver.io/en/latest/endpoints/authorize.html).

For Authorization Code Flow the application first need to call the **authorize** endpoint.  

```
Exapmle  

GET https://demodb.my.erp.net/id/connect/authorize?     
    client_id={my_client_id}&  
    redirect_uri={https://myapp/callback}&  
    response_type=code%20id_token&  
    scope=openid%20profile%20offline_access&  
    nonce=abc&  
    state=xyz&  
```
   
After successfull login the browser will be redirected to the provided redirect_uri (https://myapp/callback in the example above) with the authorization code as url parameter. This autnorization code then must be used to request an access_token from the **token** endpoint.

**Token Endpoint**

The token endpoint can be used to programmatically request tokens.  [Full list of available parameters](https://docs.identityserver.io/en/latest/endpoints/token.html)
The most used scenarios are:

* Request access_token with authorization code  
```   
POST /connect/token
CONTENT-TYPE application/x-www-form-urlencoded

    client_id=client1&
    client_secret=secret&
    grant_type=authorization_code&
    code=hdh922&
    redirect_uri=https://myapp.com/callback
```    
