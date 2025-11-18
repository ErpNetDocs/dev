# Sign in with @@name Identity Server (Multi-Platform Samples)

This page shows how to use **@@name Identity Server** as an OpenID Connect provider from different platforms.

All samples use the same key parameters:

- **Authority**: `https://id.erp.net/id`
- **Client ID**: `<your-client-id>`
- **Client Secret**: `<your-client-secret>` (confidential clients only)
- **Redirect URI**: `<your-redirect-uri>`
- **Scopes**: `openid profile` (plus `offline_access` if you need refresh tokens)

> [!WARNING]
> The Client ID and Client Secret must be created in @@name Identity Server for your application.  
> They are not public and cannot be reused from another app.

## Public vs. Confidential Clients

@@name supports both **confidential** and **public** OAuth/OIDC clients:

### Confidential Clients

Confidential clients run on a server you control (e.g., backend web apps).  
They can safely store a **Client Secret**.

Characteristics:

- Use **Client ID + Client Secret**
- Typically use the **Authorization Code** or **Hybrid** flow
- Applicable to: server-side web apps, backend services, integrations

### Public Clients

Public clients run in environments where secrets cannot be kept secure:

- Browsers (SPA apps)
- Mobile applications
- Desktop applications

Public clients **do not** use a client secret.

Characteristics:

- **No Client Secret**
- Must use **Authorization Code Flow with PKCE**
- Applicable to: Angular/React/Vue SPAs, mobile apps, desktop apps

### @@name Configuration Requirement

Whether your application is public or confidential is determined by its **Trusted Application** configuration inside the @@name Identity Server.

When you request client registration from @@name, specify:

- If your app is **public** (no secret) -> SPA/mobile/desktop  
- Or **confidential** (with secret) -> backend/server app

### Effect on the Code Samples

In the multi-platform samples on this page:

- **Backend/server examples** include `client_secret`  
- **Public-client examples** should:
  - Omit `client_secret`
  - Use PKCE (code_verifier / code_challenge)
  - Ensure the @@name application is configured as `public`

The rest of the OIDC parameters (Authority, Redirect URI, Scopes, etc.) are identical for both types.

## SPA (JavaScript, `oidc-client-ts`)

This is a front-end-only app running in the browser, using **Authorization Code Flow with PKCE** and **no client secret**.

```bash
npm install oidc-client-ts
```

```ts
// auth.ts
import { UserManager, WebStorageStateStore } from 'oidc-client-ts';

const settings = {
  authority: 'https://id.erp.net/id',
  client_id: '<your-public-client-id>',      // no secret
  redirect_uri: 'https://your-spa.com/auth/callback',
  post_logout_redirect_uri: 'https://your-spa.com/',
  response_type: 'code id_token',
  scope: 'openid profile',
  automaticSilentRenew: true,
  userStore: new WebStorageStateStore({ store: window.localStorage })
};

export const userManager = new UserManager(settings);

// Trigger login
export function login() {
  return userManager.signinRedirect();
}

// Callback handler (on /auth/callback)
export async function handleCallback() {
  const user = await userManager.signinRedirectCallback();
  console.log('Logged in user:', user.profile);
}

// Trigger logout
export function logout() {
  return userManager.signoutRedirect();
}
```

## SPA (Angular, `angular-auth-oidc-client`)

Angular SPA using `angular-auth-oidc-client` with code + PKCE and no secret.

```bash
npm install angular-auth-oidc-client
```

`app.module.ts`:

```ts
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AuthModule, LogLevel } from 'angular-auth-oidc-client';
import { AppComponent } from './app.component';

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    AuthModule.forRoot({
      config: {
        authority: 'https://id.erp.net/id',
        clientId: '<your-public-client-id>', // no secret
        redirectUrl: 'https://your-angular-app.com',
        postLogoutRedirectUri: 'https://your-angular-app.com',
        scope: 'openid profile',
        responseType: 'code id_token',
        silentRenew: true,
        useRefreshToken: true,
        logLevel: LogLevel.Warn
      }
    })
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
```

`app.component.ts`:

