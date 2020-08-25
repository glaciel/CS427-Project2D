<?PHP
$studentID = $_POST['studentID'];

$con = mysqli_connect("fdb25.awardspace.net", "2966413_admin", "Bin817111");
if (!$con)
	die ('Could not connect: ' . mysqli_error($con));
mysqli_select_db($con, "2966413_admin") or die ("Could not load the database");

$rank = 1;
$result = 0;

if($check = mysqli_query($con, "SELECT rating, studentID FROM UserManagement ORDER BY rating DESC")) 
{
	while ($row = mysqli_fetch_assoc($check)) {
		mysqli_query($con, "UPDATE `UserManagement` SET `rank` = '".$rank."' WHERE `UserManagement`.`studentID` = '".$row["studentID"]."';");
        if ($studentID == $row["studentID"]) {
			$result = $rank;
		}
		$rank++;
    }
	mysqli_free_result($check);
}
mysqli_close($con);
echo($result);
?>