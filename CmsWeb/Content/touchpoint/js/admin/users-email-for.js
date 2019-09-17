$(function () {
    $("a.editcanemail").SearchUsers({
        UpdateShared: function (parent, delegate) {
            if (parent !== undefined) {
                $.post("/UpdatePersonCanEmailForList/" + parent, { topid0: delegate }, function (ret) {
                    window.location.reload();
                });
            }
        },
        Select: function (parent, delegate) {
            if (delegate === undefined) {
                var $a = $("#canemail-" + parent);
                if ($a && $a.length) {
                    $a.click();
                } else {
                    window.setTimeout(function () {
                        $("#new-canemail").attr('href', "/PersonCanEmailForList/" + parent).click();
                    }, 300);
                }
            }
        }
    });
});
