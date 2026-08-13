const DEFAULT_SCOPES = ["openid", "profile", "offline_access", "read"];

export class ErpNetAuthenticationError extends Error {
  constructor(message, code = null) {
    super(message);
    this.name = "ErpNetAuthenticationError";
    this.code = code;
  }
}

export class ErpNetClient {
  constructor(options) {
    this.options = {
      scopes: DEFAULT_SCOPES,
      ...options
    };
    this.authority = null;
    this.odataRoot = null;
    this.authStorageKey = `erpnet.auth.${this.options.clientId}`;
    this.transactionStorageKey = `erpnet.oauth.transaction.${this.options.clientId}`;
    this.discoveryStorageKey = `erpnet.discovery.${encodeURIComponent(this.options.instance ?? "")}`;
    this.auth = this.readStorage(this.authStorageKey);
    this.redirecting = false;
    this.initialized = false;
    this.authenticationPromise = null;
  }

  async init() {
    if (this.initialized)
      return;

    this.validateOptions();
    await this.discover();

    const params = new URL(window.location.href).searchParams;
    if (params.get("auth") === "callback" || params.has("code") || params.has("error")) {
      try {
        await this.handleAuthenticationCallback();
      } catch (error) {
        this.notifyOpener({
          type: "erpnet.oauth.result",
          status: "error",
          code: error.code ?? "authentication_error",
          message: error.message
        });
        this.closePopup();
        throw error;
      }
    }

    this.initialized = true;
  }

  validateOptions() {
    for (const name of ["instance", "clientId", "redirectUri"]) {
      if (!this.options[name])
        throw new ErpNetAuthenticationError(`Missing ERP.net client option: ${name}.`);
    }

    new URL(this.options.redirectUri);
  }

  async discover() {
    const cachedDiscovery = this.readStorage(this.discoveryStorageKey);
    if (cachedDiscovery?.authority && cachedDiscovery?.odataRoot) {
      this.authority = cachedDiscovery.authority;
      this.odataRoot = cachedDiscovery.odataRoot;
      return;
    }

    const instanceUrl = this.asHttpsUrl(this.options.instance);
    const response = await fetch(`${instanceUrl}/sys/auto-discovery`, {
      headers: { Accept: "application/json" }
    });

    if (!response.ok)
      throw new Error(`ERP.net auto-discovery failed with HTTP ${response.status}.`);

    const discovery = await response.json();
    const idSite = discovery.WebSites?.find(site =>
      site.Type === "ID" && site.Status === "Working");
    const apiSite = discovery.WebSites?.find(site =>
      site.Type === "DomainAPI" && site.Status === "Working");

    if (!idSite?.Url || !apiSite?.AdditionalProperties?.ODataServiceRoot)
      throw new Error("The ERP.net instance has no working Identity and Domain API sites.");

    this.authority = this.normalizeBaseUrl(idSite.Url);
    this.odataRoot = this.normalizeBaseUrl(apiSite.AdditionalProperties.ODataServiceRoot);
    this.writeStorage(this.discoveryStorageKey, {
      authority: this.authority,
      odataRoot: this.odataRoot
    });
  }

