$(function () {
    $("a.remove").live("click", function (ev) {
        ev.preventDefault();
        var url = this.href;
        if ($("#edit-contact").length > 0) {
            $.bootstrapGrowl("update first");
            return false;
        }
        bootbox.confirm("Remove this person?", function (confirmed) {
            if (!confirmed) return;
            $.post(url, {}, function (ret) {
                window.location.reload(true);
            });
        });
        return false;
    });
    $(".addtask").live("click", function (ev) {
        ev.preventDefault();
        if ($("#edit-contact").length > 0) {
            $.bootstrapGrowl("update first");
            return false;
        }
        var f = $(this).closest("form");
        var url = this.href;
        f.attr("action", url);
        bootbox.confirm("Add new task for person?", function (confirmed) {
            if (!confirmed) return;
            f.attr("action", url);
            f.submit();
        });
        return false;
    });
    $("a.link").live("click", function (ev) {
        ev.preventDefault();
        if ($("#edit-contact").length > 0) {
            $.bootstrapGrowl("update first");
            return false;
        }
        window.location = this.href;
        return false;
    });
    $("#newteamcontact").live("click", function (ev) {
        ev.preventDefault();
        if ($("#edit-contact").length > 0) {
            $.bootstrapGrowl("update first");
            return false;
        }
        var url = this.href;
        var f = $(this).closest("form");
        bootbox.confirm("Add new contact for team?", function (confirmed) {
            if (!confirmed) return;
            f.attr("action", url);
            f.submit();
        });
        return false;
    });
    $(".addtask").live("click", function (ev) {
        ev.preventDefault();
        if ($("#edit-contact").length > 0) {
            $.bootstrapGrowl("update first");
            return false;
        }
        var url = this.href;
        var f = $(this).closest("form");
        bootbox.confirm("Add new task for person?", function (confirmed) {
            if (!confirmed) return;
            f.attr("action", url);
            f.submit();
        });
        return false;
    });
});
function AddSelected(ret) {
    switch (ret.from) {
        case 'Contactor':
            $("#contactors").load('/Contact2/Contactors/' + ret.cid, {});
            break;
        case 'Contactee':
            $("#contactees").load('/Contact2/Contactees/' + ret.cid, {});
            break;
    }
}