<?php include $_SERVER['DOCUMENT_ROOT']."/mysql_connect.php"; ?>

<html>
<head><title>Alarm Index</title></head>
<body>
<table border> 
  <tr> 
    <th>Id</th> 
    <th>Armed</th> 
    <th>Datetime</th> 
    <th>Name</th> 
  </tr> 
  
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
    echo "<tr>";
    echo "<td>" . $row['id'] . "</td>";
    echo "<td>" . $row['armed'] . "</td>";
    echo "<td>" . $row['datetime'] . "</td>";
    echo "<td>" . $row['name'] . "</td>";
    echo "<td><a href='/alarm_edit.php?id=" . $row['id'] . "'>edit</a></td>";
    echo "</tr>\n";
}

echo "<tr><td><a href='alarm_new.php'>Add new</a></td></tr>\n";

// Free the resources associated with the result set
// This is done automatically at the end of the script
mysql_free_result($result);
mysql_close($link);

?>
</table> 
</body>
</html>
