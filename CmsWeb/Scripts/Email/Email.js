$(function () {
    //    var aliveid = setInterval(KeepSessionAlive, 120000);
    $("#progress").dialog({
        autoOpen: false,
        modal: true,
        close: function () {
            $('#progress').html("<h2>Working...</h2>");
        }
    });
    $(".bt").button();
    $("#Send").click(function () {
        var d = $("#progress");
        d.dialog('open');
        $('#Body').text(CKEDITOR.instances["Body"].getData());
        var q = $(this).closest('form').serialize();
        $.post('/Email/QueueEmails', q, function (ret) {
            if (ret == "timeout") {
                window.location = "/Email/Timeout";
                return;
            }
            $("#Send").remove();
            var taskid = ret.id;
            if (taskid === 0) {
                d.html(ret.content);
            }
            else {
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
    $("#TestSend").click(function () {
        var d = $("#progress");
        d.dialog('open');
        $('#Body').text(CKEDITOR.instances["Body"].getData());
        var q = $(this).closest('form').serialize();
        $.post('/Email/TestEmail', q, function (ret) {
            if (ret == "timeout") {
                window.location = "/Email/Timeout";
                return;
            }
            d.html(ret);
        });
    });

    $("#CreateVoteTag").live("click", function (ev) {
        ev.preventDefault();
        CKEDITOR.instances["votetagcontent"].updateElement();
        var q = $(this).closest('form').serialize();
        $.post('/Email/CreateVoteTag', q, function (ret) {
            CKEDITOR.instances["votetagcontent"].setData(ret, function () {
                CKEDITOR.instances["votetagcontent"].setMode("source");
            });
        });
    });
});
//<a dir="ltr" href="http://votelink" id="798" rel="smallgroup" title="This is a message">test</a>
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
