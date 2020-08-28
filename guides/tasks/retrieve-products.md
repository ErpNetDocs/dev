# Retrieve Products Task

This task shows ways to retrieve products in order to display them in a e-commerce store.

## Retrieve Products

The following query:
1. Retrieves 10 products, without any order
2. Filters by:
   - **Active** = true
3. Selects the following attributes:
   - **Id** - the unique Id of the product.
   - **Part Number** - the product code.
   - **Name** - the [multi-language](~/apis/domainapi/complex-values.md#multi-language-string) product name.
   - **ProductGroup** - [object reference](~/apis/domainapi/referenced-object.md) to the Product Group containing the product.
   - **StandardPricePerLot** - [Amount](~/apis/domainapi/complex-values.md#amount) object, containing the standard price.
   - **Description** - textual description of the product. This description is clear text and does not support any formatting. There is also Description_Html attribute, which can be used to contain description with HTML formatting.
   - **MeasurementUnit** - [object reference](~/apis/domainapi/referenced-object.md) to measurement units. This is the default sales measurement unit of the product.

https://demodb.my.erp.net/api/domain/odata/General_Products_Products?$top=10&$filter=Active%20eq%20true&$select=CatalogDescriptionHtml,Description,Id,MeasurementUnit,Name,PartNumber,ProductGroup,StandardPricePerLot

You can edit the query in the Query Builder:

https://demodb.my.erp.net/api/domain/querybuilder#General_Products_Products?$top=10&$filter=Active%20eq%20true&$select=CatalogDescriptionHtml,Description,Id,MeasurementUnit,Name,PartNumber,ProductGroup,StandardPricePerLot

## Retrieve Product Pictures
Product Pictures and Variants are structu


## Retrieve Product Variants
Product Variants 

## Retrieve Product Prices

## Retrieve Product Codes
Retrieving product codes might sometimes be useful for getting bar-code or supplier/customer code of a product.
In order to understand the return set, the application must know in advance the id of the coding system.
For example, one coding system migth be for bar-codes, another - for the codes of the products for a prominent customer, etc.

