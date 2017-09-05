$(function () {

    $('body').on('click', 'a.remove', function (ev) {
        ev.preventDefault();
        var url = this.href;
        if ($("#edit-contact").length > 0) {
            swal("Required!", "Update contact first.", "error");
            return false;
        }
        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, remove them!",
            closeOnConfirm: false
        },
        function () {
            $.post(url, {}, function (ret) {
                if (ret && ret.error)
                    swal("Error!", ret.error, "error");
                else {
                    swal({
                        title: "Removed!",
                        type: "success"
                    },
                    function () {
                        window.location.reload(true);
                    });
                }
            });
        });
        return false;
    });

    $('body').on('click', '.addtask', function (ev) {
        ev.preventDefault();
        if ($("#edit-contact").length > 0) {
            swal("Required!", "Update contact first.", "error");
            return false;
        }
        var f = $(this).closest("form");
        var url = this.href;
        
        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-success",
            confirmButtonText: "Yes, add task!",
            closeOnConfirm: false
        },
        function () {
            f.attr("action", url);
            f.submit();
        });

        return false;
    });

    $('body').on('click', 'a.link', function (ev) {
        ev.preventDefault();
        if ($("#edit-contact").length > 0) {
            swal("Required!", "Update contact first.", "error");
            return false;
        }
        window.location = this.href;
        return false;
    });

    $('body').on('click', '#newteamcontact', function (ev) {
        ev.preventDefault();
        if ($("#edit-contact").length > 0) {
            swal("Required!", "Update contact first.", "error");
            return false;
        }
        var url = this.href;
        var f = $(this).closest("form");
        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-success",
            confirmButtonText: "Yes, add contact!",
            closeOnConfirm: false
        },
        function () {
            f.attr("action", url);
            f.submit();
        });
        return false;
    });

    $.InitFunctions.Editable = function () {
        $.InitFunctions.ExtraEditable();
    };

});

function AddSelected(ret) {
    switch (ret.from) {
        case 'Contactor':
            $("#contactors").load('/Contact2/Contactors/' + ret.cid, {});
            break;
        case 'Contactee':
            $("#contactees").load('/Contact2/Contactees/' + ret.cid, {});
            break;
    }
}
