$(function () {


    CKEDITOR.env.isCompatible = true;

    CKEDITOR.replace('body', {
        height: 400,
        allowedContent: true,
        autoParagraph: false,
        fullPage: !$("#snippet").prop("checked"),
        customConfig: "/Content/touchpoint/js/ckeditorconfig.js"
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
