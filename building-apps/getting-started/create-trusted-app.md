# Create a Trusted Application

Before an app can authenticate or call @@name APIs, it must be registered in the target @@name instance as a **Trusted Application**.

This registration defines how the instance recognizes your app and what level of access it may request.

## What You Are Doing in This Step

In this step, you will:

- Create a Trusted Application record in the target @@name instance
- Assign a unique application identifier
- Enable the access modes required by your app
- Grant the minimal scopes needed for your integration

You are **not** implementing authentication yet.  
You are defining the app's identity and permissions.

## Required Information

When creating a Trusted Application, you will need to provide:

- A **unique Application URI** that identifies your app  
- The **client type** (public or confidential), based on your app design  
- The **access modes** your app requires (interactive, service, or both)  
- The **scopes** your app is allowed to request  

If your app runs as a background service, you will also need:

- A **system user** with least-privilege permissions

## Configuration Guidance

Use the following rules when configuring your Trusted Application:

- Use a **stable, globally unique** Application URI  
- Grant **only the scopes your app requires**  
- Enable **interactive access** only for user-facing apps  
- Enable **service access** only for background integrations  
- Prefer **confidential clients** unless your app cannot keep a secret  

Avoid enabling options you do not actively need.

## Reference Documentation

This page does not describe all Trusted Application attributes.

For a complete explanation of configuration options, attributes, and security considerations, see  
[Trusted Applications](../../auth/configuration/trusted-apps-access.md).

## Next Step

After registering the Trusted Application, continue with  
[Get an Access Token](./get-access-token.md).
