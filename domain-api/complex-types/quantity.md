# Quantity (Complex value)

The ERP domain model declares a special type for quantity properties.

The quantity is represented by value and measurement unit.

## Properties

| Name | Type | Description |
| --- | --- | --- |
| Unit | String | TThe measurement unit of the quantity represented by it's code. |
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

> [!note]  
> Because _Quantity_ is odata complex object it can not participate in uri $filter query parameter. To filter by Amount or Quantity properties you can use the following:
>
> `~/Logistics_Inventory_StoreTransactionLines?$filter=QuantityValue ge 5.555 ~/Crm_Sales_SalesOrderLines?$filter=LineAmountValue ge 5.555`
