# Web Client concepts for developers

This topic introduces the concepts needed by developers who extend the ERP.net Web Client or manage its form layouts. It describes the public model used by those features.

## The Web Client and the domain model

The Web Client presents the ERP.net domain model. Development work is therefore described in terms of domain objects and the user-facing applications that present them.

The most important domain concepts are:

- **Repository** — a named collection of related domain objects.
- **Entity object** — one object from an entity repository.
- **Aggregate** — a root entity together with related objects managed as one business unit.
- **Namespace** — the domain-model grouping used to organize repositories and Web Client applications.
- **Category** — a business classification supported by some entity repositories.

The Web Client does not replace these concepts with a separate screen-specific data model. Forms, fields, and repository panels are based on the live domain model and its metadata.

## Applications

An application is a user-facing destination in the Web Client. It provides an entry point for a capability, a business area, or a way to work with domain data.

Examples include:

- a repository navigator;
- an entity object page;
- a main-menu area;
- a dashboard or assistant;
- an application supplied by an external developer.

Applications are organized into menu groups and normally have a name, caption, icon, and address. These are navigation concepts; they do not change the identity of the repositories or objects shown by the application.

### Repository applications

The standard application for working with a repository is a **repository application**. In the Web Client source this application family is internally called “Forms”, but external documentation uses “repository application” because it can host several repositories and several kinds of forms.

A repository application normally:

- represents a domain namespace or namespace area;
- provides navigation to the repositories in that area;
- opens a repository navigator for a collection;
- opens an entity form for one object;
- hosts the panels that present the selected data.

One repository application can handle many repositories. The application name must therefore not be used as a substitute for the repository name.

## Forms

A form is the user-facing presentation of a repository, an entity object, or another Web Client surface. Developers working with extensions or layouts usually encounter two repository form kinds.

### Navigator forms

A navigator form presents a collection of objects from a repository. It commonly includes filtering, sorting, selection, and one or more repository data panels.

The focused or selected object in a navigator can provide context for opening an entity form or for a context-sensitive feature.

### Entity forms

An entity form presents one entity object and related information from its aggregate. It commonly includes an object header, fields, related collections, files, history, and other object-specific panels.

An entity form is identified by its repository and object identity. A new-object form may additionally be identified by a category or other creation context.

### Form identity

When a feature addresses a form, its identity can include:

- the repository;
- the form kind, such as navigator or entity;
- the object identity for an entity form;
- the category, when the repository supports categories;
- the selected view mode;
- the active role when role-specific behavior is relevant.

The exact identity fields depend on the feature being used. A repository name alone is sufficient for some navigator operations but is not sufficient to identify a particular entity form.

## Panels

A panel is a named functional area inside a form. Panels present or operate on a specific part of the domain model or provide a supporting capability.

Examples include:

- repository data grids;
- object fields;
- related-object lists;
- document flow;
- files;
- history and messages;
- filters;
- charts, pivots, and kanban views.

### Panel registrations and panel instances

A form has a set of registered panels. A registration describes a panel that the form can provide, including its name and availability. The actual panel is created only when the form needs to display or use it.

This distinction matters to external developers:

- a registered panel may be available even when it is not currently visible;
- a registered panel can be restricted to particular form view modes;
- the full properties and fields of a panel are defined by its live panel implementation;
- panel names are the stable identifiers used when a feature addresses a specific panel.

The number of registered panels can be large, especially for entity forms with many related-data options. Discovering a panel and inspecting its schema are therefore separate concerns.

### Panel availability

Panel availability is determined by the form and its view mode. A panel can be registered for a form but unavailable in a particular mode. A feature should use the form’s reported availability rather than assuming that every registered panel can be shown everywhere.

## Extension points

An extension point is a location or capability exposed by a Web Client application for additional functionality. The main application defines the extension point and its contract; an external application supplies the extension.

The contract of an extension point defines:

- what can be registered;
- where the extension appears;
- which context is supplied to it;
- how its configuration is represented;
- which validation and security rules apply.

The same general extension model can support different Web Client locations, such as a menu application or an application hosted in a form area. The configuration and behavior are specific to the selected extension point.

External developers should treat the extension-point documentation as authoritative. A registration valid for one point is not automatically valid for another point.

## Layouts

A layout is the user-facing arrangement and presentation configuration of a form and its panels. Layout management changes how a form is presented without changing the underlying domain model.

Depending on the form, a layout can describe:

- which panels are visible;
- panel placement and order;
- visible fields or grid columns;
- sizes and widths;
- panel-specific presentation settings.

Layouts can be common or role-specific. They can also be associated with entity categories. A category layout may reuse another category layout, and a fallback layout can be applied when a category has no explicit layout.

The effective layout is the result selected for the current form, category, and role. It can therefore differ from one stored layout record when inheritance or fallback rules apply.

## Roles and categories

Role and category are independent dimensions:

- a **role** describes the user context for which a layout is managed;
- a **category** describes the business classification of an entity object or new-object form.

A common layout applies without a role-specific override. A role layout applies when the active role has a matching configuration. A category layout applies when the form’s object or creation context belongs to that category.

When designing a layout-management operation, always identify which dimensions it targets. A repository, role, and category can each affect the resulting form layout.

## Related topics

- [Layout API](layout-api.md)
- [Web Client registered panels](registered-panels.md)
- [Web Client layout properties](layout-properties.md)
