$(document).ready(function () {
    console.log("ready1!");

    $(document).on("focusin", ".datetimepicker2", setdatetimepicker2);
});
function setdatetimepicker2() {
    $(this).datetimepicker({
        locale: 'zh-cn',
        format: 'YYYY-MM'
    });
}

//选取设备时，修改该行的背景色
//选取设备时，修改该行的背景色
function imsSelectBeginPrev(element) {
    element.parent().parent().parent().find('tr').removeClass("bg-success");
    element.parent().parent().addClass("bg-success");
}

//++设备信息
function ListSelectSuccess(data, status, jqXHR) {
    ScrollToTarget($(this).data("scrollTarget"));
}
function editItemSuccess(data, status, jqXHR) {
    ResponseStatusSuccessTriggerClick(jqXHR.status, "#refresh-search-indicator-list-id");

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