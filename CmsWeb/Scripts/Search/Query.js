$(function () {
    var liedit;
    var prevliedit;
    var $backdrop = $('<div class="modal-backdrop hide" />').appendTo('body');
    $('#conditions a.edit-popover').live("click", function () {
        liedit = $(this).closest("li.condition");
        $EditCondition();
        return false;
    });
    var $EditCondition = function (option) {
        var qid = liedit.data("qid");
        if ($("#editcondition").is(":visible")) {
            var h = $("#editcondition").attr("orginalheight");
            prevliedit.animate({ height: h }, 150);
            $.HideEditCondition(prevliedit);
        }
        $.post('/Query/EditCondition/' + qid, null, function (ret) {
            $("#editcondition .popover-content").html(ret).ready(function () {
                $.AdjustEditCondition(option);
            });
        });
    };
    $.AdjustEditCondition = function (option) {
        $('#CodeValues').multiselect({
            includeSelectAllOption: true,
            enableFiltering: true,
            enableCaseInsensitiveFiltering: true
        });
        var h = $("#editcondition").outerHeight();
        $("#editcondition").attr("orginalheight", liedit.height());
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
            $("#editcondition").slideDown(500, function() {
                if(option && option.isnew)
                    $("#SelectCondition").click();
            });
        }, 10);
        prevliedit = liedit;
    };
    $.HideEditCondition = function () {
        $backdrop.addClass("hide");
        var oh = $("#editcondition").attr("orginalheight");
        $("#editcondition").slideUp(150);
        prevliedit.animate({ height: oh }, 400);
    };
    $('#CancelChange').live("click", function () {
        $.HideEditCondition();
        return false;
    });
    $('#SaveCondition').live("click", function () {
        var q = $('#editForm').serialize();
        $.post('/Query/SaveCondition/', q, function (ret) {
            $("#conditions").html(ret).ready(function () {
                $.HideEditCondition();
                RefreshList();
            });
        });
        return false;
    });
    $('#conditions a.addnewclause').live("click", function () {
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
    $('#conditions a.duplicateclause').live("click", function () {
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        $.post('/Query/DuplicateCondition/' + qid, {}, function (ret) {
            $("#conditions").html(ret).ready(function () {
                liedit = $("li[data-qid='" + $("#NewId").val() + "']");
                $EditCondition();
            });
        });
        return false;
    });
    $('#conditions a.insgroupabove').live("click", function () {
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        $.post('/Query/InsGroupAbove/' + qid, {}, function (ret) {
            $("#conditions").html(ret).ready($.AdjustEditCondition);
        });
        return false;
    });
    $('#conditions a.movetoprevgroup').live("click", function () {
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        $.post('/Query/MoveToPreviousGroup/' + qid, {}, function (ret) {
            $("#conditions").html(ret);
            RefreshList();
        });
        return false;
    });
    $('#conditions a.delete').live("click", function () {
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
    $('#Comparison').live("change", function (ev) {
        if ($("#CodesDiv").length > 0) {
            var q = $('#editForm').serialize();
            $.post('/Query/CodesDropdown', q, function (ret) {
                $("#CodesDiv").replaceWith(ret).ready(function () {
                    $('#CodeValues').multiselect();
                });
            });
        }
    });
    $('a.help').live("click", function (event) {
        event.preventDefault();
        var d = $('#QueryConditionHelp');
        if (this.href.endsWith('-'))
            $('iframe', d).attr("src", this.href + $("#ConditionName").val());
        else
            $('iframe', d).attr("src", this.href);
        d.dialog("open");
    });
    $('#Tags').click(function (ev) {
        $('#TagsPopup').show();
    });
    $(".datepicker").datepicker();

    $('#Program').live("change", function (ev) {
        $.post('/Query/Divisions/' + $(this).val(), null, function (ret) {
            $("#Division").replaceWith(ret);
            $("#Organization").replaceWith("<select id='Organization' name='Organization'><option value='0'>(not specified)</option></select>");
        });
    });
    $("#Division").live("change", function () {
        $.post('/Query/Organizations/' + $(this).val(), null, function (ret2) {
            $("#Organization").replaceWith(ret2);
        });
    });
    $('#Run').live("click", function (ev) {
        RefreshList();
        return false;
    });
    $('#Export').click(function (ev) {
        window.location = "/Query/Export/" + $("#QueryId").val();
    });
    if ($("#NewSearchId").val()) {
        liedit = $("li[data-qid='" + $("#NewSearchId").val() + "']");
        RefreshList();
        $EditCondition({ isnew: true });
    }
    else if ($("#AutoRun").val() === "True")
        RefreshList();

    $("#SelectCondition").live("click", function (ev) {
        ev.preventDefault();
        $backdrop.css({ "z-index": 1042 });
        $('#QueryConditionSelect').modal("show");
        return false;
    });
    $("#QueryConditionSelect").on("hidden", function(ev) {
        $backdrop.css({ "z-index": 1040 });
    });

    $('div.FieldLink a').click(function (ev) {
        ev.preventDefault();
        var qid = liedit.data("qid");
        $.post('/Query/SelectCondition/' + qid, { conditionName: ev.target.id }, function (ret) {
            $('#QueryConditionSelect').modal("hide");
            prevliedit = liedit;
            $("#editcondition .popover-content").html(ret).ready($.AdjustEditCondition);
        });
        return false;
    });
    $("a.closeit").click(function (ev) {
        $.unblock();
    });
    $.navigate = function (url, data) {
        url += (url.match(/\?/) ? "&" : "?") + data;
        window.location = url;
    };
    $.fn.enabled = function (bool) {
        if (bool)
            $(this).attr("href", "#").removeClass("disabled");
        else
            $(this).removeAttr("href").addClass("disabled");
        return this;
    };
    //RefreshList();
});

function RefreshList(qs) {
    //    if (!qs)
    //        qs = $("#editForm").serialize();

    $.block();
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
            $.unblock();
        },
        error: function (request, status, err) {
            $.unblock();
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