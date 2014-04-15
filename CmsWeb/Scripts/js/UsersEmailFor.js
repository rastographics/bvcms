$(function () {
    $(".bt").button();
    $("a.editcanemail").SearchUsers({
        UpdateShared: function (topid, topid0) {
            $.post("/UpdatePersonCanEmailForList/" + topid, { topid0: topid0 }, function (ret) {
                window.location.reload();
            });
        }
    });
});
