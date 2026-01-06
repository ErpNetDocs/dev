# Data Validation and Business Rules

ERP.net enforces **data validation** through a combination of **System Business Rules** and **User Business Rules**:

- **System Business Rules:** Predefined rules for each entity (e.g., `Crm.Sales.SalesOrders`) that automatically enforce business logic on the back-end. Examples include assigning a sales person, validating fiscal numbers, or restricting line directions. These rules ensure data consistency and compliance without manual intervention.

- **User Business Rules:** Custom rules defined by developers or administrators to implement organization-specific validations. These can be applied on the back-end for security and integrity, and optionally on the front-end for immediate feedback.

Together, system and user business rules ensure that all data entered or modified in ERP.net adheres to required business logic and organizational policies.

# Front-End vs. Back-End Models

ERP.net uses two models to handle entity data and business logic — the **front-end model** and the **back-end model**.  
While both represent the same entities, they differ in the **business rules** and **data validations** that are executed.

- **Front-End Model:**  
  Designed for immediate feedback in client applications.  
  It contains business rules that are triggered on **attribute change events** — for example, when a user edits a field in a form, dependent fields may be recalculated or validated right away.  
  This provides a dynamic and interactive user experience.

- **Back-End Model:**  
  Used when the data is **committed to the server** (e.g., on save, post, or document state change).  
  It contains rules and validations that enforce business integrity, such as required field checks, calculated defaults, and document-level logic triggered by workflow events like state transitions.  

In short, the **front-end model reacts to user edits**, while the **back-end model enforces consistency and performs final validations** before persisting the data.

For more details, see [Transactions in Domain API](https://docs.erp.net/dev/domain-api/transactions.html).

