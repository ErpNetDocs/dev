# Register Trusted Application

In this topic, you register an app in an ERP instance so the @@name Identity can provide authentication and authorization services for your application and its users.

Each application that uses @@name APIs needs to be registered.
Whether it's a client application like a web or mobile app, or it's a web API that backs a client app, registering it establishes a trust relationship between your application and the ERP instance.


> [!NOTE] 
> In future versions of ERP.net platform this way to register a trusted application will no longer be available, because of the _sec_ scope (Only @@name Identity itself will be granted with the _sec_ scope)
> The ERP.net instance will provide a browser accessible endpoint where the application will provide its client_id, client_secret_hash, scopes and other required parameters to register the application. This endpoint will redirect the user to the login screen and if the user is a database administrator he or she will be able to approve the application registration.



## Register an application

Registering your application establishes a trust relationship between your app and the ERP.net database instance. That means that your database trusts the application.

We'll show how to register an application manually, using the Domain API Query Tool.

Follow these steps to create the app registration:

1. Sign in to your ERP instance and open the Domain API Query Tool.  
   Every ERP.net database has it's own query tool on the Domain API site.  
   The Domain Api site is usually hosted on address  https://{COMPANY}.my.erp.net/api and the query tool is on https://{COMPANY}.my.erp.net/api/domain/query 
   
   In this example we'll use the DEMODB ERP.net instance query tool: https://demodb.my.erp.net/api/domain/query  
   
> :warning: You need to be a database administrator in order to be able to register a new trusted application.  

2. If the application is [confidential](/topics/identity/trusted-applications.html#client-type), we need to prepare an application secret. Use this endpoint to get the application secret hash (Replace _mysecret_ with your secret):

https://demodb.my.erp.net/sys/tools/sha256?secret=mysecret

1. In the query tool we'll create a new record for [System.Security.TrustedApplications](xref:Systems.Security.TrustedApplications) entity.  

## Register interactive confidential application

_Query_: Systems_Security_TrustedApplications  
_Type_: POST  
_Body_:  

```json
{
  "ApplicationUri": "MYDEMOCLIENT",
  "Name": "MY DEMO CLIENT",
  "ClientType": "Confidential",
  "ApplicationSecretHash": "T/AGymuI51LwjLeIFxRQXOs9IHnupDKs/ajhWODR2C4=",
  "ImpersonateAsCommunityUserAllowed": true,
  "ImpersonateAsInternalUserAllowed": true,
  "ImpersonateLoginUrl": "http://localhost:5080/myapp/signin-oidc",
  "ImpersonateLogoutUrl": "http://localhost:5080/myapp/",
  "SystemUserAllowed": false
}
```

- _ApplicationUri_ is the unique name that identifies the application. This is the _client_id_ in the OAuth terminology.  
- _Name_ is the display name of the application.  
- _ClientType_ - Confidential or Public.  
- _ApplicationSecretHash_ a hash of the application secret - previously created using   https://demodb.my.erp.net/sys/tools/sha256?secret=mysecret  tool.  
- _ImpersonateAsCommunityUserAllowed_ must be true if your application will work with community users. Community users are users that do not have access to system resources. They are usually customers of the company that owns the ERP.net database instance.  
- _ImpersonateAsInternalUserAllowed_ must be true if internal users will use the application.  
- _ImpersonateLoginUrl_ is a comma separated list of allowed URLs that are used from the application to receive the authorization code. When the user loads the application in the browser, if sign in is required, the browser is redirected to ERP.net Identity login page. After successful login the browser is redirected to the provided return_url that must be one of the URLs specified in _ImpersonateLoginUrl_ providing the _authorization_code_ through _code_ url parameter. This _code_ is used by the application to request an _access_code_ that is used to gain access to ERP.net server resources.
- _ImpersonateLogoutUrl_ is a comma separated list of allowed URLs that are used after the user is logged out from the @name Identity and "Return to app" button is clicked.  
- _SystemUserAllowed_ specifies if the application can act as a service application that is impersonated as specific user.

## Register interactive public application

_Query_: Systems_Security_TrustedApplications  
_Type_: POST  
_Body_:  

```json
{
  "ApplicationUri": "MYDEMOCLIENT",
  "Name": "MY DEMO CLIENT",
  "ClientType": "Public",
  "ImpersonateAsCommunityUserAllowed": true,
  "ImpersonateAsInternalUserAllowed": true,
  "ImpersonateLoginUrl": "http://localhost:5080/myapp/signin-oidc",
  "ImpersonateLogoutUrl": "http://localhost:5080/myapp/",
  "SystemUserAllowed": false
}
```

## Register service application

_Query_: Systems_Security_TrustedApplications  
_Type_: POST  
_Body_:  

```json
{
  "ApplicationUri": "MYSERVICEDEMOCLIENT",
  "Name": "Service Demo Client",
  "ApplicationSecretHash": "T/AGymuI51LwjLeIFxRQXOs9IHnupDKs/ajhWODR2C4=",  
  "ClientType": "Confidential",
  "SystemUserAllowed": true,
  "SystemUser": {
  "@odata.id": "Systems_Security_Users(cc314327-3d04-477f-ac53-cde19d8350e9)"
  }
}
```

- SystemUser is the user for the service application.
