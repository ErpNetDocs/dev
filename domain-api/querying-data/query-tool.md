# ERP.net Query Tool

The **ERP.net Query Tool** allows you to experiment with Domain API requests directly in the browser. It is available in every @@name instance.  
It supports all HTTP methods, so you can run:

- **GET** queries to retrieve data  
- **POST** requests to create new entities or execute actions
- **PATCH** requests to update existing entities  
- **DELETE** requests to remove entities  

You can also provide a JSON payload for POST and PATCH requests, allowing you to test complex object creation or updates.  

Try it here: [ERP.net Query Tool](https://testdb.my.erp.net/api/domain/query?POST+General_Products_Products&payload=)

**Example usage:**

- Select the HTTP method (GET, POST, PATCH, DELETE)  
- Enter the entity set name and any query parameters  
- Provide JSON payload for POST or PATCH  
- Execute the request and view the JSON response directly in the browser
