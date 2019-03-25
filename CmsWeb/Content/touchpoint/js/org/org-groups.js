$(function () {
    $('#org-group-tabs').tabdrop();

    $.fn.editableform.buttons = '<button type="submit" class="btn btn-primary btn-sm editable-submit">' +
                                    '<i class="fa fa-fw fa-check"></i>' +
                                '</button>' +
                                '<button type="button" class="btn btn-default btn-sm editable-cancel">' +
                                    '<i class="fa fa-fw fa-times"></i>' +
                                '</button>';

    function initializePopovers() {
        $('[data-toggle="popover"]').popover({ html: true });
        $('[data-toggle="popover"]').click(function (ev) {
            ev.preventDefault();
        });
    }

    $.fmtTable = function () {
        initializePopovers();

        $(".clickEdit").editable({
            mode: 'popup',
            type: 'text',
            url: "/OrgGroups/UpdateScore",
            success: updateScore,
            params: function (params) {
                var data = {};
                data['id'] = params.pk;
                data['value'] = params.value;
                return data;
            }
        });

        checkChanged();
    };

    $.fmtTable();

    $.loadTable = function() {
        $.block();
        $.getTable($('#groupsform'));
        $.unblock();
    };

    $('body').on("click", '#filter', function (ev) {
        ev.preventDefault();
        $.loadTable();
    });
    $('body').on("click", '#clear', function (ev) {
        location.reload();
    });

    $('body').on("click", 'a.sortable', function (ev) {
        ev.preventDefault();
        $('#sort').val($(this).text());
        $.loadTable();
    });

    $("#SelectAll").click(function () {
        if ($(this).prop("checked")) {
            $("#members input[name='list']").prop('checked', true);
        } else {
            $("#members input[name='list']").prop('checked', false);
        }
    });

    $("#ingroup, #notgroup").keypress(function (ev) {
        if (ev.keyCode == '13') {
            ev.preventDefault();
            $.loadTable();
        }
    });

    $.getTable = function (f) {
        var q = f.serialize();
        $.post("/OrgGroups/Filter", q, function (ret) {
            $('#members > tbody').html(ret).ready($.fmtTable);
            $("#totalcount").text($("#rowcount").val());
        });
        return false;
    }

    $("body").on("click", '#SelectAll', function () {
        $("input[name='list']").attr('checked', $(this).attr('checked'));
        checkChanged();
    });

    $.performAction = function (action) {
        if ($('#groupid').val() <= 0) {
            swal("Error!", 'Must select a target group first.', "error");
            return false;
        }
        $.block();
        var q = $('#groupsform').serialize();
        $.post(action, q, function (ret) {
            $("#members tbody").html(ret).ready($.fmtTable);
            $.unblock();
        });
        return false;
    };

    $('body').on('click', '#AssignSelectedToTargetGroup', function (ev) {
        ev.preventDefault();
        $.performAction("/OrgGroups/AssignSelectedToTargetGroup");
    });

    $('body').on('click', '#RemoveSelectedFromTargetGroup', function (ev) {
        ev.preventDefault();
        $.performAction("/OrgGroups/RemoveSelectedFromTargetGroup");
    });
    
     $('body').on('click', '#MakeSelectedLeaderOfTargetGroup', function (ev) {
        ev.preventDefault();
        $.performAction("/OrgGroups/MakeLeaderOfTargetGroup");
    });
    
    $('body').on('click', '#RemoveSelectedLeaderOfTargetGroup', function (ev) {
        ev.preventDefault();
        $.performAction("/OrgGroups/RemoveAsLeaderOfTargetGroup");
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

    function updateScore(response, newValue) {
        var checkID = $(this).attr("peopleID");
        $("#" + checkID).attr("score", newValue);
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
        e.preventDefault();
        var orgid = $(this).attr("orgid");

        swal({
            title: "Are you sure you want to create all teams?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-warning",
            confirmButtonText: "Yes, continue!",
            closeOnConfirm: true
        },
        function () {
            $.block();
            $.ajax({ type: "POST", url: "/OrgGroups/CreateTeams", data: { id: orgid }, success: function () { location.reload(); } });
        });
    });

    $("body").on("click", "#scoreUploadButton", function (e) {
        e.preventDefault();
        $('#scoreUploadDialog').modal('show');
    });

    $('#scoreUploadDialog').on('shown.bs.modal', function(e) {
        $("#scoreUploadData").val("");
    });

    $("body").on("click", "#scoreUploadSubmit", function (e) {
        var post = $("#scoreUploadForm").serialize();

        $.ajax({ type: "POST", url: "/OrgGroups/UploadScores", data: post, success: $.loadTable });
        $('#scoreUploadDialog').modal('hide');
    });

    var grouplastChecked = null;
    $(document).on("click", "input[name='groups']", function (e) {
        if (e.shiftKey && grouplastChecked != null) {
            var start = $("input[name='groups']").index(this);
            var end = $("input[name='groups']").index(grouplastChecked);
            $("input[name='groups']").slice(Math.min(start, end), Math.max(start, end) + 1).attr('checked', grouplastChecked.checked);
        }
        grouplastChecked = this;
        var checked = $("input[name='groups']:checked");

        if (checked.length > 0) {
            $("#deleteGroups").attr("disabled", false).val("Delete " + checked.length + " Groups");
        }
        else {
            $("#deleteGroups").attr("disabled", true).val("Delete 0 Groups");
        }

    });

    $(document).on("click", "#deleteGroups", function (e) {
        e.preventDefault();
        var f = $(this).closest('form');
        var q = f.serialize();
        var url = f.attr('action');
        swal({
            title: "Are you sure you want to delete these groups?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete groups!",
            closeOnConfirm: true
        },
        function () {
            $.block();
            $.post(url, q, function (ret) {
                $.unblock();
                if (ret.substring(0, 5) != "error") {
                    $('#groupForm').html(ret);
                }
            });
        });
    });

    $("body").on('click', 'a.create-group', function (ev) {
        ev.preventDefault();
        $('#new-group-modal input').not('[type=hidden],[type=button],[type=submit]').val('');
        $('#new-group-modal').modal('show');
    });

    $('#new-group-modal').on('shown.bs.modal', function (e) {
        $("input[name=GroupName]", this).val("").focus();
    });

    $("body").on('click', 'a.toggle-checkin', function (ev) {
        ev.preventDefault();
        var url = $(this).attr("href");

        $.post(url, null, function (ret) {
            if (ret.substring(0, 5) != "error") {
                $('#groupForm').html(ret);
            }
        });
    });

    $("body").on('click', 'a.edit-group', function (ev) {
        ev.preventDefault();
        var $this = $(this);
        var groupId = $this.attr('groupId');
        var groupName = $this.attr('groupName');
        var allowCheckin = $this.closest('tr').find('.toggle-checkin').is('.btn-success');
        var checkinOpenDefault = $this.attr('checkinOpenDefault');
        var checkinCapacityDefault = $this.attr('checkinCapacityDefault');
        var scheduleId = $this.attr('scheduleId');
        $modal = $('#edit-group-modal');
        $modal.find('input[name=groupid]').val(groupId);
        $modal.find('input[name=GroupName]').val(groupName);
        //$modal.find('input[name=AllowCheckin]').find()
        $modal.find('select[name=AllowCheckin]').val('' + allowCheckin);
        $modal.find('input[name=CheckInCapacityDefault]').val(checkinCapacityDefault);
        $modal.find('select[name=CheckInOpenDefault]').val('' + !!checkinOpenDefault);
        $modal.find('select[name=ScheduleId]').val(scheduleId || '');
        $modal.modal('show');
    });
    
    $('#edit-group-modal').on('shown.bs.modal', function (e) {
        $(this).find('input[name=GroupName]').focus();
    });
    
    $("#edit-group-modal form, #new-group-modal form").submit(function (ev) {
        ev.preventDefault();
        var f = $(this);
        var q = f.serialize();
        var url = f.attr('action');
        $.post(url, q, function (ret) {
            if (ret.substring(0, 5) != "error") {
                $('#groupForm').html(ret);
            }
        });
        f.closest('.modal').modal('hide');
    });

    $("body").on('click', 'a.delete-group', function (ev) {
        ev.preventDefault();
        var groupId = $(this).attr('groupId');
        $('input.delete-group-id').val(groupId);
        var f = $(this).closest('form');
        var q = f.serialize();
        var url = $(this).attr('href');
        swal({
            title: "Are you sure you want to delete this group?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete it!",
            closeOnConfirm: false
        },
        function () {
            $.post(url, q, function (ret) {
                if (ret.substring(0, 5) != "error") {
                    $('#groupForm').html(ret).ready(function() {
                        swal({
                            title: 'Deleted!',
                            type: 'success'
                        });
                    });
                    
                }
            });
        });
    });

    $('#group-members-tab').on('show.bs.tab', function (e) {
        $.block();
        document.location.reload();
    });
});
