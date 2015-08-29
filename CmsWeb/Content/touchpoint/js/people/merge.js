$(function () {
    $("a.submit").click(function (ev) {
        ev.preventDefault();
        var f = $(this).closest("form");
        f.attr("action", this.href);
        bootbox.confirm("Are you sure you want do this? There is no undo button.", function (result) {
            if (result) {
                $.block();
                f.submit();
            }
        });
    });
    $("a.toggledup").click(function (ev) {
        ev.preventDefault();
        var f = $(this).closest("form");
        f.attr("action", this.href);
        $.block();
        f.submit();
    });
    $("#usefrom").change(function (ev) {
        ev.preventDefault();
        $("input:radio[value=0]").attr("checked", "checked");
    });
    $("#usetarget").change(function (ev) {
        ev.preventDefault();
        $("input:radio[value=1]").attr("checked", "checked");
    });
});
