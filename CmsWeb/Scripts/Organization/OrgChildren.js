$(function () {
    $.fmtTable = function() {
        $("table.grid2 td.tip").tooltip({ showBody: "|" });
        if (!$('#canedit').val()) {
            $('table.grid2 input').attr("disabled", "disabled");
        }
        $(".bt").button();
    };
    $.fmtTable();
    $.loadTable = function() {
        $.getTable($('#groupsform'));
    };
    $('#filter').live("click", function (ev) {
        ev.preventDefault();
        $.loadTable();
    });
    $("#groupsform").delegate("#memtype", "change", $.loadTable);

    $("#namesearch").keypress(function (ev) {
        if (ev.keyCode == '13') {
            ev.preventDefault();
            $.loadTable();
        }
    });

    $.getTable = function(f) {
        var q = f.serialize();
        $.post("/OrgChildren/Filter", q, function(ret) {
            $('table.grid > tbody').html(ret).ready($.fmtTable);
        });
        return false;
    };
    $("a.display").live('click', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        var q = f.serialize();
        $.post(this.href, q, function (ret) {
            $(f).html(ret).ready(function () {
                $.fmtTable();
                return false;
            });
        });
        return false;
    });
    $(".orgcheck").live("change", function (ev) {
        var ck = $(this);
        var tr = ck.parent().parent();
        $.post("/OrgChildren/UpdateOrg/", {
            ParentOrg: $("#orgid").val(),
            ChildOrg: ck.attr("oid"),
            Checked: ck.is(':checked')
        }, function (ret) {
            tr.effect("highlight", {}, 3000);
        });
    });
});

