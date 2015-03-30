$(function () {
    $("a.regopen").click(function (ev) {
        ev.preventDefault();
        var rid = $(this).attr("rid");
        $("#d." + rid).toggle();
    });
});
