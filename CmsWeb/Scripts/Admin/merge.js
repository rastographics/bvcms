$(function () {
    $("form").submit(function () {
        $.block();
        return true;
    });
    $('table.grid2 > thead td').css('font-weight', 'bold');
    $("#usefrom").change(function (ev) {
        ev.preventDefault();
        $("input:radio[value=0]").attr("checked", "checked")
    });
    $("#usetarget").change(function (ev) {
        ev.preventDefault();
        $("input:radio[value=1]").attr("checked", "checked")
    });
});
