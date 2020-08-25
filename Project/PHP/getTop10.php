<?PHP
$con = mysqli_connect("fdb25.awardspace.net", "2966413_admin", "Bin817111");
if (!$con)
	die ('Could not connect: ' . mysqli_error($con));
mysqli_select_db($con, "2966413_admin") or die ("Could not load the database");

$check = mysqli_query($con, "SELECT nickname, fishName, rating FROM UserManagement ORDER BY rank ASC LIMIT 10");

while ($row = mysqli_fetch_assoc($check))
{
	echo ( $row['nickname'] . "/" . $row['fishName'] . "/" . $row['rating'] . "\n");
}
mysqli_free_result($check);
mysqli_close($con);
?>