$(function () {
    $("#showitem").hide();
    $("#hideitem").click(function () {
        $(".pitem").hide("slow");
        $("#showitem").show();
        $("#hideitem").hide();
    });
    $("#showitem").click(function () {
        $(".pitem").show("slow");
        $("#hideitem").show();
        $("#showitem").hide();
    });
    if ($("#IsAutoGetData").is(":checked")) {
        $("#providingDepartmentDiv").hide();
        $("#dataSourceSystemDiv").show();
    }
    else {
        $("#providingDepartmentDiv").show();
        $("#dataSourceSystemDiv").hide();
    }
    $("#IsAutoGetData").bind("click", function () {
        if ($(this).is(":checked")) {
            $("#providingDepartmentDiv").hide();
            $("#dataSourceSystemDiv").show();
        }
        else{
            $("#providingDepartmentDiv").show();
            $("#dataSourceSystemDiv").hide();
        }
    });
    $('#datetimepicker1').datetimepicker();

});
function searchFailed() {
    $("#searchresults").html("Sorry, there was a problem with the search.");
}
$("#valueSearch").submit(function (event){
    event.preventDefault();
    var form = $(this);
    

    $.ajax({
        url: form.attr("action"),
        data: form.serialize(),
        beforeSend: function () {
            $("#ajax-loader").show();
        },
        complete: function(){
            $("ajax-loader").hide();
        },
        error: searchFailed
        
    });
});