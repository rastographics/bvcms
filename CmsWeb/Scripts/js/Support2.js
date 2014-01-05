$(function () {
    $(".bt").button();
    var $searchresults;
    $("#contactsupport").prop("disabled", true)
        .css("opacity", 0.2);
    $("#reqBody").val("");
    $("#success").hide();
    $("#csearch").click(function () {
        var input = document.getElementById('cse-search-input-box-id');
        $("#success").hide();
        $searchresults = google.search.cse.element.getElement('searchresults-only0');
        if (input.value == '') {
            $searchresults.clearAllResults();
        } else {
            $("#contactsupport").prop("disabled", false).css("opacity", 1);
            $("#about").hide();
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
        e.preventDefault();
        $searchresults.clearAllResults();
        $("#supportForm").show();
        $("#success").hide();
        $("#examples").accordion({
            active: 10,
            collapsible: true,
            heightStyle: "content"
        });
        CKEDITOR.replace('reqBody', {
            height: 200,
            fullPage: false,
            filebrowserUploadUrl: '/Account/CKEditorUpload/',
            filebrowserImageUploadUrl: '/Account/CKEditorUpload/'
        });
    });
    $("#sendSupport").live("click", function (e) {
        if ($("#supportUrgency").val() == 0) {
            alert("Please select an urgency before submitting your support request");
            return;
        }

        $("#supportLastSearch").val($("#cse-search-input-box-id").val());

        var thePost = $(this).closest("form").serialize();
        $.post("/Support/SendSupportRequest", thePost, function (data) {
            if (data == "OK") {
                $("#success").show();
                $("#supportForm").hide();
            }
            else {
                alert("There was an error submitting your support request, please try again.");
            }
        });
    });
    $("[supportHelp]").live("click", function (e) {
        var contentID = $(this).attr("contentID");
        $("#supportHelp").html($("#" + contentID).html());
        $("#supportHelp").dialog("open");
    });
    $("[supportCancelHelp]").live("click", function (e) {
        $("#supportHelp").dialog("close");
    });

    $("#supportHelp").dialog({ autoOpen: false, resizable: false, width: 500, height: "auto", modal: true, dialogClass: "no-title" });

    $("#supportInstructions").show();
    $("#supportForm").hide();

    var cx = '012864427410831580707:fj9oufx9opi';
    var gcse = document.createElement('script'); gcse.type = 'text/javascript'; gcse.async = true;
    gcse.src = (document.location.protocol == 'https:' ? 'https:' : 'http:') +
        '//www.google.com/cse/cse.js?cx=' + cx;
    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(gcse, s);
});