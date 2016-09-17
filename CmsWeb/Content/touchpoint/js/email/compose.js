$(function () {
// ReSharper disable UseOfImplicitGlobalInFunctionScope

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

    // set these two lines to false will cause the editor to show on a mobile device. Expermental.
    var xsDevice = false;//$('.device-xs').is(':visible');
    var smDevice = false;//$('.device-sm').is(':visible');

    $('#editor-modal').on('shown.bs.modal', function () {
        if (!xsDevice && !smDevice) {
            if (CKEDITOR.instances['htmleditor'])
                CKEDITOR.instances['htmleditor'].destroy();

            CKEDITOR.env.isCompatible = true;
            CKEDITOR.plugins.addExternal('specialLink', '/content/touchpoint/lib/ckeditor/plugins/specialLink/', 'plugin.js');
            $.fn.modal.Constructor.prototype.enforceFocus = function () {
              var modalThis = this;
              $(document).on('focusin.modal', function (e) {
                // Fix for CKEditor + Bootstrap IE issue with dropdowns on the toolbar
                // Adding additional condition '$(e.target.parentNode).hasClass('cke_contents cke_reset')' to
                // avoid setting focus back on the modal window.
                if (modalThis.$element[0] !== e.target && !modalThis.$element.has(e.target).length
                    && $(e.target.parentNode).hasClass('cke_contents cke_reset')) {
                  modalThis.$element.focus();
                }
              });
            };

            CKEDITOR.replace('htmleditor', {
                height: 200,
                autoParagraph: false,
                fullPage: false,
                allowedContent: true,
                customConfig: '/Content/touchpoint/js/ckeditorconfig.js',
                extraPlugins: 'specialLink'
            });
        }
        var html = $(currentDiv).html();0
        if (html === "Click here to edit content") {
            if (xsDevice || smDevice)
                $('#htmleditor').val("");
            else 
                CKEDITOR.instances['htmleditor'].setData("");
        }
        else {
            if (xsDevice || smDevice) {
                $('#htmleditor').val(html);
            } else {
                CKEDITOR.instances['htmleditor'].setData(html);
            }
        }
    });
    $.fn.modal.Constructor.prototype.enforceFocus = function() {
        var modalThis = this;
        $(document).on('focusin.modal', function(e) {
            // Fix for CKEditor + Bootstrap IE issue with dropdowns on the toolbar
            // Adding additional condition '$(e.target.parentNode).hasClass('cke_contents cke_reset')' to
            // avoid setting focus back on the modal window.
            if (modalThis.$element[0] !== e.target && !modalThis.$element.has(e.target).length
                && $(e.target.parentNode).hasClass('cke_contents cke_reset')) {
                modalThis.$element.focus();
            }
        });
    };

    $('#editor-modal').on('click', '#cancel-edit', function () {
        if(!xsDevice && !smDevice)
            CKEDITOR.instances["htmleditor"].setData("");
        $('#editor-modal').modal('hide');
    });
    $('#editor-modal').on('click', '#save-edit', function () {
        var h;
        if (xsDevice || smDevice) {
            h = $('#htmleditor').val();
        } else {
            h = CKEDITOR.instances['htmleditor'].getData();
            CKEDITOR.instances["htmleditor"].setData("");
        }
        $(currentDiv).html(h);
        var eb = $('#email-body').contents().find('#tempateBody').html();
        localStorage.email = eb;
        $('#editor-modal').modal('hide');
    });

    $(".Send").click(function () {
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
                    swal({
                        title: 'Success!',
                        text: ret.content,
                        type: "success",
                        showCancelButton: false,
                    }, function () {
                        $('button.Send').prop('disabled', true);
                    });
                } else {
                    $("#send-actions").remove();
                    var intervalid = window.setInterval(function () {
                        $.post('/Email/TaskProgress/' + taskid, null, function (ret) {
                            $.unblock();
                            if (ret && ret.error) {
                                swal("Error!", ret.error, "error");
                                window.clearInterval(intervalid);
                            } else {
                                if (ret.title == 'Email has completed.') {
                                    swal({
                                        title: ret.title,
                                        text: ret.message,
                                        type: "success",
                                        showCancelButton: false,
                                    }, function () {
                                        $('button.Send').prop('disabled', true);
                                    });
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

    $(".SaveDraft").click(function () {
        if ($(this).attr("saveType") == "0") {
            $('#draft-modal').modal('show');
        } else {
            $.clearTemplateClass();
            var h = $('#email-body').contents().find('#tempateBody').html();
            $("#body").val(h);
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

    $(".TestSend").click(function () {
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



