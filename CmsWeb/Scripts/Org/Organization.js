$(function () {
    $('a[data-toggle="tab"]').on('shown', function (e) {
        e.preventDefault();
        var tab = $(e.target).attr('href').replace("#", "#tab-");
        window.location.hash = tab;
        $.cookie('lasttab', tab);
        return false;
    });
    var lastTab = $.cookie('lasttab');
    if (window.location.hash) {
        lastTab = window.location.hash;
    }
    $("a[href='#Settings-tab']").on('shown', function (e) {
        if ($("#SettingsOrg").length < 2) {
            $("a[href='#SettingsOrg']").click().tab("show");
        }
    });
    $("#tab-area > ul.nav-tabs > li > a").on('shown', function (e) {
        switch ($(this).text()) {
            case "People":
                $("#bluetoolbarstop li > a.qid").parent().removeClass("hidy");
                break;
            case "Settings":
            case "Meetings":
                $("#bluetoolbarstop li > a.qid").parent().addClass("hidy");
                break;
        }
    });
    if (lastTab) {
        var tlink = $("a[href='" + lastTab.replace("tab-", "") + "']");
        var tabparent = tlink.closest("ul").data("tabparent");
        if (tabparent) {
            $("a[href='#" + tabparent + "']").click().tab("show");
        }
        if (tlink.attr("href") !== '#') {
            $.cookie('lasttab', tlink.attr("href"));
            tlink.click().tab("show");
        }
    }

    $.InitFunctions.Editable = function () {
        $("a.editable").editable();
        $("a.editable-bit").editable({ type: 'checklist', mode: 'inline', source: { 'True': 'True' }, emptytext: 'False' });
    };
    $('a.click-pencil').live("click", function (e) {
        e.stopPropagation();
        $(this).prev().editable('toggle');
        return false;
    });
    $("#excludesg").live("click", function (ev) {
        ev.stopPropagation();
        $(this).toggleClass("active");
        if ($(this).hasClass("active"))
            $("a.selectsg .fa-minus").show();
        else
            $("a.selectsg .fa-minus").hide();
    });
    $("a.selectsg").live("click", function (ev) {
        ev.preventDefault();
        var t = $(this).text();
        var sg = $("#sgFilter").val();
        switch (t) {
            case "Match All":
                sg = "All:" + sg;
                break;
            case "None Assigned":
                sg = "None";
                break;
            default:
                if (sg && sg !== "All:")
                    sg = sg + ';';
                if ($("#excludesg").hasClass("active"))
                    t = '-' + t;
                sg = sg + t;
                break;
        }
        $("#sgFilter").val(sg);
        $("#excludesg").removeClass("active");
        $("a.selectsg .fa-minus").hide();
        return false;
    });
    $("#showhide").live("click", function (ev) {
        ev.preventDefault();
        $(this).toggleClass("active");
        $("#ShowHidden").val($(this).hasClass("active"));
        RebindMemberGrids();
        return false;
    });
    $("a.setfilter").live("click", function (ev) {
        ev.preventDefault();
        $("#FilterIndividuals").val(!$("#filter-ind").hasClass("active"));
        RebindMemberGrids();
        return false;
    });
    $("#filter-ind").live("click", function (ev) {
        ev.preventDefault();
        $(this).toggleClass("active");
        $("#FilterIndividuals").val($(this).hasClass("active"));
        RebindMemberGrids();
        return false;
    });
    $("#filter-tag").live("click", function (ev) {
        ev.preventDefault();
        $(this).toggleClass("active");
        $("#FilterTag").val($(this).hasClass("active"));
        RebindMemberGrids();
        return false;
    });
    $("#clear-filter").live("click", function (ev) {
        ev.preventDefault();
        $("textarea[name='sgFilter']").val('');
        $("textarea[name='nameFilter']").val('');
        $("#FilterTag").val(false);
        $("#FilterIndividuals").val(false);
        RebindMemberGrids();
    });
    $("#ministryinfo").live("click", function (ev) {
        ev.preventDefault();
        $(this).toggleClass("active");
        $("#ShowMinistryInfo").val($(this).hasClass("active"));
        RebindMemberGrids();
        return false;
    });
    $("#showaddress").live("click", function (ev) {
        ev.preventDefault();
        $(this).toggleClass("active");
        $("#ShowAddress").val($(this).hasClass("active"));
        RebindMemberGrids();
        return false;
    });
    $("#multigroup").live("click", function (event) {
        event.preventDefault();
        var $this = $(this);
        $this.toggleClass('active');
        var ismulti = $this.hasClass("active");
        $("#MultiSelect").val(ismulti);
        if (ismulti) {
            $("#groupSelector button.dropdown-toggle").hide();
            $("li.orgcontext").hide();
        }
        else {
            $("#groupSelector button.grp.active").removeClass("active");
            $("#groupSelector button[value='10']").addClass("active").closest("button.dropdown-toggle").show();
            $("#GroupSelect").val("10");
            $("#showhide").removeClass("active");
            $("#ShowHidden").val($("#showhide").hasClass("active"));
        }
        RebindMemberGrids();
        return false;
    });
    $("#groupSelector button.grp").live("click", function (event) {
        event.preventDefault();
        var $this = $(this);
        if ($("#multigroup").hasClass("active")) {
            $this.toggleClass('active');
        } else {
            $("#groupSelector button.grp.active").removeClass("active");
            $("#groupSelector button.dropdown-toggle").hide();
            $this.addClass("active");
            $this.next().find("button.dropdown-toggle").show();
            $("li.orgcontext").hide();;
            switch ($this.text()) {
                case "Members":
                    $("li.current-list").show();
                    break;
                case "Pending":
                    $("li.pending-list").show();
                    break;
            }
        }
        var $a = "";
        $("#groupSelector button.grp.active").each(function () {
            $a += $(this).val();
        });
        if ($a === "") {
            $this.toggleClass("active");
            return false;
        }
        $("#GroupSelect").val($a);
        RebindMemberGrids();
        return false;
    });
    var prevChecked = null;
    $("input.omck").live("click", function (e) {
        var pids = null;
        if (e.shiftKey && prevChecked) {
            var start = $("input.omck").index(this); // the one we just checked
            var end = $("input.omck").index(prevChecked); // the prev one checked
            pids = $("input.omck").slice(Math.min(start, end), Math.max(start, end) + 1);
            pids.attr('checked', prevChecked.checked); // make them all the same as the lastChecked
        } else {
            pids = $(this);
            prevChecked = this;
        }
        var a = pids.map(function () { return $(this).val(); }).get();
        $.post("/Org/ToggleCheckboxes/{0}".format($("#Id").val()), {
            pids: a,
            chkd: prevChecked.checked
        });
    });

    $('#deleteorg').click(function (ev) {
        ev.preventDefault();
        var href = $(this).attr("href");
        if (confirm('Are you sure you want to delete?')) {
            $.block("deleting org");
            $.post(href, null, function (ret) {
                if (ret != "ok") {
                    window.location = ret;
                }
                else {
                    $.block("org deleted");
                    $('.blockOverlay').attr('title', 'Click to unblock').click(function () {
                        $.unblock();
                        window.location = "/";
                    });
                }
            });
        }
        return false;
    });
    $('#sendreminders').click(function (ev) {
        ev.preventDefault();
        var href = $(this).attr("href");
        if (confirm('Are you sure you want to send reminders?')) {
            $.block("sending reminders");
            $.post(href, null, function (ret) {
                if (ret != "ok") {
                    $.unblock();
                    $.growlUI("error", ret);
                }
                else {
                    $.unblock();
                    $.growlUI("Email", "Reminders Sent");
                }
            });
        }
    });
    $('#reminderemails').click(function (ev) {
        ev.preventDefault();
        var href = $(this).attr("href");
        if (confirm('Are you sure you want to send reminders?')) {
            $.block("sending reminders");
            $.post(href, null, function (ret) {
                if (ret != "ok") {
                    $.block(ret);
                    $('.blockOverlay').attr('title', 'Click to unblock').click($.unblock);
                }
                else {
                    $.block("org deleted");
                    $('.blockOverlay').attr('title', 'Click to unblock').click(function () {
                        $.unblock();
                        window.location = "/";
                    });
                }
            });
        }
        return false;
    });

    $(".CreateAndGo").click(function (ev) {
        ev.preventDefault();
        if (confirm($(this).attr("confirm")))
            $.post($(this).attr("href"), null, function (ret) {
                window.location = ret;
            });
        return false;
    });
    $('a.members-dialog').live("click", function (ev) {
        var $a = $(this);
        ev.preventDefault();
        $("<div />").load(this.href, {}, function () {
            var d = $(this);
            var f = d.find("form");
            f.modal("show");
            $.DatePickersAndChosen();
            f.on('hidden', function () {
                d.remove();
                f.remove();
                RebindMemberGrids();
            });
        });
    });

    $("a.membertype").live("click", function (ev) {
        ev.preventDefault();
        $("<div />").load(this.href, {}, function () {
            var d = $(this);
            var f = d.find("form");
            f.modal("show");
            f.on('hidden', function () {
                d.remove();
                f.remove();
                RebindMemberGrids();
            });
        });
    });

    $("#orgpicklist").live("click", function (ev) {
        ev.preventDefault();
        $("<div />").load(this.href, {}, function () {
            var d = $(this);
            var f = d.find("form");
            f.modal("show");
            $.initializeSelectOrgsDialog(f);

            f.on('hidden', function () {
                d.remove();
                f.remove();
            });
        });
    });
//    $("a.notifylist").live("click", function (ev) {
//        ev.preventDefault();
//        $("<div />").load(this.href, {}, function () {
//            var d = $(this);
//            var f = d.find("form");
//            f.modal("show");
//            f.on('hidden', function () {
//                d.remove();
//                f.remove();
//            });
//        });
//    });

    function UpdateSelectedOrgs(list, f) {
        $.post("/Org/UpdateOrgIds", { id: $("#OrganizationId").val(), list: list }, function (ret) {
            $("#orgpickdiv").replaceWith(ret);
            f.modal("hide");
        });
    }
    $.initializeSelectOrgsDialog = function (f) {
        $("#select-orgs #UpdateSelected").live("click", function (ev) {
            ev.preventDefault();
            var list = $('#select-orgs input[type=checkbox]:checked').map(function () {
                return $(this).val();
            }).get().join(',');

            UpdateSelectedOrgs(list, f);
            return false;
        });
        $("#select-orgs").on('keydown', "#name", function (ev) {
            if (ev.keyCode === 13) {
                ev.preventDefault();
                $('#select-orgs #search').click();
                return false;
            }
            return true;
        });
        $.SaveOrgIds = function (ev) {
            var list = $('#select-orgs input[type=checkbox]:checked').map(function () {
                return $(this).val();
            }).get().join(',');
            $.post("/SearchOrgs/SaveOrgIds/" + $("#select-orgs #id").val(), { oids: list });
        };
        $('body').on('change', '#select-orgs input:checkbox', $.SaveOrgIds);
    };


    $("#divisionlist").live("click", function (ev) {
        ev.preventDefault();
        var a = $(this);
        $("<div />").load(a.attr("href"), {}, function () {
            var d = $(this);
            var f = d.find("form");
            f.modal("show");
            f.on('hidden', function () {
                a.load(a.data("refresh"), {});
                d.remove();
                f.remove();
            });
            f.on("change", "input:checkbox", function () {
                $("input[name='TargetDivision']", f).val($(this).val());
                $("input[name='Adding']", f).val($(this).is(":checked"));
                $.formAjaxClick($(this), "/SearchDivisions/AddRemoveDiv");
            });
            f.on("click", "a.move", function () {
                $("input[name='TargetDivision']", f).val($(this).data("moveid"));
                $.formAjaxClick($(this), "/SearchDivisions/MoveToTop");
            });
        });
    });

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

    $.InitFunctions.popovers = function (f) {
        $('[data-toggle="popover"]').popover({ html: true, placement: 'bottom' });

        $('body').on('click', function (e) {
            $('[data-toggle="popover"]').each(function () {
                //the 'is' for buttons that trigger popups
                //the 'has' for icons within a button that triggers a popup
                if (!$(this).is(e.target) && $(this).has(e.target).length === 0 && $('.popover').has(e.target).length === 0) {
                    $(this).popover('hide');
                }
            });
        });
    };

    $.InitFunctions.popovers();

    $.InitFunctions.timepicker = function (f) {
        $(".timepicker").datetimepicker({
            format: "H:ii P",
            showMeridian: true,
            autoclose: true,
            todayBtn: false,
            pickerPosition: "bottom-left",
            startView: 1,
            minView: 0,
            maxView: 1
        });

        $(".datetimepicker-hours table thead, .datetimepicker-minutes table thead").attr('style', 'display:block; overflow:hidden; height:0;');
    };
    $.InitFunctions.ReloadMeetings = function (f) {
        $("#Meetings-tab").load("/Org/Meetings", { id: $("input[name=Id]", "#Meetings-tab").val() });
    }
    $.InitFunctions.showHideRegTypes = function (f) {
        $("#Fees-tab").show();
        $("#Questions-tab").show();
        $("#Messages-tab").show();

        $("#QuestionList").show();
        $("#TimeSlotsList").hide();
        switch ($("#RegistrationType_Value").val()) {
            case "0":
                $("#Fees-tab").hide();
                $("#Questions-tab").hide();
                $("#Messages-tab").hide();
                break;
            case "6":
                $("#QuestionList").hide();
                $("#TimeSlotsList").show();
                break;
        }
    };
    $("#org_RegistrationTypeId").live("change", $.InitFunctions.showHideRegTypes);

    $('#selectquestions a').live("click", function (ev) {
        ev.preventDefault();
        $.post('/Org/NewAsk/', { id: 'AskItems', type: $(this).attr("type") }, function (ret) {
            $('#addQuestions').modal('hide');
            $('html, body').animate({ scrollTop: $("body").height() }, 800);
            var newli = $("#QuestionList").append(ret);
            $.InitFunctions.updateQuestionList();
            $.InitFunctions.popovers();
            $.InitFunctions.movequestions();
        });
        return false;
    });

    $("ul.enablesort a.del").live("click", function (ev) {
        ev.preventDefault();
        if (!$(this).attr("href"))
            return false;
        $(this).parent().parent().parent().remove();
        return false;
    });

    $.exceptions = [
        "AskDropdown",
        "AskCheckboxes",
        "AskExtraQuestions",
        "AskHeader",
        "AskInstruction",
        "AskMenu",
        "AskText"
    ];
    $.InitFunctions.updateQuestionList = function () {
        $("#selectquestions li").each(function () {
            var type = this.className;
            var text = $(this).text();
            if (!text)
                text = type;
            if ($.inArray(type, $.exceptions) >= 0 || $("li.type-" + type).length == 0)
                $(this).html("<a href='#' type='" + type + "'>" + text + "</a>");
            else
                $(this).html("<span style='text-decoration: line-through;'>" + text + "</span>");
        });
    };

    $(".helptip").tooltip({ showBody: "|", blocked: true });

    $("form.DisplayEdit a.submitbutton").live('click', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        if (!$(f).valid())
            return false;
        var q = f.serialize();
        $.post($(this).attr('href'), q, function (ret) {
            if (ret.startsWith("error:")) {
                $("div.formerror", f).html(ret.substring(6));
            } else {
                $(f).html(ret).ready(function () {
                    $(".submitbutton,.bt").button();
                    $.regsettingeditclick(f);
                    $.showHideRegTypes();
                });
            }
        });
        return false;
    });

    $("#Future").live('click', function () {
        $.formAjaxClick($(this));
    });

    //    $("input[name='showHidden']").live('click', function () {
    //        $.formAjaxClick($(this));
    //    });
    //    $("#Future").live('click', function () {
    //        $.formAjaxClick($(this));
    //    });

    /*
    $("form.DisplayEdit").submit(function () {
        if (!$("#submitit").val())
            return false;
        return true;
    });
*/
    $.InitFunctions.CreateMeeting = function ($a) {

    };
    $('a.taguntag').live("click", function (ev) {
        ev.preventDefault();
        $.post('/Org/ToggleTag/' + $(this).attr('pid'), null, function (ret) {
            $(ev.target).text(ret);
        });
        return false;
    });
    $.validator.addMethod("time", function (value, element) {
        return this.optional(element) || /^\d{1,2}:\d{2}\s(?:AM|am|PM|pm)/.test(value);
    }, "time format h:mm AM/PM");
    $.validator.setDefaults({
        highlight: function (input) {
            $(input).addClass("ui-state-highlight");
        },
        unhighlight: function (input) {
            $(input).removeClass("ui-state-highlight");
        }
    });
    $("#orginfoform").validate({
        rules: {
            "org.OrganizationName": { required: true, maxlength: 100 }
        }
    });
    // validate signup form on keyup and submit
    $("#settingsForm").validate({
        rules: {
            "org.SchedTime": { time: true },
            "org.OnLineCatalogSort": { digits: true },
            "org.Limit": { digits: true },
            "org.NumCheckInLabels": { digits: true },
            "org.NumWorkerCheckInLabels": { digits: true },
            "org.FirstMeetingDate": { date: true },
            "org.LastMeetingDate": { date: true },
            "org.RollSheetVisitorWks": { digits: true },
            "org.GradeAgeStart": { digits: true },
            "org.GradeAgeEnd": { digits: true },
            "org.Fee": { number: true },
            "org.Deposit": { number: true },
            "org.ExtraFee": { number: true },
            "org.ShirtFee": { number: true },
            "org.ExtraOptionsLabel": { maxlength: 50 },
            "org.OptionsLabel": { maxlength: 50 },
            "org.NumItemsLabel": { maxlength: 50 },
            "org.GroupToJoin": { digits: true },
            "org.RequestLabel": { maxlength: 50 },
            "org.DonationFundId": { number: true }
        }
    });

    $("#nameFilter,#sgFilter").live("keypress", function (e) {
        if ((e.keyCode || e.which) === 13) {
            e.preventDefault();
            RebindMemberGrids();
        }
        return true;
    });

    $("#addsch").live("click", function (ev) {
        ev.preventDefault();
        var href = $(this).attr("href");
        if (href) {
            var f = $(this).closest('form');
            $.post(href, null, function (ret) {
                $("#schedules").append(ret).ready(function () {
                    $.InitFunctions.timepicker();
                    $.renumberListItems();
                });
            });
        }
        return false;
    });

    $("a.delete-well").live("click", function (ev) {
        ev.preventDefault();
        $(this).closest("div.well").remove();
        $.renumberListItems();
    });

    $.renumberListItems = function () {
        var i = 1;
        $(".renumberMe").each(function () {
            $(this).val(i);
            i++;
        });
    };
    /*
    $('#RollsheetLink').live("click", function (ev) {
        ev.preventDefault();
        $('#grouplabel').text("By Group");
        $("tr.forMeeting").hide();
        $("tr.forRollsheet").show();
        var d = $("#NewMeetingDialog");
        d.dialog("option", "buttons", {
            "Ok": function () {
                var dt = $.GetNextMeetingDateTime();
                if (!dt.valid)
                    return false;
                var args = "?org=curr&dt=" + dt.date + " " + dt.time;
                if ($('#altnames').is(":checked"))
                    args += "&altnames=true";
                if ($('#group').is(":checked"))
                    args += "&bygroup=1";
                if ($("#highlightsg").val())
                    args += "&highlight=" + $("#highlightsg").val();
                if ($("#sgprefixrs").val())
                    args += "&sgprefix=" + $("#sgprefixrs").val();
                window.open("/Reports/Rollsheet/" + args);
                $(this).dialog("close");
            }
        });
        d.dialog('open');
    });
    $('#RallyRollsheetLink').live("click", function (ev) {
        ev.preventDefault();
        $('#grouplabel').text("By Group");
        $("tr.forMeeting").hide();
        $("tr.forRollsheet").show();
        var d = $("#NewMeetingDialog");
        d.dialog("option", "buttons", {
            "Ok": function () {
                var dt = $.GetNextMeetingDateTime();
                if (!dt.valid)
                    return false;
                var args = "?org=curr&dt=" + dt.date + " " + dt.time;
                if ($('#altnames').is(":checked"))
                    args += "&altnames=true";
                if ($('#group').is(":checked"))
                    args += "&bygroup=1&sgprefix=";
                if ($("#highlightsg").val())
                    args += "&highlight=" + $("#highlightsg").val();
                if ($("#sgprefix").val())
                    args += "&sgprefix=" + $("#sgprefix").val();
                window.open("/Reports/RallyRollsheet/" + args);
                $(this).dialog("close");
            }
        });
        d.dialog('open');
    });
    */
    $("#ScheduleListPrev").change(function () {
        var a = $(this).val().split(",");
        $("#PrevMeetingDate").val(a[0]);
        $("#NewMeetingTime").val(a[1]);
        $("#AttendCreditList").val(a[2]);
    });
    $("#ScheduleListNext").change(function () {
        var a = $(this).val().split(",");
        $("#NextMeetingDate").val(a[0]);
        $("#NewMeetingTime").val(a[1]);
        $("#AttendCreditList").val(a[2]);
    });
    $.GetPrevMeetingDateTime = function () {
        var d = $('#PrevMeetingDate').val();
        return $.GetMeetingDateTime(d);
    };
    $.GetNextMeetingDateTime = function () {
        var d = $('#NextMeetingDate').val();
        return $.GetMeetingDateTime(d);
    };
    $.GetMeetingDateTime = function (d) {
        var reTime = /^ *(\d{1,2}):[0-5][0-9] *((a|p|A|P)(m|M)){0,1} *$/;
        var t = $('#NewMeetingTime').val();
        var v = true;
        if (!reTime.test(t)) {
            $.growlUI("error", "enter valid time");
            v = false;
        }
        if (!$.DateValid(d)) {
            $.growlUI("error", "enter valid date");
            v = false;
        }
        return { date: d, time: t, valid: v };
    };

    $('a.joinlink').live('click', function (ev) {
        ev.preventDefault();
        var a = $(this);
        bootbox.confirm(a.attr("confirm"), function (result) {
            if (result) {
                $.post(a[0].href, function (ret) {
                    if (ret === "ok")
                        RebindMemberGrids();
                    else
                        alert(ret);
                });
            }
        });
        return false;
    });

    $.extraEditable = function () {
        $('.editarea').editable('/Organization/EditExtra/', {
            type: 'textarea',
            submit: 'OK',
            rows: 5,
            width: 200,
            indicator: '<img src="/Content/images/loading.gif">',
            tooltip: 'Click to edit...'
        });
        $(".editline").editable("/Organization/EditExtra/", {
            indicator: "<img src='/Content/images/loading.gif'>",
            tooltip: "Click to edit...",
            style: 'display: inline',
            width: 200,
            height: 25,
            submit: 'OK'
        });
    };
    $.extraEditable();
    $("a.deleteextra").live("click", function (ev) {
        ev.preventDefault();
        if (confirm("are you sure?"))
            $.post("/Organization/DeleteExtra/" + $("#OrganizationId").val(), { field: $(this).attr("field") }, function (ret) {
                if (ret.startsWith("error"))
                    alert(ret);
                else {
                    $("#extras > tbody").html(ret);
                    $.extraEditable();
                }
            });
        return false;
    });

    // Add for ministrEspace
    /*
    var submitDialog = $("#dialogHolder").dialog({ modal: true, width: 'auto', title: 'Select ministrEspace Event', autoOpen: false });
    $("#addMESEvent").click(function (ev) {
        ev.preventDefault();
        var id = $(this).attr("orgid");
        submitDialog.html("<div style='text-align:center; margin-top:20px;'>Loading...</div>");
        submitDialog.dialog('open');
        $.post("/Organization/DialogAdd/" + id + "?type=MES", null, function (data) {
            submitDialog.html(data);
            submitDialog.dialog({ position: { my: "center", at: "center" } });
            $(".bt").button();
        });
    });
    $("#closeSubmitDialog").live("click", null, function (ev) {
        ev.preventDefault();
        $(submitDialog).dialog("close");
    });
    */
    $.updateTable = function (a) {
        if (!a)
            return false;
        var $form = a.closest("form.ajax");
        if ($form.length)
            $.formAjaxClick(a);
        return false;
    };
    $.InitFunctions.ReloadPeople = function () {
        RebindMemberGrids();
    };
    $("#Schedule_Value").live("change", function () {
        var ss = $(this).val().split(',');
        $(".modal #MeetingDate").val(ss[0]);
        $(".modal #AttendCredit_Value").val(ss[1]);
    });

    $("body").on("click", 'div.newitem > a', function (ev) {
        if (!$(this).attr("href"))
            return false;
        ev.preventDefault();
        var a = $(this);
        var f = a.closest("form");
        $.post(a.attr("href"), null, function (ret) {
            a.parent().prev().append(ret);
            $.InitFunctions.movequestions();
            $.InitFunctions.timepicker();
        });
    });

    function clearCuttingBoard() {
        $('div.movable-list').each(function () {
            $(this).children('div.movable').each(function () {
                $(this).removeClass('cutting');
            });
        });
    }

    function initializeCutPaste() {
        $('div.movable-list').each(function () {
            if ($(this).children('div.movable').length <= 1) {
                $(this).children('div.movable').find('div a.cut').addClass('disabled');
            }
            $(this).children('div.movable').find('div a.paste').addClass('disabled');
        });
    }

    function enablePaste(ul) {
        $(ul).children('div.movable').each(function () {
            $(this).find('div a.paste').first().removeClass('disabled');
        });
    }

    function enableDisableMoveUpward() {
        $('div.movable-list').each(function () {
            $(this).children('div.movable').find('div a.movetop').removeClass('disabled');
            $(this).children('div.movable').find('div a.moveup').removeClass('disabled');
            $(this).children('div.movable').first().find('div a.movetop').addClass('disabled');
            $(this).children('div.movable').first().find('div a.moveup').addClass('disabled');
        });
    }

    function enableDisableMoveDownward() {
        $('div.movable-list').each(function () {
            $(this).children('div.movable').find('div a.movebottom').removeClass('disabled');
            $(this).children('div.movable').find('div a.movedown').removeClass('disabled');
            $(this).children('div.movable').last().find('div a.movebottom').addClass('disabled');
            $(this).children('div.movable').last().find('div a.movedown').addClass('disabled');
        });
    }

    function moveItem(a, action, e) {
        e.preventDefault();
        if ($(a).hasClass('disabled')) {
            return false;
        }
        var ul = $(a).closest('div.movable-list');
        var liToMove = $(a).closest('div.movable');

        switch (action) {
            case 'top':
                var liFirst = $(ul).children('div.movable').first();
                $(liToMove).clone(true, true).insertBefore(liFirst);
                break;
            case 'up':
                var liPrev = liToMove.prev('div.movable');
                $(liToMove).clone(true, true).insertBefore(liPrev);
                break;
            case 'cut':
                clearCuttingBoard();
                $(liToMove).addClass('cutting');
                enablePaste(ul);
                return false;
                break;
            case 'paste':
                var li = $(a).closest('div.movable');
                liToMove = $(ul).children('div.cutting').first();
                $(liToMove).clone(true, true).insertAfter(li);
                break;
            case 'down':
                var liNext = liToMove.next('div.movable');
                $(liToMove).clone(true, true).insertAfter(liNext);
                break;
            case 'bottom':
                var liLast = $(ul).children('div.movable').last();
                $(liToMove).clone(true, true).insertAfter(liLast);
                break;
            case 'delconfirm':
                if (!$(a).attr("href"))
                    return false;
                if (!confirm("are you sure?")) {
                    return false;
                }
                break;
            case 'delete':
                if (!$(a).attr("href"))
                    return false;
                break;
        }

        $(liToMove).remove();
        $.InitFunctions.updateQuestionList();
        $.InitFunctions.movequestions();
    }

    $.InitFunctions.movequestions = function () {
        clearCuttingBoard();
        initializeCutPaste();
        enableDisableMoveUpward();
        enableDisableMoveDownward();

        $('body a.movetop').off().on('click', function (e) {
            moveItem($(this), 'top', e);
        });

        $('body a.moveup').off().on('click', function (e) {
            moveItem($(this), 'up', e);
        });

        $('body a.cut').off().on('click', function (e) {
            moveItem($(this), 'cut', e);
        });

        $('body a.paste').off().on('click', function (e) {
            moveItem($(this), 'paste', e);
        });

        $('body a.movedown').off().on('click', function (e) {
            moveItem($(this), 'down', e);
        });

        $('body a.movebottom').off().on('click', function (e) {
            moveItem($(this), 'bottom', e);
        });

        $('body a.delconfirm').off().on('click', function (e) {
            moveItem($(this), 'delconfirm', e);
        });

        $('body a.delete').off().on('click', function (e) {
            moveItem($(this), 'delete', e);
        });
    };

    $.InitFunctions.movequestions();

//    $("document").on("focus", "#sgFilter", function () {
//        $(this).autosize();
//    });

});
function RebindMemberGrids() {
    $.formAjaxClick($("a.setfilter"));
}
function AddSelected() {
    RebindMemberGrids();
}
function CloseAddDialog(from) {
    $("#memberDialog").dialog("close");
}
