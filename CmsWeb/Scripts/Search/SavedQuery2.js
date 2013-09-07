$(function () {
    $("a.edit-saved-query").live("click", function(ev) {
        ev.preventDefault();
        var a = $(this);
        var modal = $("#edit-saved-query-dialog");
        var rowid = "#" + a.closest("tr").attr("id");
        modal.css({ 'margin-top': '', 'top': '' })
            .load(a.attr("href"), {}, function() {
                modal.modal("show");
                $("#SaveQueryInfo").click(function(ev) {
                    ev.preventDefault();
                    var q = modal.serialize();
                    $.post("/SavedQuery2/Update", q, function(ret) {
                        $(rowid).replaceWith(ret);
                        modal.modal("hide");
                        modal.empty();
                    });
                });
            });
    });
    /* This is a good example of how to use the single checkbox edit with a checklist
    $('.public-editable').editable({
        type: 'checklist',
        name: 'public',
        url: function (params) {
            var d = new $.Deferred;
            $.post("/SavedQuery2/PostPublic", { pk: params.pk, value: params.value[0] }, function(ret) {
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
