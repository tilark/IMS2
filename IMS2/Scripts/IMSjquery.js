$(function () {
    $("#showitem").hide();
    $("#hideitem").click(function () {
        $(".pitem").hide("slow");
        $("#showitem").show();
        $("#hideitem").hide()
    });
    $("#showitem").click(function () {
        $(".pitem").show("slow");
        $("#hideitem").show();
        $("#showitem").hide();
    });
});