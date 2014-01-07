$(function () {
    $(".bt").button();

    $("#wandtarget,#name").keypress(function (ev) {
        if (ev.which != 13)
            return true;
        $.mark();
        return false;
    });
    $(".atck").live("click", function (ev) {
        var ck = $(this);
        var tr = ck.parent().parent();
        $.post("/Meeting/MarkAttendance/", {
            MeetingId: $("#meetingid").val(),
            PeopleId: ck.attr("pid"),
            Present: ck.is(':checked')
        }, function (ret) {
            if (ret.error) {
                ck.attr("checked", !ck.is(':checked'));
                alert(ret.error);
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
                    PlaySound("zylo");
                else {
                    PlaySound("ding");
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
    $('#visitorDialog').dialog({
        title: 'Add Guests Dialog',
        bgiframe: true,
        autoOpen: false,
        width: 750,
        height: 700,
        modal: true,
        overlay: {
            opacity: 0.5,
            background: "black"
        }, close: function () {
            $('iframe', this).attr("src", "");
        }
    });
    $('#addvisitor').click(function (e) {
        e.preventDefault();
        var d = $('#visitorDialog');
        $('iframe', d).attr("src", this.href);
        d.dialog("open");
    });
    $("#wandtarget").focus();
    $("#search-add a.commit").live("click", function (ev) {
        ev.preventDefault();
        var f = $(this).closest("form");
        var q = f.serialize();
        var loc = $(this).attr("href");
        $.post(loc, q, function (ret) {
            f.modal("hide");
            if (ret.message) {
                alert(ret.message);
            } else
                switch (ret.from) {
                    case 'Menu':
                        window.location = '/Person2/' + ret.pid;
                        break;
                    default:
                        AddSelected(ret);
                        break;
                }
        });
        return false;
    });
});
function PlaySound(soundObj) {
    var sound = document.getElementById(soundObj);
    sound.Play();
}
function AddSelected(ret) {
    $('#visitorDialog').dialog("close");
    if (ret.error) {
        $("#mark").html("<div class='bgred'><h1>Error in adding Guests</h1><p>" + ret.error + "</p>");
        PlaySound("zylo");
    } else {
        $("#mark").html("<div class='bgyellow'><h1>Guests Added</h1>");
        PlaySound("ding");
    }
}

