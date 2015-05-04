$(function () {
    $('#Recipients').select2();
    $('#Recipients').select2("readonly", true);
   
    $.clearFunction = undefined;
    $.addFunction = undefined;
    $.distroyHtmlEditor = undefined;

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

    $.distroyEditor = function() {
        if (typeof $.distroyHtmlEditor != 'undefined') {
            $.distroyHtmlEditor();
        }
    }

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
        $("#body").val($('#email-body').contents().find('#tempateBody').html());
        $("#name").val($("#newName").val());
        $.addTemplateClass();

        $("#SendEmail").attr("action", "/Email/SaveDraft");
        $("#SendEmail").submit();
    });

    $("#TestSend").click(function () {
        $.block();

        $.clearTemplateClass();
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



