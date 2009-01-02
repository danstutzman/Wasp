<?php include $_SERVER['DOCUMENT_ROOT']."/mysql_connect.php"; ?>

<html>
<head><title>Alarm Index</title></head>
<body>

<form action='alarm_edit.php?id=<?php echo $_GET['id']; ?>' method='post'>

<?php if ($_SERVER['REQUEST_METHOD'] == 'POST') { 
  $id = $_GET['id'];
  $armed = $_POST['armed'];
  $datetime = $_POST['datetime'];
  $name = $_POST['name'];
  $query = sprintf("update alarms set armed = '%s', datetime = '%s', name = '%s', updated_at = now() where id = %d", mysql_escape_string($armed), mysql_escape_string($datetime), mysql_escape_string($name), $id);
  $result = mysql_query($query);
  if (!$result) { die(mysql_error()); }
} ?>

<table border>
<?php
$id = $_GET['id'];
if (!$id) { die( "Please specify an id"); }
$query = sprintf("select * from alarms where id = %d;", $id);
$result = mysql_query($query);
if (!$result) {
    $message  = 'Invalid query: ' . mysql_error() . "\n";
    $message .= 'Whole query: ' . $query;
    die($message);
}
while ($row = mysql_fetch_assoc($result)) {
    echo "<tr><th>id</th><td>" . $row['id'] . "</td></tr>";
    echo "<tr><th>armed</th><td><input type='text' name='armed' value='" . $row['armed'] . "'/></td></tr>";
    echo "<tr><th>datetime</th><td><input type='text' name='datetime' value='" . $row['datetime'] . "'/></td></tr>";
    echo "<tr><th>name</th><td><input type='text' name='name' value='" . $row['name'] . "'/></td></tr>";
}

// Free the resources associated with the result set
// This is done automatically at the end of the script
mysql_free_result($result);
mysql_close($link);

?>

</table> 
<input type='submit'/>

<a href='/alarm_index.php'>back</a>

</form>

</body>
</html>
