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
        height: 400,
        allowedContent: true,
        autoParagraph: false,
        fullPage: !$("#snippet").prop("checked"),
        customConfig: "/scripts/js/ckeditorconfig.js"
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
