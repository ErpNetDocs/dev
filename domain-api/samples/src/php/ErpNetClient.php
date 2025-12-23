<?php

/* Based on https://github.com/jumbojett/OpenID-Connect-PHP */

class ErpNetClientException extends \Exception
{}

class ErpNetClient
{
    // The name of the cookie that holds idToken, accessToken and refreshToken.
    private static $cookieName = 'ErpNetToken';
    /**
     * @var mixed holds well-known openid server properties
     */
    private $wellKnown = false;

    public $authorityUri;
    public $applicationName;
    public $applicationSecret;
    public $applicationSignInUri;
    public $applicationSignOutUri;    

    // Curl settings
    public $verifyHost = true;
    public $verifyPeer = true;
    public $timeOut = 60;
    // Used by http_build_query
    public $enc_type = PHP_QUERY_RFC1738;

    public $idToken;
    public $accessToken;
    public $refreshToken;

    /**
     * @param $authorityUri - Uri of ientity server
     * @param $applicationName - Unique name of the application. The application must be registered as trusted application in the used ErpNet database.
     * @param $applicationSecret - The secret of the trusted application.
     * @param $applicationSignInUri - The uri where the application will receive tokens and codes from ERP.net identity.
     * @param $applicationSignOutUri - The uri where the application will be redirected after sign out.
     * @param $domainApiUri - the Domain API base address.
     */

    public function __construct(
        $authorityUri,
        $applicationName, 
        $applicationSecret, 
        $applicationSignInUri = null,
        $applicationSignOutUri = null)
    {
        $this->authorityUri = $authorityUri;
        $this->applicationName = $applicationName;
        $this->applicationSecret = $applicationSecret;
        $this->applicationSignInUri = $applicationSignInUri;
        $this->applicationSignOutUri = $applicationSignOutUri;        

        // Read tokens from cookie.
         $tokens = $this->getTokensFromCookie();
        if($tokens)
        {
            $this->idToken = $tokens->it;
            $this->accessToken = $tokens->at;
            $this->refreshToken = $tokens->rt;
        }
        
    }

   

