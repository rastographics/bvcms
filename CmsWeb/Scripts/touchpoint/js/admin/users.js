$(function () {
    $("#clear").click(function (ev) {
        ev.preventDefault();
        $("input:text").val("");
        $("#Role").val(-1);
        return false;
    });

    $("#search").click(function (ev) {
        ev.preventDefault();
        $.getTable();
        return false;
    });

    $.gotoPage = function (e, pg) {
        $("#Page").val(pg);
        $.getTable();
        return false;
    };

    $.setPageSize = function (e) {
        $('#Page').val(1);
        $("#PageSize").val($(e).val());
        return $.getTable();
    };

    $('body').on('click', '#resultsTable > thead a.sortable', function (ev) {
        ev.preventDefault();
        var newsort = $(this).text();
        var sort = $("#Sort");
        var dir = $("#Direction");
        if ($(sort).val() == newsort && $(dir).val() == 'asc')
            $(dir).val('desc');
        else
            $(dir).val('asc');
        $(sort).val(newsort);
        $.getTable();
        return false;
    });

    $.getTable = function () {
        var f = $('#usersearch');
        var q = f.serialize();
        $.block();
        $.post(f.attr('action'), q, function(ret) {
            $('#results').replaceWith(ret).ready(function() {
                $("#totalcount").text($("#totcnt").val());
                $(".tip").tooltip({ showBody: "|", showURL: false });
                $.unblock();
            });
        });
        return false;
    };

    $('#Role').multiselect({
        maxHeight: 200,
        includeSelectAllOption: true
    });

    $(".tip").tooltip({ showBody: "|", showURL: false });
    $("#name").focus();
});


