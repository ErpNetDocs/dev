# Amount (Complex value)

The ERP domain model declares a special type for amount (currency) properties.

The amount is represented by value and currency.

## Properties

| Name | Type | Description |
| --- | --- | --- |
| Currency | String | The currency of the amount represented by it's sign. |
| Value | Decimal | The value of the amount. |

Domain API Example:  

```json
{
  "LineAmount": {
    "Value": "3.55",
    "Currency": "USD"
  }
}
```

> [!note]  
> Because _Amount_ is odata complex object it can not participate in $filter clause in uri. To filter by Amount or Quantity properties you can use the following:
>
> `~/Logistics_Inventory_StoreTransactionLines?$filter=QuantityValue ge 5.555 ~/Crm_Sales_SalesOrderLines?$filter=LineAmountValue ge 5.555`