  async signIn(returnUrl = this.currentReturnUrl()) {
    if (this.authenticationPromise)
      return this.authenticationPromise;

    const state = this.randomString();
    const nonce = this.randomString();
    const codeVerifier = this.randomString(64);
    const codeChallenge = await this.sha256Base64Url(codeVerifier);

    this.writeStorage(this.transactionStorageKey, {
      state,
      nonce,
      codeVerifier,
      returnUrl: this.safeReturnUrl(returnUrl),
      createdAt: Date.now()
    });

    const authorizeUrl = new URL(`${this.authority}/connect/authorize`);
    authorizeUrl.search = new URLSearchParams({
      client_id: this.options.clientId,
      redirect_uri: this.options.redirectUri,
      response_type: "code",
      response_mode: "query",
      scope: this.options.scopes.join(" "),
      state,
      nonce,
      code_challenge: codeChallenge,
      code_challenge_method: "S256"
    });

    const popup = window.open(
      authorizeUrl.href,
      "erpnet-extension-auth",
      "popup,width=520,height=720,resizable=yes,scrollbars=yes");
    if (!popup)
      throw new ErpNetAuthenticationError(
        "The authentication popup was blocked. Allow popups for this extension.",
        "popup_blocked");

    this.authenticationPromise = new Promise((resolve, reject) => {
      let timeoutId;
      const cleanup = () => {
        window.removeEventListener("message", onMessage);
        window.clearTimeout(timeoutId);
        this.authenticationPromise = null;
      };
      const onMessage = event => {
        if (event.origin !== window.location.origin
          || event.source !== popup
          || event.data?.type !== "erpnet.oauth.result")
          return;

        if (event.data.status === "callback") {
          this.exchangeAuthorizationCode(event.data.code, event.data.state)
            .then(auth => {
              cleanup();
              resolve(auth);
            })
            .catch(error => {
              cleanup();
              reject(error);
            });
          return;
        }

        cleanup();
        if (event.data.status === "success") {
          this.auth = event.data.auth;
          this.writeStorage(this.authStorageKey, this.auth);
          resolve(this.auth);
        } else {
          reject(new ErpNetAuthenticationError(
            event.data.message ?? "ERP.net authentication failed.",
            event.data.code ?? "authentication_error"));
        }
      };

      window.addEventListener("message", onMessage);
      timeoutId = window.setTimeout(() => {
        cleanup();
        reject(new ErpNetAuthenticationError(
          "The ERP.net authentication popup did not complete.",
          "authentication_timeout"));
      }, 5 * 60 * 1000);
    });

    return this.authenticationPromise;
  }

  async handleAuthenticationCallback() {
    const params = new URL(window.location.href).searchParams;
    const error = params.get("error");
    if (error) {
      const description = params.get("error_description");
      const isInvalidClient = error === "invalid_client" ||
        /invalid.?client|client.?not.?found|unknown.?client/i.test(description ?? "");
      const authenticationError = new ErpNetAuthenticationError(
        `ERP.net authorization failed: ${error}${description ? ` - ${description}` : ""}`,
        isInvalidClient ? "invalid_client" : error);
      if (this.notifyOpener({
        type: "erpnet.oauth.result",
        status: "error",
        code: authenticationError.code ?? "authentication_error",
        message: authenticationError.message
      })) {
        this.closePopup();
        return;
      }
      throw authenticationError;
    }

    const code = params.get("code");
    const returnedState = params.get("state");
    if (!code || !returnedState)
      throw new ErpNetAuthenticationError("The ERP.net authorization callback is incomplete.");

    if (this.notifyOpener({
      type: "erpnet.oauth.result",
      status: "callback",
      code,
      state: returnedState
    })) {
      this.closePopup();
      return;
    }

    const returnUrl = this.readStorage(this.transactionStorageKey)?.returnUrl;
    const auth = await this.exchangeAuthorizationCode(code, returnedState);
    window.location.replace(this.safeReturnUrl(returnUrl));
    this.redirecting = true;
    return auth;
  }

  async exchangeAuthorizationCode(code, returnedState) {
    if (!code || !returnedState)
      throw new ErpNetAuthenticationError("The ERP.net authorization callback is incomplete.");

    const transaction = this.readStorage(this.transactionStorageKey);
    this.removeStorage(this.transactionStorageKey);
    if (!transaction || !this.isFreshTransaction(transaction) || transaction.state !== returnedState)
      throw new ErpNetAuthenticationError("The ERP.net authorization state is invalid or expired.");

    const body = new URLSearchParams({
      client_id: this.options.clientId,
      grant_type: "authorization_code",
      code,
      redirect_uri: this.options.redirectUri,
      code_verifier: transaction.codeVerifier
    });
    const response = await fetch(`${this.authority}/connect/token`, {
      method: "POST",
      headers: { "Content-Type": "application/x-www-form-urlencoded" },
      body
    });

    if (!response.ok) {
      const errorResponse = await this.readErrorResponse(response);
      const code = errorResponse.error ?? `http_${response.status}`;
      const message = errorResponse.error_description ??
        `ERP.net token request failed with HTTP ${response.status}.`;
      throw new ErpNetAuthenticationError(message, code === "invalid_client" ? code : null);
    }

    const auth = await response.json();
    this.validateTokenResponse(auth, transaction.nonce);
    this.auth = auth;
    this.writeStorage(this.authStorageKey, auth);
    return auth;
  }

