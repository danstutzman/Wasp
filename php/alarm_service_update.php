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

if ($_SERVER['REQUEST_METHOD'] == 'POST') {
$query = sprintf("select max(updated_at) as max_updated_at from alarms;");
}

$id = $_GET['id'];
if (!$id) { die( "Please specify an id"); }
$armed = $_POST['armed'];
$armed_bool = ($armed == 'true' || $armed == 'True') ? 1 : 0;
$query = sprintf("update alarms set armed = %d where id = %d;", $armed_bool, $id);
$result = mysql_query($query);
if (!$result) {
    $message  = 'Invalid query: ' . mysql_error() . "\n";
    $message .= 'Whole query: ' . $query;
    die($message);
}

mysql_close($link);

?>
