$(function () {
    $("#membergroups .ckbox").live("click", function (ev) {
        $.post($(this).attr("href"), {
            ck: $(this).is(":checked")
        });
        return true;
    });
    $("#membergroups .update-smallgroup").live("click", function (ev) {
        var href = $(this).attr("href");
        var checked = $(this).is(":checked");
        var msg = checked
            ? "This will add everybody to this small group. Are you sure?"
            : "This will remove everybody from this small group. Are you sure?";
        bootbox.confirm(msg, function (confirmed) {
            if (confirmed)
                $.post(href, { ck: checked });
        });
        return true;
    });
    $("#dropmember").live("click", function (ev) {
        var f = $(this).closest('form');
        var href = this.href;
        bootbox.confirm("are you sure?", function (confirmed) {
            if (confirmed) {
                $.post(href, null, function(ret) {
                    f.modal("hide");
                    RebindMemberGrids();
                });
            }
        });
        return false;
    });
    $('#OrgSearch').live("keydown", function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            $("#orgsearchbtn").click();
        }
    });
    $("a.movemember").live('click', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        var href = $(this).attr("href");
        bootbox.confirm("are you sure?", function (confirmed) {
            if (confirmed) {
                $.post(href, null, function(ret) {
                    f.modal("hide");
                    $.RebindMemberGrids();
                });
            }
        });
        return false;
    });
});

