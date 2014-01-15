$(function () {
    $(".bt").button();
    $("#body").val("");
    $("#success").hide();
    CKEDITOR.replace('body', {
        height: 200,
        fullPage: false,
        allowedContent: true,
        filebrowserUploadUrl: '/Account/CKEditorUpload/',
        filebrowserImageUploadUrl: '/Account/CKEditorUpload/'
    });
    $("#sendSupport").live("click", function (e) {
        
        $.post("/Support/MyDataSendRequest", {
            body: CKEDITOR.instances["body"].getData()
        }, function (data) {
            if (data == "OK") {
                $("#success").show();
                $("#supportForm").hide();
            }
            else {
                alert("There was an error submitting your support request, please try again.");
            }
        });
    });
});