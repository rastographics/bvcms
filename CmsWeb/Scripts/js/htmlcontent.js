$(function () {
    $(".bt").button();
    $("#delete").click(function (ev) {
        ev.preventDefault();
        var href = this.href;
        if (confirm("Are you sure you want to delete this entry?")) {
            $.post(href, function () {
                window.location = "/Manage/Display/";
            });
        }
    });

    CKEDITOR.replace('body', {
        height: 400,
        allowedContent: true,
        autoParagraph: false,
        fullPage: !$("#snippet").prop("checked"),
        customConfig: "/Content/touchpoint/js/ckeditorconfig.js"
    });
    $("#snippet").change(function () {
        if (confirm("reload page?")) {
            if (this.checked)
                window.location.href = window.location.pathname + '?snippet=true';
            else
                window.location.href = window.location.pathname;
        }
    });
});
function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}
