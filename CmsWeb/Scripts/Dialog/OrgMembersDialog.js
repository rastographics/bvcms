$(function () {
    $.fmtTable = function () {
        $("table.grid2 td.tip").tooltip({ showBody: "|" });
    }
    $.fmtTable();
    $(".bt").button();
    $(".filter").change(function (ev) {
        var q = $("form").serialize();
        $.post("/OrgMembersDialog/Filter", q, function (ret) {
            $("table.grid2 > tbody").html(ret).ready($.fmtTable);
        });
    });

    $(".datepicker").jqdatepicker();

    $("#SelectAll").click(function () {
        $("input[name='list']:checkbox").prop('checked', this.checked);
    });
    $("body").on('click', 'a.display', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        $.post(this.href, null, function (ret) {
            $(f).html(ret).ready(function () {
                var acopts = {
                    minChars: 3,
                    matchContains: 1
                };
                return false;
            });
        });
        return false;
    });
    $("body").on("click", 'a.delete', function (ev) {
        if (confirm("are you sure?"))
            $.post($(this).attr("href"), null, function (ret) {
                self.parent.RebindMemberGrids($("#from").val());
            });
        return false;
    });
    $("body").on("click", '#ClearFilter', function (ev) {
        ev.preventDefault();
        $("#memtype,#tag").val("0");
        $("#inactivedt").val("");
        var q = $("form").serialize();
        $.post("/OrgMembersDialog/Filter", q, function (ret) {
            $("table.grid2 > tbody").html(ret).ready($.fmtTable);
        });

        return false;
    });
    $("body").on('click', 'a.move', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        if (confirm("are you sure?"))
            $.post($(this).attr('href'), null, function (ret) {
                self.parent.RebindMemberGrids($("#from").val());
            });
        return false;
    });
    $("form.DisplayEdit").on('click', 'a.submitbutton', function (ev) {
        ev.preventDefault(); p
        var f = $(this).closest('form');
        var q = f.serialize();
        $.post($(this).attr('href'), q, function (ret) {
            self.parent.RebindMemberGrids($("#from").val());
        });
        return false;
    });
});

