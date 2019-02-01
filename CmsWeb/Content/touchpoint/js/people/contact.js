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

function WireUpExtraValues(cid, locations) {
    var notSpecified = '(not specified)';

    if ($.isEmptyObject(locations))
        return;

    function sameStr(left, right) {
        if (!left) left = notSpecified;
        return left.localeCompare(right, undefined, { sensitivity: 'base' }) === 0;
    }

    function valueMatches(left, right) {
        return typeof left === 'undefined' ||
            left === null ||
            sameStr(left, right) ||
            sameStr(left, right.replace(/[^a-zA-Z0-9]/g, ''));
    }

    $('.code-dropdown select').on('change', function () {
        var data = {
            ministry: $('#Ministry_Value option:selected').text(),
            contactType: $('#ContactType_Value option:selected').text(),
            contactReason: $('#ContactReason_Value option:selected').text()
        };

        var match = _.find(locations, function(x) {
            return (valueMatches(x.Ministry, data.ministry)) &&
                (valueMatches(x.ContactType, data.contactType)) &&
                (valueMatches(x.ContactReason, data.contactReason));
        });

        if (typeof match === 'undefined') {
            $('#contact-extra-values').hide();
            return;
        } else {
            $('#contact-extra-values').show();
        }

        if (data.ministry === notSpecified)
            data.ministry = null;
        if (data.contactType === notSpecified)
            data.contactType = null;
        if (data.contactReason === notSpecified)
            data.contactReason = null;

        $.ajax({
            type: 'POST',
            url: '/Contact2/ExtraValues/' + cid,
            data: data,
            beforeSend: function () {
                $.block();
            },
            success: function (ret, status) {
                $.unblock();
                $('#contact-extra-values').empty();
                $('#contact-extra-values').append(ret);
                $.InitFunctions.ExtraEditable();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                $.unblock();
                swal({ title: "Error!", text: thrownError, type: "error", html: true });
            }
        });
    });
}
