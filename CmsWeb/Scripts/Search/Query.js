$(function () {
    var liedit;
    var $backdrop = $('<div class="modal-backdrop hide" />').appendTo('body');
    $('#conditions').on("click", 'a.edit-popover', function () {
        liedit = $(this).closest("li.condition");
        $EditCondition();
        return false;
    });
    var $EditCondition = function (option) {
        var qid = liedit.data("qid");
        $("#editcondition").attr("originalheight", liedit.height());
        if ($("#editcondition").is(":visible")) {
            $.HideEditCondition();
        }
        $.post('/Query/EditCondition/' + qid, null, function (ret) {
            $("#editcondition .popover-content").html(ret).ready(function () {
                $.AdjustEditCondition(option);
            });
        });
    };
    $("#Description").editable({
        placement: "right",
        showbuttons: "bottom",
        pk: 1,
        url: "/Query/DescriptionUpdate",
        mode: "popup"
    });
    $('#DescriptionEdit').click(function (e) {
        e.stopPropagation();
        $('#Description').editable('toggle');
    });
    $.AdjustEditCondition = function (option) {

        $("#editcondition .date").datepicker({ autoclose: true, orientation: "auto" });
        $("#editcondition select").multiselect({
            includeSelectAllOption: true,
            enableFiltering: true,
            enableCaseInsensitiveFiltering: true
        });

        var h = $("#editcondition").outerHeight();
        var pos = liedit.position();
        var wid = liedit.width();

        $("#editcondition").css({
            "left": pos.left,
            "top": pos.top + 2,
            "min-width": wid,
            "z-index": 1041
        });
        $backdrop.removeClass("hide");
        liedit.animate({ height: h }, 350);
        setTimeout(function () {
            $("#editcondition").slideDown(500, function () {
                $(this).scrollintoview();
                if (option && option.isnew)
                    $("#SelectCondition").click();
            });
        }, 10);
    };
    $.HideEditCondition = function () {
        $backdrop.addClass("hide");
        var oh = $("#editcondition").attr("originalheight");
        $("#editcondition").slideUp(150);
        var pliedit = $("li[data-qid='" + $("#SelectedId").val() + "']");
        pliedit.animate({ height: oh }, 400);
    };
    $(document).on("click", '#CancelChange', function () {
        $.HideEditCondition();
        return false;
    });
    $("#conditions").on("mouseenter", "header", function () {
        var li = $(this).closest("li");
        li.addClass("borderleftred");
    }).on("mouseleave", "header", function () {
        var li = $(this).closest("li");
        li.removeClass("borderleftred");
    });
    $(document).on("click", '#SaveCondition', function () {
        var q = $('#editForm').serialize();
        $.post('/Query/SaveCondition/', q, function (ret) {
            if (ret.startsWith("<form"))
                $("#editcondition .popover-content").html(ret).ready(function () {
                    $.InitCodeValues();
                });
            else {
                $.HideEditCondition();
                $("#conditions").html(ret).ready(function () {
                    RefreshList();
                });
            }
        });
        return false;
    });
    $(document).on("keydown", '#editForm input', function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            $('#SaveCondition').click();
            return false;
        }
    });
    $('#conditions').on("change", "select.changegroup", function () {
        var v = $(this).val();
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        $.post('/Query/ChangeGroup/' + qid, { comparison: v }, function () {
            RefreshList();
        });
        return false;
    });
    $('#conditions').on("click", 'a.addnewclause', function () {
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        $.post('/Query/AddNewCondition/' + qid, {}, function (ret) {
            $("#conditions").html(ret).ready(function () {
                liedit = $("li[data-qid='" + $("#NewId").val() + "']");
                $EditCondition({ isnew: true });
            });
        });
        return false;
    });
    $('#conditions').on("click", 'a.addnewgroup', function () {
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        $.post('/Query/AddNewGroup/' + qid, {}, function (ret) {
            $("#conditions").html(ret).ready(function () {
                liedit = $("li[data-qid='" + $("#NewId").val() + "']");
                $EditCondition({ isnew: true });
            });
        });
        return false;
    });
    if ($.ClipboardHasCondition) {
        $("li.pastecondition").show();
    }
    $('#conditions').on("click", 'a.cutcondition', function () {
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        $(this).parent().parent().prev().dropdown("toggle");
        $.post('/Query/Cut/' + qid);
        $("li.pastecondition").show();
        return false;
    });
    $('#conditions').on("click", 'a.copycondition', function () {
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        $.post('/Query/Copy/' + qid);
        $(this).parent().parent().prev().dropdown("toggle");
        $("li.pastecondition").show();
        return false;
    });
    $('#conditions').on("click", 'a.pastecondition', function () {
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        $.post('/Query/Paste/' + qid, {}, function (ret) {
            $("#conditions").html(ret);
            RefreshList();
        });
        return false;
    });
    $('#conditions').on("click", 'a.insgroupabove', function () {
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        $.post('/Query/InsGroupAbove/' + qid, {}, function (ret) {
            $("#conditions").html(ret);
            RefreshList();
        });
        return false;
    });
    $('#conditions').on("click", 'a.delete', function () {
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        bootbox.confirm("Are you sure you want to delete?", function (result) {
            if (result === true) {
                $.post('/Query/RemoveCondition/' + qid, null, function (ret) {
                    $("#conditions").html(ret);
                    RefreshList();
                });
            }
        });
        return false;
    });
    $(document).on("change", '#Comparison', function (ev) {
        var sel = "#CodeValues";
        if ($(sel).length > 0) {
            var q = $('#editForm').serialize();
            $.post('/Query/CodeSelect', q, function (ret) {
                $(sel).multiselect("destroy").ready(function() {
                    $(sel).replaceWith(ret).ready(function () {
                        $(sel).multiselect({
                            enableFiltering: true,
                            enableCaseInsensitiveFiltering: true
                        });
                    });
                });
            });
        }
    });
    $('#Tags').click(function (ev) {
        $('#TagsPopup').show();
    });

    $(document).on("change", '#Program', function (ev) {
        $.post('/Query/Divisions/' + $(this).val(), null, function (ret) {
            $("#Division").replaceWith(ret)
                .multiselect({
                    enableFiltering: true,
                    enableCaseInsensitiveFiltering: true
                });
            $("#Organization").replaceWith("<select id='Organization' name='Organization' style='display:none'><option value='0'>(not specified)</option></select>")
                .multiselect({
                    enableFiltering: true,
                    enableCaseInsensitiveFiltering: true
                });
        });
    });
    $(document).on("change", '#Division', function () {
        $.post('/Query/Organizations/' + $(this).val(), null, function (ret) {
            $("#Organization").replaceWith(ret)
                .multiselect({
                    enableFiltering: true,
                    enableCaseInsensitiveFiltering: true
                });
        });
    });
    $(document).on("click", '#Run', function (ev) {
        RefreshList();
        return false;
    });
    $('#Export').click(function (ev) {
        window.location = "/Query/Export/" + $("#QueryId").val();
    });

    $(document).on("click", "#SelectCondition", function (ev) {
        ev.preventDefault();
        $backdrop.css({ "z-index": 1042 });
        $('#QueryConditionSelect').modal("show");
        return false;
    });
    $(document).on("hidden", "#QueryConditionSelect", function (ev) {
        $backdrop.css({ "z-index": 1040 });
    });

    $('.FieldLink a').click(function (ev) {
        ev.preventDefault();
        var qid = liedit.data("qid");
        $.post('/Query/SelectCondition/' + qid, { conditionName: ev.target.id }, function (ret) {
            $('#QueryConditionSelect').modal("hide");
            $("#editcondition .popover-content").html(ret).ready($.AdjustEditCondition);
        });
        return false;
    });
    $.navigate = function (url, data) {
        url += (url.match(/\?/) ? "&" : "?") + data;
        window.location = url;
    };
    $('a.help').live("click", function (event) {
        event.preventDefault();
        var d = $('#QueryConditionHelp');
        if (this.href.endsWith('-'))
            $('iframe', d).attr("src", this.href + $("#ConditionName").val());
        else
            $('iframe', d).attr("src", this.href);
        d.dialog("open");
    });
    if ($("#NewSearchId").val()) {
        liedit = $("li[data-qid='" + $("#NewSearchId").val() + "']");
        $EditCondition({ isnew: true });
    }
    else if ($("#AutoRun").val() === "True")
        RefreshList();
});

