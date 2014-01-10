$(function () {
    $(".bt").button();
    $(".datepicker").jqdatepicker();
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
            $('#results').replaceWith(ret);
            $.unblock();
        });
    };
    $("#NewSearch").click(function () {
        form.reset();
    });
    $("#export").click(function (ev) {
        var f = $(this).closest('form');
        f.attr("action", "/Finance/Contributions/Export");
        f.submit();
    });
    $('.tip').tooltip({ showBody: "|" });
    $("a.submitit").live("click", function (ev) {
        ev.preventDefault();
        if (confirm("Are you sure you want to return/reverse?")) {
            var f = $("#form");
            f.attr("action", $(this)[0].href);
            f.submit();
        }
        return false;
    });
});