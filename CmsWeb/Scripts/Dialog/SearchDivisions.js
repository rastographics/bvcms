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
    $.getTable = function () {
        var f = $('#results').closest('form');
        var q = f.serialize();
        $.post($('#search').attr('href'), q, function (ret) {
            $('#results').replaceWith(ret).ready($.formatTable);
        });
        return false;
    };
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


