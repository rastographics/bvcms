$(function () {
    $(document).on("click", ".btnUploadDoc", function (e) {
        e.preventDefault();
        var submitBtn = $(this);
        debugger;
        var registrantId = $(document).find('#registrantId').val();
        var orgId = $(document).find('#orgId').val();
        var fileInputBtn = submitBtn.parent().parent().find('input.docInput');
        var isUploaded = submitBtn.parent().parent().find('input.hdnIsUploaded');
        var deleteBtn = submitBtn.parent().parent().find('input.btnDeleteDoc');
        var checkMark = submitBtn.parent().parent().parent().find('span.checkmark');
        var docName = fileInputBtn.attr('docname');
        var fileInput = fileInputBtn[0];
        var formdata = new FormData();
        formdata.append(fileInput.files[0].name, fileInput.files[0]);
        formdata.append('docname', docName);
        formdata.append('registrantId', registrantId);
        formdata.append('orgId', orgId);
        $.ajax({
            url: 'UploadDocument',
            type: 'POST',
            processData: false, // important
            contentType: false, // important
            dataType: 'json',
            data: formdata
        })
            .done(function (data) {
                checkMark.show();
                deleteBtn.removeAttr("disabled");
                submitBtn.attr('disabled', true);
                fileInputBtn.attr('disabled', true);
                isUploaded.val('true');
            })
            .fail(function () {
                swal("Server Error", "Something went wrong", "error");
            });
    });
});
