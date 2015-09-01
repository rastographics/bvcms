$(function () {
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
    CKEDITOR.replace('message', {
        height: 400,
        fullPage: false,
        allowedContent: true,
        customConfig: '/scripts/js/ckeditorconfig.js'
    });
    $("#smallgroups").change(function () {
        $("div.wrapper > div").show();
        var v = $(this).val();
        if (v === "0")
            return;
        $("div.wrapper > div").not(v).hide();
    });
    if (jQuery().transpose) {
        $(".wrapper .item").transpose();
    }
    $("#selectallsame").click(function () {
        if ($(this).attr("checked"))
            $("input[name='pids']:visible", "#allsame").attr('checked', true);
        else
            $("input[name='pids']:visible", "#allsame").removeAttr('checked');
    });
    $("#selectallothers").click(function() {
        if ($(this).attr("checked"))
            $("input[name='pids']:visible", "#allothers").attr('checked', true);
        else
            $("input[name='pids']:visible", "#allothers").removeAttr('checked');
    });
    $("#submitit").click(function() {
        this.value = 'Sending, please wait...';
        $("#submitit").hide();
        $("#sending").show();
        $('#sendthem').submit();
    });
});