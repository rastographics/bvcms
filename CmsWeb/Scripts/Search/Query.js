$(function () {
    var $editplaceholderheight = 0;
    var $backdrop = $('<div class="modal-backdrop hide" />').appendTo('body');
    $('#conditions a.editconditionlink').live("click", function () {
        var qid = $(this).closest("tr").attr("qid");
        $EditCondition(qid);
        return false;
    });
    var $EditCondition = function (qid) {
        $editplaceholderheight = $(this).parent().height();
        if ($("#editcondition").is(":visible")) {
            var h = $("#editcondition").attr("orginalheight");
            $("#editconditionplaceholder").animate({ height: h }, 150);
            $.HideEditCondition();
        }
        $.post('/Query/EditCondition/' + qid, null, function (ret) {
            $("#conditions").html(ret).ready($.AdjustEditCondition);
        });
    };
    $.AdjustEditCondition = function () {
        $('#CodeValues').multiselect({
            includeSelectAllOption: true,
            enableFiltering: true,
            enableCaseInsensitiveFiltering: true
        });
        var h = $("#editcondition").outerHeight();
        $("#editcondition").attr("orginalheight", $("#editconditionplaceholder").height());
        $("#editconditionplaceholder").animate({ height: h }, 200);
        var pos = $("#editconditionplaceholder").closest("td").position();
        var wid = $("#editconditionplaceholder").closest("td").width();

        $("#editcondition").css({
            "left": pos.left,
            "top": pos.top + 2,
            "min-width": wid,
            "z-index": 1050
        });
        $backdrop.removeClass("hide");
        $("#editcondition").slideDown(200);
    };
    $.HideEditCondition = function () {
        $backdrop.addClass("hide");
        var oh = $("#editcondition").attr("orginalheight");
        $("#editconditionplaceholder").animate({ height: oh }, 500);
    };
    $('#conditions a.addnewclause').live("click", function () {
        var qid = $(this).closest("tr").attr("qid");
        $.post('/Query/AddNewCondition/' + qid, {}, function (ret) {
            $("#conditions").html(ret).ready($.AdjustEditCondition);
            $('#QueryConditionSelect').modal("show");
        });
        return false;
    });
    $('#conditions a.duplicateclause').live("click", function () {
        var qid = $(this).closest("tr").attr("qid");
        $.post('/Query/DuplicateCondition/' + qid, {}, function (ret) {
            $("#conditions").html(ret).ready($.AdjustEditCondition);
        });
        return false;
    });
    $('#conditions a.insgroupabove').live("click", function () {
        var qid = $(this).closest("tr").attr("qid");
        $.post('/Query/InsGroupAbove/' + qid, {}, function (ret) {
            $("#conditions").html(ret).ready($.AdjustEditCondition);
        });
        return false;
    });
    $('#conditions a.movetoprevgroup').live("click", function () {
        var qid = $(this).closest("tr").attr("qid");
        $.post('/Query/MoveToPreviousGroup/' + qid, {}, function (ret) {
            $("#conditions").html(ret).ready($.AdjustEditCondition);
        });
        return false;
    });
    $('#SaveCondition').live("click", function () {
        var q = $('#editForm').serialize();
        $.post('/Query/SaveCondition/', q, function (ret) {
            $("#conditions").html(ret).ready(function () {
                $.HideEditCondition();
                $("#Run").click();
            });
        });
        return false;
    });
    $('#CancelChange').live("click", function () {
        $.HideEditCondition();
        $.post('/Query/Reload/', null, function (ret) {
            $("#conditions").html(ret);
        });
        return false;
    });
    $('#DeleteCondition').live("click", function () {
        bootbox.confirm("Are you sure you want to delete?", function (result) {
            if (result === true) {
                var qid = $("#SelectedId").val();
                $.post('/Query/RemoveCondition/' + qid, null, function (ret) {
                    $.HideEditCondition();
                    $("#conditions").html(ret);
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
    $(".bt").button();

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
        $EditCondition($("#NewSearchId").val());
        $('#QueryConditionSelect').modal("show");
    }
    else if ($("#AutoRun").val() === "true")
        $("#Run").click();

    $("#SelectCondition").live("click", function (ev) {
        ev.preventDefault();
        $('#QueryConditionSelect').modal("show");
        return false;
    });
    $('div.FieldLink a').click(function (ev) {
        ev.preventDefault();
        var qid = $("#SelectedId").val();
        $.post('/Query/SelectCondition/' + qid, { conditionName: ev.target.id }, function (ret) {
            $('#QueryConditionSelect').modal("hide");
            $("#conditions").html(ret).ready($.AdjustEditCondition);
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
    $("#Run").click();
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
            $('#Results').html(ret);
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