$(function () {

    $("#wandtarget,#name").keypress(function (ev) {
        if (ev.which != 13)
            return true;
        $.mark();
        return false;
    });

    $("body").on("click", '.atck', function (ev) {
        var ck = $(this);
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
                    $("#" + i).text(ret[i]);
                }
            }
        });
    });

    $.mark = function () {
        var tb = $("#wandtarget");
        var q = $("#markform").serialize();
        $.post("/Meeting/ScanTicket/", q, function (ret) {
            $("#mark").html(ret).ready(function () {
                if ($("#haserror").val())
                    $.ionSound.play("computer_error");
                else {
                    $.ionSound.play("glass");
                    $("#NumPresent").text($("#npresent").val());
                    $("#NumNewVisit").text($("#nnew").val());
                    $("#NumMembers").text($("#nmembers").val());
                    $("#NumRepeatVst").text($("#nrecent").val());
                    $("#NumOtherAttends").text($("#nother").val());
                    $("#NumVstMembers").text($("#nvmembers").val());
                }
                if ($("#SwitchMeeting").val() > 0) {
                    $.post("/Meeting/TicketMeeting/" + $("#SwitchMeeting").val(), null, function (ret) {
                        $("#meeting").html(ret).ready(function () {
                            $("#wandtarget").focus();
                        });
                    });
                }
            });
            tb.val("");
        });
    };

    $("#name").autocomplete({
        autoFocus: true,
        minLength: 3,
        source: function (request, response) {
            $.post("/Meeting/Names", request, function (ret) {
                response(ret.slice(0, 10));
            }, "json");
        },
        select: function (event, ui) {
            $("#wandtarget").val(ui.item.Pid);
            $.mark();
            $("#name").val('');
            return false;
        }
    }).data("uiAutocomplete")._renderItem = function (ul, item) {
        return $("<li>")
            .append("<a>" + item.Name + "<br>" + item.Addr + "</a>")
            .appendTo(ul);
    };

    $(".atck").change(function (ev) {
        var ck = $(this);
        var tr = ck.parent().parent();
    });

    $("#wandtarget").focus();
    $.ionSound({
        sounds: [ "computer_error", "glass" ],
        path: "/Content/touchpoint/lib/ion-sound/sounds/",
        multiPlay: false,
    });
});

function AddSelected(ret) {
    if (ret.error) {
        $("#mark").html("<div class='alert alert-danger'><strong>Error in adding Guests</strong><p>" + ret.error + "</p>");
        $.ionSound.play("computer_error");
    } else {
        $("#mark").html("<div class='alert alert-success'><strong>Success!</strong> Guests added.");
        $.ionSound.play("glass");
    }
}

