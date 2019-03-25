$(function () {
// ReSharper disable UseOfImplicitGlobalInFunctionScope

    $('#Recipients').select2();
    $('#Recipients').select2("readonly", true);

    var currentDiv = null;
    var currentDesign = null;

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

    window.displayEditor = function (div, design, useUnlayer) {
        currentDiv = div;
        currentDesign = design;
        if (useUnlayer) {
            $('#unlayer-editor-modal').modal('show');
        } else {
            $('#editor-modal').modal('show');
        }
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
        var html = $(currentDiv).html();
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
        var eb = $('#email-body').contents().find('#templateBody').html();
        localStorage.email = eb;
        $('#editor-modal').modal('hide');
    });



    $('#unlayer-editor-modal').on('shown.bs.modal', function () {
        var design = $(currentDesign).val();
        
        unlayer.init({
            id: "unlayerEditor",
            displayMode: "email"
        });
        if (design.length > 0) {
            unlayer.loadDesign(JSON.parse(design));
        }
    });

    $('#unlayer-editor-modal').on('click', '#unlayer-cancel-edit', function () {
        $('#unlayerEditor').html('');
        $('#unlayer-editor-modal').modal('hide');
    });

    $('#unlayer-editor-modal').on('click', '#unlayer-save-edit', function () {
        unlayer.exportHtml(function(data) {
            var design = data.design;
            var html = data.html; // final html
            $(currentDesign).val(JSON.stringify(design));
            $(currentDiv).html(html);
            var eb = $('#email-body').contents().find('#templateBody').html();
            localStorage.email = eb;
            $('#unlayerEditor').html('');
            $('#unlayer-editor-modal').modal('hide');
        });
    });



    $(".Send").click(function () {
        $.block();
        $('#body').val($('#email-body').contents().find('#templateBody').html());
        var q = $("#SendEmail").serialize();

        $.post('/Email/QueueEmails', q, function (ret) {
            if (ret && ret.error) {
                $.unblock();
                swal({
                    title: "Error!",
                    text: ret.error,
                    html: true,
                    type: "error"
                });
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
            var d = $('#email-body').contents().find('#templateDesign').val();
            var h = $('#email-body').contents().find('#templateBody').html();
            $("#UnlayerDesign").val(d);
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
        $("#UnlayerDesign").val($('#email-body').contents().find('#templateDesign').val());
        $("#body").val($('#email-body').contents().find('#templateBody').html());
        $("#name").val($("#newName").val());
        $.addTemplateClass();

        $("#SendEmail").attr("action", "/Email/SaveDraft");
        $("#SendEmail").submit();
    });

    $(".TestSend").click(function () {
        $.block();

        $.clearTemplateClass();
        $("#body").val($('#email-body').contents().find('#templateBody').html());
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

    // special links window
    var specialLinks = {
        el: $('#special-links-modal'),
        typeSelect: $('#special_links_type'),
        formInput: $('#special_links_form input'),
        orgRow: $('#org_id.row'),
        orgInput: $('#org_id input'),
        meetingRow: $('#meeting_id.row'),
        meetingInput: $('#meeting_id input'),
        messageInput: $('#message input'),
        confirmationSelect: $('#confirmation select'),
        smallGroupRow: $('#small_group.row'),
        smallGroupInput: $('#small_group input'),
        secondaryRows: $('#small_group.row, #message.row, #confirmation.row'),
        resultRow: $('#result.row'),
        resultInput: $('#result.row input'),

        init: function () {
            // toggle visibility
            $('#create-special-link').click(function () {
                specialLinks.reset();
                specialLinks.el.modal('show');
            });
            $('.close-special-links-modal').click(function () {
                specialLinks.el.modal('hide');
            });
            $('.done-special-links-modal').click(function () {
                specialLinks.resultInput.select();
                document.execCommand('copy');
                specialLinks.el.modal('hide');
            });

            // select contents of result
            specialLinks.resultInput.click(function () {
                $(this).select();
            });

            // update result row and toggle inputs
            specialLinks.typeSelect.change(specialLinks.refreshForm);
            specialLinks.confirmationSelect.change(specialLinks.refreshForm);
            specialLinks.formInput.on('input', specialLinks.refreshForm);

            // init
            specialLinks.refreshForm();
        },

        reset: function () {
            specialLinks.typeSelect.val('0');
            specialLinks.refreshForm();
        },

        refreshForm: function () {
            var linkType = specialLinks.typeSelect.val();
            var linkText = 'https://' + linkType;
            var orgId = specialLinks.orgInput.val();

            switch (linkType) {
                case 'registerlink':
                case 'registerlink2':
                case 'sendlink':
                case 'sendlink2':
                case 'supportlink':
                    specialLinks.orgRow.show();
                    specialLinks.meetingRow.hide();
                    specialLinks.secondaryRows.hide();
                    if (orgId.length) {
                        linkText += '/?org=' + orgId;
                    } else {
                        linkText = '';
                    }
                    break;

                case 'rsvplink':
                case 'regretslink':
                    specialLinks.orgRow.hide();
                    specialLinks.meetingRow.show();
                    specialLinks.secondaryRows.show();
                    var message = specialLinks.messageInput.val();
                    var meetingId = specialLinks.meetingInput.val();
                    var confirmation = specialLinks.confirmationSelect.val();
                    var smallGroup = specialLinks.smallGroupInput.val();
                    if (meetingId.length) {
                        linkText += '/?meeting=' + meetingId + '&confirm=' + confirmation;
                        if (smallGroup.length) {
                            linkText += '&group=' + smallGroup;
                        }
                        if (message.length) {
                            linkText += '&msg=' + message;
                        }
                    } else {
                        linkText = '';
                    }
                    break;

                case 'votelink':
                    specialLinks.orgRow.show();
                    specialLinks.meetingRow.hide();
                    specialLinks.secondaryRows.show();
                    message = specialLinks.messageInput.val();
                    smallGroup = specialLinks.smallGroupInput.val();
                    confirmation = specialLinks.confirmationSelect.val();
                    if (orgId.length) {
                        linkText += '/?org=' + orgId + '&confirm=' + confirmation;
                        if (message.length) {
                            linkText += '&msg=' + message;
                        }
                        if (smallGroup.length) {
                            linkText += '&group=' + smallGroup;
                        }
                    } else {
                        linkText = '';
                    }
                    break;

                default:
                    linkText = '';
                    specialLinks.secondaryRows.hide();
                    specialLinks.orgRow.hide();
                    specialLinks.meetingRow.hide();
                    specialLinks.formInput.val('');
                    break;
            }
            specialLinks.resultInput.val(linkText);
            if (linkText.length) {
                specialLinks.resultRow.show();
            } else {
                specialLinks.resultRow.hide();
            }
        }
    };

    specialLinks.init();
});



