$(function () {
    $("body").on("click", 'a.deltran', function (ev) {
        ev.preventDefault();
        if (confirm("are you sure"))
            $.post($(this).attr("href"), {}, function (ret) {
                $("#history").replaceWith(ret);
            });
        return false;
    });
    $("body").on("click", '#deleteall', function (ev) {
        ev.preventDefault();
        if (confirm("are you sure you want to delete all transactions (will not affect attendance)?"))
            $.post($(this).attr("href"), {}, function () {
                $("#history").replaceWith("");
                alert("You will need to refresh your organization page to see the changes there");
            });
        return false;
    });
    $("span.date").editable('/TransactionHistory/Edit/', {
        tooltip: 'click to edit...',
        event: 'click',
        submit: 'OK',
        cancel: 'Cancel',
        width: '100px',
        height: 25
    });
});
