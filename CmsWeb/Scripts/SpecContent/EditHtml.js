$(function () {

    $("a.delete").click(function (ev) {
        ev.preventDefault();
        var href = this.href;

        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete it!",
            closeOnConfirm: false
        },
        function () {
            $.post(href, null, function (ret) {
                if (ret && ret.error)
                    swal("Error!", ret.error, "error");
                else {
                    swal({
                        title: "Deleted!",
                        type: "success"
                    },
                    function () {
                        window.location = "/Manage/Display/#tab_htmlContent";
                    });
                }
            });
        });
    });

    CKEDITOR.env.isCompatible = true;

    CKEDITOR.replace('body', {
        height: 400,
        allowedContent: true,
        autoParagraph: false,
        fullPage: !$("#snippet").prop("checked"),
        customConfig: "/scripts/js/ckeditorconfig.js"
    });

    $("#snippet").change(function () {
        var checked = this.checked;
        swal({
            title: "Reload page?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-warning",
            confirmButtonText: "Yes!",
            closeOnConfirm: false
        },
        function () {
            if (checked)
                window.location.href = window.location.pathname + '?snippet=true';
            else
                window.location.href = window.location.pathname;
        });

    });
});
