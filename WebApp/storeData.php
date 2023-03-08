<?php

/* Get the name of the uploaded file */
$data = $_POST['data'];
$filename = uniqid(rand(), true).".csv";
$datalocation = "upload/".$filename;

$file = fopen($datalocation, "w");
fwrite($file, $data);
fclose($file);

$profile = $_POST['profile'];

$name = $_POST['name'];
$preview = $_POST['preview'];
$metadatalocation = "upload/".$filename."-metadata.json";

$jsonData = [
    'Name' => $name, 
    'Preview' => $preview, 
    'CsvFileName' => $filename,
    'GraphicsProfile' => $profile
];
$jsonString = json_encode($jsonData, JSON_PRETTY_PRINT);

$file = fopen($metadatalocation, "w");
fwrite($file, $jsonString);
fclose($file);

/* Save the uploaded file to the local filesystem */
if ( file_exists($datalocation) && file_exists($metadatalocation)  ) { 
  echo 'success'; 
} else { 
  echo 'failure'; 
}

?>