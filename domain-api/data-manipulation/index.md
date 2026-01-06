# Data Manipulation

* [Create](./create.md)  
   Create new entity record.

* [Update](./update.md)  
   Update  entity.

* [Delete](./delete.md)  
   Delete entity.

* [Property Dependencies and Update Order](./update-order.md)  
   The order of provided properties is important.

* [Data Validation and Business Rules](./data-validation.md)  
   ERP.net uses two models to handle entity data and business logic — the front-end model and the back-end model.
   While both represent the same entities, they differ in the business rules and data validations that are executed.

* [Transactions](./transactions.md)  
   ERP.net supports transactions to ensure that multiple operations on entities are processed consistently.
   Transactions allow you to group create, update, or delete operations into a single unit of work that is either committed or rolled back as a whole.

* [Actions](./erpnet-action.md)  
   ERP.net uses the `@erpnet.action` annotation to control how JSON data is processed when creating, updating, or importing entities.  
   It provides a flexible way to specify operations such as `create`, `update`, `delete`, `find`, and `merge` for top-level or nested objects.

* [Data Import](./import.md)  
   ERP.net provides an **Import** endpoint that allows you to insert, update, or delete multiple entities in a single request. This unbound action enables efficient bulk operations, supporting both front-end and back-end models.

* [Execute Script](./execute-script.md)  
  @@name provides an **ExecuteScript** endpoint that allows you to run **server-side JavaScript** in the Domain Model context. This unbound OData action executes the **raw JavaScript source** from the request body in a sandboxed runtime , with access to domain data and optional global `transaction` and `console` objects.

* [Data Sync](../data-sync/index.md)  
   ERP.net provides **Data Synchronization** to efficiently keep client applications up-to-date with system changes.  
   Instead of fetching all data repeatedly, Data Sync retrieves only entities that have changed since the last synchronization, ensuring minimal network usage and fast updates.

* [Error Handling](./error-handling.md)  
   When an error occurs during a Domain API operation, the service returns an HTTP status code **500 (Internal Server Error)** and a JSON body containing detailed error information.


