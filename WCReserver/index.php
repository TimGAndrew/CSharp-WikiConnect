<?php
//degreesofwikipedia.com to JSON converter.
//© Ryan Brownell

include('simple_html_dom.php');

//CONSTANTS
$URL = "http://degreesofwikipedia.com";
$LINKTYPE = "1";
$CURRENTLANG ="en";
$SKIPS = "";
$RS_KEY = "DkCt0RCDcoPob6Q1K3gl";

if (!empty($_GET["key"]) && !empty($_GET["start"]) && !empty($_GET["end"]))
{
	//Set Variables
	$key = $_GET["key"];
	$start = $_GET["start"];
	$end = $_GET["end"];	

	if ($key == $RS_KEY)
	{
		header('Access-Control-Allow-Origin: *');
		header('Content-type: application/json; charset=utf-8');
        
        $submitKey = getSubmitKey($URL);
        $HTML = getResultHTML($URL, $start, $end, $submitKey, $LINKTYPE, $SKIPS, $CURRENTLANG);
        
        echo outputJSON($submitKey, $start, $end, getDegrees($HTML), getMemoryUsed($HTML), getGenerationTime($HTML), getSteps($HTML));
	}
	else
	{
		header('HTTP/1.0 403 Forbidden');
		echo 'ERROR 403 FORBIDDEN: An invalid key was provided.';
	}
}
else
{
    header('HTTP/1.0 400 Bad Request');
	echo 'ERROR 400 BAD REQUEST: The request is missing the required parameters.';
}

function getSubmitKey($url) {
    $html = file_get_html($url);
    $submit = $html->find('input[name=submit]', 0);
    return $submit->getAttribute("value");
}

function getResultHTML($url, $A1, $A2, $submitKey, $linktype, $skips, $lang) {
    $RESURL = $url . "/?a1=" . urlencode($A1) . "&linktype=" . $linktype . "&a2=" . urlencode($A2) . "&skips=" . $skips . "&submit=" . $submitKey . "&currentlang=" . $lang;
    $resultHTML = file_get_html($RESURL);
    return $resultHTML;
}

function getSteps($resultHTML) {
    $path = array();
    preg_match_all("/\[.*?\] => (.*?) /", $resultHTML->find('pre', 0), $path);
    $path = $path[1];
    foreach ($path as &$step)
    {
        $step = str_replace("_", " ", $step);
    }
    return $path;
}

function getGenerationTime($resultHTML) {
    $generationTime = array();
    preg_match_all("/<br\/> Page generated in (.*?) sec <br>/", $resultHTML, $generationTime);
    return $generationTime[1][0];
}

function getMemoryUsed($resultHTML) {
    $memoryUsed = array();
    preg_match_all("/<br\/>Peak memory usage: (.*?)<br\/>/", $resultHTML, $memoryUsed);
    $memoryUsed = $memoryUsed[1][0];
    return str_replace(",", "", $memoryUsed);
}

function getDegrees($resultHTML) {
    $degrees = array();
    preg_match_all("/<br\/>([0-9]*?) degrees.<br\/>/", $resultHTML, $degrees);
    return $degrees[1][0];
}

function outputJSON($submit, $A1, $A2, $degrees, $memoryUsed, $generationTime, $path) {
    $i = 1;
    $len = count($path);

    $JSONoutput = "{";
    $JSONoutput .= "\"submit_token\": " . $submit . ",";
    $JSONoutput .= "\"start_article\": " . "\"" . urldecode($A1) . "\"" . ",";
    $JSONoutput .= "\"end_article\": " . "\"" . urldecode($A2) . "\"" . ",";
    if ($degrees == "")
    {
        $JSONoutput .= "\"degrees\": 0,";
    }
    else
    {
        $JSONoutput .= "\"degrees\": " . $degrees . ",";
    }
    
    $JSONoutput .= "\"memory_usage\": " . $memoryUsed . ",";
    $JSONoutput .= "\"seconds_to_generate\": " . $generationTime . ",";
    
    if ($len > 0)
    {
        $JSONoutput .= "\"steps\": " . "[";

        foreach ($path as &$step)
        {
            $JSONoutput .= "\"" . $step . "\"";
            if ($i < $len)
            {
                $JSONoutput .= ",";
            }
            $i++;
        }

        $JSONoutput .= "]";
    }
    else
    {
        $JSONoutput .= "\"steps\": " . "null";
    }
    $JSONoutput .= "}";

    return $JSONoutput;
}

?>