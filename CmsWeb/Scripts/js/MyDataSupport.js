$(function () {
    $(".bt").button();
    $("#body").val("");
    $("#success").hide();
    $.fn.modal.Constructor.prototype.enforceFocus = function () {
      var modalThis = this;
      $(document).on('focusin.modal', function (e) {
        // Fix for CKEditor + Bootstrap IE issue with dropdowns on the toolbar
        // Adding additional condition '$(e.target.parentNode).hasClass('cke_contents cke_reset')' to
        // avoid setting focus back on the modal window.
        if (modalThis.$element[0] !== e.target && !modalThis.$element.has(e.target).length
            && $(e.target.parentNode).hasClass('cke_contents cke_reset')) {
          modalThis.$element.focus();
        }
      });
    };
    CKEDITOR.replace('body', {
        height: 200,
        fullPage: false,
        allowedContent: true,
        customConfig: '/scripts/js/ckeditorconfig.js'
    });
    $("body").on("click", "#sendSupport", function (e) {
        $.post("/Support/MyDataSendRequest", {
            body: CKEDITOR.instances["body"].getData()
        }, function (data) {
            if (data == "OK") {
                $("#success").show();
                $("#supportForm").hide();
            }
            else {
                alert("There was an error submitting your support request, please try again.");
            }
        });
    });
});