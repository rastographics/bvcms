$('document').ready(function () {

    var addRangeForm = $('#createRange');
    var sendRangeBtn = addRangeForm.find('input[type="submit"]');
    var arStartWithInput = addRangeForm.find('input#startwith');
    var arEndWithInput = addRangeForm.find('input#endwith');
    var residentCodeDD = addRangeForm.find('select#marginaldd');

    $('.zipcode').mask('00000');
    sendRangeBtn.prop('disabled', true);

    var validateRangeForm = function () {
        var valid = true;
        if (arStartWithInput.val() == '' || arEndWithInput.val() == '') {
            valid = false;
        }
        if (parseInt(arStartWithInput.val()) > parseInt(arEndWithInput.val())) {
            valid = false;
        }
        if (residentCodeDD.val() == null) {
            valid = false;
        }
        if (!valid) {
            sendRangeBtn.prop('disabled', true);
        } else {
            sendRangeBtn.prop('disabled', false);
        }
    }

    arStartWithInput.keyup(function () {
        validateRangeForm();        
    });
    arEndWithInput.keyup(function () {
        validateRangeForm();
    });
    residentCodeDD.on('change', function () {
        validateRangeForm();
    });

    var delRangeForm = $('#deleteRange');
    var delRangeBtn = delRangeForm.find('input[type="submit"]');
    var drStartWithInput = delRangeForm.find('input#startwith');
    var drEndWithInput = delRangeForm.find('input#endwith');

    delRangeBtn.prop('disabled', true);

    var validateDeleteRange = function () {
        var valid = true;
        if (drStartWithInput.val() == '' || drEndWithInput.val() == '') {
            valid = false;
        }
        if (parseInt(drStartWithInput.val()) > parseInt(drEndWithInput.val())) {
            valid = false;
        }        
        if (!valid) {
            delRangeBtn.prop('disabled', true);
        } else {
            delRangeBtn.prop('disabled', false);
        }
    }

    drStartWithInput.keyup(function () {
        validateDeleteRange();
    });
    drEndWithInput.keyup(function () {
        validateDeleteRange();
    });

    delRangeForm.submit(function (e) {
        e.preventDefault();
        var startWithValue = drStartWithInput.val();
        var endWithValue = drEndWithInput.val();
        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete it!",
            closeOnConfirm: false
        },
            function () {
                $.post("/MetroZips/DeleteRange/", { startwith: startWithValue, endwith: endWithValue }, function (ret) {
                    if (ret && ret.error)
                        swal("Error!", ret.error, "error");
                    else {
                        swal({
                            title: "Deleted!",
                            type: "success"
                        },
                            function () {
                                window.location = "/MetroZips/";
                            });
                    }
                });
            });
    });
});
