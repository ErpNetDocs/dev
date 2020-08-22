# URL Components Of a OData Query

```
https://demodb.my.erp.net/api/domain/odata/General_Products_Products?$top=10&$orderby=Name
\_________________________________________/\_______________________/ \___________________/
                    |                                  |                        |
            service root URL                     resource path            query options
```

* Service Root URL - this is the address of the API + /domain/odata/
* Resource Path - The requested entity
* Query Options - optional query options

NOTES:
1. The address of the API is usually and by default:

https://<<Instance_Name>>.my.erp.net/api/

However, this is not certain. Each site in ERP.net can have its own address, which is configurable. For example, this is also valid address:

[https://erpapi.example.com/](https://erpapi.example.com/)

2. The ERP.net API Resource Path supports only specifying a single entity.
REST style sub-entities are not supported. However, the API allows many other ways to expand into sub-entities.

3. Query Options
Allow the user to specify optional [Query Options](query-options.md).

For more information, visit the OData standards page:

[http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part2-url-conventions.html#sec_URLComponents](http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part2-url-conventions.html#sec_URLComponents)
