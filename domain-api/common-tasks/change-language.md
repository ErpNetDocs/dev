# Change the response language

## Overview 

Thanks to the built-in [multilanguage support](https://docs.erp.net/tech/concepts/multi-language.html) you can save data in multiple languages.

Then, when you retrieve them back via the Domain API, you get the data all at once as a complex type of [multilanguage string](../complex-types/multi-language-string.md)- i.e. all translations you have defined.

```json
{
  "en": "Apple",
  "de": "Apfel"
}
```

## Document printout

However, sometimes there are cases where the data must be returned, visualized for a particular language. I.e. it's not appropriate to return all defined translations at once.

An example of this is when you acquire a [document printout](https://docs.erp.net/model/entities/General.Documents.html#getprintout). The result of this feature is a document, a file, rendered in a specific language. What if you want the printout to be displayed in a language other than the default?

> [!NOTE]
> The default language in Domain API is English (en).

If you want it to be generated in German, you must specify it explicitly.

## Supported ways to specify the response language

1. Url parameter: `culture=de`
2. Cookie, containing a key=value: `.AspNetCore.Culture=de`
3. Accept-Language HTTP header: `Accept-Language: de`

All you have to do is to specify the language in one of the ways listed above.

> [!WARNING]
> If there's more than one way to change the language at the same time, prioritization takes place. The priority is the same as listed above: (1) url parameter, (2) cookie, (3) accept-language header. I.e. if you send a request, containing a url parameter `culture=de` and also specify an HTTP header `Accept-Language: en`, the chosen language will be German.

Going back to the example of generating a document printout mentioned above, here's what a typical request looks like,

```HTTP
POST /api/domain/odata/Crm_Invoicing_Invoices(51a63a99-c96d-4876-b205-fced610143ae)/GetPrintout HTTP/1.1
Host: demodb.my.erp.net
Content-Type: application/json

{
    "fileFormat": "pdf",
    "printout": {
        "@odata.id": "General_Printouts(f5229037-b420-46a4-81a0-f11f7d112879)"
    }
}
```
The result of the request will be the printout as base64 encoded pdf file in the default Domain API language.

If you want to change the language, it's enough to simply specify it in one of the three ways above,

```HTTP
POST /api/domain/odata/Crm_Invoicing_Invoices(51a63a99-c96d-4876-b205-fced610143ae)/GetPrintout HTTP/1.1
Host: demodb.my.erp.net
Content-Type: application/json
Accept-Language: de

{
    "fileFormat": "pdf",
    "printout": {
        "@odata.id": "General_Printouts(f5229037-b420-46a4-81a0-f11f7d112879)"
    }
}
```

The only change is the additional request HTTP header `Accept-Language: de`.

The result will be the same printout, but in German.

> [!NOTE]
> Changing the language simply instructs @@erpnet to "choose" it from what is saved in the multilanguage string attributes.
> - If the "chosen" language isn't present in a multulanguage string attribute, it will be returned [transliterated](https://docs.erp.net/tech/concepts/multi-language.html#transliteration).
> - All non-multilanguage strings will be returned as they are.

## Other cases, where language change has an effect

### Error responses / messages

For example, we'll update a customer, but in such a way that we get an error back. We'll trigger the [R27159](https://docs.erp.net/model/business-rules/R27159.html) validation business rule.

```HTTP
PATCH /api/domain/odata/Crm_Customers(e99186bc-d3bc-4a93-b169-5baec1d45540) HTTP/1.1
Host: demodb.my.erp.net
Content-Type: application/json

{
    "FromDate": "2022-01-01",
    "ThruDate": "2021-01-01"
}
```

Accordingly, the result will be the following error:

```JSON
{
    "error": {
        "message": "An error occurred while saving data to the database\r\n  The validation R27159: FromDateLessThanThruDate failed for event Commit: \r\n\r\nCustomer's From Date 1/1/2022 12:00:00 AM should not be greater than customer's Thru Date 1/1/2021 12:00:00 AM. (Constraint R27159)"
    }
}
```

If we change the language (this time as a url parameter), the request will look like this,

```HTTP
PATCH /api/domain/odata/Crm_Customers(e99186bc-d3bc-4a93-b169-5baec1d45540)?culture=bg HTTP/1.1
Host: demodb.my.erp.net
Content-Type: application/json

{
    "FromDate": "2022-01-01",
    "ThruDate": "2021-01-01"
}
```
The difference is adding the `culture=bg` parameter to the url.

We'll receive the same message, but translated into Bulgarian:

```JSON
{
    "error": {
        "message": "Грешка при записване.\r\n  Валидацията R27159: FromDateLessThanThruDate не е била успешна при събитие Commit: \r\n\r\nСтойността на полето 'От дата' 1.01.2022 г. 0:00:00 ч. не трябва да надвишава стойността на полето 'До дата' 1.01.2021 г. 0:00:00 ч. (Constraint R27159)"
    }
}
```