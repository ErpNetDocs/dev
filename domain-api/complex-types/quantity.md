# Quantity (Complex value)

The ERP domain model declares a special type for quantity properties.

The quantity is represented by value and measurement unit.

## Properties

| Name | Type | Description |
| --- | --- | --- |
| Unit | String | The measurement unit of the quantity represented by it's code. |
| Value | Decimal | The value of the quantity. |

Domain API Example:  

```json
{
  "Quantity": {
    "Value": "5.555",
    "Unit": "PCS"
  }
}
```

## Update behavior

When a `Quantity` value is submitted through the Domain API, ERP.net evaluates the `Unit` code in the context of the current entity.

If the dependent measurement unit reference is stored in the same entity, ERP.net searches for a measurement unit whose code matches `Quantity.Unit` and assigns that reference automatically.

If the measurement unit reference is not stored in the same entity, `Quantity.Unit` does not update it. In that case, the code in `Quantity.Unit` must match the already effective measurement unit.

For more details, see [Property Dependencies and Update Order](../data-manipulation/update-order.md).


> [!note]  
> Because _Quantity_ is odata complex object it can not participate in uri $filter query parameter. To filter by Amount or Quantity properties you can use the following:
>
> `~/Logistics_Inventory_StoreTransactionLines?$filter=QuantityValue ge 5.555 ~/Crm_Sales_SalesOrderLines?$filter=LineAmountValue ge 5.555`
