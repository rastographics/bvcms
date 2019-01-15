$(function () {

    $.fn.editableform.buttons = '<button type="submit" class="btn btn-primary btn-sm editable-submit">' +
                                    '<i class="fa fa-fw fa-check"></i>' +
                                '</button>' +
                                '<button type="button" class="btn btn-default btn-sm editable-cancel">' +
                                    '<i class="fa fa-fw fa-times"></i>' +
                                '</button>';

    $(".clickSelectG").editable({
        mode: 'popup',
        type: 'select',
        url: "/Meeting/EditGroup/",
        source: "/Meeting/MeetingTypes/",
        sourceOptions: {
            type: 'post'
        },
        params: function (params) {
            var data = {};
            data['id'] = params.pk;
            data['value'] = params.value;
            return data;
        },
        success: function (response, newValue) {
            if (newValue === 'True') {
                $(".headcount").editable("enable");
            } else {
                $(".headcount").editable($("#RegularMeetingHeadCount").val());
            }
        }
    });

    $(".clickSelectC").editable({
        mode: 'popup',
        type: 'select',
        url: "/Meeting/EditAttendCredit/",
        source: "/Meeting/AttendCredits/",
        sourceOptions: {
            type: 'post'
        },
        params: function (params) {
            var data = {};
            data['id'] = params.pk;
            data['value'] = params.value;
            return data;
        }
    });

    $(".clickSelectD").editable({
        mode: 'popup',
        type: 'select',
        url: "/Meeting/EditMeetingCategory/",
        source: "/Meeting/MeetingCategories/",
        sourceOptions: {
            type: 'post'
        },
        params: function (params) {
            var data = {};
            data['id'] = params.pk;
            data['value'] = params.value;
            return data;
        }
    });

    $(".headcount").editable({
        mode: 'popup',
        type: 'text',
        url: "/Meeting/Edit/",
        success: function (response, newValue) {
            if (newValue.startsWith("error:"))
                swal("Error!", newValue, "error");
            newValue = "";
        },
        error: function (response, newValue) {
            if (newValue.startsWith("error:"))
                swal("Error!", newValue, "error");
            newValue = "";
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
        url: "/Meeting/Edit/",
        success: function (response, newValue) {
            if (newValue.startsWith("error:"))
                swal("Error!", newValue, "error");
            newValue = "";
        },
        error: function (response, newValue) {
            if (newValue.startsWith("error:"))
                swal("Error!", newValue, "error");
            newValue = "";
        },
        params: function (params) {
            var data = {};
            data['id'] = params.pk;
            data['value'] = params.value;
            return data;
        }
    });

    $('#addallguests').click(function (e) {
        swal({
            title: "Are you sure you want to join all guests to org?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-warning",
            confirmButtonText: "Yes, continue!",
            closeOnConfirm: true
        },
        function () {
            $.post("/Meeting/JoinAllVisitors/" + $("#meetingid").val(), {}, function (ret) {
                swal(ret);
            });
        });
    });

    if ($("#showbuttons input[name=show]:checked").val() === "attends") {
        $(".atck:not(:checked)").parent().parent().hide();
    }

    $.atckenabled = $('#editing').is(':checked');

    $('#showbuttons input:radio').change(function () {
        var r = $(this);
        $.block();

        $("#attends > tbody > tr").hide();
        switch (r.val()) {
            case "attends":
                $(".atck:checked").parent().parent().show();
                break;
            case "absents":
                $(".atck:not(:checked)").parent().parent().show();
                break;
            case "reg":
                $(".commitment:not(:contains('Uncommitted'))").parent().parent().show();
                $(".atck:checked").parent().parent().show();
                break;
            case "all":
                $("#attends > tbody > tr").show();
                break;
        }
        $.unblock();
    });

    $('#currmembers').change(function () {
        if ($(this).is(':checked'))
            window.location = "/Meeting/" + $("#meetingid").val() + "?CurrentMembers=true";
        else
            window.location = "/Meeting/" + $("#meetingid").val();
    });

    $('#editing').change(function () {
        if ($(this).is(':checked')) {
            if (!$("#showregistered").val()) {
                $.block();
                $('#showbuttons input:radio[value=all]').click();
                $.unblock();
            }
            $.atckenabled = true;
        }
        else
            $.atckenabled = false;
    });

    $('#sortbyname').click(function () {
        if ($("#sort").val() === "false") {
            $("#sort").val("true");
            $('#attends > tbody > tr').sortElements(function (a, b) {
                return $(a).find("td.name a").text() > $(b).find("td.name a").text() ? 1 : -1;
            });
        }
        else {
            $("#sort").val("false");
            $('#attends > tbody > tr').sortElements(function (a, b) {
                var art = $(a).attr("rowtype");
                var brt = $(b).attr("rowtype");
                if (art > brt)
                    return -1;
                else if (art < brt)
                    return 1;
                return $(a).find("td.name a").text() > $(b).find("td.name a").text() ? 1 : -1;
            });
        }
    });

    $('#registering').change(function () {
        if ($(this).is(':checked')) {
            $(".showreg").show();
            $("#addregistered").removeClass("hidden");
        }
        else {
            $(".showreg").hide();
            $("#addregistered").addClass("hidden");
        }
    });

    if ($("#showregistered").val()) {
        $('#showbuttons input:radio[value=reg]').click();
        $('#registering').click();
    }

    $('#attends').bind('mousedown', function (e) {
        if ($.atckenabled) {
            if ($(e.target).hasClass("rgck")) {
                $(e.target).editable({
                    mode: 'popup',
                    type: 'select',
                    url: "/Meeting/EditCommitment/",
                    source: "/Meeting/AttendCommitments/",
                    sourceOptions: {
                        type: 'post'
                    },
                    params: function (params) {
                        var data = {};
                        data['id'] = params.pk;
                        data['value'] = params.value;
                        return data;
                    }
                });
            }
            if ($(e.target).hasClass("atck")) {
                e.preventDefault();
                var ck = $(e.target);
                $.atckClick(ck);
            }
        }
    });

    $.atckClick = function (ck) {
        ck.prop("checked", !ck.prop("checked"));
        var tr = ck.parent().parent();
        $.post("/Meeting/MarkAttendance/", {
            MeetingId: $("#meetingid").val(),
            PeopleId: ck.attr("pid"),
            Present: ck.is(':checked')
        }, function (ret) {
            if (ret.error) {
                ck.attr("checked", !ck.is(':checked'));
                swal("Error!", ret.error, "error");
            } else {
                tr.effect("highlight", {}, 3000);
                for (var i in ret) {
                    $("#" + i + " span").text(ret[i]);
                }
            }
        });
    };

    $("#wandtarget").keypress(function (ev) {
        if (ev.which !== 13)
            return true;
        if (!$("#editing").is(':checked'))
            $("#editing").click();

        var tb = $("#wandtarget");
        var text = tb.val();
        tb.val("");
        if (text.substring(2, 0) === "M.") {
            $.post("/Meeting/CreateMeeting/", { id: text }, function (ret) {
                if (ret.substring(5, 0) === "error")
                    swal("Error!", ret, "error");
                else
                    window.location = ret;
            });
            return false;
        }
        var cb = $('input[pid=' + text + '].atck');
        if (cb[0]) {
            cb[0].scrollIntoView();
            $.atckClick(cb);
        }
        return false;
    });
    $("#wandtarget").focus();

    $.extraEditable = function () {
        $(".editarea").editable({
            mode: 'popup',
            type: 'textarea',
            url: "/Meeting/EditExtra/",
            params: function (params) {
                var data = {};
                data['id'] = params.pk;
                data['value'] = params.value;
                return data;
            }
        });

        $(".editline").editable({
            mode: 'popup',
            type: 'text',
            url: "/Meeting/EditExtra/",
            params: function (params) {
                var data = {};
                data['id'] = params.pk;
                data['value'] = params.value;
                return data;
            }
        });
    };

    $("body").on("click", '#newextravalue', function (ev) {
        ev.preventDefault();
        $('#newvalueform').modal("show");
    });

    $('#newvalueform').on('shown.bs.modal', function (e) {
        $("#fieldname").val("").focus();
    });

    $("body").on("click", '#extra-value-submit', function (ev) {
        ev.preventDefault();
        var ck = $("#multiline").is(':checked');
        var fn = $("#fieldname").val();
        var v = $("#fieldvalue").val();
        if (fn)
            $.post("/Meeting/NewExtraValue/" + $("#meetingid").val(), { field: fn, value: v, multiline: ck }, function (ret) {
                if (ret.startsWith("error"))
                    swal("Error!", ret, "error");
                else {
                    $("#extras > tbody").html(ret);
                    $.extraEditable();
                }
                $("#fieldname").val("");
            });
        $('#newvalueform').modal("hide");
    });

    $("body").on("click", 'a.deleteextra', function (ev) {
        ev.preventDefault();
        var field = $(this).attr("field");

        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete it!",
            closeOnConfirm: true
        },
        function () {
            $.post("/Meeting/DeleteExtra/" + $("#meetingid").val(), { field: field }, function (ret) {
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

    $("#contactreport").click(function (ev) {
        ev.preventDefault();
        var href = this.href;
        swal({
            title: "Guests/Absentees Contact Report",
            text: "Enter Sub-Group prefix (optional):",
            type: "input",
            showCancelButton: true,
            closeOnConfirm: true,
            animation: "slide-from-top",
            inputPlaceholder: "Sub-Group prefix"
        }, function(inputValue) {
            if (inputValue === false)
                return false;
            if (inputValue === "")
                window.open(href, '_blank');
            else
                window.open(href + "?prefix=" + inputValue, '_blank');
            return false;
        });
        return false;
    });


    $.InitFunctions.ReloadPeople = function () {
        window.location.reload(true);
    };
});

function AddSelected(ret) {
    if (ret.error)
        swal("Error!", ret.error, "error");

    window.location.reload(true);
}
