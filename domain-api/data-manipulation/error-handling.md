# Error Handling

When an error occurs during a Domain API operation, the service returns a JSON body containing detailed error information, under an `error` property.

The status code tells the caller what to do with it:

| Status | Meaning | What to do |
|---|---|---|
| **500** Internal Server Error | The operation failed. | Read the body, fix the request or the data. Retrying the same request will fail the same way. |
| **503** Service Unavailable | No license is available for the request. | Wait and retry. The response carries a `Retry-After` header with the suggested delay in seconds. |
| **429** Too Many Requests | A [rate limit](https://docs.erp.net/tech/advanced/web-sites/rate-limits.html) was exceeded. | Back off and retry. |

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

Response: `500 Internal Server Error`
```json
{
  "error": {
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
}
```

## No License Available

A request needs a live @@name session, and a session needs a license. When the instance has no license left for the caller, the request is answered with **503 (Service Unavailable)** and a `Retry-After` header, instead of failing as an internal error.

```http
HTTP/1.1 503 Service Unavailable
Retry-After: 60
Content-Type: application/json
Cache-Control: no-store
```

```json
{
  "error": {
    "message": "The maximum number of simultaneously connected users to this database is exceeded. Please contact your system administrator.",
    "code": 8,
    "type": "Aloe.SystemFrameworks.E1LicenseException",
    "info": "..."
  }
}
```

The request itself is valid and nothing was written, so the same request can be repeated once a license frees up. Clients should honor `Retry-After` and back off, rather than retrying in a tight loop.

A license frees up when a session closes, which happens after 20 minutes of inactivity or when the holder signs out. For how licenses are counted, see [License Slot Usage](../../auth/sessions/license-slot.md).

> [!NOTE]  
> A repeated 503 means the instance is genuinely out of licenses. It is not a transient network condition and will not clear by retrying faster.

---

### Notes

- The **`message`** field usually contains localized user-readable text.  
- The **`info`** field can include a stack trace for debugging in non-production environments.  
- The **`parameters`** and **`messageFormat`** fields are useful for programmatic handling or translation of structured errors.  
