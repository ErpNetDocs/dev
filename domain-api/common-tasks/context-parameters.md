# Context parameters

In some cases the behaviour of the domain model depends on the provided context parameters. Such parameters are the **current user**, **current language**, **current enterprise company**, **current enterprise company location** and **current role**. 
For example some business rules require current enterprise company to determine the costing and pricing currency of a product.

The current user is implicitly determined by the session - this is the logged in internal user.

The language may be provided by one of the standard ways defined by [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-8.0) - **culture** URL parameter, **.AspNetCore.Culture** cookie or **Accept-Language** HTTP header.


## Providing current enterprise company, enterprise company location and role for the API request.

### 1. URL Parameters

```
CurrentEnterpriseCompanyId=GUID1&CurrentEnterpriseCompanyLocationId=GUID2&CurrentRoleId=GUID3
```
OR
```
ErpContextParameters=GUID1,GUID2,GUID3
```
The ErpContextParameters URL parameter requires three guids - the first one is enterprise company Id, the second one is enterprise company location Id and the third one is the role Id.

### 2. HTTP header Prefer

The Prefer HTTP Header can be used by a client to request particular server behaviors. In this case we can append multiple Prefer headers: 

```
Prefer: CurrentEnterpriseCompanyId=GUID1
Prefer: CurrentEnterpriseCompanyLocationId=GUID2
Prefer: CurrentRoleId=GUID3
```
OR
```
Prefer: ErpContextParameters=GUID1,GUID2,GUID3
```

### 3. Cookie
We can provide cookie with name `ErpContextParameters` and value 'GUID1,GUID2,GUID3'


## Validation

- If the provided value can't be parsed to a valid guid it is ignored.  
- If the current user has no acces to the provided enterprise company by security reasons, the provided CurrentEnterpriseCompanyId is ignored.  
- If the provided company location Id is not amongs the company locations of the current enterprise company the value is ignored.  
- If the provided role Id is not accessible by the current user the value is ignored.
