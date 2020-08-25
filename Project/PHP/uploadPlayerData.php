<?PHP
$json = file_get_contents("php://input");
$obj = json_decode(''.$json.'');
$con = mysqli_connect("fdb25.awardspace.net", "2966413_admin", "Bin817111");
if (!$con)
	die ('Could not connect: ' . mysqli_error($con));
mysqli_select_db($con, "2966413_admin") or die ("Could not load the database");

mysqli_query($con, "UPDATE `UserManagement` SET `playerData` = '".$json."' WHERE `UserManagement`.`studentID` = '".$obj->studentID."';");
mysqli_close($con);
?>