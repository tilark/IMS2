$(document).ready(function () {
    console.log("ready1!");

    $(document).on("focusin", ".datetimepicker2", setdatetimepicker2);
    $(document).on("focusin", ".yearpicker", setdatetimepickerYear);

});
function setdatetimepicker2() {
    $(this).datetimepicker({
        locale: 'zh-cn',
        format: 'YYYY-MM'
    });
}
function setdatetimepickerYear() {
    $(this).datetimepicker({
        locale: 'zh-cn',
        format: 'YYYY'
    });
}
//筛选统计指标值列表成功后
function searchSatisticsIndicatorInfoSuccess(data, status, jqXHR) {
    if (jqXHR.status != 203) {
        setDataTable("#satistics-indicator-list-table");
        bindVerifyIndicatorValueChecked();
    }
}
//筛选科室指标值列表成功后
function searchIndicatorInfoSuccess(data, status, jqXHR) {
    if (jqXHR.status != 203) {
        setDataTable("#verify-indicator-list-table");

        bindVerifyIndicatorValueChecked();

    }

}
//verify begin
function verifyIndicatorInfoBegin() {
    UpdateBeginAlert();
}
function verifyIndicatorInfoSuccess(data, status, jqXHR) {

    ResponseStatusSuccessTriggerClick(jqXHR.status, "#refresh-search-indicator-list-id");
    //setDataTable("#verify-indicator-list-table");
    searchIndicatorInfoSuccess(data, status, jqXHR)
}

function bindVerifyIndicatorValueChecked() {
    $("#verify-indicator-list-table").on("click", "input[type=checkbox]", function () {
        //console.log($(this).is("input:checked") + $(this).data("id"));
        //有一个显示正在运行的标志
        
        
        var postData = {};
        postData.isLock = $(this).is("input:checked");
        postData.id = $(this).data("id");

        var verifyInput = $(this);
        var loading = $(this).parent().find(".lockstatus").first();
        loading.html($('<img src="../Images/ajax-loader.gif" />'));
        //利用ajax更新
        var jqxhrVerifyPost = $.post("VerifyDepartmentIndicators/_VerifyLocked", postData, function (data, textStatus) {          
            ////根据返回的状态码设置标志
          
            if (data.success) {
                //成功，设置成功标志
                loading.html($('<span class="glyphicon glyphicon-ok bg-success"></span>'));                
            }
            else {
            //失败，设置失败标志，并置input为相应的状态

                loading.html($('<span class="glyphicon glyphicon-remove bg-danger"></span>'));
            }
            //$(this).val(data.lockstatus);
        })            
            .fail(function (data, status) {
                alert('error');
            });
    });
}
//--verify end

//--绑定uploadFile中上传铵钮
function bindUploadFileSubmit() {
    $("#upload-file-submit-id").on("click", function () {
        UpdateBeginAlert();
    });

}
//选取设备时，修改该行的背景色
function imsSelectBeginPrev(element) {
    element.parent().parent().parent().find('tr').removeClass("bg-success");
    element.parent().parent().addClass("bg-success");
}

//++设备信息
function ListSelectSuccess(data, status, jqXHR) {
    ScrollToTarget($(this).data("scrollTarget"));
    bindEditDepartmentIndicatorValue();
}
function editItemSuccess(data, status, jqXHR) {
    ResponseStatusSuccessTriggerClick(jqXHR.status, "#refresh-search-indicator-list-id");

}

function bindEditDepartmentIndicatorValue() {
    $("#edit-indicator-value-id").on("focusout", "input", function () {
        if (Number.isNaN(Number.parseFloat($(this).val()))) {
            $(this).next().show();
        }
        else {
            $(this).next().hide();
            //var postData = {};
            //postData.value = $(this).val();
            //postData.id = $(this).data("id");
            ////利用ajax更新
            //$.post("ProvidingDepartmentIndicatorAjax/_EditValue", postData, function (data) {
            //});
        }
        //console.log($(this).val() + "_" + $(this).data("id"));
    });
}
/*
点击之后，需滚到该ID处
 */
function ScrollToTarget(element) {
    $('html,body').animate({ scrollTop: $('#' + element).offset().top }, 500);
}
//++更新成功后，根据更新情况触发按钮操作
function ResponseStatusSuccessTriggerClick(jqXHRStatus, refreshStr) {
    //HttpStatusCode.NonAuthoritativeInformation;

    if (jqXHRStatus != "203") {
        if (refreshStr !== null) {
            $(refreshStr).trigger("click");
        }
        UpdateSuccessAlert();
    }
    else {
        UpdateFailedAlert();
    }
}

//++Alert提示
function CreateSuccessAlert() {
    swal({
        title: "创建成功!",
        text: "2秒后自动关闭",
        timer: 2000,
        type: "success",
        showConfirmButton: true
    });
}
function UpdateSuccessAlert() {
    swal({
        title: "更新成功!",
        text: "2秒后自动关闭",
        timer: 2000,
        type: "success",
        showConfirmButton: true
    });
}
function DeleteSuccessAlert() {
    swal({
        title: "删除成功!",
        text: "2秒后自动关闭",
        timer: 2000,
        type: "success",
        showConfirmButton: true
    });
}
function UpdateFailedAlert() {
    swal({
        title: "更新失败!",
        text: "请检查输入项",
        type: "error",
        showConfirmButton: true
    });
}
function UpdateBeginAlert() {
    console.log("enter beginalert");
    swal({
        title: "正在处理中!",
        text: "请稍后……",
        type: "success",
        showConfirmButton: false
    });
}
function setDataTable(element) {
    if (element != null) {
        $(element).DataTable(
            {
                language: {
                    "sProcessing": "处理中...",
                    "sLengthMenu": "显示 _MENU_ 项结果",
                    "sZeroRecords": "没有匹配结果",
                    "sInfo": "显示第 _START_ 至 _END_ 项结果，共 _TOTAL_ 项",
                    "sInfoEmpty": "显示第 0 至 0 项结果，共 0 项",
                    "sInfoFiltered": "(由 _MAX_ 项结果过滤)",
                    "sInfoPostFix": "",
                    "sSearch": "搜索:",
                    "sUrl": "",
                    "sEmptyTable": "表中数据为空",
                    "sLoadingRecords": "载入中...",
                    "sInfoThousands": ",",
                    "oPaginate": {
                        "sFirst": "首页",
                        "sPrevious": "上页",
                        "sNext": "下页",
                        "sLast": "末页"
                    },
                    "oAria": {
                        "sSortAscending": ": 以升序排列此列",
                        "sSortDescending": ": 以降序排列此列"
                    }
                }
            });
    }

}