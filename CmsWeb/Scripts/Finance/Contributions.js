$(function () {
    $(".bt").button();
    $(".datepicker").datepicker();
    $.fmtTable = function () {
        $('table.grid > tbody > tr:even').addClass('alt');
    };
    $.fmtTable();
    $("#search").click(function (ev) {
        ev.preventDefault();
        $.getTable();
        return false;
    });
    $.gotoPage = function (ev, pg) {
        $("#Page").val(pg);
        $.getTable();
        return false;
    };
    $.setPageSize = function (ev) {
        $('#Page').val(1);
        $("#PageSize").val($(ev).val());
        return $.getTable();
    };
    $.getTable = function () {
        var f = $('#results').closest('form');
        var q = f.serialize();
        $.block();
        $.post('/Finance/Contributions/Results', q, function (ret) {
            $('#results').replaceWith(ret).ready($.fmtTable);
            $.unblock();
        });
    };
    $("#NewSearch").click(function () {
        form.reset();
    });
    $("#export").click(function(ev) {
        var f = $(this).closest('form');
        f.attr("action", "/Finance/Contributions/Export");
        f.submit();
    });
    $('.tip').tooltip({ showBody: "|" });
});