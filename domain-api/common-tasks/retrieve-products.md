# Retrieve Products

The following query:
1. Retrieves 10 products, without any order
2. Filters by:
   - **Active** = true
3. Selects the following attributes:
   - **Id** - the unique Id of the product.
   - **Part Number** - the product code.
   - **Name** - the [multi-language](~/topics/domain-api/multi-language-string.md) product name.
   - **ProductGroup** - reference to the Product Group containing the product.
   - **StandardPricePerLot** - [Amount](~/topics/domain-api/amount.md) object, containing the standard price.
   - **Description** - textual description of the product. This description is clear text and does not support any formatting. There is also Description_Html attribute, which can be used to contain description with HTML formatting.
   - **MeasurementUnit** - reference to measurement units. This is the default sales measurement unit of the product.

<https://demodb.my.erp.net/api/domain/odata/General_Products_Products?$top=10&$filter=Active%20eq%20true&$select=CatalogDescriptionHtml,Description,Id,MeasurementUnit,Name,PartNumber,ProductGroup,StandardPricePerLot>

You can edit the query in the Query Builder:

<https://demodb.my.erp.net/api/domain/querybuilder#General_Products_Products?$top=10&$filter=Active%20eq%20true&$select=CatalogDescriptionHtml,Description,Id,MeasurementUnit,Name,PartNumber,ProductGroup,StandardPricePerLot>

