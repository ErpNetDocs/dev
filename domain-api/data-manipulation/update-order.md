# Property Dependencies and Update Order

In ERP.net, setting a property may trigger internal **events** that automatically update other dependent properties. Because of this, the **order in which properties are set is important** when creating or updating entities through the API.

For example, the `Quantity` field is a complex type that depends on the `QuantityUnit` reference.  
If you set the `Quantity` before assigning the correct `QuantityUnit`, the system may interpret the value using an outdated or incorrect unit, leading to validation or conversion errors.

To ensure correct behavior, always set the unit **before** the quantity value.

**Example:**

```
{`
    "QuantityUnit": { "@odata.id": "General_MeasurementUnits(6f7cbe0e-9a2a-4e3b-9b64-4f86d8c9e7f0)" },
    "Quantity": { "Value": 1.00, "Unit": "PCE" }
}
```

This ensures that the system knows which measurement unit applies before processing the quantity, preventing inconsistencies when working with values in different units.


> **NOTE:** The same rule applies to other dependent fields such as `Amount` and `Currency` — always set the currency reference before specifying the amount value.