function RefreshList(qs) {
    $.ajax({
        type: "POST",
        url: "/Query/Results/",
        data: qs,
        timeout: 1200000, // in milliseconds
        success: function (ret) {
            $('#toolbar').show();
            $('#results').html(ret);
            $('#people tbody tr:even').addClass('alt');
            $('a.taguntag').click(function (ev) {
                $.post('/Query/ToggleTag/' + $(this).attr('value'), null, function (ret) {
                    if (ret.error)
                        alert(ret.error);
                    else
                        $(ev.target).text(ret.HasTag ? "Remove" : "Add");
                });
                return false;
            });
            $('#people thead a.sortable').click(function (ev) {
                var newsort = $(this).text();
                var oldsort = $("#Sort").val();
                $("#Sort").val(newsort);
                var dir = $("#Direction").val();
                if (oldsort == newsort && dir == 'asc')
                    $("#Direction").val('desc');
                else
                    $("#Direction").val('asc');
                RefreshList();
                return false;
            });
        },
        error: function (request, status, err) {
            alert(err);
        }
    });
}

function ShowErrors(j) {
    $('.validate').each(function () {
        $(this).next(".error").remove();
    });
    var e = eval('(' + j + ')');
    if (e.count == 0)
        return false;
    $('.validate').each(function () {
        if (e[this.id])
            $(this).after("<span class='error'> " + e[this.id] + "</span>");
    });
    return true;
}