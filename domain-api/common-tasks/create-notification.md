# Create notification

## Overview

As already known, notifications in @@erpnet are an entity, part of the domain model. 

[Communities.Notifications Entity](https://docs.erp.net/model/entities/Communities.Notifications.html).

Roughly, each notification is a message intended for a specific user. Additionally, the notification MAY reference a data object- e.g. the notification subject.

This is expressed through the following entity attributes:
- [User](https://docs.erp.net/model/entities/Communities.Notifications.html#user)
- [DataObject](https://docs.erp.net/model/entities/Communities.Notifications.html#dataobject)

@@erpnet creates and manages various notifications according to certain business rules, triggered by specific events. E.g. when someone replies to your comment- you'll get a notification. This flow is managed by a specific business rule ([R33428 SocialComment - Notify User Comment Replied](https://docs.erp.net/model/business-rules/R33428.html))

More information about all notification types (i.e. notification classes) and when they're created can be found in the separate topic **[Notifications](https://docs.erp.net/tech/modules/community/social-interactions/notifications/index.html)**.

## Create a notification programmatically

It's possible to create a notification yourself via the Domain API. You simple need to:
1. Specify the entity that will be the subject of the notification.
2. Invoke its method `CreateNotificaiton`.
3. Populate the required notification's attributes such as the target user, the notification class, the notification subject.

```HTTP
POST /api/domain/odata/Crm_Customers(79f3f74e-098a-4d91-9714-c4f845c2dc62)/CreateNotification HTTP/1.1
Host: demodb.my.erp.net
Content-Type: application/json

{
    "user": {
        "@odata.id": "Systems_Security_Users(9da64839-a8d0-491d-aebb-4d18fa42b014)"
    },
    "notificationClass": "NT_SOC_NEW_POST",
    "subject": "Hello from Domain API!"
}

```

That's it.

A notification will be created. Addressed to the specified user; originating from the specified customer.

## CreateNotification() in detail

`CreateNotification` is an API method, defined in the `EntityObject` type. This means that it can be invoked from any entity.

It's available for every entity in our Domain Model documententation. Here's a link to the method, used in the example from above,

https://docs.erp.net/model/entities/Crm.Customers.html#createnotification

Additionally, once the notification is created (as persistent data, in the database), it will be wrapped in a real-time event, which will be sent to the target user (as a real-time event). In this way, you can not only create notifications programmatically, but also notify users in real time.

More information about real-time events can be found in the separate topic in the documentation. [Real-time events](https://docs.erp.net/tech/advanced/concepts/real-time-events.html)