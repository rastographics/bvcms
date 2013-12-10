$(function () {
    $.editable.addInputType("checkbox", {
        element: function (settings, original) {
            var input = $('<input type="checkbox">');
            $(this).append(input);
            $(input).click(function () {
                var value = $(input).attr("checked") ? 'yes' : 'no';
                $(input).val(value);
            });
            return (input);
        },
        content: function (string, settings, original) {
            var checked = string == "yes" ? 1 : 0;
            var input = $(':input:first', this);
            $(input).attr("checked", checked);
            var value = $(input).attr("checked") ? 'yes' : 'no';
            $(input).val(value);
        }
    });
    $.onready = function () {
        $(".clickEdit").editable("/SavedQuery2/Edit/", {
            indicator: "<img src='/images/loading.gif'>",
            tooltip: "Click to edit...",
            style: 'display: inline',
            width: '200px',
            height: 25,
            submit: "OK"
        });
        $('span.yesno').editable('/SavedQuery2/Edit/', {
            type: 'checkbox',
            onblur: 'ignore',
            submit: 'OK'
        });
    };
    $('table.grid > tbody > tr:even').addClass('alt');
    $.getTable = function (f) {
        var q;
        if (f)
            q = f.serialize();
        $.post(f.attr('action'), q, function (ret) {
            $(f).html(ret).ready(function () {
                $('table.grid > tbody > tr:even', f).addClass('alt');
                //$('.dropdown', f).hoverIntent(dropdownshow, dropdownhide);
                $('.bt').button();
                $(".datepicker").datepicker();
                $.onready();
            });
        });
        return false;
    };
    $("#filter").live("click", function (ev) {
        ev.preventDefault();
        $.getTable($(this).closest("form"));
        $.onready();
    });
    $(".bt").button();

    $("a.delete").live("click", function (ev) {
        ev.preventDefault();
        var a = $(this);
        if (confirm("Delete this saved search?"))
            $.post("/SavedQuery2/Delete", { id: a.attr("id") }, function (ret) {
                a.closest("tr").fadeOut().remove();
            });
        return false;
    });
    $.onready();
});
