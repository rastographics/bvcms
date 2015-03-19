$(function() {
    $("a.refresh").click(function(ev) {
        ev.preventDefault();
        $.getTable();
        return true;
    });

    $("a.delete").click(function(ev) {
        ev.preventDefault();
        if ($("span.sharecount").text() > 0) {
            swal("Error!", "Shares exist, cannot delete tag.", "error");
            return false;
        }

        swal({
                title: "Are you sure?",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-danger",
                confirmButtonText: "Yes, delete it!",
                closeOnConfirm: false
            },
            function() {
                $.post("/Tags/Delete", null, function(ret) {
                    if (ret == "error")
                        swal("Error!", "Cannot delete tag.", "error");
                    else {
                        swal({
                                title: "Deleted!",
                                type: "success"
                            },
                            function() {
                                $("#tag").replaceWith(ret);
                                $.getTable();
                            });
                    }
                });
            });
        return true;
    });

    $("#setshared").click(function(ev) {
        ev.preventDefault();
        var f = $('#results').closest('form');
        var q = f.serialize();
        $.post("/Tags/SetShared", q, function(ret) {
            $.getTable();
        });
        return false;
    });

    $("a.makenew").click(function(ev) {
        ev.preventDefault();
        $("#new-modal").modal();
        return true;
    });

    $("#new-tag").click(function(ev) {
        ev.preventDefault();
        var f = $('#new-tag-form');
        var q = f.serialize();
        $.post("/Tags/NewTag", q, function(ret) {
            $("#new-modal").modal("hide");
            $("#tag").replaceWith(ret);
            $.getTable();
            $("#tagname").val("");
        });
        return false;
    });

    $('#new-modal').on('shown.bs.modal', function() {
        $("#tagname").val('').focus();
    });

    $("a.rename").click(function (ev) {
        ev.preventDefault();
        $("#rename-modal").modal();
        return true;
    });

    $("#rename-tag").click(function (ev) {
        ev.preventDefault();
        var f = $('#rename-tag-form');
        var q = f.serialize();
        $.post("/Tags/RenameTag", q, function (ret) {
            $("#rename-modal").modal("hide");
            $("#tag").replaceWith(ret);
            $.getTable();
        });
        return false;
    });

    $('#rename-modal').on('shown.bs.modal', function () {
        $("#renamedTag").val($("#tag option:selected").text()).focus();
    });

    $('body').on('change', '#tag', function (ev) {
        ev.preventDefault();
        $.getTable();
        return false;
    });

    $.gotoPage = function (e, pg) {
        $("#Page").val(pg);
        $.getTable();
        return false;
    };

    $.setPageSize = function (e) {
        $('#Page').val(1);
        $("#PageSize").val($(e).val());
        return $.getTable();
    };

    $.getTable = function () {
        var f = $('#results').closest('form');
        var q = f.serialize();
        $.block();
        $.post($('a.refresh').attr('href'), q, function (ret) {
            $('#results').replaceWith(ret).ready(function () {
                var curtag = $("#actag").val();
                $('#resultsTable > tbody > tr:even').addClass('alt');
                $("#activetag").text(curtag);
                $("#current-tag1").text(curtag);
                $("#current-tag2").text(curtag);
                $("#tagalltagname").val(curtag);
                $("span.sharecount").text($("#shcnt").val());
            });
            $.unblock();
        });
        return false;
    };

    $('#resultsTable > tbody > tr:even').addClass('alt');
    $('body').on('click', '#resultsTable > thead a.sortable', function (ev) {
        ev.preventDefault();
        var newsort = $(this).text();
        var sort = $("#Sort");
        var dir = $("#Direction");
        if ($(sort).val() == newsort && $(dir).val() == 'asc')
            $(dir).val('desc');
        else
            $(dir).val('asc');
        $(sort).val(newsort);
        $.getTable();
        return false;
    });

    $('body').on('click', 'a.taguntag', function (ev) {
        ev.preventDefault();
        $.block();
        var a = $(this);
        $.post(a.attr('href'), null, function (ret) {
            a.text(ret);
            var link = $(ev.target).closest('a');
            link.removeClass('btn-default').removeClass('btn-success');
            link.addClass(ret == "Remove" ? "btn-default" : "btn-success");
            link.html(ret == "Remove" ? "<i class='fa fa-tag'></i> Remove" : "<i class='fa fa-tag'></i> Add");
            $.unblock();
        });
        return false;
    });

    $("a.ShareLink").SearchUsers({
        UpdateShared: function () {
            $.post("/Tags/UpdateShared", null, function (ret) {
                $("span.sharecount").text(ret);
            });
        }
    });

    $('a.ShareLink').on("click", function (e) {
        e.preventDefault();
        var d = $('#usersDialog');
        $('iframe', d).attr("src", this.href);
        d.dialog("open");
    });
    
});

function UpdateSelectedUsers(r) {
    $.post("/Tags/UpdateShared", null, function (ret) {
        $("span.sharecount").text(ret);
        $("#usersDialog").dialog("close");
    });
}