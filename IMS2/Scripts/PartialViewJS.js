var $IMS2 = jQuery.noConflict();


window.onload = function () {


}
$IMS2("#showitem").hide();
$IMS2("#hideitem").click(function () {
    $IMS2(".pitem").hide("slow");
    $IMS2("#showitem").show();
    $IMS2(this).hide();
    $IMS2("#createIndicators").hide();
});
$IMS2("#showitem").click(function () {
    $IMS2(".pitem").show("slow");
    $IMS2("#hideitem").show();
    $IMS2(this).hide();
    $IMS2("#createIndicators").show();
});