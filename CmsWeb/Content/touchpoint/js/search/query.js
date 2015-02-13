$(function () {
    
    var liedit;
    var $backdrop = $('<div class="modal-backdrop fade in hide" style="height: 100%; background-color: #fff; z-index: 1031;"></div>').appendTo('body');

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
    
    $('#conditions').on("click", 'a.edit-popover', function () {
        liedit = $(this).closest("li.condition");
        $EditCondition();
        return false;
    });

    var $EditCondition = function (option) {
        var qid = liedit.data("qid");
        $("#editcondition").attr("originalheight", liedit.height() + 9);
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
            "width": "auto",
            "min-width": wid,
            "z-index": 1032
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

    $('body').on('click', '#CancelChange', function () {
        $.HideEditCondition();
        return false;
    });

    $('body').on('mouseenter', '#conditions header', function () {
        var li = $(this).closest("li");
        li.addClass("borderleftred");
    });

    $('body').on('mouseleave', '#conditions header', function () {
        var li = $(this).closest("li");
        li.removeClass("borderleftred");
    });

    $('body').on('click', '#SaveCondition', function () {
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

    $('body').on('keydown', '#editForm input', function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            $('#SaveCondition').click();
            return false;
        }
    });

    $('body').on('change', '#conditions select.changegroup', function () {
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

    $('body').on('click', '#conditions a.addnewclause', function () {
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

    $('body').on('click', '#conditions a.addnewgroup', function () {
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

    $('body').on('click', '#conditions a.cutcondition', function () {
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

    $('body').on('click', '#conditions a.copycondition', function () {
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        $.postQuery('Copy', qid);
        $(this).parent().parent().prev().dropdown("toggle");
        $("li.pastecondition").show();
        return false;
    });

    $('body').on('click', '#conditions a.pastecondition', function () {
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

    $('body').on('click', '#conditions a.insgroupabove', function () {
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

    $('body').on('click', '#conditions a.maketopgroup', function () {
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

    $('body').on('click', '#conditions a.delete', function () {
        liedit = $(this).closest("li.condition");
        var qid = liedit.data("qid");
        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete it!",
            closeOnConfirm: true
        },
        function () {
            $.postQuery('RemoveCondition', qid, function (ret) {
                $("#conditions").html(ret);
                if ($("#AutoRun").prop("checked"))
                    RefreshList();
                else
                    FadeList();
            });
        });
        return false;
    });

    $('body').on('change', '#Comparison', function (ev) {
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

    $('body').on('change', '#Program', function (ev) {
        $.postQuery('Divisions/' + $(this).val(), function (ret) {
            $.replaceSelect('#Division', ret);
            $.replaceSelect('#Organization', "<select id='Organization' name='Organization' style='display:none'><option value='0'>(not specified)</option></select>");
        });
    });

    $('body').on('change', '#Division', function () {
        $.postQuery('Organizations/' + $(this).val(), function (ret) {
            $.replaceSelect("#Organization", ret);
        });
    });

    $('body').on('click', '#Run', function (ev) {
        RefreshList();
        return false;
    });

    $('#Export').click(function (ev) {
        window.location = "/Query/Export/" + $("#QueryId").val();
    });

    $('body').on('click', '#SelectCondition', function (ev) {
        ev.preventDefault();
        $('#QueryConditionSelect').modal("show");
        return false;
    });

    $('#QueryConditionSelect').on('shown.bs.modal', function () {
        $("#condition-tabs").tabdrop();
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

    if ($("#NewSearchId").val()) {
        liedit = $("li[data-qid='" + $("#NewSearchId").val() + "']");
        $EditCondition({ isnew: true });
    }
    else if ($("#AutoRun").prop("checked"))
        RefreshList();

    $('#AutoRun').change(function () {
        $.post("/Query/SetAutoRun", { setting: $(this).prop("checked") });
    });

    $('body').on('click', 'a.taguntag', function (ev) {
        $.block();
        $.post('/Query/ToggleTag/' + $(this).attr('value'), function (ret) {
            if (ret.error)
                alert(ret.error);
            else {
                $(ev.target).removeClass('btn-default').removeClass('btn-success');
                $(ev.target).addClass(ret.HasTag ? "btn-default" : "btn-success");
                $(ev.target).html(ret.HasTag ? "<i class='fa fa-tag'></i> Remove" : "<i class='fa fa-tag'></i> Add");
            }
            $.unblock();
        });
        return false;
    });

    $.gotoPage = function (e, pg) {
        $("#Page").val(pg);
        RefreshList();
        return false;
    };

    $.setPageSize = function (e) {
        $('#Page').val(1);
        $("#Size").val($(e).val());
        RefreshList();
        return false;
    };

});

function FadeList() {
    $("#results").addClass("faded-results");
}

function RefreshList() {
    var qs = $("#query-form").serialize();
    $.block();
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
            $("li.hideAlt").hide();
            $('#resultsTable thead a.sortable').click(function (ev) {
                var newsort = $(this).text();
                var oldsort = $("#Sort").val();
                $("#Sort").val(newsort);
                var dir = $("#Dir").val();
                if (oldsort == newsort && dir == 'asc')
                    $("#Dir").val('desc');
                else
                    $("#Dir").val('asc');
                RefreshList();
                return false;
            });
            $.unblock();
        },
        error: function (request, status, err) {
            alert(err);
            $.unblock();
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