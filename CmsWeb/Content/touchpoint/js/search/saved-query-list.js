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

    $('body').on('click', '#filter-link', function (ev) {
        ev.preventDefault();
        $('#Page').val(1);
        $.getTable();
        return false;
    });

    $('body').on('click', '#resultsTable > thead a.sortable', function (ev) {
        ev.preventDefault();
        var newsort = $(this).text();
        var sort = $("#Sort");
        var dir = $("#Direction");
        if ($(sort).val() == newsort && $(dir).val() == 'asc')
            $(dir).val('desc');
        else
            $(dir).val('asc');
        $(sort).val(newsort);

        $.getTable();
        return false;
    });

    $.gotoPage = function (e, pg) {
        $("#Page").val(pg);
        $.getTable();
        return false;
    };

    $.setPageSize = function (e) {
        $('#Page').val(1);
        $("#PageSize").val($(e).val());
        return $.getTable();
    };

    $.getTable = function () {
        var f = $('#form-saved-query');
        var q = null;
        if (f)
            q = f.serialize();
        $.block();
        $.post("/SavedQuery/Results", q, function (ret) {
            $('#results').html(ret);
            $.unblock();
        });
        return false;
    };

    $("#SearchQuery").focus();

});
