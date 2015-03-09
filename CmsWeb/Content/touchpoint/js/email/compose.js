$(function () {
    $('#Recipients').select2();
    $('#Recipients').select2("readonly", true);

    var currentDiv = null;
    
    CKEDITOR.replace('htmleditor', {
        height: 400,
        autoParagraph: false,
        fullPage: false,
        allowedContent: true,
        customConfig: '/scripts/js/ckeditorconfig.js'
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

    $("a.save").click(function () {
        var h = CKEDITOR.instances['htmleditor'].getData();
        $(currentDiv).html(h);
        $('#popupeditor').hide();
    });

    $("a.cancel").click(function () {
        $('#popupeditor').hide();
    });

    $.hClick = function (e) {
        currentDiv = this;
        $.removeButtons();
        if($(this).html() !== "Click here to edit content")
            CKEDITOR.instances['htmleditor'].setData($(this).html());

        $("#popupeditor").show();
    };

    $('div[bvedit],div.bvedit').bind('click', $.hClick).addClass("ti");

    $("#askName").dialog({ autoOpen: false, modal: true, closeOnEscape: true, title: "Save Draft", resizable: false, width: 'auto' });

    $("#Send").click(function () {
        $.block();
        $('#body').val($("#tempateBody").html());
        var q = $("#SendEmail").serialize();

        $.post('/Email/QueueEmails', q, function (ret) {
            if (ret && ret.error) {
                $.unblock();
                swal("Error!", ret.error, "error");
            } else {
                if (ret === "timeout") {
                    window.location = "/Email/Timeout";
                    return;
                }
                var taskid = ret.id;
                if (taskid === 0) {
                    $.unblock();
                    swal(ret);
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
                                    swal(ret.title, ret.message, "info");
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
            $("#body").val($("#tempateBody").html());
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
        $("#body").val($("#tempateBody").html());
        $("#name").val($("#newName").val());
        $.addTemplateClass();

        $("#SendEmail").attr("action", "/Email/SaveDraft");
        $("#SendEmail").submit();
    });

    $("#TestSend").click(function () {
        $.block();

        $.clearTemplateClass();
        $("#body").val($("#tempateBody").html());
        $.addTemplateClass();

        var q = $("#SendEmail").serialize();

        $.post('/Email/TestEmail', q, function (ret) {
            $.unblock();
            if (ret && ret.error) {
                swal("Error!", ret.error, "error");
            } else {
                if (ret == "timeout") {
                    window.location = "/Email/Timeout";
                    return;
                }
                swal("Success!", ret, "success");
            }
        });
    });

    $.removeButtons = function () {
        $("#controlButtons").remove();
    };

    $.hClickAdd = function (e) {
        $.removeButtons();
        parentTR = $(currentHover).parent();
        var dup = $(currentHover).clone();
        $(parentTR).after($(dup));
        dup.wrap("<tr></tr>");
        dup.attr("bvrepeatadd", "");
        dup.removeAttr("bvrepeat");
        dup.mouseenter($.hAddHoverIn).mouseleave($.hHoverOut);
        $('div[bvedit],div.bvedit').bind('click', $.hClick).addClass("ti");
    };

    $.hHoverIn = function (ev) {
        currentHover = this;
        $(this).css("border", "solid 1px #ff0000");
        $(this).append("<div id='controlButtons' class='tiAdd'><input id='addButton' type='button' value='Copy Section' /></div>");
        $("#controlButtons").css("top", $(this).offset().top + 5).css("left", $(this).offset().left + 5);
        $("#addButton").bind("click", $.hClickAdd);
        ev.stopPropagation();
    };

    $.clearTemplateClass = function () {
        $.removeButtons();
        $("div[bvedit],div.bvedit").removeClass("ti");
        $("div[bveditadd],div.bveditadd").removeClass("ti");
    };

    $.addTemplateClass = function () {
        $("div[bveditadd],div.bveditadd").addClass("ti");
        $("div[bvedit],div.bvedit").addClass("ti");
    };

    $.hHoverOut = function (ev) {
        currentHover = null;
        $(this).css("border", "");
        $.removeButtons();
        ev.stopPropagation();
    };

    $('td[bvrepeat]').mouseenter($.hHoverIn).mouseleave($.hHoverOut);

    $.hAddHoverIn = function (ev) {
        currentHover = this;
        $(this).css("border", "solid 1px #ff0000");
        $(this).append("<div id='controlButtons' class='tiAdd'><input id='removeButton' type='button' value='Remove' /></div>");
        $("#controlButtons").css("top", $(this).offset().top + 5).css("left", $(this).offset().left + 5);
        $("#removeButton").bind('click', $.removeSection);
    };

    $.updateDiv = function () {
        var h = CKEDITOR.instances['htmleditor'].getData();
        $(currentDiv).html(h);
        $('#popupeditor').hide("close");
    };

    $.addRowAfter = function (ev) {
        $(this).parent().parent().after("<tr bvrepeatadd=''>" + $(this).html() + "</tr>");
        $("div[bvrepeatadd]").hover($.hAddHoverIn, $.hHoverOut);
        ev.stopPropagation();
    };

    $.removeSection = function (ev) {
        $(currentHover).parent().remove();
        ev.stopPropagation();
    };
});



