$(function () {
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
    CKEDITOR.replace('editor', {
        height: 200,
        customConfig: '/Content/touchpoint/js/ckeditorconfig.js',
        extraPlugins: 'specialLink'
    });
    $('#Name').focus();
    $(".bt").button();
    $("a.trigger-dropdown").dropdown2();
    $("#clear").click(function (ev) {
        ev.preventDefault();
        $("#PublicView").removeAttr('checked');
        $("input:text").val("");
        $("#ProgramId,#CampusId,#ScheduleId,#TypeId").val(0);
        $("#OnlineReg").val(-1);
        $.post('/OrgSearch/DivisionIds/0', null, function (ret) {
            $('#DivisionId').html(ret);
        });
        $.getTable();
        return false;
    });
    $("#PublicView").click(function () {
        $.getTable();
    });
    $("#search").click(function (ev) {
        ev.preventDefault();
        var name = $('#Name').val();
        if (name.startsWith("M."))  {
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
    $.InitFunctions.ReloadPeople = function () {
        $.getTable();
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
            } else if ($(e.target).hasClass("category")) {
                $(e.target).editable("/OrgSearch/Edit", {
                    //indicator: '<img src="/Content/images/loading.gif">',
                    loadurl: "/MobileAPI/RegCategories/",
                    placeholder: 'edit',
                    loadtype: "POST",
                    type: "select",
                    submit: "OK",
                    style: 'display: inline'
                });
            }
            else if ($(e.target).hasClass("yesno")) {
                $(e.target).editable('/OrgSearch/Edit', {
                    type: 'checkbox',
                    onblur: 'ignore',
                    submit: 'OK'
                });
            } else if ($(e.target).hasClass("publicsort")) {
                $(e.target).editable('/OrgSearch/Edit', {
                    width: 150,
                    placeholder: 'edit',
                    height: 22,
                    submit: 'OK',
                    tooltip: 'click to edit...'
                });
            }
        });
    };
    $(".descredit").live("click", function (ev) {
        $.descredit($(this).prev());
    });
    $.descredit = function($a) {
        if ($a.text() === "edit")
            $a.html('');
        CKEDITOR.instances['editor'].setData($a.html());
        dimOn();
        $("#EditorDialog").center().show();
        $("#saveedit").off("click").on("click", function (ev) {
            ev.preventDefault();
            var v = CKEDITOR.instances['editor'].getData();
            $a.html(v);
            var id = $a.attr("id");
            $.post("/OrgSearch/SetDescription", { id: id, description: v });
            CKEDITOR.instances['editor'].setData('');
            $('#EditorDialog').hide("close");
            dimOff();
            return false;
        });
        return false;
    };
    $("#canceledit").live("click", function (ev) {
        ev.preventDefault();
        $('#EditorDialog').hide("close");
        dimOff();
        return false;
    });
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
        $.post('/OrgSearch/ApplyType/' + $("#TargetType").val(), q, function (ret) {
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

    $("#Name").keypress(function (e) {
        if ((e.which && e.which === 13) || (e.keyCode && e.keyCode === 13)) {
            $('a.default').click();
            return false;
        }
        return true;
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
    $('div.dialog').dialog({ autoOpen: false });
    $("a.rollsheets").live("click", function () {
        $.dialogOptions("/Dialog/ForNewRollsheets/" + $('#ScheduleId').val(), $(this));
    });
    $.InitFunctions.Rollsheets = function(a, q) {
        hideDropdowns();
        $("#orgsearchform").attr("action", "/Reports/Rollsheets?" + q);
        $("#orgsearchform").submit();
        return false;
    }
    $.InitFunctions.RallyRollsheets = function(a, q) {
        hideDropdowns();
        $("#orgsearchform").attr("action", "/Reports/RallyRollsheets?" + q);
        $("#orgsearchform").submit();
        return false;
    }
    $('#newmeetings').click(function (ev) {
        ev.preventDefault();
        hideDropdowns();
        $.post('/OrgSearch/DefaultMeetingDate/' + $('#ScheduleId').val(), null, function (ret) {
            $('#NewMeetingDate').val(ret.date);
            $('#NewMeetingTime').val(ret.time);
            var d = $('#PanelMeetings');
            d.dialog('open');
        });
        return false;
    });
    $('#createmeetings').click(function (ev) {
        ev.preventDefault();
        hideDropdowns();
        $('div.dialog').dialog('close');
        var dt = getISODateTime(new Date($('#NewMeetingDate').val() + " " + $('#NewMeetingTime').val()));
        var args = "?dt=" + dt;
        var q = $("#orgsearchform").serialize();
        $.post("/OrgSearch/CreateMeetings" + args, q, function () {
            var args2 = "?dt1=" + dt + "&dt2=" + dt;
            $("#orgsearchform").attr("action", "/Reports/AttendanceDetail" + args2);
            $("#orgsearchform").submit();
        });
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
    $("MeetingsForDateRangeDialog").on("dialogclose", function (event, ui) {
        $("#attdetail2").off("click");
    });
    $(".StartEndReport").click(function (ev) {
        ev.preventDefault();
        hideDropdowns();
        var action = this.href;
        var d = $('#MeetingsForDateRangeDialog');
        d.dialog('open');
        $('#attdetail2').on("click", function (ev2) {
            ev2.preventDefault();
            $('div.dialog').dialog('close');
            var args = "?dt1=" + $('#MeetingDate1').val() + "&dt2=" + $('#MeetingDate2').val();
            $("#orgsearchform").attr("action", action + args);
            $("#orgsearchform").submit();
            return false;
        });
        return false;
    });
    $("MeetingsForMonthDialog").on("dialogclose", function (event, ui) {
        $("#meetingsformonth2").off("click");
    });
    $(".MonthReport").click(function(ev) {
        ev.preventDefault();
        hideDropdowns();
        var action = this.href;
        var d = $('#MeetingsForMonthDialog');
        d.dialog('open');
        $('#meetingsformonth2').on("click", function (ev2) {
            ev2.preventDefault();
            $('div.dialog').dialog('close');
            var args = "?dt1=" + $('#monthdt1').val();
            $("#orgsearchform").attr("action", action + args);
            $("#orgsearchform").submit();
            return false;
        });
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
        var href = this.href;
        var q = $("#orgsearchform").serialize();
        $.post("/OrgSearch/Count", q, function(cnt) {
            bootbox.confirm("Are you sure you want to replace settings on " + cnt + " organizations? There is no undo button.", function(result) {
                if (result) {
                    $.post(href, q, function () {
                        $.growlUI("Completed", "Settings Replaced");
                    });
                }
            });
        });
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
        //$.hideDropdowns();
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
        $("#orgsearchform").attr("action", "/Reports/EnrollmentControl2a");
        $("#orgsearchform").submit();
        return false;
    });
});


