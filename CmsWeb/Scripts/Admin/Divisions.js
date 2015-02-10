$(function() {
    $.editableOptions = {
        indicator: "<img src='/Content/images/loading.gif'>",
        tooltip: "Click to edit...",
        style: 'display: inline',
        onblur: 'submit',
        width: '200px',
        height: 25
    };
    $("#create").click(function(ev) {
        ev.preventDefault();
        if ($("#TagProgramId").val() === "0") {
            alert("choose a target program first (top of page)");
            return false;
        }
        var f = $('#progform');
        var q = f.serialize();
        $.post("/Division/Create/", q, function(ret) {
            $('#results > tbody').append(ret);
            $.fmtTable();
        });
        return false;
    });
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
    $('span.clickEdit').bind('keydown', function(event) {
        if (event.keyCode == 9) {
            $(this).find("input").blur();
            var i = $('.clickEdit').index(this);
            $(".clickEdit:eq(" + (i + 2) + ")").click();
            return false;
        }
    });
    $('body').on('click', 'a.taguntag', function(ev) {
        ev.preventDefault();
        var f = $('#progform');
        var q = f.serialize();
        var a = $(this);
        $.post(a.attr('href'), q, function(ret) {
            var tr = a.closest("tr");
            tr.replaceWith(ret);
            $.fmtTable();
        });
        return false;
    });
    $('body').on('click', 'a.mainprog', function(ev) {
        ev.preventDefault();
        var f = $('#progform');
        var q = f.serialize();
        var a = $(this);
        $.post(a.attr('href'), q, function(ret) {
            var tr = a.closest("tr");
            tr.replaceWith(ret);
            $.fmtTable();
        });
        return false;
    });
    $("body").on("click", 'a.delete', function (ev) {
        ev.preventDefault();
        var pk = $(this).data("pk");
        if (confirm("are you sure?"))
            $.post("/Division/Delete/" + pk, function(ret) {
                window.location = "/Divisions/";
            });
        return false;
    });
    $("#refresh").click(function(ev) {
        ev.preventDefault();
        $.getTable();
    });
    $('#TagProgramId').change(function() {
        $.getTable();
    });
    $('#ProgramId').change(function() {
        $.getTable();
    });
    $.getTable = function() {
        var f = $('#progform');
        var q = f.serialize();
        $.block();
        $.post("/Division/Results", q, function(ret) {
            $('#results').replaceWith(ret).ready(function() {
                $.fmtTable();
                $.unblock();
            });
        });
        return false;
    }
    $.fmtTable = function() {
        $("#results td.tip").tooltip({ showBody: "|" });
        $("#results tbody tr:even").attr("style", "background-color:#ddd");
        $("span.clickEdit").editable("/Division/Edit/", $.editableOptions);
        $('span.yesno').editable('/Division/Edit', {
            type: 'checkbox',
            onblur: 'ignore',
            submit: 'OK'
        });
    }
    $.fmtTable();
});
