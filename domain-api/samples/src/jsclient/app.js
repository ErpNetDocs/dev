// A helper function to log messages to the results <pre>.
function log() {
    document.getElementById('results').innerText = '';

    Array.prototype.forEach.call(arguments, function (msg) {
        if (msg instanceof Error) {
            msg = "Error: " + msg.message;
        }
        else if (typeof msg !== 'string') {
            msg = JSON.stringify(msg, null, 2);
        }
        document.getElementById('results').innerHTML += msg + '\r\n';
    });
}

// Register click event handlers to the three buttons:
document.getElementById("login").addEventListener("click", login, false);
document.getElementById("api").addEventListener("click", api, false);
document.getElementById("logout").addEventListener("click", logout, false);

// Use the UserManager class from the oidc-client library to manage the OpenID Connect protocol.
var config = {
    authority: "https://demodb.my.erp.net/id",
    client_id: "PublicDemoClient",
    redirect_uri: "http://localhost:5080/erpnet/jsclient/callback.html",
    response_type: "code",
    scope:"openid profile",
    post_logout_redirect_uri : "http://localhost:5080/erpnet/jsclient/index.html",
};
var mgr = new Oidc.UserManager(config);

// The UserManager provides a getUser API to know if the user is logged into the JavaScript application. 
// It uses a JavaScript Promise to return the results asynchronously. 
// The returned User object has a profile property which contains the claims for the user.
// Detect if the user is logged into the JavaScript application:
mgr.getUser().then(function (user) {
    if (user) {
        log("User logged in", user.profile);
    }
    else {
        log("User not logged in");
    }
});

// Implement the login, api, and logout functions.
// The UserManager provides a signinRedirect to log the user in, and a signoutRedirect to log the user out.
// The User object that we obtained in the above code also has an access_token property which can be used to authenticate to a web API.
// The access_token will be passed to the web API via the Authorization header with the Bearer scheme.
function login() {
    mgr.signinRedirect();
}

function api() {
    mgr.getUser().then(function (user) {
        var url = "https://demodb.my.erp.net/api/domain/odata/General_Products_Products?$top=10";

        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.onload = function () {
            log(xhr.status, JSON.parse(xhr.responseText));
        }
        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();
    });
}

function logout() {
    mgr.signoutRedirect();
}
