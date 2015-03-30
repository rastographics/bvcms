$(function () {
    $("a.formlink").live('click', function (ev) {
        ev.preventDefault();
        var a = $(this);
        var f = a.closest('form');
        var q = f.serialize();
        var h = a.attr('href');
        if (a.text() === 'Commit and Add') {
            $(this).attr("href", "");
            $.block();
        }
        $.post(h, q, function (ret) {
            if (ret.close) {
                if (ret.message) {
                    alert(ret.message);
                }
                switch (ret.how) {
                    case 'rebindgrids':
                        if (self.parent.RebindMemberGrids)
                            self.parent.RebindMemberGrids();
                        break;
                    case 'addselected':
                        if (self.parent.AddSelected)
                            self.parent.AddSelected(ret);
                        break;
                    case 'addselected2':
                        if (self.parent.AddSelected2)
                            self.parent.AddSelected2(ret);
                        break;
                    case 'CloseAddDialog':
                        if (self.parent.CloseAddDialog)
                            self.parent.CloseAddDialog();
                        break;
                }
            }
            else {
                $(f).html(ret).ready(function() {
                    $("a.bt").button();
                    $(".addrcol").tooltip({
                        showURL: false,
                        showBody: "|"
                    });
                    $('#people > tbody > tr:even').addClass('alt');
                });
            }
            $.unblock();
        });
        return false;
    });
    $("a.bt").button();
    $("a.clear").live('click', function (ev) {
        ev.preventDefault();
        $("#name").val('');
        $("#phone").val('');
        $("#address").val('');
        $("#dob").val('');
        return false;
    });
    $("form input").live("keypress", function (e) {
        if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
            $('a.default').click();
            return false;
        }
        return true;
    });
});

