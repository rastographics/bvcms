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
    $("#Field").change(function(ev) {
        ev.preventDefault();
        var f = $(this).closest("form");
        var q = f.serialize();
        $.post("/Batch/UpdateWarning/", q, function (ret) {
            $("#warning").text(ret);
            if(ret)
                $("#warning").show();
            else 
                $("#warning").hide();
        });
        return false;
    });
    $("#uform").submit(function (ev) {
        ev.preventDefault();
        var f = this;
        bootbox.confirm("Are you sure you want do this? There is no undo button.", function (result) {
            if (result)
                f.submit();
        });
    });
});

