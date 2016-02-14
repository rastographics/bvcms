$(function () {
    $('#support-tabs').tabdrop();
    $('#cse-search-input-box-id').focus();

    var cx = '012864427410831580707:fj9oufx9opi';
    var gcse = document.createElement('script'); gcse.type = 'text/javascript'; gcse.async = true;
    gcse.src = (document.location.protocol == 'https:' ? 'https:' : 'http:') + '//www.google.com/cse/cse.js?cx=' + cx;
    var s = document.getElementsByTagName('script')[0];
    s.parentNode.insertBefore(gcse, s);

    $("#csearch").click(function () {
        var input = document.getElementById('cse-search-input-box-id');
        var $searchresults = google.search.cse.element.getElement('searchresults-only0');
        if (input.value == '') {
            $searchresults.clearAllResults();
        } else {
            $('#last-search').val(input.value);
            $("#contactsupport").prop("disabled", false).css("opacity", 1);
            $searchresults.execute(input.value);
        }
        return false;
    });

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
            urgency: $("#urgency").val(),
            lastsearch: $("#last-search").val()
        };
        $.post("/Support/SendRequest", postdata, function (data) {
            if (data == "OK") {
                $("#success").show();
                $("#supportForm").hide();
            }
            else {
                swal("Error!", "There was an error submitting your support request, please try again.", "error");
            }
        });
    });
});
