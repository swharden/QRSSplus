<html>
    <head>
        <style>
        body {font-family: sans-serif;}
        a {color: blue; text-decoration: none;}
        a:hover {color: blue;text-decoration: underline;}
        </style>
    </head>
<body>
    <?php

    foreach (scandir("./") as $fname){
        if (strstr($fname,".txt"))
            $fnames[] = $fname;
    }
    sort($fnames);

    $nDays = count($fnames);
    echo "<h3>$nDays days of data:</h3>";
    foreach ($fnames as $fname){
        $name = str_replace(".txt","",$fname);
        echo "<a href='$fname'>$name</a> ";
    }

    echo "<h3>Latest data:</h3>";
    $f = fopen($fname, "r");
    $raw=fread($f,filesize($fname));
    fclose($f);
    $raw = str_replace("\n","<br><br>",$raw);
    echo "<code>$raw</code>";

    ?>
</body>
</html>
