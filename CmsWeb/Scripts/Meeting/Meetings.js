$(function () {
    $("#Dt1").change(function () {
        $("#Dt2").val("");
        $.reloadmeetings();
    });
    $("#Dt2").change(function () {
        $.reloadmeetings();
    });
    $.reloadmeetings = function() {
        $("#meetingsform").submit();
    };
    $('#Inactive').change($.reloadmeetings);
    $('#NoZero').change($.reloadmeetings);
    $("a.sortable").click(function (ev) {
        ev.preventDefault();
        var newsort = $(this).text();
        var oldsort = $("#Pager_Sort").val();
        $("#Pager_Sort").val(newsort);
        var dir = $("#Pager_Direction").val();
        if (oldsort === newsort && dir == 'asc')
            $("#Pager_Direction").val('desc');
        else
            $("#Pager_Direction").val('asc');
        $.reloadmeetings();
    });
    $("a.submit").click(function(ev) {
        ev.preventDefault();
        $("#meetingsform").attr("action", this.href);
        $("#meetingsform").submit();
        return false;
    });
});
