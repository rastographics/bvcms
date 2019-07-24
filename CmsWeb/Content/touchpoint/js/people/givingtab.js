$(document).ready(function () {
    $(".numeric").keypress(function (event) {
        // Backspace, tab, enter, end, home, left, right
        // We don't support the del key in Opera because del == . == 46.
        var controlKeys = [8, 9, 13, 35, 36, 37, 39];
        // IE doesn't support indexOf
        var isControlKey = controlKeys.join(",").match(new RegExp(event.which));
        // Some browsers just don't raise events for control keys. Easy.
        // e.g. Safari backspace.
        if (!event.which || // Control keys in most browsers. e.g. Firefox tab is 0
            (49 <= event.which && event.which <= 57) || // Always 1 through 9
            (48 == event.which && $(this).attr("value")) || // No 0 first digit
            (48 == event.which && ($(this).val() > 0)) || // allows 0.
            (46 == event.which && !($(this).val().includes("."))) || //allowe .
            isControlKey) { // Opera assigns values for control keys.
            return;
        } else {
            event.preventDefault();
        }
    });

    $("body").on("click", 'button.editpledge', function (ev) {
        ev.preventDefault();
        var a = $(this);
        $("#editpledgeid").val(a.attr("pledgeid"));
        $("#currentpledge").val(a.attr("amount"));
        $('#editPledge-modal').modal();
        return false;
    });

    $("body").on("click", 'button.deletepledge', function (ev) {
        ev.preventDefault();
        var id = $(this).attr("pledgeid");
        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete this pledge.",
            closeOnConfirm: false
        },
            function () {
                deletePledge(id);
            }
        );
        return false;
    });

    $("body").on("click", 'button.mergepledge', function (ev) {
        ev.preventDefault();
        var id = $(this).attr("pledgeid");
        if ($("#idtomerge").val() == '') {
            $("#idtomerge").val(id);
            blockPledges();
            swal("Merge Pledge", "Please select the pledge you want to merge into", "info");
        } else {
            var idToMerge = $("#idtomerge").val();
            askMergePledges(idToMerge, id);
        }
        return false;
    });

    $("body").on("click", 'button#cancelmergepledge', function (ev) {
        ev.preventDefault();
        $("#idtomerge").val('');
        unblockPledges();
        return false;
    });

    $("#puteditpledge").click(function (ev) {
        ev.preventDefault();
        $('#adjust-modal').modal('hide');
        $.block();
        var id = $('#editpledgeid').val();
        var amt = parseFloat($("#currentpledge").val());
        if (isNaN(amt)) {
            $.unblock();
            return false;
        }
        editPledge(id, amt);
    });
});

function editPledge(id, amt) {
    $.ajax({
        url: 'EditPledge',
        dataType: "json",
        type: "PUT",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ contributionId: id, amt: amt }),
        async: true,
        processData: false,
        cache: false,
        success: function (data) {
            if (data == 'OK') {
                location.reload();
            } else {
                $.unblock();
                swal("Error", data, "error");
            }
        },
        error: function () {
            $.unblock();
            swal("Error", "", "error");
        }
    });
    return false;
}

function deletePledge(id) {
    $.ajax({
        url: 'DeletePledge',
        dataType: "json",
        type: "DELETE",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ contributionId: id }),
        async: true,
        processData: false,
        cache: false,
        success: function (data) {
            if (data == 'OK') {
                swal({
                    title: "Pledge deleted.",
                    type: "success"
                },
                    function () {
                        location.reload();
                    });
            } else {
                $.unblock();
                swal("Error", data, "error");
            }
        },
        error: function () {
            $.unblock();
            swal("Error", "", "error");
        }
    });
    return false;
}

function blockPledges() {
    var idToMerge = $("#idtomerge").val();
    if (idToMerge != '') {
        $('button.editpledge').attr('disabled', 'disabled');
        $('button.deletepledge').attr('disabled', 'disabled');
        $('a.fulfillpledge').attr('disabled', 'disabled');
        $('button.mergepledge[pledgeid=' + idToMerge + ']').attr('disabled', 'disabled');

        var cancelbutton = $('button.deletepledge[pledgeid=' + idToMerge + ']');
        if (cancelbutton.length) {
            cancelbutton.removeClass('deletepledge');
            cancelbutton.attr('id', 'cancelmergepledge');
            cancelbutton.prop('disabled', false);
            cancelbutton.empty();
            cancelbutton.append('<i class=' +'"fa fa-times-circle"'+'></i> Cancel');
        }
    }
    return false;
}

function unblockPledges() {
    var idToMerge = $("#idtomerge").val();
    if (idToMerge == '') {
        var cancelbutton = $('button#cancelmergepledge');
        if (cancelbutton.length) {
            cancelbutton.removeAttr('id');
            cancelbutton.addClass('deletepledge');
            cancelbutton.prop('disabled', false);
            cancelbutton.empty();
            cancelbutton.append('<i class=' + '"fa fa-times-circle"' + '></i> Delete');
        }
        $('button.editpledge').prop('disabled', false);
        $('button.deletepledge').prop('disabled', false);
        $('button.mergepledge').prop('disabled', false);
        $('a.fulfillpledge').removeAttr('disabled');
    }
    return false;
}

function askMergePledges(idToMerge, id) {
    swal({
        title: "Are you sure?",
        text: "Do you want to merge these pledges?",
        type: "info",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Yes, merge it.",
        closeOnConfirm: false
    },
        function () {
            mergePledges(idToMerge, id);
        }
    );
}

function mergePledges(idToMerge, id) {
    $.ajax({
        url: 'MergePledge',
        dataType: "json",
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ toMerge: idToMerge, id:id}),
        async: true,
        processData: false,
        cache: false,
        success: function (data) {
            if (data == 'OK') {
                swal({
                    title: "Pledges merged.",
                    type: "success"
                },
                    function () {
                        location.reload();
                    });
            } else {
                $.unblock();
                swal("Error", data, "error");
            }
        },
        error: function () {
            $.unblock();
            swal("Error", "", "error");
        }
    });
}
