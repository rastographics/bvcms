$(function () {
    $("a.edit-saved-query").live("click", function (ev) {
        ev.preventDefault();
        var a = $(this);
        var modal = $("#edit-saved-query-dialog");
        var rowid = "#" + a.closest("tr").attr("id");
        modal.css({ 'margin-top': '', 'top': '' })
            .load(a.attr("href"), {}, function () {
                modal.modal("show");
                $("#SaveQueryInfo").click(function (ev) {
                    ev.preventDefault();
                    var q = modal.serialize();
                    $.post("/SavedQuery/Update", q, function (ret) {
                        $(rowid).replaceWith(ret);
                        modal.modal("hide");
                        modal.empty();
                    });
                });
            });
    });
    $("a.delete-saved-query").live("click", function (ev) {
        ev.preventDefault();
        var a = $(this);
        if (confirm("Delete this saved search?"))
            $.post(a.attr("href"), {}, function (ret) {
                a.closest("tr").fadeOut().remove();
            });
        return false;
    });

    $("#SearchQuery").live("keydown", function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            $("#filter-link").click();
            return false;
        }
        return true;
    });
    $.ajaxSetup({
        complete: function () {
            $("#loading-indicator").hide();
            if($("#filter-link").is(":focus"))
                $("#SearchQuery").focus().select();
        }
    });
    $("#SearchQuery").focus();

    /* This is a good example of how to use the single checkbox edit with a checklist
    $('.public-editable').editable({
        type: 'checklist',
        name: 'public',
        url: function (params) {
            var d = new $.Deferred;
            $.post("/SavedQuery/PostPublic", { pk: params.pk, value: params.value[0] }, function(ret) {
                d.resolve();
            });
            return d.promise();
        },
        title: 'Public Query',
        source: { '1': 'public' },
        emptytext: 'private'
    });
    */
});
