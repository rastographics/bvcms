$(function () {

    $('#addTask').click(function(ev) {
        ev.preventDefault();
        $('#new-task-modal').modal('show');
    });

    $('#new-task-modal').on('shown.bs.modal', function () {
        $("#description").val('').focus();
    });

    $('#newTaskSubmit').click(function (ev) {
        ev.preventDefault();
        var desc = $("#description").val();
        if (!desc || desc.length == 0) {
            $('#new-task-modal').modal('hide');
            return false;
        }

        var qs = "description=" + desc;
        $.post('/Task/AddTask/', qs, function (ret) {
            document.location.href = "/Task/Edit/" + ret;
            $('#new-task-modal').modal('hide');
        });
    });

    $.gotoPage = function (e, pg) {
        $("#Page").val(pg);
        refreshList();
        return false;
    };

    $.setPageSize = function (e) {
        $('#Page').val(1);
        $("#PageSize").val($(e).val());
        refreshList();
        return false;
    };

    $('#tasks > thead a.sortable').click(function(ev) {
        var newsort = $(this).text();
        var sort = $("#Sort");
        var dir = $("#Direction");
        if ($(sort).val() == newsort && $(dir).val() == 'asc')
            $(dir).val('desc');
        else
            $(dir).val('asc');
        $(sort).val(newsort);
        refreshList();
    });

    $(document).on("change", "input.actionitem", null, checkChanged);

    function checkChanged() {
        if ($("input.actionitem:checked").length > 0) {
            $("button.taskDelete").prop("disabled", false);
            $("button.taskDelegate").prop("disabled", false);
            $("button.taskDelete").html("<i class='fa fa-trash'></i> Delete " + $("input.actionitem:checked").length + " Task(s)");
            $("button.taskDelegate").html("<i class='fa fa-users'></i> Delegate " + $("input.actionitem:checked").length + " Task(s)");
        } else {
            $("button.taskDelete").prop("disabled", true);
            $("button.taskDelegate").prop("disabled", true);
            $("button.taskDelete").html("<i class='fa fa-trash'></i> Delete 0 Task(s)");
            $("button.taskDelegate").html("<i class='fa fa-trash'></i> Delegate 0 Task(s)");
        }
    }

    $('button.taskDelegate').click(function (ev) {
        ev.preventDefault();
        $("#delegateall").click();
    });

    $('button.taskDelete').click(function (ev) {
        ev.preventDefault();

        var v = 'delete';
        var ai = $(".actionitem:checked").getCheckboxVal().join(",");
        var qs = "option=" + v + "&items=" + ai;

        swal({
            title: "Are you sure you want to delete " + $("input.actionitem:checked").length + " task(s)?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete task(s)!",
            closeOnConfirm: false
        },
        function () {
            $.post('/Task/Action/', qs, function(ret) {
                $('#tasks > tbody').html(ret).ready(function () {
                    swal({
                        title: "Deleted!",
                        type: "success"
                    });
                    checkChanged();
                });
            });
        });
    });

    var refreshList = function(ev) {
        if (ev)
            ev.preventDefault();
        var q = $('#form').serialize();
        $.navigate("/Task/List", q);
    };

    $("#OwnerOnly").click(refreshList);
    $("#StatusId").change(refreshList);
    checkChanged();
});

function AddSelected(ret) {
    if (ret.how === "addselected2")
        return AddSelected2(ret);
    ActOnPerson(ret.url, ret.pid);
}
function AddSelected2(ret) {
    var ai = $(".actionitem:checked").getCheckboxVal().join(",");
    var qs = "items=" + ai;
    $.post('/Task/DelegateAll/' + ret.pid, qs, function (ret) {
        $('#tasks > tbody').html(ret);
    });
}

function ActOnPerson(action, peopleid) {
    var taskid = $('#TaskId').val();
    $.post(action + taskid + "?peopleid=" + peopleid, null, function (ret) {
        $('#r' + taskid).html(ret);
    });
}