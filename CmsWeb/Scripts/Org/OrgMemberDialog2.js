$(function () {
    $("#membergroups .ckbox").live("click", function (ev) {
        $.post($(this).attr("href"), {
            ck: $(this).is(":checked")
        });
        return true;
    });
    $("#dropmember").live("click", function (ev) {
        var f = $(this).closest('form');
        if (confirm("are you sure?"))
            $.post($(this).attr("href"), null, function (ret) {
                f.modal("hide");
                self.parent.RebindMemberGrids();
            });
        return false;
    });
    $('#orgsearch').live("keydown", function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            $("#orgsearchbtn").click();
        }
    });
    $("a.movemember").live('click', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        if (confirm("are you sure?"))
            $.post($(this).attr('href'), null, function (ret) {
                f.modal("hide");
                self.parent.RebindMemberGrids();
            });
        return false;
    });
});

