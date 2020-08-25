<?PHP
$studentID = $_POST['studentID'];
$con = mysqli_connect("fdb25.awardspace.net", "2966413_admin", "Bin817111");
if (!$con)
	die ('Could not connect: ' . mysqli_error($con));
mysqli_select_db($con, "2966413_admin") or die ("Could not load the database");

$data = mysqli_query($con, "SELECT playerData FROM UserManagement WHERE `studentID`='".$studentID."'");
while ($row = mysqli_fetch_assoc($data)) 
{
	echo($row['playerData']);
}
mysqli_close($con);
?>