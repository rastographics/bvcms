$(function () {
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
        //dimOff();
    });

    $("a.cancel").click(function () {
        $('#popupeditor').hide();
        //dimOff();
    });

    $.hClick = function (e) {
        currentDiv = this;
        $.removeButtons();
        if($(this).html() !== "Click here to edit content")
            CKEDITOR.instances['htmleditor'].setData($(this).html());
        //dimOn();
        $("#popupeditor").show();
    };

    $('div[bvedit],div.bvedit').bind('click', $.hClick).addClass("ti");

    $("#progress").dialog(
        {
            autoOpen: false,
            modal: true,
            close: function () {
                $('#progress').html("<h2>Working...</h2>");
            }
        });

    $("#askName").dialog({ autoOpen: false, modal: true, closeOnEscape: true, title: "Save Draft", resizable: false, width: 'auto' });

    $("#Send").click(function () {
        var d = $("#progress");
        d.dialog('open');

        $('#body').val($("#tempateBody").html());
        var q = $("#SendEmail").serialize();

        $.post('/Email/QueueEmails', q, function (ret) {
            if (ret === "timeout") {
                window.location = "/Email/Timeout";
                return;
            }
            var taskid = ret.id;
            if (taskid === 0) {
                d.html(ret.content);
            } else {
                $("#Send").remove();
                var intervalid = window.setInterval(function () {
                    $.post('/Email/TaskProgress/' + taskid, null, function (ret) {
                        if (ret.substr(0, 20).indexOf('<!--completed-->') >= 0)
                            window.clearInterval(intervalid);
                        d.html(ret);
                    });
                }, 3000);
            }
        });
    });

    $("#SaveDraft").click(function () {
        if ($(this).attr("saveType") == "0") {
            var d = $("#askName");
            d.dialog('open');
        } else {
            $.clearTemplateClass();
            $("#body").val($("#tempateBody").html());
            $("#name").val($("#newName").val());
            $.addTemplateClass();

            $("#SendEmail").attr("action", "/Email/SaveDraft");
            $("#SendEmail").submit();
        }
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
        var d = $("#progress");
        d.dialog('open');

        $.clearTemplateClass();
        $("#body").val($("#tempateBody").html());
        $.addTemplateClass();

        var q = $("#SendEmail").serialize();

        $.post('/Email/TestEmail', q, function (ret) {
            if (ret == "timeout") {
                window.location = "/Email/Timeout";
                return;
            }
            d.html(ret);
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



