# Working with documents

See [Documents](xref:Documents) on technical documentation.

Retrieving and updating documents is the same as any other entity in the domain.

However there are some specific rules that apply only to documents. For example documents on state Released or above can not be modified directly. They must be modified with [adjustment documents](xref:Adjustment-Documents).

Another important attribute of the documents that can not be modified with simple PATCH request is [State](xref:states).

The examples below show some tasks related to documents.

## Create document

Document can be created only by specifying the required properties. Other properties will be filled by it's constant default value or it's LateDefault expression.

If Front-End model is used in [API Transaction](../data-manipulation/transactions.md) dependent property values are recalculated upon property change.  For example in SalesOrderLine line.ProductDescription is set to line.Product.Name when line.Product changes.

In the example bellow a new SalesOrder is created with one SalesOrderLine.

For complex values such as [Quantity](../complex-types/quantity.md) and [Amount](../complex-types/amount.md), the behavior depends on where the dependent measurement unit or currency is stored.

If the dependent reference is stored in the same entity, ERP.net can resolve the code passed in the complex value and assign the corresponding reference automatically.

If the dependent reference is stored outside the current entity, the complex value does not update it, and the code in the complex value must match the already effective unit or currency.

For more details, see [Property Dependencies and Update Order](../data-manipulation/update-order.md).

```json
POST ~/Crm_Sales_SalesOrders
{
    "DocumentType": {
        "@odata.id": "Systems_Documents_DocumentTypes(469b67b1-8b4b-4fb4-9d97-20c96105a85a)"
    },
    "EnterpriseCompany": {
        "@odata.id": "General_EnterpriseCompanies(2115c294-33e9-41bd-bc0d-0cd0b7ee1735)"
    },
    "EnterpriseCompanyLocation": {
        "@odata.id": "General_Contacts_CompanyLocations(8e1829a8-8769-4422-8155-9cee69002f9f)"
    },
    "Customer": {
        "@odata.id": "Crm_Sales_Customers(10f76803-4130-40f8-bf73-a4e2b004417b)"
    },
    "DocumentCurrency": {
        "@odata.id": "General_Currencies_Currencies(3187833a-d3c1-4804-bfc0-e17e6aee3069)"
    },
    "Lines": [
        {
            "Product": {
                "@odata.id": "General_Products_Products(4eeb8054-f7d9-479f-a239-ed07d8145075)"
            },
            "Quantity": {
                "Value": 1,
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

> [!note]
> In a sales order line, `Quantity.Unit` can assign the line measurement unit, because that reference is stored in the same line.
>
> `UnitPrice.Currency` does not change `DocumentCurrency`, because the document currency is stored in the sales order header, not in the line. Therefore the currency code in `UnitPrice` must match the already effective document currency.
>
> For details, see [Property Dependencies and Update Order](../data-manipulation/update-order.md).

## Change document state

HTTP C#

```json
POST ~/Crm_Sales_SalesOrders(59098bcf-f331-478f-91c2-f5520590f534)/ChangeState
{
"newState" : "FirmPlanned",
"userStatus": {"@odata.id": "General_DocumentTypeUserStatuses(1ee1249e-4ef5-46b4-8409-26b2130d09c7)"}
}
```

## Make document void

HTTP C#

```json
POST ~/Crm_Sales_SalesOrders(11217345-3659-43be-a85d-005eaaa3aaac)/MakeVoid
{
"reason" : "test api method",
"voidType": "VoidDocument"
}
```

## Recalculate document

This method is used to recalculate some document details such as Document Amounts (like VAT), Bonus Programs etc. It make sense only in the context of Front-End transaction because the recalculated changes remain only in memory. They must be commited to the database with a separate call to EndTransaction{commit=true}.

HTTP C#

```json
POST ~/Crm_Sales_SalesOrders(11217345-3659-43be-a85d-005eaaa3aaac)/Recalculate
```

## Create adjustment documents

Released documents can be modified only with adjustment documents. The API provides a convenient method to create adjustment documents: CreateAdjustmentDocuments. The method requires TransactionId in the request header.

The method creates adjustment documents for modified released documents. The adjustment documents are created in separate transaction and their state is changed to 'Adjustment'. The method does not commit or rollback the current front-end transaction. 

HTTP C#

```json
// Begin a front-end transaction

POST ~/BeginTransaction

{

"model": "frontend"

}

// The returned transaction id must be set in the request header for each subsequent query. The header name is TransactionId.

