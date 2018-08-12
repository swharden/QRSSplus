<?php

/**
 * QRSS Plus - Automatically-Updated List of Active QRSS Grabbers
 * 
 * by Scott Harden (www.SWHarden.com)
 * 
 * After the QRSS Plus python script downloads the latest grabs and places
 * them in the data folder, this PHP script can read the contents and compare
 * it to the grabber file and display a nice webpage indicating which grabs
 * are active and which are inactive.
 * 
 */

class QrssPlus {
    // This standalone class provides all active QRSS Plus functionality.

    public $folderGrabs = 'data/';
    public $filesSeen = [];
    public $data = [];
    public $fileGrabber = 'grabbers.csv';
    public $dataCols = ['ID', 'call', 'title', 'name', 'loc', 'site', 'url'];

    function __construct(){
        $this->scanFiles();
        $this->dataLoad();
    }

    function file_age_string($fname){
        // return the age of a filename with a string like "3.83 hours"
        date_default_timezone_set('America/New_York');
        $ageSec=time()-filemtime($fname);
        $ageMin=$ageSec/60;
        $ageHr=$ageMin/60;
        $ageDy=$ageHr/24;
        $ageYr=$ageDy/365.25;
        $ageString=date("F d Y H:i:s.", filemtime($fname));
        if ($ageHr<1) $ageString=sprintf("%.02f minutes", $ageMin);
        else if ($ageDy<1) $ageString=sprintf("%.02f hours", $ageHr); 
        else if ($ageDy<60) $ageString=sprintf("%.02f days", $ageDy);
        else $ageString=sprintf("%.02f years", $ageYr);
        return $ageString;
    }

    function itemsMatching($arr, $match='AJ4VD'){
        // given an array of strings, return only items wit the substring
        $arr2=[];
        foreach ($arr as $item){
            if (strpos($item,$match)!==false){
                $arr2[]=$item;
            }
        }
        return $arr2;
    }

    function itemsNotMatching($arr, $match='AJ4VD'){
        // given an array of strings, return only items without the substring
        $arr2=[];
        foreach ($arr as $item){
            if (strpos($item,$match)==false){
                $arr2[]=$item;
            }
        }
        return $arr2;
    }

    function scanFiles(){
        // update $this->filesSeen based on the contents of $this->folderGrabs
        $this->filesSeen = [];
        foreach (scandir($this->folderGrabs) as $fname){
            if (strpos($fname,".fail.")) continue;
            if (count(explode(".",$fname))<4) continue;
            $this->filesSeen[]=$fname;
        }
        sort($this->filesSeen);
    }

    function dataLoad(){
        // load database by reading a CSV file
        $f = fopen($this->fileGrabber, "r");
        while (($dataRow = fgetcsv($f, 1000, ",")) !== FALSE) {
            $num = count($dataRow);
            $row++;
            $this->data[]=$dataRow;
        }
    }

    function getUpdateMessage($statusFile="data/status.txt"){
        // return a message indicating when the last update occurred
        $f = fopen($statusFile, "r");
        $raw=fread($f,filesize($statusFile));
        fclose($f);
        $msg = $this->file_age_string($statusFile)." ago";
        $msg .= "<!-- (took $raw sec) -->";
        return $msg;
    }    

    function showDatabase(){
        // display the content of the CSV file as a pretty HTML table
        echo "<div class='grabberSquare' style='padding: 10px; background-color: #EEE;'><table>";
        echo "";
        foreach ($this->data as $line){
            if ($line[0]=="#ID") $line[0]="ID";
            if ($line[0][0]=="#"){
                echo "<tr style='color: #CCC;'>";
            } else {
                echo "<tr>";
            }
            
            foreach ($line as $item){
                if ($line[0]=="ID") $item="<span style='font-weight: bold; font-size: 150%; text-decoration: underline;'>$item</span>";
                if (strpos($item,"://")!==FALSE) $item = "<a target='_blank' style='font-size: 70%;' href='$item'>$item</a>";
                echo "<td>$item</td>";    
            }
            echo "</tr>";
        }
        echo "</table></div>";
    }

