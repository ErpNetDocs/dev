# Incremental pull by last update time

## Overview

Some integrations need to periodically pull only the entities that have changed since the last successful synchronization run (incremental sync).

Starting from v26.2, aggregate root entities expose a calculated attribute `AggregateLastUpdateTimeUtc`, which can be used to implement such incremental pulls via Domain API.

## Getting started

### 1) Select the last update timestamp

To retrieve the value, explicitly select `AggregateLastUpdateTimeUtc`:

