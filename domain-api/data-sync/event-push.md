# Event push

Pushing events allows external app to be notified in real time when an event is triggered in @@name.
Event push in @@name is implemented through [Webhooks](https://en.wikipedia.org/wiki/Webhook).

## Webhooks

A Webhook is simply a way for @@name to call an external app in (near) real time.
Webhooks are setup in two steps:

* A webhook template is setup in the Web Hooks entity.
* The template is activated using the WEBHOOK action in a [User-defined business rule](xref:ubr).

>> [!note]
>> Setting up user-defined business rules (UDBR) is outside the scope of this topic.
>> UDBR allows you to select the business events which need to be pushed to the external app.
>> For assistance in setting up UDBR, seek professional advice.

## Destination

Webhooks allow @@name to call the external app in near real-time.
However, if for any reason, the app is not able to accept the call, the event might be missed.
We strongly advice, the usage of webhooks to be only for pushing an event to an enterprise grade messaging system, such as the [Azure Service Bus](https://azure.microsoft.com/en-us/services/service-bus/).

## Usage

It is up to the receiving application to decide what to do with the event.
However, it usually updates some internal state, based on the event.

In case the external app implements some forms of data sync between its private database and @@name, you might want to check the [Object version](object-version.md) topic.