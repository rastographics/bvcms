$(function () {
    $('#support-tabs').tabdrop();

    $('#supportRequestTab').on('shown.bs.tab', function (e) {
        e.preventDefault();
        $("#body").val("");
        $("#success").hide();

        if (CKEDITOR.instances["body"]) {
            CKEDITOR.instances["body"].destroy();
        }

        CKEDITOR.env.isCompatible = true;

        CKEDITOR.replace('body', {
            height: 200,
            fullPage: false,
            allowedContent: true,
            customConfig: '/Content/touchpoint/js/ckeditorconfig.js'
        });
    });

    $('body').on('click', '#sendSupport', function (e) {
        e.preventDefault();
        if ($("#urgency").val() == 0) {
            swal("Error!", "Please select an priority before submitting your support request.", "error");
            return;
        }

        var postdata = {
            body: CKEDITOR.instances["body"].getData(),
            cc: $("#cc").val(),
            subj: $("#subj").val(),
            urgency: $("#urgency").val(),
            lastsearch: $("#last-search").val()
        };
        $.post("/Support/SendRequest", postdata, function (data) {
            if (data == "OK") {
                $("#success").show();
                $("#supportForm").hide();
                $
            }
            else {
                swal("Error!", "There was an error submitting your support request, please try again.", "error");
            }
        });
    });
});
