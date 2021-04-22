<?php

include("client.php");



$client->handleAuthorizationCode($_REQUEST["code"]);

print_r($_REQUEST);
print_r($_COOKIE);
?>