$(function () {
    $.AutoSaveEnabled = true;
    function AutoSaveProgress() {
        if ($("#SavedProgress").length > 0)
            $.AutoSaveEnabled = false;
        if ($.AutoSaveEnabled) {
            var q = $("#completeReg").serialize();
            $.post("/OnlineReg/AutoSaveProgress", q);
            setTimeout(AutoSaveProgress, 30000);
        }
    }
    setTimeout(AutoSaveProgress, 30000);
});