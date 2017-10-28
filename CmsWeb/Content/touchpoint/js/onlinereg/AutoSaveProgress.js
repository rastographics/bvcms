$(function () {
    $.AutoSaveEnabled = true;
    function AutoSaveProgress() {
        if ($("#SavedProgress").length > 0)
            $.AutoSaveEnabled = false;
        if ($.AutoSaveEnabled) {
            if ($("#completeReg").valid()) {
                var q = $("#completeReg").serialize();
                $.post("/OnlineReg/AutoSaveProgress", q, function(ret) {
                    $("#DatumId").val(ret);
                });
            }
            setTimeout(AutoSaveProgress, 30000);
        }
    }
    setTimeout(AutoSaveProgress, 30000);
});
