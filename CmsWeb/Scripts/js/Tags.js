$(function () {
    $(".bt").button();
    $("#refresh").click(function (ev) {
        ev.preventDefault();
        $.getTable();
        return false;
    });
    $("#delete").click(function (ev) {
        ev.preventDefault();
        if ($("#sharecount").text() > 0) {
            $.growlUI("Error", "Shares exist, cannot delete tag", 3000, null);
            return false;
        }
        if (confirm($(this).attr("confirm"))) {
            $.post("/Tags/Delete", null, function (ret) {
                if (ret == "error") {
                    $.growlUI("Error", "cannot delete tag", 3000, null);
                } else {
                    $("#tag").replaceWith(ret);
                    $.getTable();
                }
            });
        }
        return false;
    });
    $("#setshared").click(function (ev) {
        ev.preventDefault();
        var f = $('#results').closest('form');
        var q = f.serialize();
        $.post("/Tags/SetShared", q, function (ret) {
            $.getTable();
        });
        return false;
    });
    $("#makenew").click(function (ev) {
        ev.preventDefault();
        var f = $('#results').closest('form');
        var q = f.serialize();
        $.post("/Tags/NewTag", q, function (ret) {
            $("#tag").replaceWith(ret);
            $.getTable();
            $("#tagname").val("");
        });
        return false;
    });
    $("#rename").click(function (ev) {
        ev.preventDefault();
        var f = $('#results').closest('form');
        var q = f.serialize();
        $.post("/Tags/RenameTag", q, function (ret) {
            $("#tag").replaceWith(ret);
            $.getTable();
            $("#tagname").val("");
        });
        return false;
    });
    $("#tag").live("change", function (ev) {
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
        $.blockUI();
        $.post($('#refresh').attr('href'), q, function (ret) {
            $('#results').replaceWith(ret).ready(function () {
                $('#results > tbody > tr:even').addClass('alt');
                $("#activetag").text($("#actag").val());
                $("#tagalltagname").val($("#actag").val());
                $("#sharecount").text($("#shcnt").val());
                //$('.dropdown').hoverIntent(dropdownshow, dropdownhide);
                $.unblockUI();
            });
        });
        return false;
    };
    $('#results > tbody > tr:even').addClass('alt');
    $('#results > thead a.sortable').live('click', function (ev) {
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
    $("form input").live("keypress", function (e) {
        if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
            $('#refresh').click();
            return false;
        }
        return true;
    });
    $('a.taguntag').live('click', function (ev) {
        ev.preventDefault();
        var a = $(this);
        $.post(a.attr('href'), null, function (ret) {
            a.text(ret);
        });
        return false;
    });

    $("#ShareLink").SearchUsers({
        UpdateShared: function () {
            $.post("/Tags/UpdateShared", null, function (ret) {
                $("#sharecount").text(ret);
            });
        }
    });
    $('#ShareLink').live("click", function (e) {
        e.preventDefault();
        var d = $('#usersDialog');
        $('iframe', d).attr("src", this.href);
        d.dialog("open");
    });
    //    $("#tagname").keypress(function (ev) {
    //        if (ev.which != 58) //no : character
    //            return true;
    //        return false;
    //    });
});
function UpdateSelectedUsers(r) {
    $.post("/Tags/UpdateShared", null, function (ret) {
        $("#sharecount").text(ret);
        $("#usersDialog").dialog("close");
    });
}