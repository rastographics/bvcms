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
            confirmButtonText: "Yes, delete this pledge!",
            closeOnConfirm: false
        },
            function () {
                deletePledge(id);
            }
        );
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
        error: function (xhr) {
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
                    title: "Pledge Deleted!",
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
        error: function (xhr) {
            $.unblock();
            swal("Error", "", "error");
        }
    });
    return false;
}


