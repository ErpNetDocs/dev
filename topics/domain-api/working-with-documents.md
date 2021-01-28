# Working with documents

See [Documents](xref:Documents) on technical documentation.

Retrieving and updating documents is the same as any other entity in the domain.

However there are some specific rules that apply only to documents. For example documents on state Released or above can not be modified directly. They must be modified with Adjustment Documents. [Adjustment Documents](xref:Adjustment-Documents)

Another important attribute of the documents that can not be modified with simple PATCH request is [State](xref:Document-States)

The examples below show some tasks related to documents.

## Create Document

Document can be created only by specifying the required properties. Other properties will be filled by it's constant default value or it's LateDefault expression.

If Front-End model is used in [API Transaction](transactions.md) dependent property values are recalculated upon property change.  For example in SalesOrderLine line.ProductDescription is set to line.Product.Name when line.Product changes.

In the example bellow a new SalesOrder is created with one SalesOrderLine.

Note that measurement units and currencies are specified before passing [Quantity](quantity.md) or [Amount](amount.md) values. This is required because the quantity or amount contains the code of the measurement unit or currency.
