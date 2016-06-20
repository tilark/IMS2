var $IMS = jQuery.noConflict();
//$IMS(document).ready(function () {
//    //$IMS(".navbar-wrapper").stickUp();
//    //$IMS('#testResult').load('/ProvidingDepartmentIndicator/Message');

//    $IMS('#datepicker').datepicker({
//        dateFormat: "yy-mm"
//    });
//    function searchFailed() {
//        $("#searchresults").html("Sorry, there was a problem with the search.");
//    }
//$("#valueSearch").submit(function (event) {
//    event.preventDefault();
//$IMS("#testSubmit").click(function () {
//    //var form = $(this);
//    var jsonData = {};
//    jsonData = window.performance.toJSON();
//    jsonData.searchTime = $IMS("#searchTimeID").val();
//    jsonData.department = $IMS("#departmentID").val();
//    //var json = JSON.stringify(jsonData);
//    $IMS.ajax({
//        url: $IMS(this).attr("src"),
//        data: jsonData,
//        type: 'GET',
//        dataType: 'json',
//        contentType: 'application/json; charset=utf-8',
//        beforeSend: function () {
//            $IMS("#ajax-loader").show();
//        },
//        complete: function () {
//            $IMS("#ajax-loader").hide();
//        },
//        error: searchFailed,
//        success: function (data) {
//            var html = Mustache.to_html($IMS("#resultTemplate").html(),
//                { mustacheTest: data });
//            $IMS("#showresults").empty().append(html);
//        }

//    });
//});


//$IMS("#testSubmit").click(function () {

//    $IMS("#showresults").html(jsonData.searchTime + jsonData.department);
//});
//enabling stickUp on the '.navbar-wrapper' class
//    $IMS("#showitem").hide();
//    $IMS("#hideitem").hide();

//});

window.onload = function () {

    $IMS('#providingDepartmentID').change(function () {
        var departmentID = $IMS("#providingDepartmentID option:selected").val();
        if (departmentID) {
            $IMS("#indicatorDetails").load("/ProvidingDepartmentIndicator/IndicatorDetails",
            { "providingDepartmentID": departmentID },
            function (response, status, xhr) {
                if (status == "error") {
                    var msg = "错误: ";
                    $IMS("#indicatorDetails").html(msg + xhr.status + " " + xhr.statusText);
                }
            });
            $IMS("#hideitem").show();

            $IMS("#createIndicators").show();
        }
        else {
            $IMS("#indicatorDetails").empty();
            $IMS("#createIndicators").hide();

        }
        $IMS("#searchIndicators").empty();
    });

    function searchFailed() {
        alert("error");

        $IMS("#idTest").html("Sorry, there was a problem with the search.");
    }
    $IMS("#createIndicator").click(function (event) {
        event.preventDefault();
        var jsonData = {};
        jsonData = window.performance.toJSON();
        jsonData.searchTime = $IMS("#searchTimeID").val();
        jsonData.providingDepartmentID = $IMS("#providingDepartmentID").val();
        $IMS("#ajax-loader2").show();
        $IMS("#searchIndicators").load($IMS(this).attr("src"), jsonData, function(responseTxt,statusTxt,xhr){
            if (statusTxt == "success")
            {
                $IMS("#ajax-loader2").hide();
                $IMS('#myModal').modal('hide');
            }
            if(statusTxt=="error")
                alert("Error: "+xhr.status+": "+xhr.statusText);
        });
        //$IMS.ajax({
        //    url: $IMS(this).attr("src"),
        //    data: jsonData,
        //    type: 'GET',
        //    dataType: 'json',
        //    contentType: 'application/json; charset=utf-8',
        //    beforeSend: function () {
        //        $IMS("#ajax-loader2").show();
        //    },
        //    complete: function () {
        //        alert("complete");

        //        $IMS("#ajax-loader2").hide();
        //    },
        //    error: searchFailed,
        //    success: function (data) {
        //        alert("success");
        //        $IMS("#searchIndicators2").html(data);
        //    }

        //});
    });
}

$IMS("#showitem").hide();

$IMS("#hideitem").click(function () {
    $IMS(".pitem").hide("slow");
    $IMS("#showitem").show();
    $IMS(this).hide();
    $IMS("#createIndicators").hide();
});
$IMS("#showitem").click(function () {
    $IMS(".pitem").show("slow");
    $IMS("#hideitem").show();
    $IMS(this).hide();
    $IMS("#createIndicators").show("slow");
});