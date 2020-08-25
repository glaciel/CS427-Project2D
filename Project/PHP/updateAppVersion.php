<?PHP
$studentID = $_POST['studentID'];
$appVersion = $_POST['appVersion'];

$con = mysqli_connect("fdb25.awardspace.net", "2966413_admin", "Bin817111");
if (!$con)
	die ('Could not connect: ' . mysqli_error($con));
mysqli_select_db($con, "2966413_admin") or die ("Could not load the database");

if($check = mysqli_query($con, "UPDATE `UserManagement` SET `appVersion` = '".$appVersion."' WHERE `UserManagement`.`studentID` = '".$studentID."';")) 
{
}
else
	die(mysqli_error($check));
mysqli_close($con);
?>