    function dataShow($showActive=True, $thumbsOnly=False){
        // Show just active or inactive grabbers, as full format or thumbnail

        foreach ($this->data as $dataRow){

            // determine grabber state
            $filesWithID=$this->itemsMatching($this->filesSeen,$dataRow[0]);
            $filesThumb=$this->itemsMatching($filesWithID,".thumb.");
            rsort($filesThumb);
            $filesOriginal=$this->itemsNotMatching($filesWithID,".thumb.");
            $hashes=[];
            foreach ($filesThumb as $fname){$hashes[]=explode(".",$fname)[2];}
            $active=TRUE;
            if (count($filesThumb)==0) $active=FALSE;
            if (count($filesThumb)>1 && count(array_unique($hashes))<=1) $active=FALSE;
            if ($active!=$showActive) continue;

            // populate info
            $grabberID=$dataRow[0];
            $grabbercall=$dataRow[1];
            $grabberTitle=$dataRow[2];
            $grabberName=$dataRow[3];
            $grabberLoc=$dataRow[4];
            $grabberSite=$dataRow[5];
            $grabberURL=$dataRow[6];
            $info = "$grabberName in $grabberLoc";
            $info .= "<span style='color: #CCCCCC;'> | </span>";
            $info .= "<a target='_blank' href='https://www.qrz.com/db/$grabbercall'>QRZ</a>";
            $info .= "<span style='color: #CCCCCC;'> | </span>";
            $info .= "<a target='_blank' href='$grabberSite'>site</a>";
            $info .= "<span style='color: #CCCCCC;'> | </span>";
            $info .= "<a target='_blank' href='$grabberURL'>file</a>";

            // don't display commented-out grabbers
            if ($grabberID[0]=="#") continue;

            // determine what color to make the grabber
            $bgcolor = "green";
            if ($active){
                $bgcolor='#4a8e6a';
                $bgcolor2='#c9e0d4';
            } else {
                $bgcolor='#ba4a4a';
                $bgcolor2='#e0c3c3';
            }
			
			if ($thumbsOnly==False){				
				echo "<div class='grabberSquare'>";
				echo "<div class='grabberTitle' style='background-color: $bgcolor;'><b><a name='$grabberID' href='#$grabberID' style='color: white;'>$grabberID</a></b></div>";
				echo "<div class='grabberSubTitle' style='background-color: $bgcolor;'>$info</div>";
				echo "<div style='padding: 10px; background-color: $bgcolor2'>";
				if ($active){
                    // display the full view for active grabbers

                    // show latest pic large
					echo "<table><tr>";
					$bigPic=$filesOriginal[count($filesOriginal)-1];
					$url="$this->folderGrabs/$bigPic";
					$style='';
					if (count($filesThumb)<2 || ($hashes[0]==$hashes[1])) $style='opacity: 0.3;';
					echo "<td valign='top' style='padding-right: 50px;'><a target='_blank' href='$url'>";
					echo "<img style='$style' class='grabLatest' width='600' src='$url'></a></td>";
		
					// show all thumbnails
					echo "<td valign='top'><br>";
					for ($i=0; $i<count($filesThumb); $i++){
						$fname=$filesThumb[$i];
						$fnameOriginal=str_replace(".thumb","",$fname);
						if (!in_array($fnameOriginal,$this->filesSeen)) continue;
						$url="$this->folderGrabs/$fname";
						$url2="$this->folderGrabs/$fnameOriginal";
						$style='';
						if (($i<count($filesThumb)-1) && ($hashes[$i]==$hashes[$i+1])) $style='opacity: 0.3;';
						echo "<a target='_blank' href='$url2'><img class='grabThumb' style='$style' src='$url'></a> ";
					}
					echo "</td></tr></table>";

				} else {
					// display the full view of inactive grabbers
					echo "<div style='font-family: monospace; font-size: 200%;'><a target='_blank' style='color: red;' href='$grabberURL'>$grabberURL</a></div>";
					echo "<a target='_blank' href='$grabberURL'><img class='grabLatest' height='300' src='$grabberURL'></a>";
				}
				echo "</div></div>";
			} else {
				// show only thumbnails
				$bigPic=$filesOriginal[count($filesOriginal)-1];
                $url="$this->folderGrabs/$bigPic";
                $callSign = explode(".",$bigPic)[0];
                echo "<div style='float: left; margin: 5px;'>";
                echo "<a href='#$callSign' style=''><b>$callSign</b><br>";
                echo "<img src='$url' style='height: 125px; border: 1px solid black; box-shadow: 5px 5px 10px rgba(0,0,0,.2);'>";
                echo "</a></div>";
            }
        }
        echo "<div style='clear: left;'></div>";
    }
}


