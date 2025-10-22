# Error Handling

When an error occurs during a Domain API operation, the service returns an HTTP status code **500 (Internal Server Error)** and a JSON body containing detailed error information.

The JSON response provides:
- A user-readable message (`message`)  
- A technical error code (`code`)  
- The error type (`type`)  
- Additional diagnostic details (`info`)  

Example:

POST https://testdb.my.erp.net/api/domain/odata/General_Products_Products

```json
{
  "PartNumber": "DAT100",
  "Name": { "EN": "Duplicate Test" },
  "MeasurementUnit@odata.bind": "General_Products_MeasurementUnits(7dbe6d6a-22ef-4c2f-a798-054bc2d13c8b)"
}
```

Response:
```json
{
  "message": "Prohibited duplication in table 'dbo.Gen_Products_Table'.\n\nThe set of values for the following fields could not be saved more than once...",
  "code": 2129,
  "type": "Aloe.EnterpriseOne.Server.ServerAPI.Exceptions.EnterpriseOneServerException",
  "info": "System.Exception: Prohibited duplication in table 'dbo.Gen_Products_Table'...",
  "messageFormat": "Prohibited duplication in table '{0}'.\n\nThe set of values for the following fields could not be saved more than once.\n\nFields: ({1})\n\nDuplicated value: ({2})\n\nIndex: {3}\n\nPlease remove the duplicate record or change the value of any of the fields listed.",
  "parameters": [
    "dbo.Gen_Products_Table",
    "IX_Inv_Materials_Table_Number",
    "DAT100",
    "IX_Inv_Materials_Table_Number"
  ]
}
```

---

### Notes

- The **`message`** field usually contains localized user-readable text.  
- The **`info`** field can include a stack trace for debugging in non-production environments.  
- The **`parameters`** and **`messageFormat`** fields are useful for programmatic handling or translation of structured errors.  
