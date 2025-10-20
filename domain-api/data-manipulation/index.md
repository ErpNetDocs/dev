# Data Manipulation


### Create Operations

#### Creating an Entity

To create a new entity in ERP.net, you use the **POST** method of the OData service. The request body contains the entity data in JSON format.  

**Example:** Creating a new `Crm_Sales_SalesOrder`:

---
POST https://testdb.my.erp.net/api/domain/odata/Crm_Sales_SalesOrders
Content-Type: application/json

{
    "DocumentType": { "@odata.id": "Systems_Documents_DocumentTypes(469b67b1-8b4b-4fb4-9d97-20c96105a85a)" }, 
    "EnterpriseCompany": {"@odata.id": "General_EnterpriseCompanies(2115c294-33e9-41bd-bc0d-0cd0b7ee1735)" }, 
    "Customer": { "@odata.id": "Crm_Sales_Customers(c8a65f60-26d0-4c3a-81c9-f367dd811908)" },
    "DocumentNotes": "New sales order for testing"
}
---

**Required Fields:**

- **DocumentType:** Type of the document.  
- **EnterpriseCompany:** The company that owns the order.  
- **Customer:** The customer for this sales order.  

Other fields are optional or have defaults. Always validate your data according to the entity model.

For full details on the `Crm_Sales_SalesOrder` entity and all available fields, see the [ERP.net Entity Documentation](https://docs.erp.net/model/entities/Crm.Sales.SalesOrders.html).

### Data Validation and Business Rules

ERP.net enforces **data validation** through a combination of **System Business Rules** and **User Business Rules**:

- **System Business Rules:** Predefined rules for each entity (e.g., `Crm.Sales.SalesOrders`) that automatically enforce business logic on the back-end. Examples include assigning a sales person, validating fiscal numbers, or restricting line directions. These rules ensure data consistency and compliance without manual intervention.

- **User Business Rules:** Custom rules defined by developers or administrators to implement organization-specific validations. These can be applied on the back-end for security and integrity, and optionally on the front-end for immediate feedback.

Together, system and user business rules ensure that all data entered or modified in ERP.net adheres to required business logic and organizational policies.




## Update Operations

Updating Entities: Describe how to modify existing records.

Partial Updates: Explain partial update strategies.

Concurrency Control: Discuss handling concurrent updates.


## Delete Operations

Deleting Entities: Provide steps to delete records.

Soft vs. Hard Deletes: Explain the difference and when to use each.

Cascade Deletes: Discuss implications of cascading deletes.


## Transactions

BeginTransaction and EndTransaction: Explain how to start and commit transactions.

Entity Changes: Detail how to track and apply changes to entities within a transaction.

Concurrency Handling: Discuss strategies for managing concurrent data modifications.



## ERP.net action annotation


## Data Import

Import Strategies: Outline methods for importing large datasets.

Data Mapping: Discuss mapping external data to ERP.net entities.

Error Handling: Provide guidelines for managing import errors.



## Data Synchronization

Sync Mechanisms: Describe methods for synchronizing data between ERP.net and external systems.

Conflict Resolution: Explain strategies for resolving data conflicts.

Scheduling Syncs: Discuss scheduling and automating synchronization tasks.
