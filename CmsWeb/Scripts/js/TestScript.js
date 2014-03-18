$(function () {
    $("#script").tabby();
    $(".bt").button();
    $("#run").click(function (ev) {
        ev.preventDefault();
        $.post(this.href, { script: $("#script").val() }, function (ret) {
            $("#results").html(ret);
        });
    });
});
