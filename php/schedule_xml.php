<?php include $_SERVER['DOCUMENT_ROOT']."/mysql_connect.php";
$query = sprintf("select max(updated_at) as max_updated_at from alarms;");
$result = mysql_query($query);
if (!$result) {
    $message  = 'Invalid query: ' . mysql_error() . "\n";
    $message .= 'Whole query: ' . $query;
    die($message);
}
while ($row = mysql_fetch_assoc($result)) {
  $max_updated_at = $row['max_updated_at'];
}
$last_modified = gmdate('D, d M Y H:i:s', strtotime($max_updated_at)) . ' GMT';
//die($last_modified);
header("Last-Modified: " . $last_modified);
mysql_free_result($result);
//die($last_modified);

    // A PHP implementation of conditional get, see 
    //   http://fishbowl.pastiche.org/archives/001132.html
    $if_modified_since = isset($_SERVER['HTTP_IF_MODIFIED_SINCE']) ?
        stripslashes($_SERVER['HTTP_IF_MODIFIED_SINCE']) :
        false;
    if ($if_modified_since && $if_modified_since == $last_modified) {
    // Nothing has changed since their last request - serve a 304 and exit
    header('HTTP/1.0 304 Not Modified');
    exit;
    }
?>

<schedule>
<?php
$query = sprintf("select * from alarms;");
// WHERE firstname='%s' AND lastname='%s'", mysql_real_escape_string($firstname), mysql_real_escape_string($lastname));
$result = mysql_query($query);
if (!$result) {
    $message  = 'Invalid query: ' . mysql_error() . "\n";
    $message .= 'Whole query: ' . $query;
    die($message);
}
while ($row = mysql_fetch_assoc($result)) {
    echo "<alarm id='" . $row['id'] . "' armed='" . ($row['armed'] ? 'true' : 'false') . "' datetime='" . date('Y-m-d H:i:s', strtotime($row['datetime'])) . "' name='" . $row['name'] . "'/>\n";
}

// Free the resources associated with the result set
// This is done automatically at the end of the script
mysql_free_result($result);
mysql_close($link);

?>
</schedule> 
