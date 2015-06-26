$(document).ready(function () {
    $('#AddDialog').dialog({
        bgiframe: true,
        autoOpen: false,
        width: 750,
        height: 700,
        modal: true,
        overlay: {
            opacity: 0.5,
            background: "black"
        }, close: function () {
            $('iframe', this).attr("src", "");
        }
    });
    $('#addorg').click(function (e) {
        e.preventDefault();
        var d = $('#AddDialog');
        $('iframe', d).attr("src", "/AddOrganization");
        d.dialog("option", "title", "Add Organization");
        d.dialog("open");
    });
    $('#cleartag').click(function (e) {
        e.preventDefault();
        if (confirm("are you sure you want to empty the active tag?"))
            $.post("/Tags/ClearTag", {}, function () {
                window.location.reload();
            });
    });
    $('.warntip').tooltip({
        delay: 150,
        showBody: "|",
        showURL: false
    });
});
function CloseAddOrgDialog(id) {
    $("#AddDialog").dialog("close");
    window.location = "/Org/" + id;
}