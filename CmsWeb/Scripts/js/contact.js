$(function () {
    $(".bt").button();
    $("#edit").live("click", function (ev) {
        ev.preventDefault();
        $.post($(this)[0].href, {}, function (ret) {
            $("#contact").html(ret).ready(function () {
                $(".bt").button();
                $(".datepicker").datepicker();
                $(".in").addClass("out");
            });
        });
        return false;
    });
    $("#update").live("click", function (ev) {
        ev.preventDefault();
        var q = $("#contact").serialize();
        $.post($(this)[0].href, q, function (ret) {
            $("#contact").html(ret).ready(function () {
                $(".bt").button();
                $(".out").removeClass("out");
            });
        });
        return false;
    });
    $("#cancel").live("click", function (ev) {
        ev.preventDefault();
        $.post($(this)[0].href, {}, function (ret) {
            $("#contact").html(ret).ready(function () {
                $(".bt").button();
                $(".out").removeClass("out");
            });
        });
        return false;
    });
    $("a.remove").click(function (ev) {
        ev.preventDefault();
        if ($(this).hasClass("out"))
            return false;
        if (confirm("Remove this person?")) {
            $.post($(this)[0].href, {}, function(ret) {
                window.location.reload(true);
            });
        }
        return false;
    });
    $(".addtask").live("click", function (ev) {
        ev.preventDefault();
        if ($(this).hasClass("out"))
            return false;
        if (confirm("Add new task for person?")) {
            var f = $("#contact");
            f.attr("action", $(this)[0].href);
            f.submit();
        }
        return false;
    });
    $("a.link").live("click", function(ev) {
        ev.preventDefault();
        if ($(this).hasClass("out"))
            return false;
        window.location = $(this)[0].href;
        return false;
    });
    $("#delete").live("click", function (ev) {
        ev.preventDefault();
        if (confirm("Delete this contact?")) {
            var f = $("#contact");
            f.attr("action", $(this)[0].href);
            f.submit();
        }
        return false;
    });
    $("#newteamcontact").live("click", function (ev) {
        ev.preventDefault();
        if (confirm("Add new contact for team?")) {
            var f = $("#contact");
            f.attr("action", $(this)[0].href);
            f.submit();
        }
        return false;
    });

    $('#AddDialog').dialog({
        title: 'Add Dialog',
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
    $('a.addperson.bt').click(function (e) {
        e.preventDefault();
        var d = $('#AddDialog');
        $('iframe', d).attr("src", this.href);
        d.dialog("open");
    });
});
function AddSelected() {
    $('#AddDialog').dialog("close");
    window.location.reload(true);
}