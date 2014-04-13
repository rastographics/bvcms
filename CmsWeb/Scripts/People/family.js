$(function () {
    $(".clickSelect").editable("/Family/EditPosition/", {
        indicator: '<img src="/Content/images/loading.gif">',
        loadurl: "/Family/PositionCodes/",
        loadtype: "POST",
        type: "select",
        submit: "OK",
        style: 'display: inline'
    });
    $(".clickEdit").editable("/Family/EditRelation/", {
        indicator: "<img src='/images/loading.gif'>",
        tooltip: "Click to edit...",
        style: 'display: inline',
        width: '400px',
        height: 25
    });

    $(".bt").button();
    $('#memberDialog').dialog({
        title: 'Add Members Dialog',
        bgiframe: true,
        autoOpen: false,
        width: 750,
        height: 700,
        modal: true,
        overlay: {
            opacity: 0.5,
            background: "black"
        }, close: function () {
            $('iframe', this).attr("src", "");
        }
    });
    $('#AddMembers2,#AddRelatedFamily2').live("click", function (e) {
        e.preventDefault();
        var d = $('#memberDialog');
        $('iframe', d).attr("src", this.href);
        d.dialog("option", "title", this.title);
        d.dialog("open");
    });
    $("a.delete").click(function (ev) {
        ev.preventDefault();
        if (confirm("are you sure?"))
            $.post("/Family/DeleteRelation/" + $(this).attr("id"), null, function (ret) {
                window.location.reload(true);
            });
        return false;
    });
    $("a.split").click(function (ev) {
        ev.preventDefault();
        if (confirm("are you sure?"))
            $.post($(this).attr("href"), null, function (ret) {
                window.location = ret;
            });
        return false;
    });
    $.editable.addInputType('datepicker', {
        element: function (settings, original) {
            var input = $('<input>');
            if (settings.width != 'none') { input.width(settings.width); }
            if (settings.height != 'none') { input.height(settings.height); }
            input.attr('autocomplete', 'off');
            $(this).append(input);
            return (input);
        },
        plugin: function (settings, original) {
            var form = this;
            settings.onblur = 'ignore';
            $(this).find('input').jqdatepicker().bind('click', function () {
                $(this).jqdatepicker('show');
                return false;
            }).bind('dateSelected', function (e, selectedDate, $td) {
                $(form).submit();
            });
        }
    }); 
    $.editable.addInputType("checkbox", {
        element: function (settings, original) {
            var input = $('<input type="checkbox">');
            $(this).append(input);
            $(input).click(function () {
                var value = $(input).attr("checked") ? 'True' : 'False';
                $(input).val(value);
            });
            return (input);
        },
        content: function (string, settings, original) {
            var checked = string == "True" ? true : false;
            var input = $(':input:first', this);
            $(input).attr("checked", checked);
            var value = $(input).attr("checked") ? 'True' : 'False';
            $(input).val(value);
        }
    });


    $.editable.addInputType("multiselect", {
        element: function (settings, original) {
            var select = $('<select multiple="multiple" />');

            if (settings.width != 'none') { select.width(settings.width); }
            if (settings.size) { select.attr('size', settings.size); }

            $(this).append(select);
            return (select);
        },
        content: function (json, settings, original) {
            for (var key in json) {
                var option = $('<option />').val(key).text(key);
                if (json[key] == true)
                    option.attr("selected", true);
                $('select', this).append(option);
            }
            $("select", this).multiselect({
                close: function (event, ui) {
                    var values = $("select").val();
                },
                position: {
                    my: 'left bottom',
                    at: 'left top'
                }
            });
        }
    });
    $.extraEditable = function (table) {
        $('.editarea', table).editable('/Family/EditExtra/', {
            type: 'textarea',
            submit: 'OK',
            rows: 10,
            width: 600,
            indicator: '<img src="/Content/images/loading.gif">',
            tooltip: 'Click to edit...'
        });
        $(".clickEdit", table).editable("/Family/EditExtra/", {
            indicator: "<img src='/images/loading.gif'>",
            tooltip: "Click to edit...",
            style: 'display: inline',
            width: '300px',
            height: 25,
            submit: 'OK'
        });
        $(".clickDatepicker").editable('/Family/EditExtra/', {
            type: 'datepicker',
            tooltip: 'Click to edit...',
            style: 'display: inline',
            width: '300px',
            submit: 'OK'
        });
        $(".clickSelect", table).editable("/Family/EditExtra/", {
            indicator: '<img src="/Content/images/loading.gif">',
            loadurl: "/Family/ExtraValues/",
            loadtype: "POST",
            type: "select",
            submit: "OK",
            style: 'display: inline'
        });
        $(".clickCheckbox", table).editable('/Family/EditExtra', {
            type: 'checkbox',
            onblur: 'ignore',
            submit: 'OK'
        });
        $('.clickMultiselect', table).editable('/Family/EditExtra', {
            indicator: '<img src="/Content/images/loading.gif">',
            loadurl: "/Family/ExtraValues2/",
            loadtype: "POST",
            type: "multiselect",
            submit: "OK",
            onblur: 'ignore',
            style: 'display: inline'
        });
    };
    $.extraEditable('#extravalues');
    $("#newvalueform").dialog({
        autoOpen: false,
        buttons: {
            "Ok": function () {
                var v = $("input[name='typeval']:checked").val();
                var fn = $("#fieldname").val();
                var va = $("#fieldvalue").val();
                if (fn)
                    $.post("/Family/NewExtraValue/" + $("#FamilyId").val(), { field: fn, type: v, value: va }, function (ret) {
                        if (ret.startsWith("error"))
                            alert(ret);
                        else {
                            window.location = "/Family/" + $("#FamilyId").val();
                        }
                        $("#fieldname").val("");
                        $("#fieldvalue").val("");
                    });
                $(this).dialog("close");
            }
        }
    });
    $("body").on("click", '#newextravalue', function (ev) {
        ev.preventDefault();
        var d = $('#newvalueform');
        d.dialog("open");
    });
    $("body").on("click", 'a.deleteextra', function (ev) {
        ev.preventDefault();
        if (confirm("are you sure?"))
            $.post("/Family/DeleteExtra/" + $("#FamilyId").val(), { field: $(this).attr("field") }, function (ret) {
                if (ret.startsWith("error"))
                    alert(ret);
                else {
                    window.location = "/Family/" + $("#FamilyId").val();
                }
            });
        return false;
    });
});
function AddSelected() {
    $('#memberDialog').dialog("close");
    window.location.reload(true);
}
