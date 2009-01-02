<?php
// we connect to example.com and port 3307
$link = mysql_connect('localhost:3307', 'Aqua_dstutzman', 'Aqua_dstutzman') or die('Could not connect to mysql server.' );
mysql_select_db('Aqua_dstutzman') or die('Could not select database.');
?>
