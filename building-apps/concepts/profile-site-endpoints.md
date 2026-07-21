# Profile Site Endpoints

The **Profile site** provides the authenticated user with profile, security, access-token, and device-management functionality.

In addition to its browser user interface, the site exposes a small HTTP API that applications can use to identify the current @@name user and retrieve the user's profile picture.

By default, the Profile site is available under `/profile`:

```http
https://<my-instance>.my.erp.net/profile
```

The path can be changed in the instance configuration and is not guaranteed to be `/profile`. Applications must discover the actual URL through the `/tools/auto-discovery` endpoint described in [Application Server Endpoints](application-server-endpoints.md) and select the site whose type is `UserProfile`.

## Authentication

All Profile API endpoints described here require authentication.

The following authentication methods are intended for normal use:

- **Authentication cookie** - Used by browsers already signed in to the Profile site.
- **Bearer access token** - Used by external applications and services. Send the token in the `Authorization` header.

```http
Authorization: Bearer <access_token>
```

The access token can be a JWT access token or a supported reference access token. The returned information always describes the @@name user represented by the authenticated request.

### Obtaining an access token

Start with [Get an Access Token](../getting-started/get-access-token.md) for a guided setup, or see [Choosing the Right Flow](../../auth/flows/choosing-flow.md) to select the appropriate authentication flow.

Common options are:

- [Authorization Code flow](../../auth/flows/auth-code/interactive-apps-step-by-step.md) for an interactive application acting on behalf of a signed-in user.
- A Personal Access Token (PAT), described under [Reference Access Tokens](../../auth/tokens/reference-access-tokens.md), for long-lived access representing the user who issued it.

> [!WARNING]
> The Client Credentials flow and Service Access Tokens (SATs) authenticate a service through its configured System User. They do not provide impersonated user access and cannot be used with the Profile `/me` endpoints.

For details about JWT format, lifetime, and claims, see [Access Tokens](../../auth/tokens/access-tokens.md).

## Current User - `/me`

Returns information about the authenticated @@name user, the current instance, and useful Profile-site links.

```http
GET https://<profile-site-url>/me
```

### Bearer token example

```http
GET https://<my-instance>.my.erp.net/profile/me
Authorization: Bearer <access_token>
Accept-Language: en
```

### Browser cookie example

```http
GET https://<my-instance>.my.erp.net/profile/me
Cookie: <profile-authentication-cookie>
```

### Response example

```json
{
  "id": "2e91c669-1c96-4e42-bafb-663d883abca1",
  "login": "john.smith",
  "name": "John Smith",
  "email": "john.smith@example.com",
  "phoneNumber": "+359000000000",
  "isAdmin": false,
  "isActive": true,
  "userType": "InternalUser",
  "preferredLanguage": "en",
  "pictureUrl": "https://<my-instance>.my.erp.net/profile/me/picture/64",
  "largePictureUrl": "https://<my-instance>.my.erp.net/profile/me/picture/128",
  "person": {
    "id": "fd2e789e-9f0f-4f31-910b-9209acdd038f",
    "name": "John Smith"
  },
  "domain": {
    "id": "79c465c7-1368-45b2-b4a9-775a63801dbe",
    "name": "default"
  },
  "instance": {
    "database": "MY_DATABASE",
    "url": "https://<my-instance>.my.erp.net"
  },
  "links": {
    "profile": "https://<my-instance>.my.erp.net/profile/",
    "changePassword": "https://<my-instance>.my.erp.net/profile/security/password",
    "accessTokens": "https://<my-instance>.my.erp.net/profile/security/tokens",
    "devices": "https://<my-instance>.my.erp.net/profile/devices"
  }
}
```

### Properties

| Field | Type | Notes |
|-------|------|-------|
| **id** | `guid` | Unique @@name user identifier. |
| **login** | `string` | @@name user login. |
| **name** | `string` | Localized display name. |
| **email** | `string \| null` | User email address, when available. |
| **phoneNumber** | `string \| null` | User phone number, when available. |
| **isAdmin** | `boolean` | Whether the user is an instance administrator. |
| **isActive** | `boolean` | Whether the @@name user account is active. |
| **userType** | `string` | @@name user type. The endpoint supports internal and external community users. |
| **preferredLanguage** | `string` | Language configured for the user. It can differ from the language selected for this response. |
| **pictureUrl** | `string \| null` | Absolute URL of the standard 64 x 64 picture. |
| **largePictureUrl** | `string \| null` | Absolute URL of the large 128 x 128 picture. |
| **person** | `object \| null` | Identifier and localized name of the associated person. |
| **domain** | `object \| null` | Identifier and name of the user's security domain. |
| **instance** | `object` | Database name and instance root URL. |
| **links** | `object` | Absolute links to commonly used Profile-site pages. |

`pictureUrl` and `largePictureUrl` are `null` when the corresponding custom picture is not available. The same applies to `person` and `domain` when the user has no associated object.

The `/me` response contains user-specific information and is returned with `Cache-Control: no-store`.

## Response language

Multilanguage values such as `name` and `person.name` are resolved for the response language.

For external applications, specify the preferred response language with the standard `Accept-Language` header:

```http
Accept-Language: bg
```

The response includes a `Content-Language` header indicating the selected language.

When the request does not explicitly select a culture, the Profile site uses:

1. The user's `preferredLanguage`.
2. The database default language.
3. The current server request culture.

If an exact translation is unavailable, the platform applies its normal language fallback and transliteration behavior.

## User Picture - `/me/picture`

Returns the authenticated user's picture. Both `GET` and `HEAD` are supported.

### Default picture

Returns the standard 64 x 64 picture:

```http
GET https://<profile-site-url>/me/picture
Authorization: Bearer <access_token>
```

### Picture by size

```http
GET https://<profile-site-url>/me/picture/64
Authorization: Bearer <access_token>
```

Supported sizes are:

| Size | Endpoint |
|------|----------|
| 64 x 64 | `/me/picture/64` |
| 128 x 128 | `/me/picture/128` |

The picture endpoint can return:

| Status | Meaning |
|--------|---------|
| `200 OK` | Embedded picture bytes. The response includes the stored media type. |
| `302 Found` | The picture is stored at an external URL. Follow the redirect. |
| `401 Unauthorized` | The request is not authenticated. |
| `403 Forbidden` | The authenticated @@name user type is not supported. |
| `404 Not Found` | No picture exists, or the requested size is not supported. |
| `413 Content Too Large` | The stored picture exceeds the accepted content limit. |

Picture responses are private and can be cached for up to two hours. They vary by the `Authorization` and `Cookie` request headers.

> [!IMPORTANT]
> The picture URLs are protected resources. A service using Bearer authentication must send the same `Authorization` header when retrieving a picture. A browser `<img>` element cannot add a Bearer header, so bearer-only clients should download the picture themselves or serve it through an authenticated backend. Cookie-authenticated browser sessions can use the URL directly.

## Typical use

An external application can use the Profile API as follows:

1. Discover the `UserProfile` site URL from `/tools/auto-discovery`.
2. Obtain an access token representing the required @@name user.
3. Call `GET <profile-site-url>/me` with the Bearer token.
4. Use `id` as the stable user identifier and `login` or `name` for display purposes.
5. Retrieve `pictureUrl` or `largePictureUrl` with the same authentication when a picture is available.

Do not use `name`, `email`, or `login` as a stable identifier. These values can change; use `id` when storing a reference to the @@name user.