```ts
import { Component, OnInit } from '@angular/core';
import { AuthService } from 'angular-auth-oidc-client';

@Component({
  selector: 'app-root',
  template: `
    <button *ngIf="!isAuthenticated" (click)="login()">Login with ERP.net</button>
    <div *ngIf="isAuthenticated">
      <p>Hello, {{ userName }}</p>
      <button (click)="logout()">Logout</button>
    </div>
  `
})
export class AppComponent implements OnInit {
  isAuthenticated = false;
  userName: string | undefined;

  constructor(private authService: AuthService) {}

  ngOnInit() {
    this.authService.checkAuth().subscribe(({ isAuthenticated, userData }) => {
      this.isAuthenticated = isAuthenticated;
      this.userName = userData?.name || userData?.preferred_username;
    });
  }

  login() {
    this.authService.authorize();
  }

  logout() {
    this.authService.logoff();
  }
}
```

## Mobile (React Native, react-native-app-auth)

Example using `react-native-app-auth`, which handles code + PKCE as a **public client** (no secret stored on device).

```bash
npm install react-native-app-auth
```

```ts
// auth.ts (React Native)
import { authorize, refresh, revoke, AuthConfiguration } from 'react-native-app-auth';

const config: AuthConfiguration = {
  issuer: 'https://id.erp.net/id',
  clientId: '<your-public-client-id>',   // no secret
  redirectUrl: 'myapp://auth/callback',  // custom scheme
  scopes: ['openid', 'profile'],
  additionalParameters: {},
  dangerouslyAllowInsecureHttpRequests: false
};

export async function login() {
  const result = await authorize(config);
  // result includes accessToken, idToken, refreshToken (if allowed)
  return result;
}
```

## JavaScript (Node.js + Express, using `openid-client`)

