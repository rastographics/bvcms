$(function () {
    CKEDITOR.replace('message', {
        height: 400,
        fullPage: false,
        allowedContent: true,
        customConfig: '/Content/touchpoint/js/ckeditorconfig.js'
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
    $("body").on("click", "#selectallsame", function () {
        $("input[name='pids']:visible", "#allsame").prop('checked', $(this).prop("checked"));
    });
    $("body").on("click", "#selectallothers", function () {
        $("input[name='pids']:visible", "#allothers").prop('checked', $(this).prop("checked"));
    });
    $("#submitit").click(function() {
        this.value = 'Sending, please wait...';
        $("#submitit").hide();
        $("#sending").show();
        $('#sendthem').submit();
    });
});