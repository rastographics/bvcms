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
        $.post('/ContactSearch/Results', q, function (ret) {
            $('#results').replaceWith(ret);
        });
    };
    $("#ConvertToQuery").click(function (ev) {
        ev.preventDefault();
        var f = $('#results').closest('form');
        f.attr("action", $(this).attr('href'));
        f.submit();
        return false;
    });
    $("#ContactorSummary").click(function (ev) {
        ev.preventDefault();
        if ($("#StartDate").val() && $("#EndDate").val() && $("#form").valid()) {
            window.location = $(this).attr('href')
                + "?start=" + $.SortableDate($("#StartDate").val())
                + "&end=" + $.SortableDate($("#EndDate").val())
                + "&ministry=" + $("#Ministry").val();
        } else
            alert("need valid dates");

        return false;
    });
    $("#ContactSummary").click(function (ev) {
        ev.preventDefault();
        var loc = window.location = $(this).attr('href') + 
            "?ministry=" + $("#Ministry").val()
            + "&typeid=" + $("#ContactType").val()
            + "&reas=" + $("#ContactReason").val();
        if ($("#StartDate").val())
            loc = loc.appendQuery("start=" + $.SortableDate($("#StartDate").val()));
        if ($("#EndDate").val())
            loc = loc.appendQuery("end=" + $.SortableDate($("#EndDate").val()));
        window.location = loc;
        return false;
    });
    $("#ContactTypeTotals").click(function (ev) {
        ev.preventDefault();
        var loc = window.location = $(this).attr('href') + "?ministry=" + $("#Ministry").val();
        if ($("#StartDate").val())
            loc = loc.appendQuery("start=" + $.SortableDate($("#StartDate").val()));
        if ($("#EndDate").val())
            loc = loc.appendQuery("end=" + $.SortableDate($("#EndDate").val()));
        hideDropdowns();
        $.block();
        window.location = loc;
        return false;
    });
    $("#NewSearch").click(function () {
        form.reset();
    });
    $.validator.addMethod("date2", function (value, element, params) {
        var v = $.DateValid(value);
        return this.optional(element) || v;
    }, $.format("Please enter valid date"));
    
    $.validator.setDefaults({
        highlight: function (input) {
            $(input).addClass("ui-state-highlight");
        },
        unhighlight: function (input) {
            $(input).removeClass("ui-state-highlight");
        },
        rules: {
            "StartDate": { date2: true },
            "EndDate": { date2: true }
        }
    });
    $('#form').validate();
});