    public function fetchURL($url, $headers = array(), $method = "GET", $content_type = 'application/json', $post_body = null)
    {

        // OK cool - then let's create a new cURL resource handle
        $ch = curl_init();

        // Determine whether this is a GET or POST
        if ($method != "GET") {
            // curl_setopt($ch, CURLOPT_POST, 1);
            // Alows to keep the POST method even after redirect
            curl_setopt($ch, CURLOPT_CUSTOMREQUEST, $method);
            curl_setopt($ch, CURLOPT_POSTFIELDS, $post_body);

            // Add POST-specific headers
            $headers[] = "Content-Type: {$content_type}";

        }

        // If we set some headers include them
        if (count($headers) > 0) {
            curl_setopt($ch, CURLOPT_HTTPHEADER, $headers);
        }

        // Set URL to download
        curl_setopt($ch, CURLOPT_URL, $url);

        // Include header in result? (0 = yes, 1 = no)
        curl_setopt($ch, CURLOPT_HEADER, 0);

        // Allows to follow redirect
        curl_setopt($ch, CURLOPT_FOLLOWLOCATION, true);

        // Verify host and peer?
        if ($this->verifyHost) {
            curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, 2);
        } else {
            curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, 0);
        }

        if ($this->verifyPeer) {
            curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, true);
        } else {
            curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
        }

        // Should cURL return or print out the data? (true = return, false = print)
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);

        // Timeout in seconds
        $timeOut = $this->timeOut;
        curl_setopt($ch, CURLOPT_TIMEOUT, $timeOut);

        // Download the given URL, and return output
        $output = curl_exec($ch);

        // HTTP Response code from server may be required from subclass
        $info = curl_getinfo($ch);
        $responseCode = $info['http_code'];

        if($responseCode > 250) {
            trigger_error("Fetch url $url response code is $responseCode.");
        }

        if ($output === false) {
            throw new Exception('Curl error: (' . curl_errno($ch) . ') ' . curl_error($ch));
        }

        // Close the cURL resource, and free system resources
        curl_close($ch);

        return $output;
    }

    /**
     * A wrapper around base64_decode which decodes Base64URL-encoded data,
     * which is not the same alphabet as base64.
     * @param string $base64url
     * @return bool|string
     */
    public static function base64url_decode($base64url)
    {
        return base64_decode(ErpNetClient::b64url2b64($base64url));
    }

    /**
     * Per RFC4648, "base64 encoding with URL-safe and filename-safe
     * alphabet".  This just replaces characters 62 and 63.  None of the
     * reference implementations seem to restore the padding if necessary,
     * but we'll do it anyway.
     * @param string $base64url
     * @return string
     */
    public static function b64url2b64($base64url)
    {
        // "Shouldn't" be necessary, but why not
        $padding = strlen($base64url) % 4;
        if ($padding > 0) {
            $base64url .= str_repeat('=', 4 - $padding);
        }
        return strtr($base64url, '-_', '+/');
    }

     /**
     * @param string $url
     */
    public function redirect($url) {
        header('Location: ' . $url);
        exit;
    }

    /**
     * @param string $jwt encoded JWT
     * @param int $section the section we would like to decode
     * @return object
     */
    protected function decodeJWT($jwt, $section = 0)
    {
        $parts = explode('.', $jwt);
        return json_decode(ErpNetClient::base64url_decode($parts[$section]));
    }

    /**
     * Used for arbitrary value generation for nonces and state
     *
     * @return string
     */
    protected function generateRandString() {
        return md5(uniqid(rand(), TRUE));
    }
    /**
     * Sets a cookie holding the access and refresh tokens.
     * @param string $accessToken - the access token returned from ERP.net identity.
     * @param string $refreshToken - the refresh token returned from ERP.net identity.
     */
    protected function setTokensCookie()
    {
        // set a cookie that holds the access token and refresh token.
        $data = json_encode([
            "it" => $this->idToken,
            "at" => $this->accessToken,
            "rt" => $this->refreshToken,
        ]);
        $tok = $this->encrypt($data);
        $expire = time() + 60 * 60; //set expiration to 1 hour
        setcookie(self::$cookieName, $tok, $expire, "/");
    }

    protected function getTokensFromCookie()
    {
        if(isset($_COOKIE[self::$cookieName]))
        {
            $data = $this->decrypt($_COOKIE[self::$cookieName]);
            return json_decode($data);
        }   
        return null;     
    }
	
	
    public function encrypt($data)
    {
        $data = gzencode($data, 9);
        $method = "aes-128-ctr";
        $key = openssl_digest("C297896842E8E1118E9000155D001F00", 'SHA256', true);
        $iv_bytes = openssl_cipher_iv_length($method);
        $iv = openssl_random_pseudo_bytes($iv_bytes);        
        return bin2hex($iv) . openssl_encrypt($data, $method, $key, 0, $iv);;
    }

    // decrypt encrypted string
    public function decrypt($data)
    {
        $method = "aes-128-ctr";
        $key = openssl_digest("C297896842E8E1118E9000155D001F00", 'SHA256', true);
        $iv_bytes = openssl_cipher_iv_length($method);

        $iv_strlen = 2 * $iv_bytes;
        if (preg_match("/^(.{" . $iv_strlen . "})(.+)$/", $data, $regs)) {
            list(, $iv, $crypted_string) = $regs;
            if (ctype_xdigit($iv) && strlen($iv) % 2 == 0) {
                $result = openssl_decrypt($crypted_string, $method, $key, 0, hex2bin($iv));
                $result = gzdecode($result);
                return $result;
            }
        }
        return false; // failed to decrypt
    }

    public function isAuthenticated()
    {
        if(!isset($this->accessToken))
            return false;
        // TODO: check the expiration time.
        return true;
    }

    /**
     * Gets the claims from the access token. If access token is not set returns false.
     */
    public function getClaims()
    {
        if(!isset($this->accessToken))
            return false;
        return $this->decodeJWT($this->accessToken, 1);
    }

    /**
     * Gets the claims from the id token. If id token is not set returns false.
     */
    public function getIdClaims()
    {
        if(!isset($this->idToken))
            return false;
        return $this->decodeJWT($this->idToken, 1);
    }
   
    /**
     * Start Here
     * @return void
     * @throws ErpNetClientException
     */
    public function requestAuthorizationCode($returnUrl = null) 
    {
        $returnUrl = $returnUrl ?? "";

        $auth_endpoint = $this->getWellKnownConfigValue('authorization_endpoint');        
        // Generate and store a nonce in the session as cookies.
        // The nonce is an arbitrary value
        $nonce = $this->setNonce($this->generateRandString());

        // State essentially acts as a session key for OIDC

        $state = $this->generateRandString() . $returnUrl;
        $this->setState($state);

        $auth_params = array(
            'response_type' => "code id_token",
            'redirect_uri' => $this->applicationSignInUri,
            'client_id' => $this->applicationName,
            'nonce' => $nonce,
            'state' => $state,
            'scope' => 'openid profile offline_access',
            'response_mode' => 'form_post'
        );

      
        $auth_endpoint .= (strpos($auth_endpoint, '?') === false ? '?' : '&') . http_build_query($auth_params, "", '&', $this->enc_type);

        $this->redirect($auth_endpoint);
    }

    public function handleAuthorizationCode($code)
    {
        $token_json = $this->requestTokens($code);

        // Throw an error if the server returns one
        if (isset($token_json->error)) {
            if (isset($token_json->error_description)) {
                throw new ErpNetClientException($token_json->error_description);
            }
            throw new ErpNetClientException('Got response: ' . $token_json->error);
        }

        // Do an OpenID Connect session check
        $state = $_REQUEST['state'];
        if ($state !== $this->getState()) {
            
            //print_r($_REQUEST);            
            throw new ErpNetClientException('Unable to determine state.');
        }

        // Cleanup state
        $this->unsetState();

        if (!property_exists($token_json, 'id_token')) {
            throw new ErpNetClientException('User did not authorize openid scope.');
        }

        $claims = $this->decodeJWT($token_json->id_token, 1);

        if($claims->nonce != $this->getNonce()){
            throw new ErpNetClientException('Invalid nonce in id token.');
        }
        $this->unsetNonce();
        
        // Save the id token
        $this->idToken = $token_json->id_token;
        
        // Save the access token
        $this->accessToken = $token_json->access_token;
        
        // Save the refresh token, if we got one
        if (isset($token_json->refresh_token)) 
            $this->refreshToken = $token_json->refresh_token;                       
        else
            $this->refreshToken = null;

        // Save tokens into cookie.
        $this->setTokensCookie();

        $returnUrl = substr($state, 32);
        if($returnUrl)
            $this->redirect($returnUrl);
    }

    /**
     * Requests ID and Access tokens
     *
     * @param string $code
     * @return mixed
     * @throws ErpNetClientException
     */
    protected function requestTokens($code) {
        $token_endpoint = $this->getWellKnownConfigValue('token_endpoint');
        
        $headers = [];

        $grant_type = 'authorization_code';

        $token_params = array(
            'grant_type' => $grant_type,
            'code' => $code,
            'redirect_uri' => $this->applicationSignInUri,
            'client_id' => $this->applicationName,
            'client_secret' => $this->applicationSecret            
        );

        

        // Convert token params to string format
        $token_params = http_build_query($token_params, '', '&', $this->enc_type);

        $this->tokenResponse = json_decode($this->fetchURL(
            $token_endpoint,
            $headers, 
            "POST",
            'application/x-www-form-urlencoded' ,
            $token_params));
        //json_decode($this->fetchURL($token_endpoint, $token_params, $headers));

        return $this->tokenResponse;
    }

    /**
    * Validates a token lifetime according to current time.
    */
    public function validateToken($tok)
    {
        try
        {
            $claims = $this->decodeJWT($tok, 1);
            $expireTime = new DateTime();
            $expireTime->setTimestamp($claims->exp);
            $now = new DateTime();
            
            $diff = $expireTime->getTimestamp() - $now->getTimestamp();

            if($diff < 0)
                return false;
            return true;
        }
        catch(Exception $ex)
        {
            trigger_error($ex->message);
            return false;
        }
    }

    /**
     * Sets the given access token if it is a valid token. Otherwise returns false.
     */
    public function setAccessToken($tok)
    {
        if(!$this->validateToken($tok))
            return false;
        $this->accessToken = $tok;
        return true;
    }

     /**
     * Requests an access token using client credentials
     *
     * @throws ErpNetClientException
     */
    public function requestClientCredentialsToken() {
        $token_endpoint = $this->getWellKnownConfigValue('token_endpoint');

        $headers = [];

        $post_data = array(
            'grant_type'    => 'client_credentials',
            'client_id'     => $this->applicationName,
            'client_secret' => $this->applicationSecret,
            'scope'         => 'DomainApi'
        );

        // Convert token params to string format
        $post_params = http_build_query($post_data, "", '&', $this->enc_type);

        $token_json = json_decode($this->fetchURL(
            $token_endpoint,
            $headers, 
            "POST",
            'application/x-www-form-urlencoded' ,
            $post_params));

        $this->tokenResponse = $token_json;

         // Throw an error if the server returns one
         if (isset($token_json->error)) {
            if (isset($token_json->error_description)) {
                throw new ErpNetClientException($token_json->error_description);
            }
            throw new ErpNetClientException('Got response: ' . $token_json->error);
        }

        // Client credentials flow only use access_token
        if (isset($token_json->access_token)) {
            $this->accessToken = $token_json->access_token;
        }

        return $token_json;
    }

     /**
     * Requests access token by refresh token using offline access (not using browser redirects).
     * The function sets the authorization cookie usually called 'ErpNetToken.
     *
     * @param string $refresh_token or null to use the stored refresh token.
     * @return mixed
     * @throws ErpNetClientException
     */
    public function refreshAccessToken($refresh_token = null) {

        $refresh_token = $refresh_token ?? $this->refreshToken;

        $token_endpoint = $this->getWellKnownConfigValue('token_endpoint');

        $grant_type = 'refresh_token';

        $token_params = array(
            'grant_type' => $grant_type,
            'refresh_token' => $refresh_token,
            'client_id' => $this->applicationName,
            'client_secret' => $this->applicationSecret,
        );

        // Convert token params to string format
        $token_params = http_build_query($token_params, "", '&', $this->enc_type);
        $headers = array();

        $token_json = json_decode($this->fetchURL(
            $token_endpoint,
            $headers, 
            "POST",
            'application/x-www-form-urlencoded' ,
            $token_params));
        
        $this->tokenResponse = $token_json;

         // Throw an error if the server returns one
         if (isset($token_json->error)) {
            if (isset($token_json->error_description)) {
                throw new ErpNetClientException($token_json->error_description);
            }
            throw new ErpNetClientException('Got response: ' . $token_json->error);
        }


        if (isset($token_json->access_token)) {
            $this->accessToken = $token_json->access_token;
        }

        if (isset($token_json->refresh_token)) {
            $this->refreshToken = $token_json->refresh_token;
        }

        if (isset($token_json->id_token)) {
            $this->idToken = $token_json->id_token;
        }

        // Save tokens into cookie.
        $this->setTokensCookie();


        return $token_json;
    }

    /**
     * Stores nonce
     *
     * @param string $nonce
     * @return string
     */
    protected function setNonce($nonce) {        
        setcookie("nonce", $nonce, time() + 20, "/");
        return $nonce;
    }

    /**
     * Get stored nonce
     *
     * @return string
     */
    protected function getNonce() {
        if (isset($_COOKIE['nonce']))
            return $_COOKIE['nonce'];
        return null;        
    }

    /**
     * Cleanup nonce
     *
     * @return void
     */
    protected function unsetNonce() {        
        $this->unsetCookie('nonce');
    }

    protected function unsetCookie($name)
    {
        if (isset($_COOKIE[$name])) {
            unset($_COOKIE[$name]); 
            setcookie($name, null, -1, '/'); 
            return true;
        } else {
            return false;
        }        
    }

    /**
     * Stores $state
     *
     * @param string $state
     * @return string
     */
    protected function setState($state) {
        setcookie("state", $state, time() + 20, '/');
        return $state;
    }

    /**
     * Get stored state
     *
     * @return string
     */
    protected function getState() {
        if (isset($_COOKIE['state']))
            return $_COOKIE['state'];
        return null;        
    }

    /**
     * Cleanup state
     *
     * @return void
     */
    protected function unsetState() {
        $this->unsetCookie('state');        
    }

    /**
     * Get's anything that we need configuration wise including endpoints, and other values
     *
     * @param string $param
     * @param string $default optional
     * @throws ErpNetClientException
     * @return string
     *
     */
    private function getWellKnownConfigValue($param, $default = null)
    {

        // If the configuration value is not available, attempt to fetch it from a well known config endpoint
        // This is also known as auto "discovery"
        if (!$this->wellKnown) {
            $well_known_config_url = rtrim($this->authorityUri, '/') . '/.well-known/openid-configuration';
            $this->wellKnown = json_decode($this->fetchURL($well_known_config_url));
        }

        $value = false;
        if (isset($this->wellKnown->{$param})) {
            $value = $this->wellKnown->{$param};
        }

        if ($value) {
            return $value;
        }

        if (isset($default)) {
            // Uses default value if provided
            return $default;
        }

        throw new ErpNetClientException("The provider {$param} could not be fetched. Make sure your provider has a well known configuration available.");
    }

    /**
     * Redirects the browser to the end-session endpoint of the OpenID Connect provider to notify the OpenID
     * Connect provider that the end-user has logged out of the relying party site
     * (the client application).
     *
     * @param string $accessToken ID token (obtained at login)
     * @param string|null $redirect URL to which the RP is requesting that the End-User's User Agent
     * be redirected after a logout has been performed. The value MUST have been previously
     * registered with the OP. Value can be null.
     *
     * @throws ErpNetClientException
     */
    public function signOut($redirectUri = null)
    {        
        $redirectUri = $redirectUri ?? $this->applicationSignOutUri;
        $signout_endpoint = $this->getWellKnownConfigValue('end_session_endpoint');

        $signout_params = null;
        if ($redirectUri === null) {
            $signout_params = array('id_token_hint' => $this->idToken);
        } else {
            $signout_params = array(
                'id_token_hint' => $this->idToken,
                'post_logout_redirect_uri' => $redirectUri);
        }

        $signout_endpoint .= (strpos($signout_endpoint, '?') === false ? '?' : '&') . http_build_query($signout_params, "", '&', $this->enc_type);        
        $this->unsetCookie(self::$cookieName);
        $this->redirect($signout_endpoint);
    }
}

?>
