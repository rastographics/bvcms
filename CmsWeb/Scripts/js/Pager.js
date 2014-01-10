$(document).ready(function () {
    $.gotoPage = function (e, pg) {
        var f = $(e).closest('form');
        $("#Page", f).val(pg);
        return $.getTable(f);
    };
    $.setPageSize = function (e) {
        var f = $(e).closest('form');
        $('#Page', f).val(1);
        $("#PageSize", f).val($(e).val());
        return $.getTable(f);
    };
    $.getTable = function (f) {
        var q;
        if (f)
            q = f.serialize();
        $.post(f.attr('action'), q, function (ret) {
            $(f).html(ret).ready(function () {
                $("a.trigger-dropdown", f).dropdown();
                $('.bt').button();
                $(".datepicker").jqdatepicker();
            });
        });
        return false;
    };
    $('table.grid2 > thead a.sortable').live("click", function () {
        var f = $(this).closest("form");
        var newsort = $(this).text();
        var sort = $("#Sort", f);
        var dir = $("#Direction", f);
        if ($(sort).val() == newsort && $(dir).val() == 'asc')
            $(dir).val('desc');
        else
            $(dir).val('asc');
        $(sort).val(newsort);
        $.getTable(f);
        return false;
    });
    $.showTable = function (f) {
        if ($('table.grid2', f).size() == 0)
            $.getTable(f);
        return false;
    };
    $.updateTable = function (f) {
        if ($('table.grid2', f).size() > 0)
            $.getTable(f);
        return false;
    };
    $("body").on("click", "input[name='toggletarget']", function (ev) {
        if ($('a.target[target="people"]').length == 0) {
            $("a.target").attr("target", "people");
            $("input[name='toggletarget']").attr("checked", true);
        } else {
            $("a.target").removeAttr("target");
            $("input[name='toggletarget']").removeAttr("checked");
        }
    });
});
