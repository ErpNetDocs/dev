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

## Update behavior

When an `Amount` value is submitted through the Domain API, ERP.net evaluates the `Currency` code in the context of the current entity.

If the dependent currency reference is stored in the same entity, ERP.net searches for a currency whose code matches `Amount.Currency` and assigns that reference automatically.

If the currency reference is not stored in the same entity, `Amount.Currency` does not update it. In that case, the code in `Amount.Currency` must match the already effective currency.

For more details, see [Property Dependencies and Update Order](../data-manipulation/update-order.md).


> [!note]  
> Because _Amount_ is odata complex object it can not participate in uri $filter query parameter. To filter by Amount or Quantity properties you can use the following:
>
> `~/Logistics_Inventory_StoreTransactionLines?$filter=QuantityValue ge 5.555 ~/Crm_Sales_SalesOrderLines?$filter=LineAmountValue ge 5.555`
