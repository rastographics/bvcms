$(function () {
    $('#Name').focus();

    $('#org-search-tabs').tabdrop();

    function initializePopovers() {
        $('[data-toggle="popover"]').popover({ html: true });
        $('[data-toggle="popover"]').click(function (ev) {
            ev.preventDefault();
        });
    }

    initializePopovers();

    $.fn.editableform.buttons = '<button type="submit" class="btn btn-primary btn-sm editable-submit">' +
                                    '<i class="fa fa-fw fa-check"></i>' +
                                '</button>' +
                                '<button type="button" class="btn btn-default btn-sm editable-cancel">' +
                                    '<i class="fa fa-fw fa-times"></i>' +
                                '</button>';
    $.InitFunctions.ReloadPeople = function () {
        $.getTable();
    };

    $("#clear").click(function (ev) {
        ev.preventDefault();
        $("#PublicView").val(false);
        $("input:text").val("");
        $("#ProgramId,#CampusId,#ScheduleId,#TypeId").val(0);
        $("#OnlineReg").val(-1);
        $.post('/OrgSearch/DivisionIds/0', null, function (ret) {
            $('#DivisionId').html(ret);
        });
        $.getTable();
        $('.ev-input').val($('.ev-input option:first').val());
        return false;
    });

    $("#view-main").click(function () {
        $("#PublicView").val(false);
        $("#management-view").hide();
        $('#TagProgramId').val(0);
        $('#TagDiv').val(0);
        $('#TargetType').val(0);
        $.getTable();
    });

    $("#view-manage").click(function () {
        $("#PublicView").val(false);
        $('#TagProgramId').val(0);
        $('#TagDiv').val(0);
        $('#TargetType').val(0);
        $("#management-view").show();
        $.getTable();
    });

    $("#view-public").click(function () {
        $("#PublicView").val(true);
        $("#management-view").hide();
        $('#TagProgramId').val(0);
        $('#TagDiv').val(0);
        $('#TargetType').val(0);
        $.getTable();
    });

    $("#search").click(function (ev) {
        ev.preventDefault();
        var name = $('#Name').val();
        if (name.startsWith("M.")) {
            $('#Name').val("");
            var f = $('#resultsTable').closest('form');
            f.attr("action", "/OrgSearch/CreateMeeting/" + name);
            f.submit();
        }
        $.getTable();
        return false;
    });

    $.gotoPage = function (e, pg) {
        $("#Pager_Page").val(pg);
        $.getTable();
        return false;
    };

    $.setPageSize = function (e) {
        $('#Pager_Page').val(1);
        $("#Pager_PageSize").val($(e).val());
        return $.getTable();
    };

    $.getTable = function () {
        var f = $('#results').closest('form');
        var evs = [];
        $(".ev-input").each(function (i, e) {
            console.log($(e).attr("name"));
            console.log($(e).val());
            console.log("---");
            evs.push($(e).attr("name") + "=" + $(e).val());
        });
        $("#ExtraValues").val(evs);

        var q = f.serialize();
        $.block();
        $.post($('#search').attr('href'), q, function (ret) {
            $('#results').replaceWith(ret).ready(function () {
                $.fmtTable();
                $("#totalcount").text($("#totcnt").val());
                $.setFiltered();
                $.unblock();
            });
        });
        return false;
    };
    $.setFiltered = function () {
        if ($("#filtered").val() === "True")
            $("#totalcount").addClass("alert-danger");
        else
            $("#totalcount").removeClass("alert-danger");
    }
    $.setFiltered();

    $('body').on('click', '#resultsTable > thead a.sortable', function (ev) {
        ev.preventDefault();
        var newsort = $(this).text();
        var sort = $("#Pager_Sort");
        var dir = $("#Pager_Direction");
        if ($(sort).val() == newsort && $(dir).val() == 'asc')
            $(dir).val('desc');
        else
            $(dir).val('asc');
        $(sort).val(newsort);
        $.getTable();
        return false;
    });

    $.fmtTable = function () {
        $('.clickSelect').editable({
            mode: 'popup',
            type: 'select',
            sourceOptions: {
                type: 'post'
            },
            url: '/OrgSearch/Edit',
            params: function (params) {
                var data = {};
                data['id'] = params.pk;
                data['value'] = params.value;
                return data;
            }
        });

        $('.clickDate').editable({
            mode: 'popup',
            type: 'date',
            url: '/OrgSearch/Edit',
            format: 'mm/dd/yyyy',
            viewformat: 'mm/dd/yyyy',
            datepicker: {
                weekStart: 1
            },
            params: function (params) {
                var data = {};
                data['id'] = params.pk;
                data['value'] = params.value;
                return data;
            }
        });

        $(".clickEdit").editable({
            mode: 'popup',
            type: 'text',
            url: "/OrgSearch/Edit/",
            params: function (params) {
                var data = {};
                data['id'] = params.pk;
                data['value'] = params.value;
                return data;
            }
        });

        initializePopovers();
    };

    $('body').on('click', '.descredit', function (ev) {
        ev.preventDefault();
        return $.descredit($(this).prev());
    });

    var xsDevice = $('.device-xs').is(':visible');
    var smDevice = $('.device-sm').is(':visible');

    $.descredit = function ($a) {
        if ($a.text() === "edit")
            $a.html('');

        if (!xsDevice && !smDevice) {
            if (CKEDITOR.instances['editor'])
                CKEDITOR.instances['editor'].destroy();

            CKEDITOR.env.isCompatible = true;

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
                customConfig: '/Content/touchpoint/js/ckeditorconfig.js'
            });
        }
        if (xsDevice || smDevice) {
            $('#editor').val($a.html());
        } else {
            CKEDITOR.instances['editor'].setData($a.html());
        }

        $('#editor-modal').modal('show');

        $("#save-edit").off("click").on("click", function (ev) {
            ev.preventDefault();
            var v;
            if (xsDevice || smDevice) {
                v = $('#editor').val();
            } else {
                v = CKEDITOR.instances['editor'].getData();
            }

            $a.html(v);
            var id = $a.attr("id");
            $.post("/OrgSearch/SetDescription", { id: id, description: v });

            if (xsDevice || smDevice) {
                $('#editor').val('');
            } else {
                CKEDITOR.instances['editor'].setData('');
            }

            $('#editor-modal').modal('hide');
            return false;
        });
        return false;
    };

    $.fmtTable();

    $('#ProgramId').change(function () {
        $.post('/OrgSearch/DivisionIds/' + $(this).val(), null, function (ret) {
            $('#DivisionId').html(ret);
        });
    });

    $("#ApplyType").click(function (ev) {
        ev.preventDefault();
        var f = $('#resultsTable').closest('form');
        var q = f.serialize();

        swal({
            title: "Are you sure?",
            text: "This will apply the selected target type to the entire filtered list.",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-warning",
            confirmButtonText: "Yes, apply change!",
            closeOnConfirm: true
        },
        function () {
            $.block();
            $.post('/OrgSearch/ApplyType/' + $("#TargetType").val(), q, function (ret) {
                $.unblock();
                if (ret !== "") {
                    if (ret === "ok")
                        $.getTable();
                    else {
                        swal("Error!", ret, "error");
                    }
                }
            });
        });
        return false;
    });
    $("#MakeChildrenOf").click(function (ev) {
        ev.preventDefault();
        var f = $('#resultsTable').closest('form');
        var q = f.serialize();
        swal({
            title: "Are you sure?",
            text: "This will make the entire filtered list children of the indicated org",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-warning",
            confirmButtonText: "Yes, apply change!",
            closeOnConfirm: true
        },
        function () {
            $.block();
            $.post('/OrgSearch/MakeChildrenOf/' + $("#ParentOrg").val(), q, function (ret) {
                $.unblock();
                if (ret !== "") {
                    if (ret === "ok")
                        $.getTable();
                    else {
                        swal("Error!", ret, "error");
                    }
                }
            });
        });
        return false;
    });

    $("#AddNewDiv").click(function (ev) {
        ev.preventDefault();
        $('#NewDivProgramId').val($("#TagProgramId option:selected").val());
        $("#NewDiv").val("");
        $('#new-div-modal').modal('show');
    });

    $("#MakeNewDiv").click(function (ev) {
        ev.preventDefault();
        $.post('/OrgSearch/MakeNewDiv/', { id: $("#NewDivProgramId").val(), name: $("#NewDiv").val() }, function (ret) {
            $('#TagDiv').html(ret);
            $("#NewDiv").val("");
            $('#new-div-modal').modal('hide');
            $.getTable();
        });
        return false;
    });

    $("#RenameDivision").click(function (ev) {
        ev.preventDefault();
        if ($('#TagDiv option:selected').val() == 0) {
            swal("Error!", "Must select a target division.", "error");
            return false;
        }
        $("#RenamedDiv").val($("#TagDiv option:selected").text());
        $('#rename-div-modal').modal('show');
    });

    $("#RenameDiv").click(function (ev) {
        ev.preventDefault();
        $.post('/OrgSearch/RenameDiv/', { id: $("#TagProgramId").val(), divid: $("#TagDiv").val(), name: $("#RenamedDiv").val() }, function (ret) {
            if (ret == "error") {
                swal("Error!", "Unexpected error.", "error");
            } else {
                $('#TagDiv').html(ret);
                $("#RenamedDiv").val("");
                $('#rename-div-modal').modal('hide');
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

    $('#ViewAttNotices').click(function (ev) {
        ev.preventDefault();
        $("#orgsearchform").attr("action", "/OrgSearch/DisplayAttendanceNotices");
        $.block();
        $("#orgsearchform").submit();
        return true;
    });
    $('#AttNotices').click(function (ev) {
        ev.preventDefault();
        var did = $('#DivisionId').val();

        swal({
            title: "Are you sure?",
            text: "This will send email notices to leaders based on your filters.",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-warning",
            confirmButtonText: "Yes, send notices!",
            closeOnConfirm: true
        },
        function () {
            $.block();
            var q = $("#orgsearchform").serialize();
            $.post("/OrgSearch/EmailAttendanceNotices", q, function () {
                $.unblock();
                swal({
                    title: "Complete!",
                    text: "Email notices sent.",
                    type: "success"
                });
            });
        });

        return true;
    });

    $('#rollsheet1').click(function (ev) {
        ev.preventDefault();
        var scheduleId = 0;
        if ($('#ScheduleId').length) {
            scheduleId = $('#ScheduleId').val();
        }
        $.post('/OrgSearch/DefaultMeetingDate/' + scheduleId, null, function (ret) {
            $('#MeetingDate').val(ret.date);
            $('#MeetingTime').val(ret.time);
            $("#rollsheet2").attr("href", "/Reports/Rollsheet");
            $('#rollsheet-modal').modal('show');
        });
        return true;
    });

    $('#rollsheet2').click(function (ev) {
        ev.preventDefault();
        $('#rollsheet-modal').modal('hide');
        var args = "?dt=" + $('#MeetingDate').val() + " " + $('#MeetingTime').val();
        if ($('#altnames').is(":checked"))
            args += "&altnames=true";
        if ($('#bygroup').is(":checked"))
            args += "&bygroup=1";
        if ($("#highlightsg").val())
            args += "&highlight=" + $("#highlightsg").val();
        if ($("#sgprefix").val())
            args += "&sgprefix=" + $("#sgprefix").val();
        if ($('#useword').is(":checked"))
            args += "&useword=1";

        if ($('#rallymode').is(":checked"))
            $("#orgsearchform").attr("action", "/Reports/RallyRollsheets" + args);
        else
            $("#orgsearchform").attr("action", "/Reports/Rollsheet" + args);
        $("#orgsearchform").attr("target", "_blank");
        $("#orgsearchform").submit();
        $("#orgsearchform").removeAttr("target");
        return false;
    });

    $('#newmeetings').click(function (ev) {
        ev.preventDefault();
        $.post('/OrgSearch/DefaultMeetingDate/' + $('#ScheduleId').val(), null, function (ret) {
            $('#NewMeetingDate').val(ret.date);
            $('#NewMeetingTime').val(ret.time);
            $('#new-meetings-modal').modal('show');
        });
        return true;
    });

    $('#createmeetings').click(function (ev) {
        ev.preventDefault();
        var dt = getISODateTime(new Date($('#NewMeetingDate').val() + " " + $('#NewMeetingTime').val()));
        var args = "?dt=" + dt + "&noautoabsents=" + ($("#NoAbsenteeRecords").is(":checked") ? "true" : "false");
        var q = $("#orgsearchform").serialize();
        $.post("/OrgSearch/CreateMeetings" + args, q, function () {
            var args2 = "?dt1=" + dt + "&dt2=" + dt;
            $("#orgsearchform").attr("action", "/Reports/AttendanceDetail" + args2);
            $("#orgsearchform").submit();
        });
        $('#new-meetings-modal').modal('hide');
        return false;
    });
    $('#ExportExcel').click(function (ev) {
        ev.preventDefault();
        $("#orgsearchform").attr("action", "/OrgSearch/ExportExcel");
        $("#orgsearchform").submit();
        return true;
    });


    $('#pendingtomember').click(function (ev) {
        ev.preventDefault();
        var q = $("#orgsearchform").serialize();
        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-warning",
            confirmButtonText: "Yes, Move Pending to Member for all orgs shown!",
            closeOnConfirm: true
        },
        function () {
            $.block();
            $.post("/OrgSearch/MovePendingToMember", q, function (ret) {
                $.unblock();
                swal({
                    title: "Completed!",
                    text: ret,
                    type: "success"
                });
            });
        });
    });

    $('#weeklyattendance').click(function (ev) {
        ev.preventDefault();
        $("#orgsearchform").attr("action", "/Reports/WeeklyAttendance");
        $("#orgsearchform").submit();
        return true;
    });

    $('#ExportMembersExcel').click(function (ev) {
        ev.preventDefault();
        $("#orgsearchform").attr("action", "/OrgSearch/ExportMembersExcel");
        $("#orgsearchform").submit();
        return true;
    });

    $('#Meetings').click(function (ev) {
        ev.preventDefault();
        $("#orgsearchform").attr("action", "/Reports/Meetings");
        $("#orgsearchform").submit();
        return true;
    });

    $('#Structure').click(function (ev) {
        ev.preventDefault();
        $("#orgsearchform").attr("action", "/OrgSearch/OrganizationStructure");
        $("#orgsearchform").submit();
        return true;
    });

    $('#RecentAbsents').click(function (ev) {
        ev.preventDefault();
        $("#orgsearchform").attr("action", "/Reports/RecentAbsents");
        $("#orgsearchform").submit();
        return true;
    });

    $(".ReportDate").click(function (ev) {
        ev.preventDefault();
        $('#report-date-action').val(this.href);
        $('#report-date-modal').modal('show');
        return true;
    });
    $('#report-date-modal').on('hidden.bs.modal', function () {
        $("#reportdate2").off("click");
    });
    $('#report-date-modal').on('shown.bs.modal', function () {
        $('#reportdate2').on("click", function (ev2) {
            ev2.preventDefault();
            $('#report-date-modal').modal('hide');
            var args = "?dt=" + $('#ReportDate').val();
            $("#orgsearchform").attr("action", $('#report-date-action').val() + args);
            $("#orgsearchform").submit();
            return false;
        });
    });

    $(".StartEndReport").click(function (ev) {
        ev.preventDefault();
        $('#meetings-daterange-action').val(this.href);
        $('#meetings-daterange-modal').modal('show');
        return true;
    });
    $('#meetings-daterange-modal').on('hidden.bs.modal', function () {
        $("#attdetail2").off("click");
    });

    $('#meetings-daterange-modal').on('shown.bs.modal', function () {
        $('#attdetail2').on("click", function (ev2) {
            ev2.preventDefault();
            $('#meetings-daterange-modal').modal('hide');
            var args = "?dt1=" + $('#MeetingDate1').val() + "&dt2=" + $('#MeetingDate2').val();
            $("#orgsearchform").attr("action", $('#meetings-daterange-action').val() + args);
            $("#orgsearchform").submit();
            return false;
        });
    });


    $(".MonthReport").click(function (ev) {
        ev.preventDefault();
        $('#meetings-month-action').val(this.href);
        $('#meetings-month-modal').modal('show');
        return true;
    });

    $('#meetings-month-modal').on('shown.bs.modal', function () {
        $('#meetingsformonth2').on("click", function (ev2) {
            ev2.preventDefault();
            $('#meetings-month-modal').modal('hide');
            var args = "?dt1=" + $('#monthdt1').val();
            $("#orgsearchform").attr("action", $('#meetings-month-action').val() + args);
            $("#orgsearchform").submit();
            return false;
        });
    });

    $('#meetings-month-modal').on('hidden.bs.modal', function () {
        $("#meetingsformonth2").off("click");
    });

    $('#PasteSettings').click(function (ev) {
        ev.preventDefault();
        var href = this.href;
        var q = $("#orgsearchform").serialize();
        $.post("/OrgSearch/Count", q, function (cnt) {
            swal({
                title: "Are you sure?",
                text: "This will replace settings on " + cnt + " organizations. There is no undo button.",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-warning",
                confirmButtonText: "Yes, replace settings!",
                closeOnConfirm: true
            },
            function () {
                $.block();
                $.post(href, q, function () {
                    $.unblock();
                    swal({
                        title: "Completed!",
                        text: "Settings replaced.",
                        type: "success"
                    });
                });
            });
        });
        return true;
    });

    $('#RepairTransactions').click(function (ev) {
        ev.preventDefault();
        var f = $('form');
        var q = f.serialize();
        var url = this.href;

        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-warning",
            confirmButtonText: "Yes, repair transactions!",
            closeOnConfirm: true
        },
        function () {
            $.block();
            $.post(url, q, function (ret) {
                $.unblock();
                swal({
                    title: "Completed!",
                    text: "Repair completed.",
                    type: "success"
                });
            });
        });
        return true;
    });

    $('a.ViewReport').click(function (ev) {
        ev.preventDefault();
        var href = $(this).attr("href");
        $("#orgsearchform").attr("action", href);
        $("#orgsearchform").attr("target", "_blank");
        $("#orgsearchform").submit();
        $("#orgsearchform").removeAttr("target");
        return true;
    });

    $('body').on('click', 'a.taguntag', function (ev) {
        ev.preventDefault();
        var a = $(this);
        var td = $('#TagDiv').val();
        if (td > 0)
            $.post(a.attr('href'), { tagdiv: td }, function (ret) {
                if (ret == "error")
                    swal("Error!", "Unexpected error, refresh page.", "error");
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
                swal("Error!", "Unexpected error, refresh page.", "error");
            else {
                $(a).parent().parent().replaceWith(ret);
                $.fmtTable();
            }
        });
        return false;
    });

    $('#checkincontrol').click(function (ev) {
        ev.preventDefault();
        $('#checkin-modal').modal('show');
        return true;
    });

    $('#checkincontrol2').click(function (ev) {
        ev.preventDefault();
        $('#checkin-modal').modal('hide');
        var url = "/Reports/CheckinControl/";
        $("#CheckinDate").val($("#checkindate").val());
        $("#CheckinExport").val($("#checkinexport").prop("checked"));
        $("#orgsearchform").attr("action", url);
        $("#orgsearchform").submit();
        return false;
    });

    $('#enrollmentcontrol1').click(function (ev) {
        ev.preventDefault();
        $('#enrollment-modal').modal('show');
        return true;
    });

    $('#enrollmentcontrol2').click(function (ev) {
        ev.preventDefault();
        $('#enrollment-modal').modal('hide');
        var url = "/Reports/EnrollmentControl";
        if ($('#enrcontrolfiltertag').is(":checked"))
            url = $.appendQuery(url, "usecurrenttag=true");
        if ($('#enrcontrolexcel').is(":checked"))
            url = $.appendQuery(url, "excel=true");
        $("#orgsearchform").attr("action", url);
        $("#orgsearchform").submit();
        return false;
    });

    $('#enrollmentcontrol2i').click(function (ev) {
        ev.preventDefault();
        $("#orgsearchform").attr("action", "/Reports/EnrollmentControl2a");
        $("#orgsearchform").submit();
        return true;
    });
    $('#export-messages').click(function (ev) {
        ev.preventDefault();
        $('#export-messages-modal').modal('show');
        return true;
    });

    $('#ExportMessages').click(function (ev) {
        ev.preventDefault();
        $('#export-messages-modal').modal('hide');
        var args = $('#export-messages-modal form').serialize();
        $("#orgsearchform").attr("action", "/OrgSearch/RegMessages?" + args);
        $("#orgsearchform").submit();
    });
    $.appendQuery = function (s, q) {
        if (s && s.length > 0)
            if (s.indexOf("&") != -1 || s.indexOf("?") != -1)
                return s + '&' + q;
            else
                return s + '?' + q;
        return q;
    };

    function toggleExtraValueDisplay(elem) {
        var orgtype = $(elem).find(':selected').text().replace(" ", "").toLowerCase();

        if (orgtype != '(notspecified)') {
            $(".ev-orgtype-cell").hide();
            $("." + orgtype + "-cell").show();
        }
    }

    $("#TypeId").change(function() {
        toggleExtraValueDisplay(this);
    });

    toggleExtraValueDisplay($("#TypeId"));
});


