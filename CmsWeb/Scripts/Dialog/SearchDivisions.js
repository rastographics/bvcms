$(function () {
    $("#clear").click(function () {
        $("input:text").val("");
    });
    //$('#name').focus();
    $("#search").click(function (ev) {
        ev.preventDefault();
        $.getTable();
        return false;
    });
    $(".bt").button();
    $.getTable = function () {
        var f = $('#results').closest('form');
        var q = f.serialize();
        $.post($('#search').attr('href'), q, function (ret) {
            $('#results').replaceWith(ret).ready($.formatTable);
        });
        return false;
    };
    $("#close").click(function () {
        window.parent.$('#divisionsDialog').dialog('close');
    });
    $.formatTable = function () {
        $("td.tip").tooltip({
            showURL: false,
            showBody: "|"
        });
    };
    $.formatTable();
    $('input:checkbox').live("change", function (ev) {
        var sp = $(this).parents('tr:eq(0)').find("span.move");
        var ck = $(this).is(":checked");
        var did = $(this).attr("value");
        $.post("/SearchDivisions/AddRemoveDiv/",
            {
                id: $("#id").val(), divid: did, ischecked: ck
            }, function (ret) {
                if (ck)
                    sp.html("<a href='#' class='move' value='" + did + "'>move to top</a>");
                else
                    sp.empty();
            });
    });
    $("a.move").live('click', function (ev) {
        ev.preventDefault();
        var f = $('#results').closest('form');
        $("#topid").val($(this).attr("value"));
        var q = f.serialize();
        $.post("/SearchDivisions/MoveToTop", q, function (ret) {
            $('#results').replaceWith(ret).ready($.formatTable);
        });
    });
    $("input").keypress(function (e) {
        if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
            $('#search').click();
            return false;
        }
        return true;
    });
});


