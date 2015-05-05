$(function () {
    $('#Recipients').select2();
    $('#Recipients').select2("readonly", true);
   
    $.clearFunction = undefined;
    $.addFunction = undefined;

    $.clearTemplateClass = function () {
        if (typeof $.clearFunction != 'undefined') {
            $.clearFunction();
        }
    };

    $.addTemplateClass = function () {
        if (typeof $.addFunction != 'undefined') {
            $.addFunction();
        }
    };

    $.distroyEditor = function () {
        $('#email-body').contents().find('div[bvedit],div.bvedit').each(function (index) {
            var div = this;
            if ($(div).data('fa.editable')) {
                $(div).editable('destroy');
            }
        });
    }

    window.displayEditor = function (div) {
        if (!$(div).data('fa.editable')) {
            $(div).editable({
                inlineMode: false,
                theme: 'custom',
                buttons: ['bold', 'italic', 'underline', 'fontFamily', 'sep', 'formatBlock', 'align', 'insertOrderedList', 'insertUnorderedList', 'outdent', 'indent', 'sep', 'createLink', 'specialLink', 'sep', 'insertImage', 'table', 'html', 'fullscreen'],
                imageUploadURL: '/Account/FroalaUpload'
            });

            var html = $(div).editable('getHTML');
            if (html == "Click here to edit content") {
                $(div).editable('setHTML', '');
            }
        }
    };

    $("#Send").click(function () {
        $.block();
        $.distroyEditor();
        $('#body').val($('#email-body').contents().find('#tempateBody').html());
        var q = $("#SendEmail").serialize();

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

    $("#SaveDraft").click(function () {
        if ($(this).attr("saveType") == "0") {
            $('#draft-modal').modal('show');
        } else {
            $.clearTemplateClass();
            $.distroyEditor();
            $("#body").val($('#email-body').contents().find('#tempateBody').html());
            $("#name").val($("#newName").val());
            $.addTemplateClass();

            $("#SendEmail").attr("action", "/Email/SaveDraft");
            $("#SendEmail").submit();
        }
    });

    $('#draft-modal').on('shown.bs.modal', function () {
        $("#newName").val('').focus();
    });

    $("#SaveDraftButton").click(function () {
        $.clearTemplateClass();
        $.distroyEditor();
        $("#body").val($('#email-body').contents().find('#tempateBody').html());
        $("#name").val($("#newName").val());
        $.addTemplateClass();

        $("#SendEmail").attr("action", "/Email/SaveDraft");
        $("#SendEmail").submit();
    });

    $("#TestSend").click(function () {
        $.block();

        $.clearTemplateClass();
        $.distroyEditor();
        $("#body").val($('#email-body').contents().find('#tempateBody').html());
        $.addTemplateClass();

        var q = $("#SendEmail").serialize();

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

    $('#Subject').focus();
});