  async readErrorResponse(response) {
    try {
      const body = await response.json();
      return body && typeof body === "object" ? body : {};
    } catch {
      return {};
    }
  }

  async getValidAccessToken() {
    const accessToken = this.auth?.access_token;
    if (accessToken && this.tokenIsUsable(accessToken))
      return accessToken;

    if (!this.auth?.refresh_token) {
      this.clearAuth();
      return null;
    }

    try {
      const body = new URLSearchParams({
        client_id: this.options.clientId,
        grant_type: "refresh_token",
        refresh_token: this.auth.refresh_token
      });
      const response = await fetch(`${this.authority}/connect/token`, {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body
      });

      if (!response.ok)
        throw new Error(`ERP.net token refresh failed with HTTP ${response.status}.`);

      const auth = await response.json();
      this.validateTokenResponse(auth);
      this.writeStorage(this.authStorageKey, {
        ...this.auth,
        ...auth
      });
      return this.auth.access_token;
    } catch (error) {
      this.clearAuth();
      throw new ErpNetAuthenticationError(error instanceof Error ? error.message : String(error));
    }
  }

  async api(path, options = {}) {
    await this.init();
    let accessToken = await this.getValidAccessToken();
    if (!accessToken)
      throw new ErpNetAuthenticationError(
        "Authentication is required. Click the sign-in button.",
        "authentication_required");

    const request = {
      method: "GET",
      headers: {
        Accept: "application/json",
        Authorization: `Bearer ${accessToken}`,
        ...(options.body ? { "Content-Type": "application/json" } : {})
      },
      ...options
    };
    request.body = options.body && typeof options.body !== "string"
      ? JSON.stringify(options.body)
      : options.body;

    let response = await this.fetchWithRateLimit(this.apiUrl(path), request);
    if (response.status === 401 && this.auth?.refresh_token) {
      this.auth.access_token = null;
      accessToken = await this.getValidAccessToken();
      request.headers.Authorization = `Bearer ${accessToken}`;
      response = await this.fetchWithRateLimit(this.apiUrl(path), request);
    }

    if (!response.ok) {
      const error = new Error(`ERP.net Domain API returned HTTP ${response.status}.`);
      error.status = response.status;
      throw error;
    }

    return response.status === 204 ? null : response.json();
  }

  async fetchWithRateLimit(url, request, maxRetries = 2) {
    for (let attempt = 0; ; attempt++) {
      const response = await fetch(url, request);
      if (response.status !== 429 || attempt >= maxRetries)
        return response;

      const retryAfter = Number(response.headers.get("Retry-After"));
      const delay = Number.isFinite(retryAfter) && retryAfter > 0
        ? Math.min(retryAfter * 1000, 10_000)
        : Math.min(1_000 * 2 ** attempt, 10_000);
      await new Promise(resolve => window.setTimeout(resolve, delay));
    }
  }

