<?PHP
$studentID = $_POST['studentID'];

$con = mysqli_connect("fdb25.awardspace.net", "2966413_admin", "Bin817111");
if (!$con)
	die ('Could not connect: ' . mysqli_error($con));
mysqli_select_db($con, "2966413_admin") or die ("Could not load the database");

$check = mysqli_query($con, "SELECT fishName FROM UserManagement WHERE `studentID`='".$studentID."'");

while ($row = mysqli_fetch_assoc($check))
{
	echo($row['fishName']);
}
mysqli_free_result($check);
mysqli_close($con);
?>