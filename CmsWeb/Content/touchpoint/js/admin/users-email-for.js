$(function () {
    $("a.editcanemail").SearchUsers({
        UpdateShared: function (topid, topid0) {
            $.post("/UpdatePersonCanEmailForList/" + topid, { topid0: topid0 }, function (ret) {
                window.location.reload();
            });
        },
        Select: function (id) {
            var $a = $("#canemail-" + id);
            if ($a && $a.length)
                $a.click();
            else {
                $("#new-canemail")[0].href = "/PersonCanEmailForList/" + id;
                $("#new-canemail").click();
            }
        }
    });
});
