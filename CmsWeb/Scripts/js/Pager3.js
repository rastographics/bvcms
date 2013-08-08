$(function () {
    //$.getTable = function (d) {
    //    var q = {};
    //    if (d.hasClass("loaded"))
    //        q = d.find("form").serialize();
    //    $.ajax({
    //        type: 'POST',
    //        url: d.data("action"),
    //        data: q,
    //        success: function (data, status) {
    //            d.html(data);
    //            d.addClass("loaded");
    //        }
    //    });
    //    return false;
    //};
    //$('table.grid > thead a.sortable').live("click", function () {
    //    var d = $(this).closest("div.loaded");
    //    var newsort = $(this).text();
    //    var sort = $("#Sort", d);
    //    var dir = $("#Direction", d);
    //    if ($(sort).val() == newsort && $(dir).val() == 'asc')
    //        $(dir).val('desc');
    //    else
    //        $(dir).val('asc');
    //    $(sort).val(newsort);
    //    $.getTable(d);
    //    return false;
    //});
    //$.showTable = function (d) {
    //    if (!d.hasClass("loaded"))
    //        $.getTable(d);
    //    return false;
    //};
    //$.updateTable = function (d) {
    //    if (!d.hasClass("loaded"))
    //        $.getTable(f);
    //    return false;
    //};
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
