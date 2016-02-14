$(function () {
    $("#search input").keypress(function (ev) {
        if (ev.which == 32) {
            $("li.ckline").show();
        }
        if ((ev.which >= 65 && ev.which <= 90) || (ev.which >= 97 && ev.which <= 122)) {
            var c = String.fromCharCode(ev.which).toUpperCase();
            $("#search").hide();
            document.activeElement.blur();
            $("input").blur();
            $("li.ckline").hide();
            $("." + c).show();
            $('html, body').animate({ scrollTop: 0 }, 'fast');
        }
        return false;
    });
    $("#search").hide();
    $('#searchpage').on('tap', function(ev) {
        $("input").focus();
        $("#search").show();
    });
});
