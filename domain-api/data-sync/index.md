# Data synchronization

Data sync occurs in many scenarios, where an external app synchronizes data betweed it and @@name.
Data sync tasks of an external app include:

* Receive real time notifications for changes in @@name
* Check whether an object is updated in @@name since last check
* Implement UI with [Optimistic locking](https://en.wikipedia.org/wiki/Optimistic_concurrency_control) for data, stored in @@name

The data sync framework implemented in @@name Domain API allows efficient implementation of such scenarios.
For more information, select from the topics below:

* [Event push](event-push.md)
* [Object version](object-version.md)
* [Optimistic locking](optimistic-locking.md)