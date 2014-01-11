$(function () {
    $("a.delete").click(function (ev) {
        ev.preventDefault();
        var d = $(this);
        var url = this.href;
        bootbox.confirm("are you sure you want to delete?", function (ret) {
            if (ret === true)
                $.post(url, null, function () {
                    d.closest("tr").remove();
                });
        });
    });
    $("a.rename").click(function (ev) {
        ev.preventDefault();
        var url = this.href;
        bootbox.prompt("New name:", "Cancel", "Rename", function (result) {
            if (result !== null) {
                $.post(url, { newname: result }, function () {
                    window.location.reload();
                });
            }
        }, $(this).data("default"));
    });
});
