$(function () {
    $(".bt").button();
    var cx = '012864427410831580707:fj9oufx9opi';
    var gcse = document.createElement('script'); gcse.type = 'text/javascript'; gcse.async = true;
    gcse.src = (document.location.protocol == 'https:' ? 'https:' : 'http:') + '//www.google.com/cse/cse.js?cx=' + cx;
    var s = document.getElementsByTagName('script')[0];
    s.parentNode.insertBefore(gcse, s);
    $("#body").val("");
    $("#success").hide();
    $("#csearch").click(function () {
        var input = document.getElementById('cse-search-input-box-id');
        var $searchresults = google.search.cse.element.getElement('searchresults-only0');
        $("#success").hide();
        $("#about").hide();
        if (input.value == '') {
            $searchresults.clearAllResults();
        } else {
            $("#contactsupport").prop("disabled", false).css("opacity", 1);
            $searchresults.execute(input.value);
        }
        return false;
    });
    $("#cse-search-input-box-id").keyup(function (e) {
        if (e.keyCode == 13) {
            $("#success").hide();
            $("#contactsupport").prop("disabled", false).css("opacity", 1);
            $("#about").hide();
        }
    });
    $("#contactsupport").click(function (e) {
        var $searchresults = google.search.cse.element.getElement('searchresults-only0');
        e.preventDefault();
        $searchresults.clearAllResults();
        $("#supportForm").show();
        $("#success").hide();
        $("#about").hide();
        $("#examples").accordion({
            active: 10,
            collapsible: true,
            heightStyle: "content"
        });
        CKEDITOR.replace('body', {
            height: 200,
            fullPage: false,
            allowedContent: true,
            customConfig: '/scripts/js/ckeditorconfig.js'
        });
    });
    $("#sendSupport").live("click", function (e) {
        e.preventDefault();
        if ($("#urgency").val() == 0) {
            alert("Please select an urgency before submitting your support request");
            return;
        }

        var postdata = {
            body: CKEDITOR.instances["body"].getData(),
            cc: $("#cc").val(),
            urgency: $("#urgency").val(),
            lastsearch: $("#cse-search-input-box-id").val()
        };
        $.post("/Support/SendRequest", postdata, function (data) {
            if (data == "OK") {
                $("#success").show();
                $("#supportForm").hide();
            }
            else {
                alert("There was an error submitting your support request, please try again.");
            }
        });
    });
    $("#supportForm").hide();
});