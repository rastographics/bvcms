$(function () {
    $('#organization-tabs').tabdrop();

    $('#org-main-section').on('show.bs.collapse', function () {
        toggleIcons($('#org-main-collapse i'), true);
    });

    $('#org-main-section').on('hide.bs.collapse', function () {
        toggleIcons($('#org-main-collapse i'), false);
    });

    function toggleIcons(ele, expand) {
        if (expand) {
            $(ele).removeClass("fa-chevron-circle-right").addClass('fa-chevron-circle-down');
        } else {
            $(ele).removeClass("fa-chevron-circle-down").addClass('fa-chevron-circle-right');
        }
    }

    var xs = $('.device-xs').is(':visible');
    if (xs) {
        $('#org-main-section').collapse('hide');
    } else {
        $('#org-main-section').collapse('show');
    }

    $.fn.editableform.buttons = '<button type="submit" class="btn btn-primary btn-sm editable-submit">' +
                                    '<i class="fa fa-fw fa-check"></i>' +
                                '</button>' +
                                '<button type="button" class="btn btn-default btn-sm editable-cancel">' +
                                    '<i class="fa fa-fw fa-times"></i>' +
                                '</button>';

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
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

    $("a[href='#Settings-tab']").on('shown.bs.tab', function (e) {
        if ($("#SettingsOrg").length < 2) {
            $("a[href='#SettingsOrg']").click().tab("show");
        }
    });

    $("#tab-area > ul.nav-tabs > li > a").on('shown.bs.tab', function (e) {
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

    $('body').on('click', 'a.click-pencil', function (e) {
        e.stopPropagation();
        $(this).prev().editable('toggle');
        return false;
    });

    $('body').on('click', '#excludesg', function (ev) {
        ev.stopPropagation();
        $(this).toggleClass("active");
        if ($(this).hasClass("active"))
            $("a.selectsg .fa-minus").show();
        else
            $("a.selectsg .fa-minus").hide();
    });

    $('body').on('click', 'a.selectsg', function (ev) {
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

    $('body').on('click', '#showhide', function (ev) {
        ev.preventDefault();
        $(this).toggleClass("active");
        $("#ShowHidden").val($(this).hasClass("active"));
        $.RebindMemberGrids();
        return false;
    });

    $('body').on('click', 'a.setfilter', function (ev) {
        ev.preventDefault();
        $("#FilterIndividuals").val(!$("#filter-ind").hasClass("active"));
        $.RebindMemberGrids();
        return false;
    });

    $('body').on('click', '#filter-ind', function (ev) {
        ev.preventDefault();
        $(this).toggleClass("active");
        $("#FilterIndividuals").val($(this).hasClass("active"));
        $.RebindMemberGrids();
        return false;
    });

    $('body').on('click', '#filter-tag', function (ev) {
        ev.preventDefault();
        $(this).toggleClass("active");
        $("#FilterTag").val($(this).hasClass("active"));
        $.RebindMemberGrids();
        return false;
    });

    $('body').on('click', '#clear-filter', function (ev) {
        ev.preventDefault();
        $("textarea[name='sgFilter']").val('');
        $("textarea[name='nameFilter']").val('');
        $("#FilterTag").val(false);
        $("#FilterIndividuals").val(false);
        $.RebindMemberGrids();
    });

    $('body').on('click', '#ministryinfo', function (ev) {
        ev.preventDefault();
        $(this).toggleClass("active");
        $("#ShowMinistryInfo").val($(this).hasClass("active"));
        $.RebindMemberGrids();
        return false;
    });

    $('body').on('click', '#showaddress', function (ev) {
        ev.preventDefault();
        $(this).toggleClass("active");
        $("#ShowAddress").val($(this).hasClass("active"));
        $.RebindMemberGrids();
        return false;
    });

    $('body').on('click', '#multigroup', function (event) {
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
        $.RebindMemberGrids();
        return false;
    });

    $('body').on('click', '#groupSelector button.grp', function (event) {
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
        $.RebindMemberGrids();
        return false;
    });

    var prevChecked = null;

    $('body').on('click', 'input.omck', function (e) {
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
        var url = "/Org/ToggleCheckboxes/" + $("#QueryId").val();
        $.post(url, {
            pids: a,
            chkd: prevChecked.checked
        });
    });

    $('#deleteorg').click(function (ev) {
        ev.preventDefault();
        var href = $(this).attr("href");
        swal({
            title: "Are you sure you want to delete?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete organization!",
            closeOnConfirm: true
        },
        function () {
            $.block();
            $.post(href, null, function (ret) {
                if (ret.startsWith("error")) {
                    $.unblock();
                    swal("Error!", ret, "error");
                }
                else if (ret !== "ok") {
                    $.unblock();
                    window.location = ret;
                }
                else {
                    $.unblock();
                    swal({
                        title: "Organization Deleted!",
                        type: "success"
                    },
                    function () {
                        window.location = "/";
                    });
                }
            });
        });
        return false;
    });

    $('#sendreminders').click(function (ev) {
        ev.preventDefault();
        var href = $(this).attr("href");

        swal({
            title: "Are you sure you want to send reminders?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-warning",
            confirmButtonText: "Yes, send reminders!",
            closeOnConfirm: true
        },
        function () {
            $.block();
            $.post(href, null, function (ret) {
                if (ret != "ok") {
                    $.unblock();
                    swal("Error!", ret, "error");
                }
                else {
                    $.unblock();
                    swal({
                        title: "Reminders Sent!",
                        type: "success"
                    });
                }
            });
        });

    });

    $('body').on('click', 'a.members-dialog', function (ev) {
        var $a = $(this);
        ev.preventDefault();
        $("<form class='ajax modal-form' />").load(this.href, {}, function () {
            var f = $(this);
            $('#empty-dialog').html(f);
            $('#empty-dialog').modal("show");

            $('#empty-dialog').on('hidden', function () {
                f.remove();
                $.RebindMemberGrids();
            });
        });
    });

    $('body').on('click', 'a.membertype', function (ev) {
        ev.preventDefault();
        var $a = $(this);
        $("<form class='modal-form validate ajax' />").load(this.href, {}, function () {
            var f = $(this);
            $('#empty-dialog').html(f);
            $('#empty-dialog').modal("show");

            $(".clickEdit", f).editable({
                mode: 'popup',
                type: 'textarea',
                url: "/OrgMemberDialog/EditQuestion/",
                params: function (params) {
                    var data = {};
                    data['id'] = params.pk;
                    data['value'] = params.value;
                    return data;
                }
            });
            $(".delete", f).click(function () {
                ev.preventDefault();
                var dd = $(this).parent();
                var dt = dd.prev();
                $.post("/OrgMemberDialog/DeleteQuestion/" + this.id, function() {
                    dd.remove();
                    dt.remove();
                });
                return false;
            });


            $('#empty-dialog').on('hidden', function () {
                f.remove();
                $.RebindMemberGrids();
            });
        });
    });

    $('body').on('click', '#orgpicklist', function (ev) {
        ev.preventDefault();
        $("<form id='select-orgs' class='modal-form validate ajax' />").load(this.href, {}, function () {
            var f = $(this);
            $('#empty-dialog').html(f);
            $('#empty-dialog').modal("show");
            $.initializeSelectOrgsDialog(f);

            $('#empty-dialog').on('hidden', function () {
                f.remove();
            });
        });
    });

    function UpdateSelectedOrgs(list, f) {
        $.post("/Org/UpdateOrgIds", { id: $("#OrganizationId").val(), list: list }, function (ret) {
            $("#orgpickdiv").replaceWith(ret);
            $('#empty-dialog').modal("hide");
        });
    }

    $.initializeSelectOrgsDialog = function (f) {
        $('body').on('click', '#select-orgs .UpdateSelected', function (ev) {
            ev.preventDefault();
            var list = $('#select-orgs input[type=checkbox]:checked').map(function () {
                return $(this).val();
            }).get().join(',');

            UpdateSelectedOrgs(list, f);
            return false;
        });
        $('body').on('click', '#select-orgs a.move', function (ev) {
            ev.preventDefault();
            var $this = $(this).parent().parent();
            var $a = $this.find("a");
            $this.insertBefore($this.siblings(':eq(0)'));
            $a.appendTo($this.next().find("td")[1]);
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

    $('body').on('click', '#divisionlist', function (ev) {
        ev.preventDefault();
        var a = $(this);
        $("<div />").load(a.attr("href"), {}, function () {
            var d = $(this);
            var dialog = d.find('div.modal-dialog');
            var f = d.find("form");
            $('#empty-dialog').html(dialog);
            $('#empty-dialog').modal("show");
            dialog.on('hidden', function () {
                d.remove();
                dialog.remove();
            });
            dialog.on("click", "#select-div-ok", function () {
                $('#empty-dialog').modal("hide");
                a.load(a.data("refresh"), {});
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
        $('[data-toggle="popover"]').popover({ html: true });
        $('[data-toggle="popover"]').click(function (ev) {
            ev.preventDefault();
        });
    };

    $.InitFunctions.popovers();

    $.InitFunctions.timepicker = function (f) {
        $.InitializeDateElements();
    };

    $.InitFunctions.ReloadMeetings = function (f) {
        $("#Meetings-tab").load("/Org/Meetings", { id: $("input[name=Id]", "#Meetings-tab").val() });
    }

    $.InitFunctions.showHideRegTypes = function (f) {
        $('#Reg-tab').show();
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

    $('body').on('change', '#org_RegistrationTypeId', $.InitFunctions.showHideRegTypes);

    $('body').on('click', '#selectquestions a', function (ev) {
        ev.preventDefault();
        $.post('/Org/NewAsk/', { id: 'AskItems', type: $(this).attr("type") }, function (ret) {
            $('html, body').animate({ scrollTop: $("body").height() }, 800);
            var newli = $("#QuestionList").append(ret);
            $.InitFunctions.updateQuestionList();
            $.InitFunctions.popovers();
            $.InitFunctions.movequestions();
            $('#QuestionList').children().last().effect("highlight", { color: '#eaab00' }, 2000);
        });
        return false;
    });

    $('body').on('click', 'ul.enablesort a.del', function (ev) {
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
            if ($.inArray(type, $.exceptions) >= 0 || $("div.type-" + type).length == 0)
                $(this).html("<a href='#' type='" + type + "'>" + text + "</a>");
            else
                $(this).html("<span style='text-decoration: line-through;'>" + text + "</span>");
        });
    };

    $(".helptip").tooltip({ showBody: "|", blocked: true });

    $('body').on('click', 'form.DisplayEdit a.submitbutton', function (ev) {
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

    $('body').on('click', '#Future', function () {
        $.formAjaxClick($(this));
    });

    $.InitFunctions.CreateMeeting = function ($a) {
    };

    $('body').on('click', 'a.taguntag', function (ev) {
        ev.preventDefault();
        $.block();
        var a = $(this);
        $.post('/Org/ToggleTag/' + $(this).attr('pid'), null, function (ret) {
            var link = $(ev.target).closest('a');
            link.removeClass('btn-default').removeClass('btn-success');
            link.addClass(ret == "Remove" ? "btn-default" : "btn-success");
            link.html(ret == "Remove" ? "<i class='fa fa-tag'></i> Remove" : "<i class='fa fa-tag'></i> Add");
            $.unblock();
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

    $('body').on('keypress', '#nameFilter,#sgFilter', function (e) {
        if ((e.keyCode || e.which) === 13) {
            e.preventDefault();
            $.RebindMemberGrids();
        }
        return true;
    });

    $('body').on('click', '#addsch', function (ev) {
        ev.preventDefault();
        var href = $(this).attr("href");
        if (href) {
            var f = $(this).closest('form');
            $.post(href, null, function (ret) {
                $("#schedules").append(ret).ready(function () {
                    $.InitFunctions.timepicker();
                    $.renumberListItems();
                    $('#schedules').children().last().children().first().effect("highlight", { color: '#eaab00' }, 2000);
                });
            });
        }
        return false;
    });

    $('body').on('click', 'a.delete-well', function (ev) {
        ev.preventDefault();
        $(this).closest("div.well").parent().remove();
        $.renumberListItems();
    });

    $.renumberListItems = function () {
        var i = 1;
        $(".renumberMe").each(function () {
            $(this).val(i);
            i++;
        });
    };

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
            swal("Error!", "Enter valid time.", "error");
            v = false;
        }
        if (!$.DateValid(d)) {
            swal("Error!", "Enter valid time.", "error");
            v = false;
        }
        return { date: d, time: t, valid: v };
    };

    $('body').on('click', 'a.joinlink', function (ev) {
        ev.preventDefault();
        var a = $(this);

        swal({
            title: a.attr("confirm"),
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-warning",
            confirmButtonText: "Yes, continue!",
            closeOnConfirm: true
        },
        function () {
            $.post(a[0].href, function (ret) {
                if (ret === "ok")
                    $.RebindMemberGrids();
                else
                    swal("Error!", ret, "error");
            });
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

    $('body').on('click', 'a.deleteextra', function (ev) {
        ev.preventDefault();
        var a = $(this);
        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete it!",
            closeOnConfirm: false
        },
        function () {
            $.post("/Organization/DeleteExtra/" + $("#OrganizationId").val(), { field: $(a).attr("field") }, function (ret) {
                if (ret.startsWith("error"))
                    swal("Error!", ret, "error");
                else {
                    $("#extras > tbody").html(ret);
                    $.extraEditable();
                }
            });
        });
        return false;
    });

    $.updateTable = function (a) {
        if (!a)
            return false;
        var $form = a.closest("form.ajax");
        if ($form.length)
            $.formAjaxClick(a);
        return false;
    };

    $.InitFunctions.ReloadPeople = function () {
        $.RebindMemberGrids();
    };

    $('body').on('change', '#Schedule_Value', function () {
        var ss = $(this).val().split(',');
        var meetingDate = ss[0];
        if ($('.modal #MeetingDate').parent().data("DateTimePicker")) {
            $('.modal #MeetingDate').parent().data("DateTimePicker").date(meetingDate);
        } else {
            var meetingDateIso = moment(new Date(meetingDate)).format("YYYY-MM-DDTHH:mm");
            $('.modal #MeetingDate').val(meetingDateIso);
        }
        $(".modal #AttendCredit_Value").val(ss[1]);
    });

    $("body").on("click", 'div.newitem > a', function (ev) {
        if (!$(this).attr("href"))
            return false;
        ev.preventDefault();
        var a = $(this);
        var f = a.closest("form");
        $.post(a.attr("href"), null, function (ret) {
            var dest = a.data('dest');
            var $destTag = $(dest, a.closest('.movable'));
            if (!$destTag.length)
                $destTag = $(dest, a.closest('.well'));
            if (!$destTag.length)
                $destTag = $(dest);
            $destTag.append(ret);
            $.InitFunctions.movequestions();
            $.InitFunctions.timepicker();
            $(dest).children().last().children().first().effect("highlight", { color: '#eaab00' }, 2000);
        });
    });

    $('body').on('click', '.dropMember', function(ev) {
        ev.preventDefault();

        var orgId = $('#Id').val();
        var peopleId = $(this).data('people-id');

        swal({
            title: 'Are you sure?',
            type: 'warning',
            showCancelButton: true,
            confirmButtonClass: "btn-warning",
            confirmButtonText: "Yes, Drop Member",
            closeOnConfirm: true
        },
        function() {
            $.post('/OrgDrop/DropSingleMember', { orgId: orgId, peopleId: peopleId }, function(ret) {
                window.location.reload(true);
            });
        });

        return false;
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
            if ($(this).children('div.movable').length < 3) {
                $(this).children('div.movable').find('div a.cut').addClass('disabled');
            } else {
                $(this).children('div.movable').find('div a.cut').removeClass('disabled');
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

                swal({
                    title: "Are you sure?",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonClass: "btn-danger",
                    confirmButtonText: "Yes, delete it!",
                    closeOnConfirm: true
                },
                function (isConfirm) {
                    if (isConfirm) {
                        $(liToMove).remove();
                        $.InitFunctions.updateQuestionList();
                        $.InitFunctions.movequestions();
                    } else {
                        return false;
                    }
                });
                return false;
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

    if (window.collapseOrgSection === true) {
        $('#org-main-section').collapse('hide');
    }
});

$.RebindMemberGrids = function() {
    $.formAjaxClick($("a.setfilter"));
}

function AddSelected() {
    $.RebindMemberGrids();
}

function CloseAddDialog(from) {
    $("#memberDialog").dialog("close");
}
