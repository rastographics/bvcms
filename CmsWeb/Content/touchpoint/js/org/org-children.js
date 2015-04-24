$(function () {

    function initializePopovers() {
        $('[data-toggle="popover"]').popover({ html: true });
        $('[data-toggle="popover"]').click(function (ev) {
            ev.preventDefault();
        });
    }

    initializePopovers();

    $.loadTable = function () {
        $.getTable($('#groupsform'));
    };

    $('body').on('click', '#filter', function (ev) {
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

    $.getTable = function (f) {
        var q = f.serialize();
        $.post("/OrgChildren/Filter", q, function (ret) {
            $('#table-results > tbody').html(ret).ready($.fmtTable);
            initializePopovers();
        });
        return false;
    };

    $('body').on('click', 'a.display', function (ev) {
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

    $('body').on('change', '.orgcheck', function (ev) {
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