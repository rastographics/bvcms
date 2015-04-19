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

        swal({
            title: "Are you sure?",
            text: "This will add or remove everybody to/from this sub-group.",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-warning",
            confirmButtonText: "Yes, update!",
            closeOnConfirm: true
        },
        function () {
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

        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-warning",
            confirmButtonText: "Yes, move member!",
            closeOnConfirm: true
        },
        function () {
            $.post(href, q, function (ret) {
                $('#empty-dialog').modal('hide');
                $.RebindMemberGrids();
            });
        });
        return false;
    });
});

