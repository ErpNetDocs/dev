<?php 

const AUTHORIZE_URI = "https://demodb.my.erp.net/id/connect/authorize";
const TOKEN_URI = "https://demodb.my.erp.net/id/connect/token";
const CALLBACK_URI = "https://my.trusted.app/app.php";
const TRUSTED_APP_URI = "my.trusted.app";
const TRUSTED_APP_SECRET = "<my_plain_app_secret>";

const DOMAIN_API_TEST_URI = "https://demodb.my.erp.net/api/domain/odata/Crm_Customers?\$top=10";

if (isset($_POST) && isset($_POST["code"])) {
	
  $authCode = $_POST["code"];
  $accessToken = acquireAccessToken($authCode);

  domainApiCall($accessToken);

  exit();
}

sendAuthorizationRequest();

function sendAuthorizationRequest() {
  $authorizeRequest = AUTHORIZE_URI . "?" .
    "client_id=" . TRUSTED_APP_URI . "&" .
    "redirect_uri=" . CALLBACK_URI . "&" .
    "response_type=code%20id_token&" .
    "response_mode=form_post&" .
    "scope=openid%20profile%20offline_access%20DomainApi&" .
    "nonce=abc&" .
    "state=xyz";
  
  header("Location: $authorizeRequest");
}

function acquireAccessToken($authCode) {
  $tokenRequestBody = array(
    "client_id" => TRUSTED_APP_URI,
    "client_secret" => TRUSTED_APP_SECRET,
    "grant_type" => "authorization_code",
    "code" => $authCode,
    "redirect_uri" => CALLBACK_URI
  );
  
  $opt = array(
    'http' => array(
      'header' => "Content-type: application/x-www-form-urlencoded\r\n",
      'method' => 'POST',
      'content' => http_build_query($tokenRequestBody)
    )
  );

  $context = stream_context_create($opt);
  $result = file_get_contents(TOKEN_URI, false, $context);
  if ($result == FALSE) {
    return false;
  }
  
  $clientAuthData = json_decode($result, true);
  
  return $clientAuthData["access_token"];
}

function domainApiCall($accessToken) {
  $opt = array(
    'http' => array(
      'header' => 'Authorization: Bearer ' . $accessToken
    )
  );

  $context  = stream_context_create($opt);
  $response = file_get_contents(DOMAIN_API_TEST_URI, false, $context);

  print_r($response);
}

?>
