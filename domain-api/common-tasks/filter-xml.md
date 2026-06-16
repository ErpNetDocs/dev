# Filter XML

`Filter_XML` is the internal XML format used in some ERP.net condition fields. It is intended for developers and AI agents that need to read or generate these values.

Examples of fields that use this format include:

- `Condition Customer Filter XML`
- `Condition Ship To Customer Filter XML`
- `Condition Distribution Channel Filter XML`
- `Valid For Customer Filter XML`
- `Valid for Ship to customer Filter XML`
- `Valid for Distribution channel Filter XML`

For example, these fields are used in [Bonus programs](https://docs.erp.net/tech/modules/crm/marketing/bonus-programs/index.html) and [Promotional packages](https://docs.erp.net/tech/modules/crm/pricing/promotional-packages.html).

For public query scenarios, prefer OData `$filter`. `Filter_XML` is an internal format.

## Format

The XML contains one or more filter expressions inside a root `<FILTER>` element.

Each condition is represented by an `<EXP>` element with the following child elements:

- `<TABLE_NAME>`
- optional `<TABLE_ALIAS>`
- `<COLUMN_NAME>`
- `<FILTER_TYPE>`
- optional `<FILTER_VALUE>`
- `<INCLUDE_NULLS>`

Common `FILTER_TYPE` values:

- `Equals`
- `Greater_Than`
- `Less_Than`
- `Like`

## Example: active customers

```xml
<FILTER>
  <EXP>
    <TABLE_NAME>Crm_Customers_Table</TABLE_NAME>
    <COLUMN_NAME>Active</COLUMN_NAME>
    <FILTER_TYPE>Equals</FILTER_TYPE>
    <FILTER_VALUE>1</FILTER_VALUE>
    <INCLUDE_NULLS>0</INCLUDE_NULLS>
  </EXP>
</FILTER>
```

## Example: customers with related party in a specific country

```xml
<FILTER>
  <EXP>
    <TABLE_NAME>Gen_Parties_Table</TABLE_NAME>
    <COLUMN_NAME>Country_Id</COLUMN_NAME>
    <FILTER_TYPE>Equals</FILTER_TYPE>
    <FILTER_VALUE>7d0a2b87-60da-4bb1-aad9-1c4ef1e1f001</FILTER_VALUE>
    <INCLUDE_NULLS>0</INCLUDE_NULLS>
  </EXP>
</FILTER>
```

## Example: ship-to customers whose party name contains "Retail"

```xml
<FILTER>
  <EXP>
    <TABLE_NAME>Gen_Parties_Table</TABLE_NAME>
    <COLUMN_NAME>Name</COLUMN_NAME>
    <FILTER_TYPE>Like</FILTER_TYPE>
    <FILTER_VALUE>%Retail%</FILTER_VALUE>
    <INCLUDE_NULLS>0</INCLUDE_NULLS>
  </EXP>
</FILTER>
```

## Example: distribution channels with code starting with `ONLINE`

```xml
<FILTER>
  <EXP>
    <TABLE_NAME>Crm_Marketing_Distribution_Channels_Table</TABLE_NAME>
    <COLUMN_NAME>Code</COLUMN_NAME>
    <FILTER_TYPE>Like</FILTER_TYPE>
    <FILTER_VALUE>ONLINE%</FILTER_VALUE>
    <INCLUDE_NULLS>0</INCLUDE_NULLS>
  </EXP>
</FILTER>
```

## Example: customers without GLN

```xml
<FILTER>
  <EXP>
    <TABLE_NAME>Gen_Parties_Table</TABLE_NAME>
    <COLUMN_NAME>GLN</COLUMN_NAME>
    <FILTER_TYPE>Equals</FILTER_TYPE>
    <INCLUDE_NULLS>1</INCLUDE_NULLS>
  </EXP>
</FILTER>
```

## Example: custom property by code

Custom properties can participate in the filter by using the property code as the column name.

```xml
<FILTER>
  <EXP>
    <TABLE_NAME>Crm_Customers_Table</TABLE_NAME>
    <COLUMN_NAME>CUSTOMER_GROUP</COLUMN_NAME>
    <FILTER_TYPE>Equals</FILTER_TYPE>
    <FILTER_VALUE>VIP</FILTER_VALUE>
    <INCLUDE_NULLS>0</INCLUDE_NULLS>
  </EXP>
</FILTER>
```

## Internal capabilities

`Filter_XML` also supports internal elements such as:

- `<ORDERBY>`
- `<OFFSET>`
- `<FETCH>`

These capabilities are for internal use.

### ORDERBY

`<ORDERBY>` defines sorting. The value is in the format `TableName_Table.ColumnName`. Append `DESC` for descending order.

Example:

```xml
<FILTER>
  <ORDERBY>Crm_Customers_Table.Customer_No</ORDERBY>
  <ORDERBY>Crm_Customers_Table.Creation_Time DESC</ORDERBY>
  <EXP>
    <TABLE_NAME>Crm_Customers_Table</TABLE_NAME>
    <COLUMN_NAME>Active</COLUMN_NAME>
    <FILTER_TYPE>Equals</FILTER_TYPE>
    <FILTER_VALUE>1</FILTER_VALUE>
    <INCLUDE_NULLS>0</INCLUDE_NULLS>
  </EXP>
</FILTER>
```

### OFFSET

`<OFFSET>` defines how many rows to skip before returning results.

Example:

```xml
<FILTER>
  <ORDERBY>Crm_Customers_Table.Customer_No</ORDERBY>
  <OFFSET>50</OFFSET>
  <EXP>
    <TABLE_NAME>Crm_Customers_Table</TABLE_NAME>
    <COLUMN_NAME>Active</COLUMN_NAME>
    <FILTER_TYPE>Equals</FILTER_TYPE>
    <FILTER_VALUE>1</FILTER_VALUE>
    <INCLUDE_NULLS>0</INCLUDE_NULLS>
  </EXP>
</FILTER>
```

### FETCH

`<FETCH>` defines the maximum number of rows to return.

Example:

```xml
<FILTER>
  <ORDERBY>Crm_Customers_Table.Customer_No</ORDERBY>
  <FETCH>25</FETCH>
  <EXP>
    <TABLE_NAME>Crm_Customers_Table</TABLE_NAME>
    <COLUMN_NAME>Active</COLUMN_NAME>
    <FILTER_TYPE>Equals</FILTER_TYPE>
    <FILTER_VALUE>1</FILTER_VALUE>
    <INCLUDE_NULLS>0</INCLUDE_NULLS>
  </EXP>
</FILTER>
```
