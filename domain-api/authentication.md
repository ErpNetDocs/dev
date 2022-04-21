# Authentication

Authentication is needed when an application needs to know the identity of the current user. Typically these applications manage data on behalf of that user and need to make sure that this user can only access the data for which he is allowed. The most common example for that is (classic) web applications – but native and JS-based applications also have a need for authentication.

ERP.net uses OpenID Connect authentication protocol which is an extension of OAuth2.

## API Access
Applications have two fundamental ways with which they communicate with APIs – using the application identity, or delegating the user’s identity. Sometimes both methods need to be combined.

OAuth2 is a protocol that allows applications to request access tokens from a security token service and use them to communicate with APIs. This delegation reduces complexity in both the client applications as well as the APIs since authentication and authorization can be centralized.

## Identity Server
Each [ERP.net instance](https://docs.erp.net/dev/topics/erp-instances.html) has it's own authentication provider site which is **usually** located on https://{ERP-INSTANCE-ROOT-URL}/id. For example the identity server for _demodb_ ERP instance is located at https://demodb.my.erp.net/id.

ERP.net Identity Server is build ot top of [IdentityServer4](https://docs.identityserver.io/) which documentation can be used to understand better the authentication process.

## Terminology

* **IdentityServer** - IdentityServer is an OpenID Connect provider - it implements the OpenID Connect and OAuth 2.0 protocols.
* **User** - A user is a human that is using a registered client to access resources.
* **Client** - A client is a piece of software that requests tokens from IdentityServer - either for authenticating a user (requesting an identity token) or for accessing a resource (requesting an access token). A client must be first registered with IdentityServer before it can request tokens. In ERP.net registered clients are addressed as trusted applications.
* **Resources** - Resources are something you want to protect with IdentityServer - either identity data of your users, or APIs. In our case this is the Domain API.
* **Identity Token** - An identity token represents the outcome of an authentication process. It contains at a bare minimum an identifier for the user (called the sub aka subject claim) and information about how and when the user authenticated. It can contain additional identity data.
* **Access Token** - An access token allows access to an API resource. Clients request access tokens and forward them to the API. Access tokens contain information about the client and the user (if present). APIs use that information to authorize access to their data.  
* **Refresh Token** - Since access tokens have finite lifetimes, refresh tokens allow requesting new access tokens without user interaction. In ERP.net Refresh tokens are supported only for interactive confidential applications that use the authorization code flow. 

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


For detailed instructions how to register trusted application visit this [topic](common-tasks/register-trusted-app.md)


## Internal and External Users

Client Applications can use the Identity Server to authenticate two type of users:
* **Internal Users** 
  These are the users that have access to the ERP.net instance database. Only internal users can obtain a valid access_token.
* **External Users**
  These are users that can be authenticated by Identity Server but can not obtain a valid access_token for Domain API or Table API. They are usually customers of the company-owner of the ERP.net instance. When an external user logs in, using the Identity Server login page, only an id_token is issued by the identity server. This id_token prooves that the user is properly authenticated. For example the users of a web store are external users. 

## Endpoints

### ERP.net Discovery Endpoint

The address of Identity Server as all ERP.net sites can be configured. To find out where any ERP.net site is located we must call the **/sys/auto-discovery** endpoint.

For example 

https://demodb.my.erp.net/sys/auto-discovery

The result of this request will be:

```
{
  "WebSites": [
    {
      "Type": "ID",
      "Status": "Working",
      "Url": "https://demodb.my.erp.net/id",
      "AdditionalProperties": null
    },
    {
      "Type": "API",
      "Status": "Working",
      "Url": "https://demodb.my.erp.net/api",
      "AdditionalProperties": {
        "ODataServiceRoot": "https://demodb.my.erp.net/api/domain/odata/"
      }
    }
  ]
}
```

The result contains all configured web sites for this ERP.net instance. 

By the result of this request you can understand where the Identity Server is located as well where the ODATA service root of the Domain API is located.
The site types at the moment are:

* ID - Identity Server Site - the authorization provider for the ERP.net instance.
* API - Domain API Site 
* TableAPI - Table API Site
* Other site types that provide different functionalities.


### Identity Server Discovery Endpoint  

The discovery endpoint can be used to retrieve metadata about your IdentityServer - it returns information like the issuer name, key material, supported scopes etc. See the [spec](https://openid.net/specs/openid-connect-discovery-1_0.html) for more details.

The discovery endpoint is available via /.well-known/openid-configuration relative to the base address, e.g.:

https://demodb.my.erp.net/id/.well-known/openid-configuration

### Authorize Endpoint  

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

### Token Endpoint  

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

The result of this request is a JSON containing access_token, id_token and refresh_token. The refresh_token can be used to obtain a new access_token (along with new refresh_token).


* Request access_token with client credentials. 
  This is the case when a service application can use the Domain API without an interactive user. The trusted application must be configured with system/service user that will be used to create the ERP session.  
  
```   
POST /connect/token
CONTENT-TYPE application/x-www-form-urlencoded

    client_id=client1&
    client_secret=secret&
    grant_type=client_credentials
```   

> [!NOTE] 
> It is possible a client application to act as interactive application and service application at the same time. That means the application can use two different access tokens to communicate with Domain API or only use the client_credentials token. In order to achieve this the application must append the **-service** suffix to the client_id parameter of the Identity Server token endpoint when the application uses the client_credentials grant_type to obtain an access_token. For example a web store site can use a system user to load the products and to relate the logged in external user to a customer entry in the database.
> For example if the client application is registered as trusted application with ApplicationUri='ClientApp' and SystemUserAllowed=true and ImpersonateLoginUrl,ImpersonateLogoutUrl not empty, access_token can be requested with athorization_code grant_type and client_credentials grant_type, but when the client_credentials grant_type is used the provided client_id must be 'ClientApp-service':  
>   
> POST /connect/token  
> CONTENT-TYPE application/x-www-form-urlencoded  
>   
>   client_id=ClientApp-service&client_secret=secret&grant_type=client_credentials  


* Request access_token with refresh_token  
```   
POST /connect/token
CONTENT-TYPE application/x-www-form-urlencoded

    client_id=client1&
    client_secret=secret&
    grant_type=refresh_token&
    refresh_token=xxxxxxxxxxxxxxxxxxxxxxxx
```  
The result contains new access_token and new refresh_token.

### End Session Endpoint

The end session endpoint can be used to trigger single sign-out (see [spec](https://openid.net/specs/openid-connect-rpinitiated-1_0.html)).

To use the end session endpoint a client application will redirect the user’s browser to the end session URL. All applications that the user has logged into via the browser during the user’s session can participate in the sign-out.

 [Full list of available parameters](https://docs.identityserver.io/en/latest/endpoints/endsession.html)
 
 Example
 
 ```
 GET /connect/endsession?id_token_hint=eyJhbGciOiJSUzI1NiIsImtpZCI6IjdlOGFkZmMzMjU1OTEyNzI0ZDY4NWZmYmIwOThjNDEyIiwidHlwIjoiSldUIn0.eyJuYmYiOjE0OTE3NjUzMjEsImV4cCI6MTQ5MTc2NTYyMSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiYXVkIjoianNfb2lkYyIsIm5vbmNlIjoiYTQwNGFjN2NjYWEwNGFmNzkzNmJjYTkyNTJkYTRhODUiLCJpYXQiOjE0OTE3NjUzMjEsInNpZCI6IjI2YTYzNWVmOTQ2ZjRiZGU3ZWUzMzQ2ZjFmMWY1NTZjIiwic3ViIjoiODg0MjExMTMiLCJhdXRoX3RpbWUiOjE0OTE3NjUzMTksImlkcCI6ImxvY2FsIiwiYW1yIjpbInB3ZCJdfQ.STzOWoeVYMtZdRAeRT95cMYEmClixWkmGwVH2Yyiks9BETotbSZiSfgE5kRh72kghN78N3-RgCTUmM2edB3bZx4H5ut3wWsBnZtQ2JLfhTwJAjaLE9Ykt68ovNJySbm8hjZhHzPWKh55jzshivQvTX0GdtlbcDoEA1oNONxHkpDIcr3pRoGi6YveEAFsGOeSQwzT76aId-rAALhFPkyKnVc-uB8IHtGNSyRWLFhwVqAdS3fRNO7iIs5hYRxeFSU7a5ZuUqZ6RRi-bcDhI-djKO5uAwiyhfpbpYcaY_TxXWoCmq8N8uAw9zqFsQUwcXymfOAi2UF3eFZt02hBu-shKA&post_logout_redirect_uri=http%3A%2F%2Flocalhost%3A7017%2Findex.html
 ```
 
