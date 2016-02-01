$(function () {
    $('body').on('click', 'a.edit-saved-query', function (ev) {
        ev.preventDefault();
        var a = $(this);
        var rowid = "#" + a.closest("tr").attr("id");
        $("<div />").load(a.attr("href"), {}, function () {
            var div = $(this);
            var dialog = div.find("#edit-query-modal");
            $('#empty-dialog').html(dialog);
            $('#empty-dialog').modal("show");

            $("#SaveQueryInfo").click(function (ev) {
                ev.preventDefault();
                var f = $(this).closest("form");
                var q = f.serialize();
                $.post("/SavedQuery/Update", q, function (ret) {
                    $(rowid).replaceWith(ret);
                    $('#empty-dialog').modal("hide");
                    div.remove();
                    dialog.remove();
                });
            });
        });
    });

    $('body').on('click', '#code-link', function (ev) {
        var href = this.href;
        var f = $(this).closest("form");
        var q = f.serialize();
        $.ajax({
            type: "POST",
            async: false,
            url: href,
            data: q
        });
    });

    $('body').on('click', 'a.delete-saved-query', function (ev) {
        ev.preventDefault();
        var a = $(this);
        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete it!",
            closeOnConfirm: true
        },
        function () {
            $.post(a.attr("href"), {}, function (ret) {
                a.closest("tr").fadeOut().remove();
            });
        });
        return false;
    });

    $('body').on('keydown', '#SearchQuery', function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            $("#filter-link").click();
            return false;
        }
        return true;
    });

    $("#SearchQuery").focus();

});
