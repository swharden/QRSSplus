<?php

function echoDirContents($path)
{
    $files = scandir($path);
    sort($files);
    foreach ($files as $fileName) {
        if (strlen($fileName) > 3) {
            echo "$fileName\n";
        }

    }
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

function getUpdateMessage($statusFile="data/status.txt"){
    // return a message indicating when the last update occurred
    $f = fopen($statusFile, "r");
    $raw=fread($f,filesize($statusFile));
    fclose($f);
    $msg = file_age_string($statusFile)." ago";
    return $msg;
}  

?>

<html>

<head>
    <title>QRSS Plus - Automatically Updating Active QRSS Grabbers List</title>
	<link rel="shortcut icon" href="favicon.ico">
    <link rel="stylesheet" href="style2.css">
    <script src="papaparse.js"></script>
    <script src="qrssPlus.js?2020-12-18-05-36"></script>
    <script async src='https://www.googletagmanager.com/gtag/js?id=UA-560719-1'></script>
    <script>
        window.dataLayer = window.dataLayer || [];
        function gtag() { dataLayer.push(arguments); }
        gtag('js', new Date());
        gtag('config', 'UA-560719-1');
    </script>
</head>

<body>

    <div>
        <span class='logo1'>QRSS Plus</span><br>
        <span class='logo2'>Automatically-Updating Active QRSS Grabber List
            <!--<span style='font-size: 50%;'>by <a target='_blank' href='http://www.SWHarden.com/'>Scott Harden</a></span>-->
        </span>
    </div>

    <div class='display'>
        <b>Display:</b>
        <input class='checkbox' type="checkbox" onchange='generateContent();' id="showSummary" checked> summary,
        <input class='checkbox' type="checkbox" onchange='generateContent();' id="showLatestGrab" checked> grabs,
        <input class='checkbox' type="checkbox" onchange='generateContent();' id="showLatestStack" > stacks,
        <input class='checkbox' type="checkbox" onchange='generateContent();' id="showAllGrabs"> history,
        <input class='checkbox' type="checkbox" onchange='generateContent();' id="showInactiveGrabbers"> inactive
		<div class='displayMessage' id='grabbersMessage'>
		Loading grabber data...
		</div>
		<div class='displayMessage'>
        Last grabber update: <?php echo getUpdateMessage(); ?>
        </div>
    </div>

    <div class='message' style='line-height: 200%;'>
	
			<div>
				QRSS Plus is maintained by <a href="https://swharden.com">Scott Harden (AJ4VD)</a> and <a href="https://sites.google.com/view/andy-g0ftd">Andy (G0FTD)</a>
			</div>
	
			<div>
				Those new to QRSS are encouraged to review <a href="https://swharden.com/blog/2020-10-03-new-age-of-qrss">The New Age of QRSS</a> 
				and join the <a href="https://groups.io/g/qrssknights">Knights QRSS Mailing List</a>
			</div>
			
			<div>
				See <a href="https://swharden.com/blog/2020-10-03-new-age-of-qrss/#qrss-frequency-bands">QRSS Frequency Bands</a> 
				and the <a href="https://groups.io/g/qrssknights/wiki">Knights QRSS WIKI</a> for the latest QRSS frequencies
			</div>
			
			<div>
				Check out <i><a href='https://github.com/swharden/QRSSplus/tree/master/newsletter'>74!, The Knights QRSS Newsletter</a></i> 
				for highlights from the last year in the QRSS community
            </div>
						
			<div>
				Update grabber information by following instructions on the <a href="https://github.com/swharden/qrssplus/#updating-the-grabber-list">QRSS Plus GitHub page</a>
			</div>
    </div>

    <hr>

    <div id="content"></div>
    <div style='color: #EEE;'>
        <pre id="grabberDataCsv" style="display: none"><?php include 'grabbers.csv';?></pre>
        <pre id="latestGrabs" style="display: none"><?php echoDirContents("data");?></pre>
        <pre id="latestStacks" style="display: none"><?php echoDirContents("data/averages");?></pre>
        <pre id="thumbnails" style="display: none"><?php echoDirContents("data/thumbs");?></pre>
    </div>

    <hr>            

    <div style='font-family: monospace; font-size: 80%;'>
    <b>To view QRSS Plus program output (useful for debugging) view the source of this page.</b>
    <!--

    <?php include "data/status.txt";?>

    -->
    </div>

</body>

</html>