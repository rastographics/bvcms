$(function () {
    $.fmtTable = function() {
        $("table.grid2 td.tip").tooltip({ showBody: "|" });
        $(".bt").button();

        $(".clickEdit").editable("/OrgGroups/UpdateScore", {
            indicator: "<img src='/images/loading.gif'>",
            width: 40,
            height: 22,
            tooltip: "Click to edit...",
            select: true,
            callback: updateScore
        });

        checkChanged();
    };

    $.fmtTable();

    $(".helptip").tooltip({ showBody: "|" });

    $.loadTable = function() {
        $.block();
        $.getTable($('#groupsform'));
        $.unblock();
    };

    $('body').on("click", '#filter', function (ev) {
        ev.preventDefault();
        $.loadTable();
    });

    $('body').on("click", 'a.sortable', function (ev) {
        ev.preventDefault();
        $('#sort').val($(this).text());
        $.loadTable();
    });

    $("#groupsform").delegate("#memtype", "change", $.loadTable);

    $("#SelectAll").click(function () {
        if ($(this).attr("checked"))
            $("table.grid2 input[name='list']").attr('checked', true);
        else
            $("table.grid2 input[name='list']").removeAttr('checked');
    });

    $("#ingroup, #notgroup").keypress(function (ev) {
        if (ev.keyCode == '13') {
            ev.preventDefault();
            $.loadTable();
        }
    });
    //$("#groupidDD").multiselect();
    $("#groupsform").delegate("#groupid", "change", $.loadTable);
    $.getTable = function (f) {
        var q = f.serialize();
        $.post("/OrgGroups/Filter", q, function (ret) {
            $('table.grid > tbody').html(ret).ready($.fmtTable);
        });
        return false;
    }
    $(".datepicker").jqdatepicker();

    $("body").on("click", '#SelectAll', function () {
        $("input[name='list']").attr('checked', $(this).attr('checked'));
        checkChanged();
    });
    $("body").on('click', 'a.display', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        $.post(this.href, q, function (ret) {
            $(f).html(ret).ready(function () {
                $.fmtTable();
                return false;
            });
        });
        return false;
    });
    $("body").on('click', 'a.groupmanager', function (ev) {
        ev.preventDefault();
        if (confirm("are you sure?")) {
            var f = $(this).closest('form');
            var q = f.serialize();
            $.block();
            $.post($(this).attr('href'), q, function (ret) {
                if (ret.substring(0, 5) != "error") {
                    f.html(ret).ready(function () {
                        if ($('#newgid').val())
                            $('#groupid').val($('#newgid').val());
                        $('#GroupName').val('');
                        $.fmtTable();
                    });
                }
                $.unblock();
            });
        }
    });
    $("form").submit(function (ev) {
        ev.preventDefault();
        return false;
    });
    $.performAction = function (action) {
        if ($('#groupid').val() <= 0) {
            $.growlUI("error", 'select target group first');
            return false;
        }
        $.block();
        var q = $('form').serialize();
        $.post(action, q, function (ret) {
            $("table.grid2 > tbody").html(ret).ready($.fmtTable);
            $.unblock();
        });
        return false;
    };
    $('body').on('click', '#AssignSelectedToTargetGroup', function (ev) {
        $.performAction("/OrgGroups/AssignSelectedToTargetGroup");
    });
    $('body').on('click', '#RemoveSelectedFromTargetGroup', function (ev) {
        $.performAction("/OrgGroups/RemoveSelectedFromTargetGroup");
    });
    var lastChecked = null;
    $("body").on("click", "input[name = 'list']", function (e) {
        if (!lastChecked) {
            lastChecked = this;
            checkChanged();
            return;
        }
        if (e.shiftKey) {
            var start = $("input[name = 'list']").index(this);
            var end = $("input[name = 'list']").index(lastChecked);
            $("input[name = 'list']").slice(Math.min(start, end), Math.max(start, end) + 1).attr('checked', lastChecked.checked);
        }
        lastChecked = this;
        checkChanged();
    });

    var scoreTrackerShowing = false;

    function updateScore(value, settings) {
        var checkID = $(this).attr("peopleID");
        $("#" + checkID).attr("score", value);
        checkChanged();
    }

    if ($("#scoreTracker") !== undefined) {
        $("#scoreTracker").draggable({ axis: "x" });
    }

    function checkChanged() {
        // Check to see if tracker is enabled, if not we don't need this other stuff
        if ($("#scoreTracker") === undefined) return;

        var checkedList = $("input[name='list']:checked");
        if (checkedList.length > 0) {

            if (checkedList.length == 2) $("#swapPlayers").show();
            else $("#swapPlayers").hide();

            var totalScore = 0;
            for (var iX = 0; iX < checkedList.length; iX++) totalScore += Number($(checkedList[iX]).attr("score"));

            $("#playerCount").html(checkedList.length);
            $("#lastScore").html($(lastChecked).attr("score"));
            $("#avgScore").html(Number(totalScore / checkedList.length).toFixed(1));
            $("#totalScore").html(totalScore);


            if (!scoreTrackerShowing) {
                scoreTrackerShowing = true;
                $("#scoreTracker").slideDown(200);
            }
        } else {
            scoreTrackerShowing = false;
            $("#scoreTracker").slideUp(200);
        }
    }

    $("body").on("click", "#swapPlayers", function (e) {
        $(this).hide();
        var checkedList = $("input[name='list']:checked");
        if (checkedList.length == 2) {
            var swapFirst = $(checkedList[0]).attr("swap");
            var swapSecond = $(checkedList[1]).attr("swap");

            $.ajax({ type: "POST", url: "/OrgGroups/SwapPlayers", data: { pOne: swapFirst, pTwo: swapSecond }, success: $.loadTable });
        }
    });

    $("body").on("click", "#createTeams", function (e) {
        var acceptDelete = confirm("Are you sure you want to create all teams?");
        if (!acceptDelete) return;

        var orgid = $(this).attr("orgid");
        $.block();
        $.ajax({ type: "POST", url: "/OrgGroups/CreateTeams", data: { id: orgid }, success: function () { location.reload(); } });
    });

    $("body").on("click", "#scoreUploadButton", function (e) {
        $("#scoreUploadDialog").dialog({ width: "auto", title: "Upload Player Scores", modal: true, close: function () { $("#scoreUploadData").val(""); } });
    });

    $("body").on("click", "#scoreUploadSubmit", function (e) {
        var post = $("#scoreUploadForm").serialize();

        $.ajax({ type: "POST", url: "/OrgGroups/UploadScores", data: post, success: $.loadTable });
        $("#scoreUploadDialog").dialog("close");
    });
    
    $("body").on("click", "#scoreUploadDismiss", function (e) {
        $("#scoreUploadDialog").dialog("close");
    });
});