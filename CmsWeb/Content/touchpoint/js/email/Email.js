$(function () {
    $('#Recipients').select2();
    $('#Recipients').select2("readonly", true);

    $('#Body').froalaEditable({
        inlineMode: false,
        spellcheck: true,
        height: 400,
        theme: 'custom',
        buttons: ['bold', 'italic', 'underline', 'fontSize', 'fontFamily', 'color', 'sep', 'formatBlock', 'align', 'insertOrderedList', 'insertUnorderedList', 'outdent', 'indent', 'sep', 'createLink', 'specialLink', 'sep', 'insertImage', 'uploadFile', 'table', 'undo', 'redo', 'html', 'fullscreen'],
        imageUploadURL: '/Account/FroalaUpload',
        fileUploadURL: '/Account/FroalaUpload'
    });

    $("#Send").click(function () {
        $.block();
        $('#Body').text($('#Body').froalaEditable('getHTML'));
        var q = $(this).closest('form').serialize();

        $.post('/Email/QueueEmails', q, function (ret) {
            if (ret && ret.error) {
                $.unblock();
                swal("Error!", ret.error, "error");
            } else {
                if (ret === "timeout") {
                    swal("Session Timeout!", 'Your session timed out. Please copy your email content and start over.', "error");
                    return;
                }
                var taskid = ret.id;
                if (taskid === 0) {
                    $.unblock();
                    swal("Success!", ret.content, "success");
                } else {
                    $("#send-actions").remove();
                    var intervalid = window.setInterval(function () {
                        $.post('/Email/TaskProgress/' + taskid, null, function (ret) {
                            $.unblock();
                            if (ret && ret.error) {
                                swal("Error!", ret.error, "error");
                            } else {
                                if (ret.title == 'Email has completed.') {
                                    swal(ret.title, ret.message, "success");
                                    window.clearInterval(intervalid);
                                } else {
                                    swal({
                                        title: ret.title,
                                        text: ret.message,
                                        imageUrl: '/Content/touchpoint/img/spinner.gif'
                                    });
                                }
                            }
                        });
                    }, 3000);
                }
            }
        });
    });

    $("#TestSend").click(function () {
        $.block();
        $('#Body').text($('#Body').froalaEditable('getHTML'));
        var q = $(this).closest('form').serialize();

        $.post('/Email/TestEmail', q, function (ret) {
            $.unblock();
            if (ret && ret.error) {
                swal("Error!", ret.error, "error");
            } else {
                if (ret == "timeout") {
                    swal("Session Timeout!", 'Your session timed out. Please copy your email content and start over.', "error");
                    return;
                }
                swal("Success!", ret, "success");
            }
        });
    });
});
