$(function () {
    $("a.remove").live("click", function (ev) {
        ev.preventDefault();
        if ($("#edit-contact").length > 0) {
            $.bootstrapGrowl("update first");
            return false;
        }
        bootbox.confirm("Remove this person?", function(confirmed) {
            if (!confirmed) return;
            $.post($(this)[0].href, {}, function(ret) {
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
        bootbox.confirm("Add new task for person?", function(confirmed) {
            if (!confirmed) return;
            var f = $("#contact");
            f.attr("action", $(this)[0].href);
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
        window.location = $(this)[0].href;
        return false;
    });
    $("#delete").live("click", function (ev) {
        ev.preventDefault();
        bootbox.confirm("Delete this contact?", function(confirmed) {
            if (!confirmed) return;
            var f = $("#contact");
            f.attr("action", $(this)[0].href);
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
        bootbox.confirm("Add new contact for team?", function(confirmed) {
            if (!confirmed) return;
            var f = $("#contact");
            f.attr("action", $(this)[0].href);
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
        bootbox.confirm("Add new task for person?", function(confirmed) {
            if (!confirmed) return;
            var f = $("#contact");
            f.attr("action", $(this)[0].href);
            f.submit();
        });
        return false;
    });
});