function validate_grabber_list($fname, $verbose=false){
    // this function verifies that a grabber file is properly formatted.

    function display_message($msg, $class, $verbose){
        if ($verbose==false and $class!="lineBad") {
            return;
        } else {
            echo "<div class='$class'>$msg</div>";
        }
    }

    $totalErrors=0;
    $row=0;
    $f = fopen($fname, "r");
    
    if ($verbose){
        echo "<div style='font-size: 150%; font-weight: bold'>Grabber List File Analysis</div>";
        echo "<div style='font-family: monospace;'>$fname</div>";
        echo "<div>&nbsp</div>";
    }

    $uniqueIDs = [];

    while (($dataRow = fgetcsv($f, 1000, ",")) !== FALSE) {
        $row++;
        $line=$dataRow;
        $lineText = implode(", ", $line);
        $lineItemCount = count($line);
    
        if ($verbose){
            echo "<div class='lineBlock'>";
            echo "<div class='lineNumber'>Line #$row</div>";
            echo "<div class='lineCode'>$lineText</div>";
        }

        // check for empty lines
        $lineLength = strlen($lineText);
        if ($lineLength<3){
            display_message("GRABBER FILE ERROR (LINE $row):<br><code>$lineText</code><br>Blank lines are NOT allowed.", "lineBad", $verbose);
            $totalErrors+=1;
        } else {
            display_message("Line length: $lineLength characters", "lineOK", $verbose);
        }

        // check each line has 7 items
        if ($lineItemCount==7){
            display_message("Number of items: $lineItemCount", "lineOK", $verbose);
        } else {
            display_message("GRABBER FILE ERROR (LINE $row):<br><code>$lineText</code><br>Each line must contain 7 items. This line contains $lineItemCount.", "lineBad", $verbose);
            $totalErrors+=1;
        }
        
        if ($row==1){
            // check the first row is exactly what is needed
            $firstLineFormat = "#ID, callsign, title, name, location, website, file";
            if ($lineText==$firstLineFormat){
                display_message("First line format: OK", "lineOK", $verbose);
            } else {
                display_message("GRABBER FILE ERROR (LINE $row):<br><code>$lineText</code><br>First line must contain this exact text:<br>$firstLineFormat", "lineBad", $verbose);
                $totalErrors+=1;
            }
        } else {
            // ensure URLs are present for every grabber file and link
            if (strstr($line[5], "://") && strstr($line[6], "://")){
                display_message("URL format: OK", "lineOK", $verbose);
            } else {
                display_message("GRABBER FILE ERROR (LINE $row):<br><code>$lineText</code><br>URL format is bad!", "lineBad", $verbose);
                $totalErrors+=1;
            }
        }
        
        
        if (in_array($line[0],$uniqueIDs)){
            display_message("GRABBER FILE ERROR (LINE $row):<br><code>$lineText</code><br>Grabber IDs (the first element) must be unique!", "lineBad", $verbose);           
            $totalErrors+=1;
        } else {
            display_message("Unique grabber ID: OK", "lineOK", $verbose);
        }
        $uniqueIDs[]=$line[0];
        
        echo "</div>";
    } 

    return $totalErrors;
}

$contributeMsg = "QRSS Plus is community maintained.<br>Update grabber information, add your grabber,<br>or improve this page by contributing to ";
$contributeMsg .= "<a target='_blank' href='https://github.com/swharden/QRSSplus'>QRSS Plus on GitHub</a>";
$contributeMsgFlat = str_replace("<br>"," ",$contributeMsg);
?>

<html>
<head>
<title>QRSS Plus - Automatically Updating Active QRSS Grabbers List</title>
<link rel="stylesheet" href="styles.css">
<style>
body {font-family: sans-serif;}
a {color: blue; text-decoration: none;}
a:hover {color: blue;text-decoration: underline;}

