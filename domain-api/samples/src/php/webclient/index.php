<?php

// Include the ErpNetClient initialization.
include("client.php");


$action = $_GET["action"] ?? null;

// Sign Out?
if($action == 'signout')
{
    $redirecturi = $client->applicationSignOutUri;
    $client->signOut($redirecturi);
    exit;    
}

$debug = false;
if($debug)
{
    echo "<pre>";
    echo "ErpNetClient Options:";
    print_r([
        "authority uri" => $client->authorityUri,
        "application name" => $client->applicationName,
        "application sign in uri" => $client->applicationSignInUri,
        "application sign out uri" => $client->applicationSignOutUri,
    ]);
    echo "REQUEST:";
    print_r($_REQUEST);
    echo "COOKIES:";
    print_r($_COOKIE);

    echo "</pre>";
}

// Use Authorization Code flow 

if(!$client->isAuthenticated())
{
    
    if($action == 'authorize')
    {
        $returnUrl = explode('?', $_SERVER["REQUEST_URI"], 2)[0];
        $client->requestAuthorizationCode($returnUrl);
        exit;    
    }

    echo "<div>Not logged in!</div>\r\n<div><a href='?action=authorize'>Log In</a></div>";
    exit;
}



$claims = $client->getClaims();

$user = $claims->name;

// Print menu
echo "<h2>Hello $user!</h2>\r\n";
echo "<div><a href='?action=signout'>Log Out</a></div>\r\n";
echo "<div><a href='?action=refreshToken'>Refresh access token</a></div>\r\n";
echo "<div><a href='?action=loadProducts'>Load products using Domain API</a></div>\r\n";

// Print results
echo "<pre>";

// The user clicked the refresh token link
if($action == 'refreshToken')
{    
    // Request access token by refresh token using offline access (not using browser redirects).
    $client->refreshAccessToken();    
    $returnUrl = explode('?', $_SERVER["REQUEST_URI"], 2)[0];
    $client->redirect($returnUrl);
    exit;   
}

echo "<h3>Access Token</h3>";

print_r($claims);

// This is a interactive application. Interactive application show the ERP.net identity login page. 
// For each user login id_token is issued and stored in cookie. 
// The id_token identifies one occupied license on the Erp Application Server.

echo "<h3>Id Token</h3>";

print_r($client->getIdClaims());


// Check if access token is expired and request new token by using a refresh token.

$expireTime = new DateTime();
$expireTime->setTimestamp($claims->exp);
$now = new DateTime();

$diff = $expireTime->getTimestamp() - $now->getTimestamp();
$diff = $diff / 60.0;

echo "Access token expires at ". $expireTime->format('Y-m-d H:i:s').". Now: " . $now->format('Y-m-d H:i:s') . ". $diff minutes remaining.\r\n";

if($diff < 0)
{
    // Request access token by refresh token using offline access (not using browser redirects).
    try
    {
        $client->refreshAccessToken();   
    }
    catch(ErpNetClientException $ex)
    {
        // The server may return an error if the refresh token is expired.
        // In this case show a message or directly request new login by calling $client->requestAuthorizationCode().
        $returnUrl = explode('?', $_SERVER["REQUEST_URI"], 2)[0];
        $client->requestAuthorizationCode($returnUrl);
        exit;  
    }
    echo "<b>Access token refreshed! Reload to view the new claims.</b>";
}

// Use Domain API
if($action == 'loadProducts')
{   
    echo "<h3>Products</h3>";
    $products_endpoint =  "https://$erpHostName/api/domain/odata/General_Products_Products?\$top=10";
    echo "$products_endpoint\r\n";
    //echo $client->accessToken;
    $headers = $headers = ["Authorization: Bearer $client->accessToken"];

    $json = $client->fetchURL($products_endpoint, $headers);
    $products = json_decode($json);    
    
    print_r($products);    
}

echo "</pre>";

