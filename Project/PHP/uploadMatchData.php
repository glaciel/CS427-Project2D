<?PHP
$input = file_get_contents("php://input");
$decode = explode("/", $input);
$player1Data = $decode[0];
$player2Data = $decode[1];
$disconnectPlayer = $decode[2];
$con = mysqli_connect("fdb25.awardspace.net", "2966413_admin", "Bin817111");
if (!$con)
	die ('Could not connect: ' . mysqli_error($con));
mysqli_select_db($con, "2966413_admin") or die ("Could not load the database");

mysqli_query($con, "INSERT INTO `MatchData` (`player1`, `player2`, `disconnectPlayer`, `player1Nick`, `player2Nick`) VALUES ('".$player1Data."', '".$player2Data."', '".$disconnectPlayer."', '".$decode[3]."', '".$decode[4]."');");
mysqli_close($con);
?>