.logo1 {
	color: #000;
	font-size: 80px;
	font-family:"Times New Roman",Georgia,Serif;
    text-shadow: 3px 3px 10px #AAA;
}
.logo2 {
	color: #000;
	font-size: 24px;
	font-style: italic;
	position: relative;
	top: -19px;
	left: +60px;
	font-family:"Times New Roman",Georgia,Serif;
    text-shadow: 3px 3px 10px #AAA;
}
.grabThumb {
	border: 1px solid black;
	box-shadow: 5px 5px 10px rgba(0,0,0,.5); 
	margin:4px; 
}
.grabLatest {
	border: 1px solid black;
	box-shadow: 5px 5px 10px rgba(0,0,0,.5); 
	margin:20px 4px 20px 4px; 
}
.grabberSquare{
	border: 1px solid black;
	box-shadow: 10px 10px 20px rgba(0,0,0,.2); 
	margin:50px 30px 50px 30px; 
}
.grabberTitle {
    color: white;
	font-size: 300%;
	font-weight: bold;
    padding: 10px 10px 0px 10px;
}
.grabberSubTitle a{
    color: #FFFFAA;
}
.grabberSubTitle{
    color: white;
    padding: 0px 10px 10px 10px;
}
.bigHeading {
    font-size: 400%;
    font-weight: bold;
    text-shadow: 3px 3px 5px #AAA;
}
.lineNumber{
    font-weight: bold;
    text-decoration: underline;
}
.lineCode{
    font-family: monospace;
}
.lineOK{
    color: #CCC;
}
.lineBad{
    line-height: 150%;
    font-family: sans-serif;
    background-color: #FFFFDD;
    font-weight: bold;
    margin: 10px;
    padding: 10px;
    border: 1px solid black;
}
.lineBlock{
    padding: 5px;
    margin-bottom: 20px;
    background-color: #EEE;
}
</style>
</head>
<body>
<?php
    $qp = new QrssPlus();
    echo "<span class='logo1'>QRSS Plus</span><br>";
    echo "<span class='logo2'>Automatically-Updating Active QRSS Grabber List ";
    echo "<span style='font-size: 50%;'>by <a target='_blank' href='http://www.SWHarden.com/'>Scott Harden</a></span>";
    echo "</span>";
    echo "<div style='color: #CCC; padding: 10px; float: right; top: 0; position: absolute; right: 0; text-align: center;'>$contributeMsg</div>";

    $totalErrors = validate_grabber_list("grabbers.csv");
    if ($totalErrors==0){
        //echo "<div style='border: 2px solid #DDD; background-color: #EFEFEF; margin: 0px 5px 10px 5px; padding: 5px;'>";
        //echo "<div>Grabber list file validated.</div>";
        //echo "</div>";
    } else {
        echo "<div style='border: 2px solid #000; background-color: #EFEFEF; margin: 0px 5px 10px 5px; padding: 5px; background-color: #FFFFCC; font-weight: bold;'>";
        echo "<div>Grabber list file contained $totalErrors errors.</div>";
        echo "<div>Correct these errors by modifying <a href='https://github.com/swharden/QRSSplus/blob/master/grabbers.csv'>grabbers.csv on GitHub</a></div>";
        echo "</div>";    
    }

    $warn_if_outdated_data = true;
    if ($warn_if_outdated_data && !strpos($qp->getUpdateMessage(),"minutes")){
        echo "<div style='border: 2px solid #000; background-color: #EFEFEF; margin: 0px 5px 10px 5px; padding: 5px; background-color: red; color: white;'>";
        echo "<b>ERROR!:</b> The server is set to update this page every 10 minutes, but the last update was ".$qp->getUpdateMessage();
        echo "</div>";
    } else {
        echo "<div style='border: 2px solid #DDD; background-color: #EFEFEF; margin: 0px 5px 10px 5px; padding: 5px;'>";
        echo "<b>Last grabber update:</b> ".$qp->getUpdateMessage();
        echo "</div>";
    }

    // show thumbnails first
	$qp->dataShow(TRUE,TRUE);
    
    echo "<div style='color: #AAA; margin: 20px 5px 10px 5px; padding: 5px; font-size: 150%; font-style: italic;'>$contributeMsgFlat</div>";
    
	echo "<div class='bigHeading'><br>Individual Active Grabbers</div>";
    $qp->dataShow(TRUE);
	
    echo "<div class='bigHeading'>Individual Inactive Grabbers</div>";
    $qp->dataShow(FALSE);

    echo "<div class='bigHeading'>QRSS Grabber Database</div>";
    echo "<div style='color: #AAA; margin: 20px 5px 10px 5px; padding: 5px; font-size: 150%; font-style: italic;'>$contributeMsgFlat</div>";
    $qp->showDatabase();
?>
</body>
</html>