  apiUrl(path) {
    const url = new URL(path.replace(/^\//, ""), `${this.odataRoot}/`);
    const root = `${this.odataRoot}/`;
    if (!url.href.startsWith(root))
      throw new Error("Domain API request path is outside the discovered API root.");
    return url.href;
  }

  validateTokenResponse(auth, expectedNonce) {
    if (!auth?.access_token)
      throw new ErpNetAuthenticationError("ERP.net returned no access token.");

    const accessPayload = this.parseJwt(auth.access_token);
    if (accessPayload.exp && accessPayload.exp <= Math.floor(Date.now() / 1000))
      throw new ErpNetAuthenticationError("ERP.net returned an already expired access token.");

    if (auth.id_token) {
      const idPayload = this.parseJwt(auth.id_token);
      const audience = Array.isArray(idPayload.aud) ? idPayload.aud : [idPayload.aud];
      if (this.normalizeBaseUrl(idPayload.iss) !== this.authority
        || !audience.includes(this.options.clientId)
        || (expectedNonce && idPayload.nonce !== expectedNonce))
        throw new ErpNetAuthenticationError("ERP.net returned an invalid ID token.");
    }
  }

  tokenIsUsable(token) {
    const payload = this.parseJwt(token);
    return typeof payload.exp === "number"
      && payload.exp > Math.floor(Date.now() / 1000) + 60;
  }

  parseJwt(token) {
    try {
      const parts = token.split(".");
      if (parts.length !== 3)
        throw new Error("Invalid JWT.");
      const base64 = parts[1].replace(/-/g, "+").replace(/_/g, "/");
      const json = decodeURIComponent(atob(base64.padEnd(Math.ceil(base64.length / 4) * 4, "=")
        ).split("").map(char => `%${(`00${char.charCodeAt(0).toString(16)}`).slice(-2)}`).join(""));
      return JSON.parse(json);
    } catch {
      throw new ErpNetAuthenticationError("ERP.net returned an invalid JWT.");
    }
  }

  async sha256Base64Url(value) {
    const digest = await crypto.subtle.digest("SHA-256", new TextEncoder().encode(value));
    return this.base64Url(new Uint8Array(digest));
  }

  randomString(byteLength = 32) {
    const bytes = new Uint8Array(byteLength);
    crypto.getRandomValues(bytes);
    return this.base64Url(bytes);
  }

  base64Url(bytes) {
    let binary = "";
    for (const byte of bytes)
      binary += String.fromCharCode(byte);
    return btoa(binary).replace(/\+/g, "-").replace(/\//g, "_").replace(/=+$/, "");
  }

  currentReturnUrl() {
    return `${window.location.pathname}${window.location.search}${window.location.hash}`;
  }

  safeReturnUrl(value) {
    if (typeof value !== "string")
      return this.options.appUrl;

    try {
      const url = new URL(value, window.location.origin);
      return url.origin === window.location.origin
        ? url.href
        : this.options.appUrl;
    } catch {
      return this.options.appUrl;
    }
  }

  isFreshTransaction(transaction) {
    return typeof transaction.createdAt === "number"
      && Date.now() - transaction.createdAt < 10 * 60 * 1000;
  }

  asHttpsUrl(value) {
    const url = new URL(value.includes("://") ? value : `https://${value}`);
    if (url.protocol !== "https:")
      throw new ErpNetAuthenticationError("ERP.net must be accessed over HTTPS.");
    return this.normalizeBaseUrl(url.href);
  }

  normalizeBaseUrl(value) {
    return String(value).replace(/\/+$/, "");
  }

  readStorage(key) {
    try {
      return JSON.parse(this.storageForKey(key).getItem(key) ?? "null");
    } catch {
      return null;
    }
  }

  writeStorage(key, value) {
    this.storageForKey(key).setItem(key, JSON.stringify(value));
  }

  removeStorage(key) {
    this.storageForKey(key).removeItem(key);
  }

  storageForKey(key) {
    return sessionStorage;
  }

  clearAuth() {
    this.auth = null;
    this.removeStorage(this.authStorageKey);
  }

  notifyOpener(message) {
    if (!window.opener || window.opener === window)
      return false;

    window.opener.postMessage(message, new URL(this.options.appUrl).origin);
    return true;
  }

  closePopup() {
    this.redirecting = true;
    window.close();
  }
}
