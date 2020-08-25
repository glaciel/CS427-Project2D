<?PHP
$con = mysqli_connect("fdb25.awardspace.net", "2966413_admin", "Bin817111");
if (!$con)
	die ('Could not connect: ' . mysqli_error($con));
mysqli_select_db($con, "2966413_admin") or die ("Could not load the database");

$rank = 1;

$data = mysqli_query($con, "SELECT JSON_EXTRACT(`playerData`, '$.nickname') as nickname FROM `UserManagement` ORDER BY rank ASC");
while ($row = mysqli_fetch_assoc($data)) {
		$decode = explode('"', $row["nickname"]);
		$nick = $decode[1];
		mysqli_query($con, "UPDATE `UserManagement` SET `nickname` = '".$nick."' WHERE `UserManagement`.`rank` = '".$rank."';");
		print($nick);
		echo nl2br("\r\n");
		$rank++;
    }
mysqli_free_result($data);
mysqli_close($con);
?>