# **$search** query option

## Description

`$search` is a system query option, a standard one in the OData protocol.

The `$search` system query option restricts the result to include only those entities, matching the specified search expression. The search expression is a freeform string.

For more detailed info, strictly defined according to the OData protocol, check this resource [here](https://www.odata.org/getting-started/basic-tutorial/#search).

## How the search works

When an entity is searched, it's checked for a match between the searched string and the value of one or more attributes. Exactly which attributes of the entity will be checked for a match, depends on which are specified in the entity's default search members and display text format.

> [!NOTE]
> The attributes for an entity to search for a match are those, defined in the **Default search members** and the **Display text format**. For each different entity.

### Default search members

They're defined at the system level and may differ for different entities. The exact search members for each entity can be found in the documentation, but there's a rule that's generally true in most cases,

> [!NOTE]
> The default search members (i.e. attributes) for an entity are these, supporting Code and Name.

E.g. the Customers entity has its default search members: `Number; Party.PartyName`

https://docs.erp.net/model/entities/Crm.Customers.html

### Display text

The search is also performed on the attributes, part of the display text attribute. It's available for all entities.

https://docs.erp.net/model/entities/Systems.Core.EntitySettings.html#displaytextformat
https://docs.erp.net/tech/advanced/data-objects/display-format.html

## Usage

Just see the query below,

```http
GET 
~/Crm_Customers?$select=Number&$search="015"
```

This will return all customers, matching the searched string `015`. E.g.,

```json
{
    "@odata.context": "~/$metadata#Crm_Customers",
    "value": [
        {
            "@odata.id":"Crm_Customers(79480957-f0b6-49c4-9874-2cd150de982a)",
            "Number": "aa00015"
        },
        {
            "@odata.id":"Crm_Customers(806637f2-abd1-4e7b-8ac0-71222a0b1afd)",
            "Number": "ab30151"
        },
        {
            "@odata.id":"Crm_Customers(f812a533-0e56-4e57-8d1f-52d05b98c8b6)",
            "Number": "ab30156"
        }
    ]
}
```

The result contains all customers that contain `015` in their number. OK, let's make an another try:

```http
GET 
~/Crm_Customers?$select=Party&$expand=Party($select=PartyName)&$search="UNI"
```

Here's the result:

```json
{
    "@odata.context": "~/$metadata#Crm_Customers",
    "value": [
        {
            "@odata.id":"Crm_Customers(eebf02a5-052e-4a8d-9a24-270546d73942)",
            "Party": {
                "@odata.id":"General_Contacts_Parties(b8aa4272-3e55-435b-b1ab-170afee896d4)",
                "PartyName": {
                    "EN": "UNI Sofia Ltd",
                    "BG": "УНИ София Лтд"
                }
            }
        }
    ]
}
```

Obviously, the result contains all customers, having a "UNI" in their name.

The examples above were when we have a match on the default search members- i.e. the [Number](https://docs.erp.net/model/entities/Crm.Customers.html#number) and [Party.PartyName](https://docs.erp.net/model/entities/Crm.Customers.html#party) in the [Customers](https://docs.erp.net/model/entities/Crm.Customers.html) entity.

Let's see an example where the search will be performed on the members, defined in the entity's display text attribute.

To do this, we'll first change the display text format for the customers entity (because the default one is `{Party.PartyName:T}`) to the following one:
```cs
{Party.PartyName:T} / {GracePeriodDays}
```

Now if we make a request such as:
```http
GET 
~/Crm_Customers?$select=Number,GracePeriodDays,Party&$expand=Party($select=PartyName)&$search="50"
```

The result will be the following:

```json
{
    "@odata.context": "~/$metadata#Crm_Customers",
    "value": [
        {
            "@odata.id":"Crm_Customers(eca3ca4d-c4fa-44df-9983-d69388a8893a)",
            "GracePeriodDays": 50,
            "DisplayText": "Test Company 1 Ltd / 50",
            "Number": "number001",
            "Party": {
                "@odata.id":"General_Contacts_Parties(841e89e4-44c2-4c8f-b4d9-6402c3e5fb28)",
                "PartyName": {
                    "EN": "Test Company 1 Ltd"
                }
            }
        },
        {
            "@odata.id":"Crm_Customers(bc446b31-7326-4c35-bca6-55c918e33215)",
            "GracePeriodDays": 7,
            "DisplayText": "Test Company 2 Ltd / 7",
            "Number": "number050",
            "Party": {
                "@odata.id":"General_Contacts_Parties(395fa6b1-8fd4-418a-87f6-d8bece1fc7ad)",
                "PartyName": {
                    "EN": "Test Company 2 Ltd"
                }
            }
        }
    ]
}
```

Two customers- the first has a match on the attribute `GracePeriodDays` (50) and the second has a match on the `Number` (number050) attribute.
