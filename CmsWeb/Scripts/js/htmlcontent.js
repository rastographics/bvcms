$(function () {
    $(".bt").button();
    $("#delete").click(function (ev) {
        ev.preventDefault();
        if (confirm("Are you sure you want to delete this entry?")) {
            $.post("/Display/ContentDelete", { id: "@Model.Id" }, function () {
                window.location = "/Manage/Display/";
            });
        }
    });
    var fullPage = getParameterByName("fullPage") === 'true';
    if (fullPage)
        $("#fullpage").prop("checked", true);
    CKEDITOR.replace('body', {
        height: 400,
        allowedContent: true,
        fullPage: fullPage,
        filebrowserUploadUrl: '/Account/CKEditorUpload/',
        filebrowserImageUploadUrl: '/Account/CKEditorUpload/'
    });
    $("#fullpage").change(function () {
        if (confirm("reload page?")) {
            if (this.checked)
                window.location.href = window.location.pathname + '?fullPage=true';
            else
                window.location.href = window.location.pathname;
        }
    });
});
function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}
