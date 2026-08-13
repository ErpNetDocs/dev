import { ErpNetAuthenticationError, ErpNetClient } from "./erpnet-client.js";

const CLIENT_ID = "erpnet.demo-extension.3e1";
const pageUrl = new URL(window.location.href);
const instance = pageUrl.searchParams.get("instance")?.trim() || null;
const appUrl = new URL("./", pageUrl).href;
const redirectUrl = new URL(appUrl);
redirectUrl.searchParams.set("instance", instance ?? "");
redirectUrl.searchParams.set("auth", "callback");
const client = new ErpNetClient({ instance, appUrl, clientId: CLIENT_ID, redirectUri: redirectUrl.href });

const el = {
  status: document.querySelector("#status"), setup: document.querySelector("#setup"), setupUrl: document.querySelector("#setup-url"),
  signIn: document.querySelector("#sign-in"), retry: document.querySelector("#retry"), context: document.querySelector("#context"),
  arguments: document.querySelector("#arguments")
};
const displayTextCache = new Map();
const CACHE_TTL = 30_000;
let lastMessage = null, currentContext = new Map(), objectController = null, retryAction = null;

function logError(context, error) { console.error(`[ERP.net form context] ${context}`, error); }
window.addEventListener("error", e => logError("Uncaught browser error", e.error ?? e.message));
window.addEventListener("unhandledrejection", e => logError("Unhandled promise rejection", e.reason));
function setStatus(message, error = false) { el.status.textContent = message; el.status.classList.toggle("error", error); }
function showRetry(action) { retryAction = action; el.retry.hidden = false; }
function hideRetry() { retryAction = null; el.retry.hidden = true; }
function showSignIn(message = "Sign in to ERP.net to load repository display text.") { el.signIn.hidden = false; el.signIn.disabled = false; setStatus(message, true); }
function showAuthError(error) { logError("Authentication error", error); showSignIn(error.code === "invalid_client" ? "The trusted application is not registered for this instance." : error.message); }
function parseMessage(message) { return new Map(new URLSearchParams(message).entries()); }
function renderArguments(context) {
  el.arguments.replaceChildren();
  for (const [name, value] of context) {
    const dt = document.createElement("dt"), dd = document.createElement("dd");
    dt.textContent = `$${name}`; dd.textContent = value === "" ? "(empty)" : value; el.arguments.append(dt, dd);
    if (name === "id" && value) {
      const entityLabel = document.createElement("dt"), entityValue = document.createElement("dd");
      entityLabel.textContent = "ENTITY";
      entityValue.id = "entity-display-text";
      entityValue.textContent = "Loading…";
      el.arguments.append(entityLabel, entityValue);
    }
  }
  if (!context.size) { const dd = document.createElement("dd"); dd.textContent = "No arguments were provided."; el.arguments.append(dd); }
}
function renderObject(displayText) {
  const entityDisplayText = document.querySelector("#entity-display-text");
  if (entityDisplayText)
    entityDisplayText.textContent = displayText || "(empty)";
}

async function loadDisplayText(repository, id, force = false) {
  if (!repository || !id) return;
  const key = `${instance ?? ""}|${repository}|${id}`, cached = displayTextCache.get(key);
  if (!force && cached?.expiresAt > Date.now()) { renderObject(cached.value); return; }
  objectController?.abort(); const controller = new AbortController(); objectController = controller;
  renderObject("Loading…"); setStatus("Loading entity display text from the Domain API…"); hideRetry();
  try {
    const entitySet = repository.replaceAll(".", "_");
    const query = new URLSearchParams({ "$select": "Id,DisplayText" });
    const object = await client.api(`${entitySet}(${encodeURIComponent(id)})?${query}`, { signal: controller.signal });
    const value = object?.DisplayText ?? ""; displayTextCache.set(key, { value, expiresAt: Date.now() + CACHE_TTL }); renderObject(value); setStatus("Context received from the ERP.net Web Client.");
  } catch (error) {
    if (error?.name === "AbortError") return; logError("Repository display-text request failed", error);
    if (error instanceof ErpNetAuthenticationError) { showAuthError(error); return; }
    renderObject("Unavailable"); setStatus(error instanceof Error ? error.message : String(error), true); showRetry(() => loadDisplayText(repository, id, true));
  } finally { if (objectController === controller) objectController = null; }
}
function handleContext(message) {
  if (message === lastMessage) return; lastMessage = message; currentContext = parseMessage(message); el.context.hidden = false; renderArguments(currentContext); setStatus("Context received from the ERP.net Web Client.");
  const repository = currentContext.get("repository"), id = currentContext.get("id");
  if (repository && id) loadDisplayText(repository, id); else objectController?.abort();
}
window.addEventListener("message", event => { if (event.data?.type === "erpnet.extension.message") handleContext(String(event.data.data ?? "")); });
async function signIn() {
  el.signIn.disabled = true; setStatus("Opening ERP.net sign-in…");
  try { await client.signIn(); el.signIn.hidden = true; if (lastMessage != null) { const message = lastMessage; lastMessage = null; handleContext(message); } }
  catch (error) { logError("Sign-in failed", error); showAuthError(error instanceof Error ? error : new Error(String(error))); }
  finally { el.signIn.disabled = false; }
}
async function start() {
  if (!instance) { el.setup.hidden = false; el.setupUrl.textContent = `${window.location.origin}${window.location.pathname}?instance=<instance>`; setStatus("An ERP.net instance is required.", true); return; }
  try { await client.init(); if (client.redirecting) return; if (!client.auth?.access_token) showSignIn(); }
  catch (error) { logError("Extension startup failed", error); if (error instanceof ErpNetAuthenticationError) showAuthError(error); else setStatus(error instanceof Error ? error.message : String(error), true); }
}
el.signIn.addEventListener("click", signIn); el.retry.addEventListener("click", () => { const action = retryAction; hideRetry(); action?.(); }); start();
