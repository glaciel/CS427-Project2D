<?PHP
$studentID = $_POST['studentID'];
$usingTime = $_POST['usingTime'];

$con = mysqli_connect("fdb25.awardspace.net", "2966413_admin", "Bin817111");
if (!$con)
	die ('Could not connect: ' . mysqli_error($con));
mysqli_select_db($con, "2966413_admin") or die ("Could not load the database");

if($data = mysqli_query($con, "SELECT usingAppTime FROM UserManagement WHERE `studentID`='".$studentID."'")) 
{
	$row = mysqli_fetch_assoc($data);
	$result = $row["usingAppTime"] + $usingTime;
	mysqli_query($con, "UPDATE `UserManagement` SET `usingAppTime` = '".$result."' WHERE `UserManagement`.`studentID` = '".$studentID."';");
}
else
	echo(mysqli_error($check));
mysqli_close($con);
?>