<?PHP

$nickname = $_POST['nickname'];
$studentID = $_POST['studentID'];
$password = $_POST['password'];

$con = mysqli_connect("fdb25.awardspace.net", "2966413_admin", "Bin817111");
if (!$con)
	exit ('Could not connect: ' . mysqli_error($con));
mysqli_select_db($con, "2966413_admin") or exit ("Could not load the database");

$check = mysqli_query($con, "SELECT studentID FROM UserManagement WHERE `studentID`='".$studentID."'");
$numrows = mysqli_num_rows($check);

if ($numrows == 0)
{
	$password = md5($password);
	$ins = mysqli_query($con, "INSERT INTO `UserManagement` (`nickname`, `studentID`, `password`) VALUES ('".$nickname."', '".$studentID."', '".$password."');");
	if ($ins)
		echo("Register successfully. You can now login to your account");
	else
		echo("There is an error with your registration. Please try again later.");
}
else
{
	echo("This studentID already exist.");
}
mysqli_close($con);
?>