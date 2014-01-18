$(function () {
    $('#Name').focus();
    $(".bt").button();
    $("#clear").click(function (ev) {
        ev.preventDefault();
        $("input:text").val("");
        $("#ProgramId,#CampusId,#ScheduleId,#TypeId").val(0);
        $("#OnlineReg").val(-1);
        $("#MainFellowship")[0].checked = false;
        $("#ParentOrg")[0].checked = false;
        $.post('/OrgSearch/DivisionIds/0', null, function (ret) {
            $('#DivisionId').html(ret);
        });
        return false;
    });
    $("#search").click(function (ev) {
        ev.preventDefault();
        var name = $('#Name').val();
        if (name.match("^" + "M\.") == "M.") {
            $('#Name').val("");
            var f = $('#results').closest('form');
            f.attr("action", "/OrgSearch/CreateMeeting/" + name);
            f.submit();
        }
        $.getTable();
        return false;
    });
    $("#hideshow").click(function () {
        $(".managedivisions").toggle();
    });
    $.getTable = function () {
        var f = $('#results').closest('form');
        var q = f.serialize();
        $.block();
        $.post($('#search').attr('href'), q, function (ret) {
            $('#results').replaceWith(ret).ready(function () {
                $.fmtTable();
                $.unblock();
            });
        });
        return false;
    };
    $(".datepicker").jqdatepicker();
    $.editable.addInputType('datepicker', {
        element: function (settings, original) {
            var input = $('<input>');
            if (settings.width != 'none') { input.width(settings.width); }
            if (settings.height != 'none') { input.height(settings.height); }
            input.attr('autocomplete', 'off');
            $(this).append(input);
            return (input);
        },
        plugin: function (settings, original) {
            var form = this;
            settings.onblur = 'ignore';
            $(this).find('input').jqdatepicker().bind('click', function () {
                $(this).datepickerjq('show');
                return false;
            }).bind('dateSelected', function (e, selectedDate, $td) {
                $(form).submit();
            });
        }
    });
    $.editable.addInputType("checkbox", {
        element: function (settings, original) {
            var input = $('<input type="checkbox">');
            $(this).append(input);
            $(input).click(function () {
                var value = $(input).attr("checked") ? 'yes' : 'no';
                $(input).val(value);
            });
            return (input);
        },
        content: function (string, settings, original) {
            var checked = string == "yes" ? 1 : 0;
            var input = $(':input:first', this);
            $(input).attr("checked", checked);
            var value = $(input).attr("checked") ? 'yes' : 'no';
            $(input).val(value);
        }
    });
    $.fmtTable = function () {
        $("#results td.tip[title]").tooltip({
            showBody: "|"
        });
        $('#results > tbody > tr:even').addClass('alt');

        $('#results').bind('mousedown', function (e) {
            if ($(e.target).hasClass("bday")) {
                $(e.target).editable('/OrgSearch/Edit/', {
                    type: 'datepicker',
                    tooltip: 'click to edit...',
                    event: 'click',
                    submit: 'OK',
                    cancel: 'Cancel',
                    width: '100px',
                    height: 25
                });
            } else if ($(e.target).hasClass("yesno")) {
                $(e.target).editable('/OrgSearch/Edit', {
                    type: 'checkbox',
                    onblur: 'ignore',
                    submit: 'OK'
                });
            }
        });
    };
    $.fmtTable();
    $.maxZIndex = $.fn.maxZIndex = function (opt) {
        var def = { inc: 10, group: "*" };
        $.extend(def, opt);
        var zmax = 0;
        $(def.group).each(function () {
            var cur = parseInt($(this).css('z-index'));
            zmax = cur > zmax ? cur : zmax;
        });
        if (!this.jquery)
            return zmax;
        return this.each(function () {
            zmax += def.inc;
            $(this).css("z-index", zmax);
        });
    };
    $("#searchvalues select").css("width", "100%");
    $('#ProgramId').change(function () {
        $.post('/OrgSearch/DivisionIds/' + $(this).val(), null, function (ret) {
            $('#DivisionId').html(ret);
        });
    });

    $("#ApplyType").click(function (ev) {
        ev.preventDefault();
        var f = $('#results').closest('form');
        var q = f.serialize();
        $.post('/OrgSearch/ApplyType/' + $("#TargetType").val(), q, function(ret) {
            if (ret !== "") {
                if (ret === "ok")
                    $.getTable();
                else {
                    $.growlUI("Apply Type", ret);
                }
            }
        });
        return false;
    });
    $("#MakeNewDiv").click(function (ev) {
        ev.preventDefault();
        $.post('/OrgSearch/MakeNewDiv/', { id: $("#TagProgramId").val(), name: $("#NewDiv").val() }, function (ret) {
            $('#TagDiv').html(ret);
            $("#NewDiv").val("");
        });
        return false;
    });
    $("#RenameDiv").click(function (ev) {
        ev.preventDefault();
        $.post('/OrgSearch/RenameDiv/', { id: $("#TagProgramId").val(), divid: $("#TagDiv").val(), name: $("#NewDiv").val() }, function (ret) {
            if (ret == "error")
                alert("expected error");
            {
                $('#TagDiv').html(ret);
                $("#NewDiv").val("");
            }
        });
        return false;
    });
    $("#TagProgramId").change(function () {
        $.post('/OrgSearch/TagDivIds/' + $(this).val(), null, function (ret) {
            $('#TagDiv').html(ret);
            $("#search").click();
        });
    });
    $("#TagDiv").change(function () {
        $("#search").click();
    });

    $("form").on("keypress", 'input', function (e) {
        if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
            $('a.default').click();
            return false;
        }
        return true;
    });
    $('div.dialog').dialog({ autoOpen: false });
    $('#rollsheet1').click(function (ev) {
        ev.preventDefault();
        hideDropdowns();
        $.post('/OrgSearch/DefaultMeetingDate/' + $('#ScheduleId').val(), null, function (ret) {
            $('#MeetingDate').val(ret.date);
            $('#MeetingTime').val(ret.time);
            var d = $('#PanelRollsheet');
            d.dialog('open');
        });
        return false;
    });
    $('#AttNotices').click(function (ev) {
        ev.preventDefault();
        hideDropdowns();
        var did = $('#DivisionId').val();
        if (!confirm("This will send email notices to leaders based on your filters, continue?"))
            return false;
        $.block();
        var q = $("#orgsearchform").serialize();
        $.post("/OrgSearch/EmailAttendanceNotices", q, function () {
            $.unblock();
            $.growlUI("complete", "Email Notices Sent");
        });
        return false;
    });
    $('#attdetail1').click(function (ev) {
        ev.preventDefault();
        hideDropdowns();
        var d = $('#PanelAttDetail');
        d.dialog('open');
        return false;
    });
    $('#rollsheet2').click(function (ev) {
        ev.preventDefault();
        hideDropdowns();
        $('div.dialog').dialog('close');
        var args = "?dt=" + $('#MeetingDate').val() + " " + $('#MeetingTime').val();
        if ($('#altnames').is(":checked"))
            args += "&altnames=true";
        if ($('#bygroup').is(":checked"))
            args += "&bygroup=1";
        if ($("#highlightsg").val())
            args += "&highlight=" + $("#highlightsg").val();
        if ($("#sgprefix").val())
            args += "&sgprefix=" + $("#sgprefix").val();

        if ($('#rallymode').is(":checked"))
            $("#orgsearchform").attr("action", "/Reports/RallyRollsheet" + args);
        else
            $("#orgsearchform").attr("action", "/Reports/Rollsheet" + args);
        $("#orgsearchform").attr("target", "_blank");
        $("#orgsearchform").submit();
        $("#orgsearchform").removeAttr("target");
        return false;
    });
    $('#ExportExcel').click(function (ev) {
        ev.preventDefault();
        hideDropdowns();
        $("#orgsearchform").attr("action", "/OrgSearch/ExportExcel");
        $("#orgsearchform").submit();
        return false;
    });
    $('#weeklyattendance').click(function (ev) {
        ev.preventDefault();
        hideDropdowns();
        $("#orgsearchform").attr("action", "/Reports/WeeklyAttendance");
        $("#orgsearchform").submit();
        return false;
    });
    $('#ExportMembersExcel').click(function (ev) {
        ev.preventDefault();
        hideDropdowns();
        $("#orgsearchform").attr("action", "/OrgSearch/ExportMembersExcel");
        $("#orgsearchform").submit();
        return false;
    });
    $('#Meetings').click(function (ev) {
        ev.preventDefault();
        $('div.dialog').dialog('close');
        hideDropdowns();
        $("#orgsearchform").attr("action", "/Reports/Meetings");
        $("#orgsearchform").submit();
        return false;
    });
    $('#Structure').click(function (ev) {
        ev.preventDefault();
        hideDropdowns();
        $("#orgsearchform").attr("action", "/OrgSearch/OrganizationStructure");
        $("#orgsearchform").submit();
        return false;
    });
    $('#RecentAbsents').click(function (ev) {
        ev.preventDefault();
        hideDropdowns();
        $("#orgsearchform").attr("action", "/Reports/RecentAbsents");
        $("#orgsearchform").submit();
        return false;
    });
    $('#attdetail2').click(function (ev) {
        ev.preventDefault();
        $('div.dialog').dialog('close');
        var args = "?dt1=" + $('#MeetingDate1').val() + "&dt2=" + $('#MeetingDate2').val();
        $("#orgsearchform").attr("action", "/Reports/AttendanceDetail" + args);
        $("#orgsearchform").submit();
        return false;
    });
//    $('#Roster').click(function (ev) {
//        ev.preventDefault();
//        hideDropdowns();
//        $("#orgsearchform").attr("action", "/Reports/Roster");
//        $("#orgsearchform").attr("target", "_blank");
//        $("#orgsearchform").submit();
//        $("#orgsearchform").removeAttr("target");
//        return false;
//    });
    $('#PasteSettings').click(function (ev) {
        ev.preventDefault();
        hideDropdowns();
        if (!confirm("Are you sure you want to replace all these settings?"))
            return false;
        var f = $('form');
        var q = f.serialize();
        $.post(this.href, q, function (ret) {
            $.growlUI("Completed", "Settings Replaced");
        });
        return false;
    });
    $('#RepairTransactions').click(function (ev) {
        ev.preventDefault();
        hideDropdowns();
        if (!confirm("Are you sure you want to run repair transactions?"))
            return false;
        var f = $('form');
        var q = f.serialize();
        $.post(this.href, q, function (ret) {
            $.growlUI("Completed", "Repair Completed");
        });
        return false;
    });
    $('a.ViewReport').click(function (ev) {
        ev.preventDefault();
        hideDropdowns();
//        var did = $('#DivisionId').val();
//        if (did == '0') {
//            $.growlUI("error", 'must choose division');
//            return false;
//        }
        var href = $(this).attr("href");
        $("#orgsearchform").attr("action", href);
        $("#orgsearchform").attr("target", "_blank");
        $("#orgsearchform").submit();
        $("#orgsearchform").removeAttr("target");
        return false;
    });
    $('body').on('click', 'a.taguntag', function (ev) {
        ev.preventDefault();
        hideDropdowns();
        var a = $(this);
        var td = $('#TagDiv').val();
        if (td > 0)
            $.post(a.attr('href'), { tagdiv: td }, function (ret) {
                if (ret == "error")
                    alert("unexpected error, refresh page");
                else {
                    $(a).parent().parent().replaceWith(ret);
                    $.fmtTable();
                }
            });
        return false;
    });
    $('body').on('click', 'a.maindiv', function (ev) {
        ev.preventDefault();
        var a = $(this);
        $.post(a.attr('href'), { tagdiv: $('#TagDiv').val() }, function (ret) {
            if (ret == "error")
                alert("unexpected error, refresh page");
            else {
                $(a).parent().parent().replaceWith(ret);
                $.fmtTable();
            }
        });
        return false;
    });
    $('#checkincontrol').click(function (ev) {
        ev.preventDefault();
        hideDropdowns();
        var d = $('#PanelCheckinControl');
        d.dialog('open');
        return false;
    });
    $('#enrollmentcontrol1').click(function (ev) {
        ev.preventDefault();
        hideDropdowns();
        var d = $('#PanelEnrollmentControl');
        d.dialog('open');
        return false;
    });
    $('#enrollmentcontrol2').click(function (ev) {
        ev.preventDefault();
        hideDropdowns();
        $('div.dialog').dialog('close');
//        var pid = $('#ProgramId').val();
//        var did = $('#DivisionId').val();
//        if (pid == '0') {
//            $.growlUI("error", 'must choose program');
//            return false;
//        }
        var url = "/Reports/EnrollmentControl";
        if ($('#enrcontrolfiltertag').is(":checked"))
            url = url.appendQuery("usecurrenttag=true");
        if ($('#enrcontrolexcel').is(":checked"))
            url = url.appendQuery("excel=true");
        $("#orgsearchform").attr("action", url);
        $("#orgsearchform").submit();
        return false;
    });
    $('#checkincontrol2').click(function (ev) {
        ev.preventDefault();
        hideDropdowns();
        $('div.dialog').dialog('close');
        var url = "/Reports/CheckinControl/";
        $("#CheckinDate").val($("#checkindate").val());
        $("#orgsearchform").attr("action", url);
        $("#orgsearchform").submit();
        return false;
    });
    $('#enrollmentcontrol2i').click(function (ev) {
        ev.preventDefault();
        hideDropdowns();
        $('div.dialog').dialog('close');
//        var pid = $('#ProgramId').val();
//        var did = $('#DivisionId').val();
//        if (pid == '0') {
//            $.growlUI("error", 'must choose program');
//            return false;
//        }
        $("#orgsearchform").attr("action", "/Reports/EnrollmentControl2");
        $("#orgsearchform").submit();
        return false;
    });
});