// Update some sales order lines.

PATCH ~/Crm_Sales_SalesOrderLines(34217345-3659-43be-a85d-005eaaa3aaac)

TransactionId: xxxx

{

"Quantity": {"Value": 5.0, "Unit": "PCE"}

}

// Update another line.

PATCH ~/Crm_Sales_SalesOrderLines(65217345-3659-43be-a85d-005eaaa3aaac)

TransactionId: xxxx

{

"Quantity": {"Value": 15.0, "Unit": "PCE"}

}



// Call CreateAdjustmentDocuments to create the adjustment documents and apply the changes to the original document.

// Adjustment documents will be created for all modified released documents in the current transaction.

POST ~/CreateAdjustmentDocuments

TransactionId: xxxx



// End the transaction without committing because updating released document directly is not allowed.

POST ~/EndTransaction

TransactionId: xxxx

{

"commit": false

}
```

## Create multiple lines to an existing document

A possible scenario is when you have a document created, but later you want to add its lines.

```json
PATCH ~/Crm_Sales_SalesOrders(283e4c71-2d77-4083-81b6-4c7f17668d7e)
{
  "Lines": [
    {
      "LineNo": 10,
      "Product": {
        "@odata.id": "General_Products_Products(81d38b50-fd06-e611-8292-b31071e2ee7f)"
      },
      "QuantityUnit": {
        "@odata.id": "General_MeasurementUnits(7dbe6d6a-22ef-4c2f-a798-054bc2d13c8b)"
      },
      "Quantity": {
        "Value": 1,
        "Unit": "pcs"
      },
      "UnitPrice": {
        "Value": 20,
        "Currency": "BGN"
      }
    },
    {
      "LineNo": 20,
      "Product": {
        "@odata.id": "General_Products_Products(08fc0b95-01d8-4876-9074-67898c0bd98b)"
      },
      "QuantityUnit": {
        "@odata.id": "General_MeasurementUnits(7dbe6d6a-22ef-4c2f-a798-054bc2d13c8b)"
      },
      "Quantity": {
        "Value": 5,
        "Unit": "pcs"
      },
      "UnitPrice": {
        "Value": 10,
        "Currency": "BGN"
      }
    },
    {
      "LineNo": 30,
      "Product": {
        "@odata.id": "General_Products_Products(396f958d-1952-4c6f-ac66-9211962720d4)"
      },
      "QuantityUnit": {
        "@odata.id": "General_MeasurementUnits(7dbe6d6a-22ef-4c2f-a798-054bc2d13c8b)"
      },
      "Quantity": {
        "Value": 67,
        "Unit": "pcs"
      },
      "UnitPrice": {
        "Value": 1.23,
        "Currency": "BGN"
      }
    }    
  ]
}
```

## Create a sales order with automatic calculation of line prices and amounts

The term "automatic" actually means the execution of the front-end business rules, related to the sales order and its sales order lines.

Therefore, this is only possible in a front-end transaction.

```json
POST ~/BeginTransaction

{
  "model": "frontend"
}
```

After, just create the sales order and its lines at once:
```json
POST ~/Crm_Sales_SalesOrders
TransactionId: xxxx

{
  "DocumentNo": "12345",
  "DocumentType": {
    "@odata.id": "General_DocumentTypes(469b67b1-8b4b-4fb4-9d97-20c96105a85a)"
  },
  "EnterpriseCompany": {
    "@odata.id": "General_EnterpriseCompanies(b11b2f31-71b5-4443-a4b7-b5e9cd664a64)"
  },
  "Customer": {
    "@odata.id": "Crm_Customers(eebf02a5-052e-4a8d-9a24-270546d73942)"
  },
  "DocumentCurrency": {
    "@odata.id": "General_Currencies(3187833a-d3c1-4804-bfc0-e17e6aee3069)"
  },
  "Lines": [
    {
      "LineNo": 10,
      "QuantityUnit": {
        "@odata.id": "General_MeasurementUnits(7dbe6d6a-22ef-4c2f-a798-054bc2d13c8b)"
      },
      "Quantity": {
        "Value": 2,
        "Unit": "PCS"
      },
      "Product": {
        "@odata.id": "General_Products_Products(bc5a90ed-eca3-4e70-9987-395da25f6487)"
      }
    }
  ]
}
```

Finally, commit the transaction:
```json
POST ~/EndTransaction
TransactionId: xxxx

{
  "commit": true
}
```