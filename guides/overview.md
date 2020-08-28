# ERP.net Developer Guides

Welcome to the How-to guides for ERP.net Developers.

The how-to guides contain guidance for common developer scenarios.

## E-Commerce Website

Building an E-Commerce Website is the most common scenario for ERP.net applications.

Important skills:

* [Register UI Trusted App](tasks/register-trusted-app.md)
* [Non-confidential login workflow (required for javascript apps)](tasks/login-non-confidential.md)
* [Retrieve Products](tasks/retrieve-products.md)
* [Create Sales Order](tasks/create-sales-order.md)

For more information, see [Building E-Commerce Website](samples/build-ecommerce-website).

## Payment Connector

Connector services are service applications, which connect ERP.net to other platforms.
They frequently have few, if any, UI forms.

The example payment connector is a service, which exports Payment Orders to external service.  
Then, once the orders are paid, it creates Payment Transactions.

Important skills:

* Register Service-type Trusted App
* Application secret login workflow (required for service app)
* Retrieve Unpaid Orders
* Create Payment Orders

For more information, see [Building Connector Service](samples/build-payment-connector.md).

## Business Intelligence

Building Business Intelligence with Power BI allows the user to receive beautiful dashboards, based on recent or real-time data.

## Web-Integrated App

Web Integration refers to using web applications in a WinClient panel.

To get most out of the web-integrated app:

* The app should use single sign-on
* The app should be able to receive and show data based on URL parameters
