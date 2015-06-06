$(function() {
    setInterval(function() {
        var q = $("#completeReg").serialize();
        $.post("/OnlineReg/AutoSaveProgress", q);
    }, 30000);
});