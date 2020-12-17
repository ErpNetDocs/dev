# Application types

The aid in correctly choosing the authentication and API types, we have compiled a list of common application types.

## Summary of application types

Below, you can find a summarization of the application types and the respective user, authentication and API types.

Application Type | User Type | Authentication | API
---------------- | --------- | -------------- | ---
Internal Web app | Internal | Authorization Code | Domain API
External Web app | External | Client Credentials | Domain API
Service application | Application | Client Credentials | Domain API
Business Intelligence | Application | Basic Authentication | Table API
Backup | Application | Basic Authentication | Table API

## Application type details

### Internal Web App

These are internal enterprise applications, used by the [internal users](https://info.erp.net/information/licensing/user-types.html#internal-users).

* **User type:** [Internal users](https://info.erp.net/information/licensing/user-types.html#internal-users)
* **Authentication:** [Authorization Code](identity/authentication-flows.md#authorization-code)
* **API:** [Domain API](domain-api/index.md)

### External Web App

External web apps are geared towards [external users](https://info.erp.net/information/licensing/user-types.html#external-users) and allow them limited and specialized access.
External web apps use [Application Accounts](https://info.erp.net/information/licensing/user-types.html#application-accounts) to access the ERP resources on behalf of the external users.

* **User type:** [External users](https://info.erp.net/information/licensing/user-types.html#external-users)
* **Authentication:** [Client Credentials](identity/authentication-flows.md#client-credentials)
* **API:** [Domain API](domain-api/index.md)

> [!NOTE]
> Some applications might allow both external and internal users.
> In this case, the app should use the same authentication like internal web app.

### Service application

Service applications are applications, which run usually in background and do not require user login.

* **User type:** [Application Account](https://info.erp.net/information/licensing/user-types.html#application-accounts)
* **Authentication:** [Client Credentials](identity/authentication-flows.md#client-credentials)
* **API:** [Domain API](domain-api/index.md)

### BI application

Business Intelligence (BI) apps use platforms like Power BI to visualize user data in various dashboards, charts, etc.

* **User type:** [Application Account](https://info.erp.net/information/licensing/user-types.html#application-accounts)
* **Authentication:** Basic Authentication
* **API:** [Table API](table-api/index.md)

### Backup

Backup applications transfer data to external sources.

* **User type:** [Application Account](https://info.erp.net/information/licensing/user-types.html#application-accounts)
* **Authentication:** Basic Authentication
* **API:** [Table API](table-api/index.md)
