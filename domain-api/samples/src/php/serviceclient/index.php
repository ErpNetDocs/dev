<?php

ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);
error_reporting(E_ALL | E_STRICT);

date_default_timezone_set('Europe/Sofia');

include("../ErpNetClient.php");

$erpHostName = "demodb.my.erp.net";
//$erpHostName = "e1-nstable.local";

$client = new ErpNetClient(
    // The address of the ErpNet Identity Server.
    "https://$erpHostName/id",
    // The client_id registered in the demo database as trusted application.
    'ServiceDemoClient',
    // The client secret.
    'DEMO'
);

// While developing we may need to not verify SSL certificates.
$client->verifyHost = false;
$client->verifyPeer = false;

// We need an access_token issued by the erp database instance to access api resources.
// Each access_token occupies one license on Erp Application Server so we need to reuse the 
// issued access_token in order to keep occupied licenses as low as possible.

// First check the stored access_token. 
$accessTokenIsValid = false;
$accessTokenFile = "access_token.txt";
if(file_exists(($accessTokenFile)))
{
    $accessToken = file_get_contents($accessTokenFile);
    $accessTokenIsValid = $client->setAccessToken($accessToken);
}

if(!$accessTokenIsValid)
{
    // request new access_token from identity server. 
    $client->requestClientCredentialsToken();
    // store the access_token locally to reuse it.
    file_put_contents($accessTokenFile, $client->accessToken);
}

$claims = $client->getClaims();

// Print the access token claims
echo "Access Token Claims:\r\n";

print_r($claims);

// Load producs from domain api

echo "Products:\r\n";
$products_endpoint =  "https://$erpHostName/api/domain/odata/General_Products_Products?\$top=10";

$headers = $headers = ["Authorization: Bearer $client->accessToken"];

$json = $client->fetchURL($products_endpoint, $headers);
$products = json_decode($json);    

print_r($products);  


?>