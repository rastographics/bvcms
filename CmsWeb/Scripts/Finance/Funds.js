$(function () {
    $('table.grid2').bind('mousedown', function (e) {
        if ($(e.target).hasClass("clickEdit")) {
            $(e.target).editable("/Fund/EditOrder/", {
                tooltip: "Click to edit ...",
                style: 'display: inline',
                width: '60px',
                submit: "OK",
                placeholder: "edit",
                height: 25
            });
        }
        else if ($(e.target).hasClass("clickSelect")) {
            $(e.target).editable("/Fund/EditStatus/", {
                tooltip: "Click to edit...",
                data: " {'1':'Open','2':'Closed'}",
                loadtype: "POST",
                type: "select",
                submit: "OK",
                style: 'display: inline'
            });
        }
    });
    $("a.sortable").click(function (ev) {
        ev.preventDefault();
        if ($("#status").val() == 2)
            window.location = "/Fund?status=2&sort=" + $(this).text();
        else
            window.location = "/Fund?sort=" + $(this).text();
    });
    $(".bt").button();
    $("form").submit(function (ev) {
        ev.preventDefault();
        $.post("/Fund/Create", { fundid: $("#fundid").val() }, function (ret) {
            if (ret.error)
                $("#errormessage").text(ret.error);
            else 
                window.location = ret.edit;
        });
        return false;
    });
});
