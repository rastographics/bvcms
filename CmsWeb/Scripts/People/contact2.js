$(function () {
    $("a.remove").live("click", function (ev) {
        ev.preventDefault();
        var url = this.href;
        if ($("#edit-contact").length > 0) {
            $.bootstrapGrowl("update first");
            return false;
        }
        bootbox.confirm("Remove this person?", function(confirmed) {
            if (!confirmed) return;
            $.post(url, {}, function(ret) {
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
        var url = this.href;
        var f = $(this).closest("form");
        bootbox.confirm("Add new task for person?", function(confirmed) {
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
    $("#delete").live("click", function (ev) {
        ev.preventDefault();
        var url = this.href;
        var f = $(this).closest("form");
        bootbox.confirm("Delete this contact?", function(confirmed) {
            if (!confirmed) return;
            f.attr("action", url);
            f.submit();
        });
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
        bootbox.confirm("Add new contact for team?", function(confirmed) {
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
        bootbox.confirm("Add new task for person?", function(confirmed) {
            if (!confirmed) return;
            f.attr("action", url);
            f.submit();
        });
        return false;
    });
    $("#search-add a.commit").live("click", function (ev) {
        ev.preventDefault();
        var f = $(this).closest("form");
        var q = f.serialize();
        var url = this.href;
        $.post(url, q, function (ret) {
            f.modal("hide");
            if (ret.message) {
                alert(ret.message);
            } else
                switch (ret.from) {
                    case 'Contactor':
                        $("#contactors").load('/Contact2/Contactors/' + ret.cid, {});
                        break;
                    case 'Contactee':
                        $("#contactees").load('/Contact2/Contactees/' + ret.cid, {});
                        break;
                }
        });
        return false;
    });
});