$(function () {
    $(".ckbox").click(function (ev) {
        var e = $(this);
        $.post("/OrgMemberDialog/CheckBoxChanged/{0}/{1}/{2}".format(e.data("id"), e.data("pid"), e.data("sgid")), {
            ck: $(this).is(":checked")
        });
        return true;
    });
    $(".bt").button();
    $("body").on('click', 'a.display', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        $.post($(this).attr('href'), null, function (ret) {
            $.displayEdit(f, ret);
        });
        return false;
    });
    $("body").on("click", 'a.delete', function (ev) {
        if (confirm("are you sure?"))
            $.post($(this).attr("href"), null, function (ret) {
                self.parent.RebindMemberGrids();
            });
        return false;
    });
    $("body").on('click', 'a.move', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        if (confirm("are you sure?"))
            $.post($(this).attr('href'), null, function (ret) {
                self.parent.RebindMemberGrids();
            });
        return false;
    });
    $("form.DisplayEdit").on('click', 'a.submitbutton', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        var q = f.serialize();
        $.post($(this).attr('href'), q, function (ret) {
            if (ret.substring(0, 5) != "error")
                $.displayEdit(f, ret);
            self.parent.RebindMemberGrids();
        });
        return false;
    });
    $.displayEdit = function (f, ret) {
        $(f).html(ret).ready(function () {
            var acopts = {
                minChars: 3,
                matchContains: 1
            };
            $(".datepicker").jqdatepicker();
            $(".bt").button();
            $("a.move").tooltip({
                showBody: "|",
                showURL: false
            });

        });
    }
});

