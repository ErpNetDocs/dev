# Property Dependencies and Update Order

In ERP.net, setting a property may trigger internal **events** that automatically update other dependent properties. Because of this, the **order in which properties are set can be important** when creating or updating entities through the API.

For complex values such as `Quantity` and `Amount`, the behavior depends on where the dependent measurement unit or currency is stored.

## Same-entity dependent references

If the dependent reference is stored in the **same entity**, ERP.net can resolve the code passed in the complex value and assign the corresponding reference automatically.

For quantities, ERP.net searches for a measurement unit whose code matches `Quantity.Unit`.

Example:

```json
{
  "Quantity": { "Value": 1.00, "Unit": "PCE" }
}
```
If the entity stores the corresponding measurement unit reference in the same row, ERP.net assigns that reference automatically.

The same applies to amounts. If the entity stores the corresponding currency reference in the same row, ERP.net searches for a currency whose code matches Amount.Currency and assigns it automatically.

Example:



```json
{
  "LineAmount": { "Value": 100.00, "Currency": "EUR" }
}
```

## External dependent references

If the dependent measurement unit or currency is not stored in the same entity, the complex value does not update that external reference.

For example, in sales order lines, the line quantity and its measurement unit are stored in the line, so Quantity.Unit can update the line's measurement unit.

However, the document currency is stored in the sales order header, while amounts such as UnitPrice are stored in the line. Because of this, sending:


```json
{
  "UnitPrice": { "Value": 200, "Currency": "EUR" }
}
```
does not change the currency in the sales order header.

In such cases, the code passed in the complex value must match the already effective measurement unit or currency.

## Consistency rules

When both the dependent reference and the complex value are sent explicitly, they must remain consistent with each other and with the business rules applied by the model.

For example, if both QuantityUnit and Quantity.Unit are provided, they must refer to the same measurement unit. Likewise, if both a currency reference and Amount.Currency are provided in the same entity, they must refer to the same currency.

## Practical guidance
 - Use the code inside Quantity.Unit or Amount.Currency when you want ERP.net to resolve the reference automatically.
 - Do not expect Quantity or Amount to update references stored in another entity.
 - When working with documents, pay special attention to header/line separation of dependent fields.

## Examples

### Example 1: `Quantity.Unit` and `Amount.Currency` resolve dependent references stored in the same entity

In `Crm_Pricing_ProductPrices`, both dependent references are stored in the same entity:

- `PriceQuantity` depends on `PriceQuantityMeasurementUnit`
- `Price` depends on `Currency`

Because both dependencies are local to the same row, ERP.net can resolve the codes in the complex values and assign the corresponding references automatically.

```http
POST https://testdb.my.erp.net/api/domain/odata/Crm_Pricing_ProductPrices
Content-Type: application/json

{
  "Product": {
    "@odata.id": "General_Products_Products(81d38b50-fd06-e611-8292-b31071e2ee7f)"
  },
  "PriceQuantity": {
    "Value": 1,
    "Unit": "pcs"
  },
  "Price": {
    "Value": 25,
    "Currency": "BGN"
  },
  "Notes": "update-order same-entity example"
}
```

In this request:

- `PriceQuantity.Unit = "pcs"` can resolve and assign `PriceQuantityMeasurementUnit`
- `Price.Currency = "BGN"` can resolve and assign `Currency`

This works because both dependent references are stored in `Crm_Pricing_ProductPrices`.

### Example 2: `Quantity.Unit` can update a line-local unit, but `UnitPrice.Currency` cannot update the document header currency

In `Crm_Sales_SalesOrderLines`:

- `Quantity` depends on `QuantityUnit`, which is stored in the same line
- `UnitPrice` depends on `SalesOrder.DocumentCurrency`, which is stored in the sales order header

Because of this, `Quantity.Unit` can resolve the line measurement unit, but `UnitPrice.Currency` cannot change the document currency in the header.

```http
POST https://testdb.my.erp.net/api/domain/odata/Crm_Sales_SalesOrders
Content-Type: application/json

{
  "DocumentType": {
    "@odata.id": "Systems_Documents_DocumentTypes(469b67b1-8b4b-4fb4-9d97-20c96105a85a)"
  },
  "EnterpriseCompany": {
    "@odata.id": "General_EnterpriseCompanies(b0e80577-fbbe-4c9b-811e-20b6c6dd465f)"
  },
  "Customer": {
    "@odata.id": "Crm_Sales_Customers(15f2640f-f374-4017-ae2d-d2a41535f054)"
  },
  "DocumentCurrency": {
    "@odata.id": "General_Currencies_Currencies(3187833a-d3c1-4804-bfc0-e17e6aee3069)"
  },
  "Lines": [
    {
      "Product": {
        "@odata.id": "General_Products_Products(81d38b50-fd06-e611-8292-b31071e2ee7f)"
      },
      "Quantity": {
        "Value": 2,
        "Unit": "pcs"
      },
      "UnitPrice": {
        "Value": 20,
        "Currency": "BGN"
      }
    }
  ]
}
```

In this request:

- `Quantity.Unit = "pcs"` can resolve and assign `QuantityUnit`
- `UnitPrice.Currency = "BGN"` does **not** update `DocumentCurrency`

`UnitPrice.Currency` must match the currency already effective for the sales order header.

### Example 3: `PATCH` uses the same rule during update

The same behavior applies when updating an existing entity.

In the following example, `Quantity.Unit` is evaluated against the line-local dependency, while `UnitPrice.Currency` is validated against the already effective sales order currency.

```http
PATCH https://testdb.my.erp.net/api/domain/odata/Crm_Sales_SalesOrderLines(34217345-3659-43be-a85d-005eaaa3aaac)
Content-Type: application/json

{
  "Quantity": {
    "Value": 5,
    "Unit": "pcs"
  },
  "UnitPrice": {
    "Value": 22,
    "Currency": "BGN"
  }
}
```

In this request:

- `Quantity.Unit = "pcs"` can resolve the dependent `QuantityUnit` reference in the same entity
- `UnitPrice.Currency = "BGN"` does **not** update `SalesOrder.DocumentCurrency`

The amount value must remain consistent with the currency already effective for the parent sales order.

### Example 4: explicitly sending both the dependent reference and the complex value

If both the dependent reference and the complex value are provided explicitly, they must remain consistent.

```http
PATCH https://testdb.my.erp.net/api/domain/odata/Crm_Sales_SalesOrderLines(34217345-3659-43be-a85d-005eaaa3aaac)
Content-Type: application/json

{
  "QuantityUnit": {
    "@odata.id": "General_Products_MeasurementUnits(7dbe6d6a-22ef-4c2f-a798-054bc2d13c8b)"
  },
  "Quantity": {
    "Value": 3,
    "Unit": "pcs"
  }
}
```

In this request:

- `QuantityUnit` is set explicitly
- `Quantity.Unit` is also provided explicitly

Both values must refer to the same effective measurement unit. If they do not match, the request is inconsistent.
