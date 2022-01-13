# Optimistic locking

[Optimistic concurrency control](https://en.wikipedia.org/wiki/Optimistic_concurrency_control) is a no-lock concurrency control method. In @@name it is implemented through the [Object version](object-version.md) API.

Optimistic locking is usually used in the following scenarios:

* Replication of data between external data source and @@name.
* Implementing external UI for data apps.

## Usage scenario

External app allows its users to modify an entity, stored in an @@name instance.

1. The external app reads the entity, including the Object version.
1. The external app presents a UI to the end user and allows them to edit the entity.
1. When the user saves the data, the app call the update API, providing the data + the previously read Object version.
1. The system checks whether the Object version in the @@name instance is still the same and proceeds with the update ONLY if it is the same; otherwise, it returns error.

>> [!note]
>> For replication scenarios, the steps are the same, with the exception of step 2, which might not be a UI, but a time frame between two synchronizations.

By implementing the above scenario, the app guarantees that it would not overwrite other apps changes if such have occurred in @@name between step 1 and step 3.

>> [!note]
>> It is the decision of the app to provide Object version when updating the entity.
>> If Object version is not provided, the system would not perform the optimistic locking checking.
