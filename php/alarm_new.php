<?php include $_SERVER['DOCUMENT_ROOT']."/mysql_connect.php"; ?>

<html>
<head><title>Alarm New</title></head>
<body>

<form action='alarm_new.php' method='post'>

<?php if ($_SERVER['REQUEST_METHOD'] == 'POST') { 
  $armed = $_POST['armed'];
  $datetime = $_POST['datetime'];
  $name = $_POST['name'];
  $query = sprintf("insert into alarms (armed, datetime, name, updated_at) values ('%s', '%s', '%s', now())", mysql_escape_string($armed), mysql_escape_string($datetime), mysql_escape_string($name));
  $result = mysql_query($query);
  if (!$result) { die(mysql_error()); }
} ?>

<table border>
<tr><th>armed</th><td><input type='text' name='armed' value=''/></td></tr>
<tr><th>datetime</th><td><input type='text' name='datetime' value=''/></td></tr>
<tr><th>name</th><td><input type='text' name='name' value=''/></td></tr>

</table> 
<input type='submit'/>

<a href='/alarm_index.php'>back</a>

</form>

</body>
</html>
