$(function () {

    $('#Recipients').select2();
    $('#Recipients').select2("readonly", true);

    var currentDiv = null;
    
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

    window.displayEditor = function (div) {
        currentDiv = div;
        $('#editor-modal').modal('show');
    };

    $('#editor-modal').on('shown.bs.modal', function () {
        if (CKEDITOR.instances['htmleditor'])
            CKEDITOR.instances['htmleditor'].destroy();

        CKEDITOR.env.isCompatible = true;

        CKEDITOR.replace('htmleditor', {
            height: 400,
            autoParagraph: false,
            fullPage: false,
            allowedContent: true,
            customConfig: '/Content/touchpoint/lib/ckeditor/js/ckeditorconfig.js'
        });

        CKEDITOR.on('dialogDefinition', function (ev) {
            var dialogName = ev.data.name;
            var dialogDefinition = ev.data.definition;
            if (dialogName == 'link') {
                var advancedTab = dialogDefinition.getContents('advanced');
                advancedTab.label = "SpecialLinks";
                advancedTab.remove('advCSSClasses');
                advancedTab.remove('advCharset');
                advancedTab.remove('advContentType');
                advancedTab.remove('advStyles');
                advancedTab.remove('advAccessKey');
                advancedTab.remove('advName');
                advancedTab.remove('advId');
                advancedTab.remove('advTabIndex');

                var relField = advancedTab.get('advRel');
                relField.label = "SmallGroup";
                var titleField = advancedTab.get('advTitle');
                titleField.label = "Message";
                var idField = advancedTab.get('advLangCode');
                idField.label = "OrgId/MeetingId";
                var langdirField = advancedTab.get('advLangDir');
                langdirField.label = "Confirmation";
                langdirField.items[1][0] = "Yes, send confirmation";
                langdirField.items[2][0] = "No, do not send confirmation";
            }
        });


        var html = $(currentDiv).html();
        if (html !== "Click here to edit content") {
            CKEDITOR.instances['htmleditor'].setData(html);
        }
    });

    $('#editor-modal').on('click', '#save-edit', function () {
        var h = CKEDITOR.instances['htmleditor'].getData();
        $(currentDiv).html(h);
        $('#editor-modal').modal('hide');
    });

    $("#Send").click(function () {
        $.block();
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



