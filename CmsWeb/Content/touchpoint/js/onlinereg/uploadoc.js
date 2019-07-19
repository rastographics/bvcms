$(function () {
    $('.checkmark').hide();
    $(document).on("click", ".btnUploadDoc", function (e) {
        e.preventDefault();
        var submitBtn = $(this);
        var fileInputBtn = submitBtn.parent().parent().find('input.docInput');
        var deleteBtn = submitBtn.parent().parent().find('input.btnDeleteDoc');
        var checkMark = submitBtn.parent().parent().parent().find('span.checkmark');
        var docName = fileInputBtn.attr('name');
        var fileInput = fileInputBtn[0];
        var formdata = new FormData();
        formdata.append(fileInput.files[0].name, fileInput.files[0]);
        formdata.append('docname', docName);
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
            })
            .fail(function () {
                alert("fail");
            });
    });
});