Minimal example showing a login endpoint and callback using the [`openid-client`](https://github.com/panva/node-openid-client) library.

```bash
npm install openid-client express express-session
```

```js
// app.js
const express = require('express');
const session = require('express-session');
const { Issuer, generators } = require('openid-client');

const app = express();

app.use(session({
  secret: 'replace-with-strong-secret',
  resave: false,
  saveUninitialized: false
}));

let client; // will hold the ERP.net OIDC client

async function initClient() {
  const erpnetIssuer = await Issuer.discover('https://id.erp.net/id');
  client = new erpnetIssuer.Client({
    client_id: '<your-client-id>',
    client_secret: '<your-client-secret>',
    redirect_uris: ['https://your-app.com/auth/callback'],
    response_types: ['code id_token'],
  });
}
initClient().catch(console.error);

// Login endpoint
app.get('/login', (req, res) => {
  const codeVerifier = generators.codeVerifier();
  const codeChallenge = generators.codeChallenge(codeVerifier);

  req.session.codeVerifier = codeVerifier;

  const authUrl = client.authorizationUrl({
    scope: 'openid profile',
    code_challenge: codeChallenge,
    code_challenge_method: 'S256'
  });

  res.redirect(authUrl);
});

// Callback endpoint
app.get('/auth/callback', async (req, res, next) => {
  try {
    const params = client.callbackParams(req);
    const tokenSet = await client.callback(
      'https://your-app.com/auth/callback',
      params,
      { code_verifier: req.session.codeVerifier }
    );

    req.session.user = tokenSet.claims();
    res.send(`Hello, ${req.session.user.name || req.session.user.sub}`);
  } catch (err) {
    next(err);
  }
});

app.listen(3000, () => console.log('Listening on http://localhost:3000'));
```

## Java (Spring Boot, Spring Security OAuth2 Client)

Using Spring Boot's built-in OAuth2 client support.

`application.yml`:

```yaml
spring:
  security:
    oauth2:
      client:
        registration:
          erpnet:
            client-id: <your-client-id>
            client-secret: <your-client-secret>
            scope: openid,profile,offline_access
            redirect-uri: "{baseUrl}/login/oauth2/code/erpnet"
            client-name: ERP.net
            authorization-grant-type: authorization_code
        provider:
          erpnet:
            issuer-uri: https://id.erp.net/id
```

Security configuration (Spring Security 5+ lambda style):

```java
import org.springframework.context.annotation.Bean;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.web.SecurityFilterChain;
import org.springframework.context.annotation.Configuration;

@Configuration
public class SecurityConfig {

    @Bean
    SecurityFilterChain filterChain(HttpSecurity http) throws Exception {
        http
            .authorizeHttpRequests(auth -> auth
                .requestMatchers("/", "/public/**").permitAll()
                .anyRequest().authenticated()
            )
            .oauth2Login(oauth2 -> oauth2
                .loginPage("/oauth2/authorization/erpnet") // optional
            )
            .logout(logout -> logout
                .logoutSuccessUrl("/")
            );

        return http.build();
    }
}
```

Once configured, accessing any protected URL will redirect to @@name for login.

## PHP (using jumbojett/openid-connect-php)

Install the library:

```bash
composer require jumbojett/openid-connect-php
```

Minimal example:

```php
<?php
require 'vendor/autoload.php';

use Jumbojett\OpenIDConnectClient;

$oidc = new OpenIDConnectClient(
    'https://id.erp.net/id',
    '<your-client-id>',
    '<your-client-secret>'
);

$oidc->setRedirectURL('https://your-app.com/callback');
$oidc->addScope(['openid', 'profile']);

// Trigger login (redirect to @@name if not authenticated)
$oidc->authenticate();

// Get user info
$userInfo = $oidc->requestUserInfo();

echo 'Hello, ' . htmlspecialchars($userInfo->name ?? $userInfo->sub);
```

Configure your web server so `/callback points` to the same script, or handle it in a dedicated callback file that calls `$oidc->authenticate()`.

## Go (using coreos/go-oidc + golang.org/x/oauth2)

Minimal example with an auth endpoint and callback:

```bash
go get github.com/coreos/go-oidc/v3/oidc
go get golang.org/x/oauth2
```

```go
package main

import (
    "context"
    "fmt"
    "log"
    "net/http"

    "github.com/coreos/go-oidc/v3/oidc"
    "golang.org/x/oauth2"
)

var (
    oauth2Config *oauth2.Config
    verifier     *oidc.IDTokenVerifier
)

func main() {
    ctx := context.Background()

    provider, err := oidc.NewProvider(ctx, "https://id.erp.net/id")
    if err != nil {
        log.Fatal(err)
    }

    oauth2Config = &oauth2.Config{
        ClientID:     "<your-client-id>",
        ClientSecret: "<your-client-secret>",
        RedirectURL:  "https://your-app.com/callback",
        Endpoint:     provider.Endpoint(),
        Scopes:       []string{oidc.ScopeOpenID, "profile"},
    }

    verifier = provider.Verifier(&oidc.Config{ClientID: "<your-client-id>"})

    http.HandleFunc("/login", handleLogin)
    http.HandleFunc("/callback", handleCallback)

    log.Println("Listening on :8080")
    log.Fatal(http.ListenAndServe(":8080", nil))
}

func handleLogin(w http.ResponseWriter, r *http.Request) {
    state := "random-state" // generate real random value in production
    url := oauth2Config.AuthCodeURL(state)
    http.Redirect(w, r, url, http.StatusFound)
}

func handleCallback(w http.ResponseWriter, r *http.Request) {
    ctx := r.Context()

    code := r.URL.Query().Get("code")
    if code == "" {
        http.Error(w, "missing code", http.StatusBadRequest)
        return
    }

    token, err := oauth2Config.Exchange(ctx, code)
    if err != nil {
        http.Error(w, "failed to exchange token: "+err.Error(), http.StatusInternalServerError)
        return
    }

    rawIDToken, ok := token.Extra("id_token").(string)
    if !ok {
        http.Error(w, "no id_token", http.StatusInternalServerError)
        return
    }

    idToken, err := verifier.Verify(ctx, rawIDToken)
    if err != nil {
        http.Error(w, "failed to verify id_token: "+err.Error(), http.StatusInternalServerError)
        return
    }

    var claims struct {
        Sub  string `json:"sub"`
        Name string `json:"name"`
    }
    if err := idToken.Claims(&claims); err != nil {
        http.Error(w, "failed to parse claims: "+err.Error(), http.StatusInternalServerError)
        return
    }

    fmt.Fprintf(w, "Hello, %s (%s)", claims.Name, claims.Sub)
}
```

## Python (Flask + Authlib)

Using [Authlib](https://docs.authlib.org/en/latest/) for OIDC.

```bash
pip install flask authlib
```

```python
from flask import Flask, redirect, url_for, session, request
from authlib.integrations.flask_client import OAuth

app = Flask(__name__)
app.secret_key = 'replace-with-strong-secret'

oauth = OAuth(app)

erpnet = oauth.register(
    name='erpnet',
    client_id='<your-client-id>',
    client_secret='<your-client-secret>',
    server_metadata_url='https://id.erp.net/id/.well-known/openid-configuration',
    client_kwargs={
        'scope': 'openid profile',
    },
)

@app.route('/')
def index():
    user = session.get('user')
    if user:
        return f"Hello, {user.get('name') or user.get('sub')}"
    return '<a href="/login">Login with ERP.net</a>'

@app.route('/login')
def login():
    redirect_uri = url_for('auth', _external=True)
    return erpnet.authorize_redirect(redirect_uri)

@app.route('/auth/callback')
def auth():
    token = erpnet.authorize_access_token()
    userinfo = erpnet.parse_id_token(token)
    session['user'] = userinfo
    return redirect(url_for('index'))

if __name__ == '__main__':
    app.run(debug=True)
```

## .NET / Blazor WebAssembly

`Program.cs` (Blazor WebAssembly):

```csharp
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using YourApp;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("ServerAPI", client =>
    client.BaseAddress = new Uri("https://your-api/"))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("ServerAPI"));

builder.Services.AddOidcAuthentication(options =>
{
    // ERP.net ID configuration
    options.ProviderOptions.Authority = "https://id.erp.net/id";
    options.ProviderOptions.ClientId = "<your-public-client-id>"; // no secret
    options.ProviderOptions.ResponseType = "code id_token";                // code + PKCE
    options.ProviderOptions.DefaultScopes.Add("openid");
    options.ProviderOptions.DefaultScopes.Add("profile");
    // options.ProviderOptions.DefaultScopes.Add("offline_access"); // if allowed

    // Redirect URI is usually auto-derived as:
    // https://your-blazor-app.com/authentication/login-callback
});

await builder.Build().RunAsync();
```

`App.razor`:

```html
<CascadingAuthenticationState>
    <Router AppAssembly="@typeof(App).Assembly">
        <Found Context="routeData">
            <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)">
                <NotAuthorized>
                    <RedirectToLogin />
                </NotAuthorized>
            </AuthorizeRouteView>
        </Found>
        <NotFound>
            <LayoutView Layout="@typeof(MainLayout)">
                <p>Sorry, there's nothing at this address.</p>
            </LayoutView>
        </NotFound>
    </Router>
</CascadingAuthenticationState>
```

## ASP.NET Core (Confidential Web App)

ASP.NET Core MVC / Razor Pages applications run on the **server**, so they are **confidential clients**.  
The app can safely store a **Client Secret** and use the standard Authorization Code or Hybrid flow.

```csharp
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "ErpNet";
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect("ErpNet", options =>
    {
        options.Authority = "https://id.erp.net/id";

        // Must match the confidential client registered in ERP.net ID
        options.ClientId = "<your-client-id>";
        options.ClientSecret = "<your-client-secret>";

        options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
        options.CallbackPath = "/signin-erpnet";
        options.SignedOutCallbackPath = "/signout-erpnet";

        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;

        options.Scope.Add("openid");
        options.Scope.Add("profile");
        // options.Scope.Add("offline_access"); // if you need refresh tokens
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
```

## Summary

All platforms use the same core OIDC parameters:

- Authority: `https://id.erp.net/id`
- Client ID / Client Secret: from @@name
- Redirect URI: your app's callback
- Scopes: openid profile (and others as needed)
- Choose a library for your platform, plug in these values, and you can sign users in with @@name Identity Server.
