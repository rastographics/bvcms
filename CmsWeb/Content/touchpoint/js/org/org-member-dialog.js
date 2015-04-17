$(function () {
    $('body').on('click', '#membergroups .ckbox', function (ev) {
        var f = $(this).closest("form");
        var q = f.serialize() + '&' + $.param({
             'ck': $(this).is(":checked")
        });
        $.post($(this).attr("href"), q);
        return true;
    });

    $('body').on('click', '#membergroups .update-smallgroup', function (ev) {
        ev.preventDefault();
        var href = $(this).attr("href");
        var msg = "This will add or remove everybody to/from this sub-group. Are you sure?";

        bootbox.confirm(msg, function (confirmed) {
            if (confirmed)
                $.post(href);
        });
        return false;
    });

    $('body').on('keydown', '#OrgSearch', function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            $("#orgsearchbtn").click();
        }
    });

    $('body').on('click', 'a.movemember', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        var q = f.serialize();
        var href = $(this).attr("href");
        bootbox.confirm("are you sure?", function (confirmed) {
            if (confirmed) {
                $.post(href, q, function (ret) {
                    f.modal("hide");
                    RebindMemberGrids();
                });
            }
        });
        return false;
    });
});

