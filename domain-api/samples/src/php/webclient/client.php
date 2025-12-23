<?php

ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);
error_reporting(E_ALL | E_STRICT);

date_default_timezone_set('Europe/Sofia');

include("../ErpNetClient.php");

$erpHostName = "demodb.my.erp.net";

$client = new ErpNetClient(
    // The address of the ErpNet Identity.
    "https://$erpHostName/id",
    // The client_id registered in the demo database as trusted application.
    'ConfidentialDemoClient',
    // The client secret.
    'DEMO',
    // The endpoind that will receive the requested authentication code that must be used to request an access token.
    // Must be registered as allowed 'ImpersonateLoginUrl' in the trusted application definition.
    "http://localhost:5080/erpnet/php/webclient/signin-oidc",
    // The address where the browser will be redirected after sign out. 
    // Must be registered as allowed 'ImpersonateLogoutUrl' in the trusted application definition.
    "http://localhost:5080/erpnet/php/webclient/"    
);

// While developing we may need to not verify SSL certificates.
$client->verifyHost = false;
$client->verifyPeer = false;

?>