<?php include $_SERVER['DOCUMENT_ROOT']."/mysql_connect.php"; ?>
<?php
$query = sprintf("SELECT version + 1 as version FROM schema_info");
// WHERE firstname='%s' AND lastname='%s'", mysql_real_escape_string($firstname), mysql_real_escape_string($lastname));
$result = mysql_query($query);
if (!$result) {
    $message  = 'Invalid query: ' . mysql_error() . "\n";
    $message .= 'Whole query: ' . $query;
    die($message);
}
while ($row = mysql_fetch_assoc($result)) {
    echo $row['version'];
}

// Free the resources associated with the result set
// This is done automatically at the end of the script
mysql_free_result($result);
mysql_close($link);

?>
