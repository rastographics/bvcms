$(function () {

    var submitDialog;

    $(".bt").button();

    $(".showSubmitDialog").click(function (ev) {
        ev.preventDefault();
        var id = $(this).attr("cid");
        submitDialog = $("#dialogHolder");
        $.post("/Volunteering/DialogSubmit/" + id, null, function (data) {
            submitDialog.html(data).dialog({ modal: true, width: 'auto', title: 'Submit Check' }).dialog('open');
            $(".bt").button();
        });
    });

    $(".showCreateDialog").click(function (ev) {
        ev.preventDefault();
        var id = $(this).attr("pid");
        var type = $(this).attr("ctype");
        submitDialog = $("#dialogHolder");
        $.post("/Volunteering/DialogType/" + id + "?type=" + type, null, function (data) {
            submitDialog.html(data).dialog({ modal: true, width: 'auto', title: 'Select Check Type' }).dialog('open');
            $(".bt").button();
        });
    });

    $(".showEditDialog").click(function (ev) {
        ev.preventDefault();
        var id = $(this).attr("cid");
        submitDialog = $("#dialogHolder");
        $.post("/Volunteering/DialogEdit/" + id, null, function (data) {
            submitDialog.html(data).dialog({ modal: true, width: 'auto', title: 'Edit Check' }).dialog('open');
            $(".bt").button();
        });
    });

    $(".showDeleteDialog").click(function (ev) {
        ev.preventDefault();
        var id = $(this).attr("cid");
        submitDialog = $("#dialogHolder");
        $.post("/Volunteering/DialogDelete/" + id, null, function (data) {
            submitDialog.html(data).dialog({ modal: true, width: 'auto', title: 'Delete Check' }).dialog('open');
            $(".bt").button();
        });
    });

    $("span.editable").editable("/Volunteering/Edit/", {
        indicator: "<img src='/Content/images/loading.gif'>",
        tooltip: "Click to edit...",
        style: 'display: inline',
        width: '200px',
        height: 25
    });

    $("#closeSubmitDialog").live("click", function (ev) {
        ev.preventDefault();
        $(submitDialog).dialog("close");
    });
});
function confirmDelete() {
    return confirm("Are you sure you want to delete this file?");
}
