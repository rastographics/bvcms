$(function () {
    $("#refresh").click(function (ev) {
        ev.preventDefault();
        location.reload();
    });
    $("#back").click(function (ev) {
        ev.preventDefault();
        location = "/Meeting/" + $("#meetingid").val();
    });
    $("#abcsdialog a").click(function (ev) {
        ev.preventDefault();
        $('.ui-dialog').dialog('close')
        var v = $(this).text();
        $("div.ckline").hide();
        if (v === "All")
            $("div.ckline").show();
        else
            $("." + v).show();
        $('html, body').animate({ scrollTop: 0 }, 'fast');
    });
    $("input[type='radio']").bind("change", function (event, ui) {
        var v = $(this).val();
        $("div.ckline").hide();
        if (v === "back")
            location = "/Meeting/" + $("#meetingid").val();
        else if (v === "refresh")
            location.reload();
        else if (v === "all")
            $("div.ckline").show();
        else
            $("." + v).show();
        $('html, body').animate({ scrollTop: 0 }, 'fast');
    });
    $("#abcs2").change(function () {
        var v = $(this).val();
        $("div.ckline").hide();
        if (v === "all")
            $("div.ckline").show();
        else
            $("." + v).show();
        $('html, body').animate({ scrollTop: 0 }, 'fast');
    });
    $("div.na").bind("click", function (ev) {
        ev.preventDefault();
        return false;
    });
    $("div.ckline input:checkbox").change(function (ev) {
        var ck = $(this);
        $.post("/Meeting/MarkAttendance/", {
            MeetingId: $("#meetingid").val(),
            PeopleId: ck.attr("id").substr(2),
            Present: ck.is(':checked')
        }, function (ret) {
            if (ret.error) {
                ck.attr("checked", !ck.is(':checked'));
                alert(ret.error);
            }
        });
    });
});
