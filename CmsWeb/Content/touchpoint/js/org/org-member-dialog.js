$(function () {
    $('body').on('click', '#membergroups .ckbox', function (ev) {
        var f = $(this).closest("form");
        var q = f.serialize() + '&' + $.param({
             'ck': $(this).is(":checked")
        });
        $.post($(this).attr("href"), q);
        return true;
    });

    $('body').on('click', '#org-member-drop', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        var q = f.serialize();
        var href = $(this).attr("href");

        $('#empty-dialog').modal('hide');
        $.block();
        $.post(href, q, function (ret) {
            $.unblock();
            if (ret === "Done") {
                swal({
                    title: "Member Dropped!",
                    type: "success"
                });
            } else {
                swal("Error!", 'An error occurred attempting to drop the member.', "error");
            }
            $.RebindMemberGrids();
        });

        return false;
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
    $('body').on('click', '#addQuestions', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        var q = f.serialize();

        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-warning",
            confirmButtonText: "Yes, Add Questions",
            closeOnConfirm: true
        },
        function () {
            $.post("/OrgMemberDialog/AddQuestions", q, function (ret) {
                $('#empty-dialog').modal('hide');
            });
        });
        return false;
    });
});

