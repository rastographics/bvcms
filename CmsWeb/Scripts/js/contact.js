$(function () {
    $(".bt").button();
    //$('table.grid > tbody > tr:even').addClass('alt');
    $("#edit").live("click", function (ev) {
        ev.preventDefault();
        $.post($(this)[0].href, {}, function (ret) {
            $("#contact").html(ret).ready(function () {
                $(".bt").button();
                $(".datepicker").datepicker();
                $("#newteamcontact,a.addperson.bt").hide();
                $('a.goto').bind('click', false);
                $('a.remove').bind('click', false);
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
                $("#newteamcontact,a.addperson.bt").show();
                $('a.remove').unbind('click', false);
                $('a.goto').unbind('click', false);
            });
        });
        return false;
    });
    $("#cancel").live("click", function (ev) {
        ev.preventDefault();
        $.post($(this)[0].href, {}, function (ret) {
            $("#contact").html(ret).ready(function () {
                $(".bt").button();
                $("#newteamcontact,a.addperson.bt").show();
                $('a.goto').unbind('click', false);
                $('a.remove').unbind('click', false);
            });
        });
        return false;
    });
    $("a.remove").click(function (ev) {
        ev.preventDefault();
        $.post($(this)[0].href, {}, function (ret) {
            window.location.reload(true);
        });
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
    $(".addtask").live("click", function (ev) {
        ev.preventDefault();
        if (confirm("Add new task for person?")) {
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