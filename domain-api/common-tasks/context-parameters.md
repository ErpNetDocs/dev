# Context parameters

In some cases the behaviour of the domain model depends on the provided context parameters. Such parameters are the **current user**, **current language**, **current enterprise company**, **current enterprise company location** and **current role**. 
For example some business rules require current enterprise company location to determine the curency of a product.

The current user is implicitly determined by the session - it is the logged in internal user.

The language mey be provided by one of the standard ways defined by ASP.NET Core - **culture** URL parameter, **.AspNetCore.Culture** cookie or **Accept-Language** HTTP header.


# Providing current enterprise company, enterprise company location and role for the API request.

## 1. URL Parameters

CurrentEnterpriseCompanyId=GUID1&CurrentEnterpriseCompanyLocationId=GUID2&CurrentRoleId=GUID3

OR

ErpContextParameters=GUID1,GUID2,GUID3

The ErpContextParameters URL parameter requires three guids - the first one is enterprise company Id, the second one is enterprise company location Id and the third one is the role Id.



## 2. HTTP hreader Prefer

The Prefer HTTP Header can be used by a client to request particular server behaviors. In this case we can append multiple Prefer headers: 

Prefer: CurrentEnterpriseCompanyId=GUID1

Prefer: CurrentEnterpriseCompanyLocationId=GUID2

Prefer: CurrentRoleId=GUID3

OR

Prefer: ErpContextParameters=GUID1,GUID2,GUID3



## 3. Cookie
We can provide cookie with name ErpContextParameters and value 'GUID1,GUID2,GUID3'
