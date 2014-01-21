$(function () {
    $.postQuery = function (action, selectedid, cb) {
        if ($.isFunction(selectedid)) {
            cb = selectedid;
            selectedid = undefined;
        }
        if (selectedid !== undefined)
            $("#SelectedId").val(selectedid);
        var q = $('#query-form').serialize();
        $.post("/Query/" + action, q, cb);
    };
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
        $.postQuery("EditCondition", qid, function (ret) {
            $("#editcondition .popover-content").html(ret).ready(function () {
                $.AdjustEditCondition(option);
            });
        });
    };
    $("#SaveAs").click(function (ev) {
        ev.preventDefault();
        var href = this.href;
        $("<div />").load("/Query/SaveAs", {
            id: $("#QueryId").val(),
            nametosaveas: $("#SaveToDescription").val()
        }, function () {
            var d = $(this);
            var f = d.find("form");
            f.modal("show");
            f.on('hidden', function () {
                d.remove();
                f.remove();
            });
        });
    });
    $.AdjustEditCondition = function (option) {
        $("#editcondition .date").datepicker({
            keyboardNavigation: true,
            orientation: "auto",
            forceParse: false
        });
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
        pliedit.animate({ height: oh }, 400, function () {
            $("#editcondition .popover-content").empty();
        });
    };
    $('#CancelChange').live("click", function () {
        $.HideEditCondition();
        return false;
    });
    $("#conditions header").live("mouseenter", function () {
        var li = $(this).closest("li");
        li.addClass("borderleftred");
    });
    $("#conditions header").live("mouseleave", function () {
        var li = $(this).closest("li");
        li.removeClass("borderleftred");
    });
    $('#SaveCondition').live("click", function () {
        $.postQuery('SaveCondition', function (ret) {
            if (ret.startsWith("<fieldset"))
                $("#editcondition .popover-content").html(ret).ready(function () {
                    $.InitCodeValues();
                });
            else {
                $.HideEditCondition();
                $("#conditions").html(ret).ready(function () {
                    if ($("#AutoRun").prop("checked"))
                        RefreshList();
                    else
                        FadeList();
                });
            }
        });
        return false;
    });
    $('#editForm input').live("keydown", function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            $('#SaveCondition').click();
            return false;
        }
    });
    $('#conditions select.changegroup').live("change", function () {
        var v = $(this).val();
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        $.postQuery('ChangeGroup/' + v, qid, function () {
            if ($("#AutoRun").prop("checked"))
                RefreshList();
            else
                FadeList();
        });
        return false;
    });
    $('#conditions a.addnewclause').live("click", function () {
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        $.postQuery('AddNewCondition', qid, function (ret) {
            $("#conditions").html(ret).ready(function () {
                liedit = $("li[data-qid='" + $("#NewId").val() + "']");
                $EditCondition({ isnew: true });
            });
        });
        return false;
    });
    $('#conditions a.addnewgroup').live("click", function () {
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        $.postQuery('AddNewGroup', qid, function (ret) {
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
    $('#conditions a.cutcondition').live("click", function () {
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        $(this).parent().parent().prev().dropdown("toggle");
        $.postQuery('Cut', qid, function (ret) {
            $("#conditions").html(ret).ready(function () {
                $("li.pastecondition").show();
                if ($("#AutoRun").prop("checked"))
                    RefreshList();
                else
                    FadeList();
            });
        });
        return false;
    });
    $('#conditions a.copycondition').live("click", function () {
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        $.postQuery('Copy', qid);
        $(this).parent().parent().prev().dropdown("toggle");
        $("li.pastecondition").show();
        return false;
    });
    $('#conditions a.pastecondition').live("click", function () {
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        $.postQuery('Paste', qid, function (ret) {
            $("#conditions").html(ret);
            if ($("#AutoRun").prop("checked"))
                RefreshList();
            else
                FadeList();
        });
        return false;
    });
    $('#conditions a.insgroupabove').live("click", function () {
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        $.postQuery('InsGroupAbove', qid, function (ret) {
            $("#conditions").html(ret);
            if ($("#AutoRun").prop("checked"))
                RefreshList();
            else
                FadeList();
        });
        return false;
    });
    $('#conditions a.maketopgroup').live("click", function () {
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        $.postQuery('MakeTopGroup', qid, function (ret) {
            $("#conditions").html(ret);
            if ($("#AutoRun").prop("checked"))
                RefreshList();
            else
                FadeList();
        });
        return false;
    });
    $('#conditions a.delete').live("click", function () {
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        bootbox.confirm("Are you sure you want to delete?", function (result) {
            if (result === true) {
                $.postQuery('RemoveCondition', qid, function (ret) {
                    $("#conditions").html(ret);
                    if ($("#AutoRun").prop("checked"))
                        RefreshList();
                    else
                        FadeList();
                });
            }
        });
        return false;
    });
    $('#Comparison').live("change", function (ev) {
        var sel = "#CodeValues";
        if ($(sel).length > 0) {
            $.postQuery('CodeSelect', function (ret) {
                $.replaceSelect("#CodeValues", ret);
            });
        }
    });
    $('#Tags').click(function (ev) {
        $('#TagsPopup').show();
    });

    $.replaceSelect = function (sel, ret) {
        $(sel).multiselect("destroy").ready(function () {
            $(sel).replaceWith(ret).ready(function () {
                $(sel).multiselect({
                    enableFiltering: true,
                    enableCaseInsensitiveFiltering: true
                });
            });
        });
    };
    $('#Program').live("change", function (ev) {
        $.postQuery('Divisions/' + $(this).val(), function (ret) {
            $.replaceSelect('#Division', ret);
            $.replaceSelect('#Organization', "<select id='Organization' name='Organization' style='display:none'><option value='0'>(not specified)</option></select>");
        });
    });
    $('#Division').live("change", function () {
        $.postQuery('Organizations/' + $(this).val(), function (ret) {
            $.replaceSelect("#Organization", ret);
        });
    });
    $('#Run').live("click", function (ev) {
        RefreshList();
        return false;
    });
    $('#Export').click(function (ev) {
        window.location = "/Query/Export/" + $("#QueryId").val();
    });

    $("#SelectCondition").live("click", function (ev) {
        ev.preventDefault();
        $backdrop.css({ "z-index": 1042 });
        $('#QueryConditionSelect').modal("show");
        return false;
    });
    $("#QueryConditionSelect").live("hidden", function (ev) {
        $backdrop.css({ "z-index": 1040 });
    });

    $('.FieldLink a').click(function (ev) {
        ev.preventDefault();
        var qid = liedit.data("qid");
        $("#ConditionName").val(ev.target.id);
        $.postQuery('SelectCondition', qid, function (ret) {
            $('#QueryConditionSelect').modal("hide");
            $("#editcondition .popover-content").html(ret).ready($.AdjustEditCondition);
        });
        return false;
    });
    $.navigate = function (url, data) {
        url += (url.match(/\?/) ? "&" : "?") + data;
        window.location = url;
    };
    $("a.help").live("click", function (ev) {
        ev.preventDefault();
        var href = "//www.bvcms.com/DocDialog/" + $(this).data("name");
        if (href.endsWith('-'))
            href = href + $("#ConditionName").val();
        $("#helpcontent iframe").attr("src", href).ready(function () {
            $("#QueryConditionHelp").modal("show");
        });
    });
    if ($("#NewSearchId").val()) {
        liedit = $("li[data-qid='" + $("#NewSearchId").val() + "']");
        $EditCondition({ isnew: true });
    }
    else if ($("#AutoRun").prop("checked"))
        RefreshList();
    $('#AutoRun').change(function () {
        $.post("/Query/SetAutoRun", { setting: $(this).prop("checked") });
    });
    $('a.taguntag').live("click", function (ev) {
        $.post('/Query/ToggleTag/' + $(this).attr('value'), function (ret) {
            if (ret.error)
                alert(ret.error);
            else
                $(ev.target).text(ret.HasTag ? "Remove" : "Add");
        });
        return false;
    });
});

function FadeList() {
    $("#results").addClass("faded-results");
}

function RefreshList(qs) {
    $.ajax({
        type: "POST",
        url: "/Query/Results/",
        data: qs,
        timeout: 1200000, // in milliseconds
        success: function (ret) {
            $('#toolbar').show();
            $('#results').html(ret);
            $("#results").removeClass("faded-results");
            $('#people tbody tr:even').addClass('alt');
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