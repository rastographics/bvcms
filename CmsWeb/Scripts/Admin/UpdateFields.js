$(function() {
    $("#Tag").change(function(ev) {
        ev.preventDefault();
        var f = $(this).closest("form");
        var q = f.serialize();
        $.post("/Batch/UpdateFieldsCount/", q, function(ret) {
            $("#count").text(ret + " records will be updated");
            $("#count").closest("div.control-group").removeClass("hide");
            $("#Count").val(ret);
        });
        return false;
    });
});

