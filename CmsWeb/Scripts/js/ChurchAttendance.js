$(function () {
    $(".datepicker").jqdatepicker();
    $("a.run").button();
    $("a.run").click(function (ev) {
        ev.preventDefault();
        if (!$.DateValid($("#Sunday").val(), true))
            return;
        window.location = "/Reports/ChurchAttendance/" + $.SortableDate($("#Sunday").val());
    });
    $("table.centered tbody tr:odd").addClass("alt");
});