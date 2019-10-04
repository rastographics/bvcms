$(function () {
    $(document).on("click", ".btnUploadDoc", function (e) {
        e.preventDefault();
        var submitBtn = $(this);
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
            .done(function () {
                checkMark.show();
                deleteBtn.removeAttr("disabled");
                submitBtn.attr('disabled', true);
                fileInputBtn.attr('disabled', true);
                isUploaded.val('true');
            })
            .fail(function () {
                swal("Server Error", "Something went wrong", "error");
                fileInputBtn.val('');
            });
    });

    $(document).on("click", ".btnDeleteDoc", function (e) {
        e.preventDefault();
        var deleteBtn = $(this);
        var fileInputBtn = deleteBtn.parent().parent().find('input.docInput');
        var isUploaded = deleteBtn.parent().parent().find('input.hdnIsUploaded');
        var checkMark = deleteBtn.parent().parent().parent().find('span.checkmark');
        var submitBtn = deleteBtn.parent().parent().find('input.btnUploadDoc');
        var fileInput = fileInputBtn[0];
        checkMark.hide();
        deleteBtn.attr("disabled", true);
        submitBtn.removeAttr('disabled');
        fileInputBtn.removeAttr('disabled');
        isUploaded.val('false');
        fileInputBtn.val('');
        var clone = fileInput.cloneNode(true);
        fileInput.parentNode.replaceChild(clone, fileInput);
    });    

});
