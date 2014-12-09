$(function () {
    $('.input-group.date').datepicker();
    $("a.run").click(function (ev) {
        ev.preventDefault();
        if (!$.DateValid($("#Sunday").val(), true))
            return;
        window.location = "/Reports/ChurchAttendance/" + $.SortableDate($("#Sunday").val());
    });
});