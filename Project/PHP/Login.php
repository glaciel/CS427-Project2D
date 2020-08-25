<?PHP
$studentID = $_POST['studentID'];
$password = $_POST['password'];

$con = mysqli_connect("fdb25.awardspace.net", "2966413_admin", "Bin817111");
if (!$con)
	die ('Could not connect: ' . mysqli_error($con));
mysqli_select_db($con, "2966413_admin") or die ("Could not load the database");

$check = mysqli_query($con, "SELECT password FROM UserManagement WHERE `studentID`='".$studentID."'");
$numrows = mysqli_num_rows($check);

if ($numrows == 0)
	echo ("Student ID does not exist");
else
{
	$password = md5($password);
	while ($row = mysqli_fetch_assoc($check))
	{
		if ($password == $row['password'])
			echo ("login-SUCCESS");
		else
			echo ("Incorrect password");
	}
}
mysqli_close($con);
?>