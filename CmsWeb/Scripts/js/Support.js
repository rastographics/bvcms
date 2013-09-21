var mustSearch = 1;

$(function () {
    $(document).on("click", "#supportfeedback,#emailsupport", null, supportOpen);
    $(document).on("keyup", "#gsc-i-id1", null, searchRequested);
    $(document).on("click", ".gsc-search-button", null, searchDone);
    $(document).on("click", "#showSupportForm", null, supportShowForm);
    $(document).on("click", "#sendSupport", null, supportSend);
    $(document).on("click", "#cancelSupport", null, supportClose);
    $(document).on("click", "[supportHelp]", null, supportHelpOpen);
    $(document).on("click", "[supportCancelHelp]", null, supportHelpClose);

    $("#supportDialog").dialog({ autoOpen: false, resizable: false, width: 800, height: 500, modal: true, dialogClass: "no-title" });
    $("#supportHelp").dialog({ autoOpen: false, resizable: false, width: 500, height: "auto", modal: true, dialogClass: "no-title" });
});

function searchRequested(e) {
    if (e.keyCode == 13)
        searchDone(e);
}

function searchDone(e) {
    mustSearch--;
    if (mustSearch <= 0) {
        $("#showSupportForm").button( "option", "disabled", false );
    }
}

function supportShowForm(e) {
    $("#reqBody").val("");

    $("#supportInstructions").css("display", "none");
    $("#supportForm").css("display", "block");
}

function supportOpen(e) {
    mustSearch = 1;

    $("#showSupportForm").button("option", "disabled", true);
    $("#supportInstructions").css("display", "block");
    $("#supportForm").css("display", "none");

    $("#supportDialog").dialog("open");
}

function supportClose(e) {
    $("#supportDialog").dialog("close");
}

function supportSend(e) {
    if( $("#supportUrgency")[0].selectedIndex == 0 )
    {
        alert( "Please select an urgency before submitting your support request" );
        return;
    }

    $("#supportLastSearch").val($("#gsc-i-id1").val());

    var theURL = "/Support/SendSupportRequest";
    var thePost = $(this).closest("form").serialize();
    $.ajax(theURL, { data: thePost, type: "POST", success: supportSentComplete });
}

function supportSentComplete(data, status, xhr) {
    if (data == "OK") {
        alert("Your request has been submitted!");
        $("#supportDialog").dialog("close");
    }
    else {
        alert("There was an error submitting your support request, please try again.");
    }
}

function supportHelpOpen(e) {
    var contentID = $(this).attr("contentID");
    $("#supportHelp").html($("#" + contentID).html());
    $("#supportHelp").dialog("open");
}

function supportHelpClose(e) {
    $("#supportHelp").dialog("close");
}