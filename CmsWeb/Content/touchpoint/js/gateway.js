$(".addnew").click(function () {
    document.getElementsByClassName("noprocess")[0].hidden = true;
    document.getElementsByClassName("noprocess")[1].hidden = false;
    checkGatewayAvailability(document.getElementById('gatewaylist').value);
});

$(".more-configs").click(function () {
    document.getElementsByClassName("noprocess")[0].hidden = true;
    document.getElementsByClassName("noprocess")[1].hidden = false;
    checkGatewayAvailability(document.getElementById('gatewaylist').value);
});

$('.cancel').click(function () {
    document.getElementsByClassName("noprocess")[0].hidden = false;
    document.getElementsByClassName("noprocess")[1].hidden = true;
    document.getElementById("AddGatewaySettings").reset();
});

$('.applyall').click(function (e) {
    if (this.checked)
        $(this).attr('value', 'true');
    else
        $(this).attr('value', 'false');
});

function editprocess(GatewaySettingId, proccessId, gatewayId, processName) {
    document.getElementById('GatewaySettingId').value = GatewaySettingId;
    document.getElementById('secproccessId').value = proccessId;
    document.getElementById('secgatewaylist').value = gatewayId;
    document.getElementById('processName').innerHTML = processName;

    $('#processModal').modal();
}

function deleteprocess(id) {
    swal({
        title: "Are you sure?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Yes, delete it!",
        closeOnConfirm: false
    },
    function () {
        $.ajax("../Gateway/AddGatewaySettings/", {
            type: "POST",
            dataType: "json",
            data: {
                GatewaySettingId: id,
                ProcessId: 0,
                GatewayId: 0,
                Operation: 2
            },
            statusCode: {
                200: function () {
                    console.log('200 Response');
                },
                400: function () {
                    console.log('400 Response');
                }
            },
            success: function (response) {
                console.log(response);
                swal({
                    title: "Ok!",
                    text: "Your process has been deleted",
                    icon: "success"
                });
                window.location.reload();
            },
            error: function (err) {
                console.log(err);
            }
        });
    });
}

$('.saveconfiguration').click(function () {
    var forupdate = $('#gatewayValues .forupdate input');
    var forinsert = $('#gatewayValues .forinsert input');

    try {
        for (let i = 0; i < forupdate.length; i++) {
            $.ajax("../Gateway/AddGatewayDetail/", {
                type: "POST",
                dataType: "json",
                data: {
                    GatewayDetailId: forupdate[i].value,
                    GatewayId: sessionStorage.getItem('CurrentGatewayId'),
                    GatewayDetailName: forupdate[i + 1].value,
                    GatewayDetailValue: forupdate[i + 2].value,
                    IsDefault: false,
                    Operation: 1
                },
                statusCode: {
                    200: function () {
                        console.log('200 Response');
                    },
                    404: function () {
                        console.log('400 Response');
                    }
                },
                success: function (response) {
                    console.log(response);
                },
                error: function (err) {
                    console.log(err);
                }
            });
            i = i + 2;
        }

        for (let i = 0; i < forinsert.length; i++) {
            $.ajax("../Gateway/AddGatewayDetail/", {
                type: "POST",
                dataType: "json",
                data: {
                    GatewayDetailId: null,
                    GatewayId: sessionStorage.getItem('CurrentGatewayId'),
                    GatewayDetailName: forinsert[i].value,
                    GatewayDetailValue: forinsert[i + 1].value,
                    IsDefault: forinsert[i + 2].checked,
                    Operation: 0
                },
                statusCode: {
                    400: function (response) {
                        console.log(response);
                    },
                    404: function (response) {
                        console.log(response);
                    }
                },
                success: function (response) {
                    console.log(response);
                },
                error: function (err) {
                    console.log(err);
                }
            });
            i = i + 2;
        }

        swal({
            title: "Ok!",
            text: "Configuration saved",
            icon: "success"
        });
        GetWatewayConfig();
    }
    catch (err) {
        console.log(err);
        swal({
            title: "Ups!",
            text: "Something went wrong, please try again later",
            icon: "warning"
        });
    }
});

$('.newconfiguration').click(function () {
    appendInputs();
});

$('#gatewaylist').change(function () {
    sessionStorage.setItem('CurrentGatewayId', this.value);
    checkGatewayAvailability(this.value);
});

$('.getconfig').click(function () {
    sessionStorage.setItem('CurrentGatewayId', $('#gatewaylist').val());
    GetWatewayConfig();
});

function checkGatewayAvailability(Id) {
    $.ajax("../Gateway/Get_Gateway_Config/" + Id, {
        type: "GET",
        statusCode: {
            400: function (response) {
                console.log(response);
                swal({
                    title: "Ups!",
                    text: "Something went wrong, please try again later",
                    icon: "warning"
                });
            },
            404: function (response) {
                console.log(response);
                swal({
                    title: "Error!",
                    text: "Something went wrong, we're having difficulty connecting",
                    icon: "error"
                });
            }
        },
        success: function (response) {
            console.log(response);
            if (response.length === 0) {
                swal({
                    title: "Epa!",
                    text: "It seems you need to add some configurations",
                    icon: "info"
                });
                if (document.getElementById('saveproccess') !== null) {
                    document.getElementById('saveproccess').disabled = true;
                    document.getElementById('saveproccess').className = 'btn btn-default';
                }
                
                $('#gatewayModal').modal();
                $('#gatewayValues').html('');
            }
            else {
                document.getElementById('saveproccess').disabled = false;
                document.getElementById('saveproccess').className = 'btn btn-success';
            }
        },
        error: function (err) {
            console.log(err);
        }
    });
}

function deleteDetail(id) {
    if (document.getElementsByClassName('forupdate').length === 1) {
        swal({
            title: "Caution!",
            text: "Your configuration cannot be empty",
            icon: "warning"
        });
    }
    else {
        console.log('Delete From DB...');
        $.ajax("../Gateway/AddGatewayDetail/", {
            type: "POST",
            dataType: "json",
            data: {
                GatewayDetailId: id,
                GatewayId: sessionStorage.getItem('CurrentGatewayId'),
                GatewayDetailName: '',
                GatewayDetailValue: '',
                Operation: 2
            },
            statusCode: {
                400: function (response) {
                    console.log(response);
                },
                404: function (response) {
                    console.log(response);
                }
            },
            success: function (response) {
                console.log(response);
                GetWatewayConfig();
            },
            error: function (err) {
                console.log(err);
            }
        });
    }
}

function addRemoveListeners() {
    var removefiguration = document.getElementsByClassName('removefiguration');
    for (let i = 0; i < removefiguration.length; i++) {
        removefiguration[i].addEventListener("click", function () {
            var curIdx = $('.removefiguration').index($(this));
            try {
                document.getElementsByClassName('divkeyvalue')[curIdx].remove();
                console.log('Removed from DOM');
            }
            catch (err) {
                console.log(err);
            }
        });
    }
}

function appendInputs(response) {
    if (response !== undefined) {
        $('#gatewayValues').html('');
        for (let i = 0; i < response.length; i++) {
            if (response[i].IsDefault === true)
                $('#gatewayValues').append('<div class="form-inline text-center divkeyvalue forupdate"> ' +
                    '<div style="display:none;" class="form-group">' +
                    '<a class="removefiguration btn"><i class="fa fa-minus-circle"></i></a>' +
                    '</div>' +
                    '<input readonly hidden type="text" value="' + response[i].GatewayDetailId + '">' +
                    '<div class="form-group">' +
                    '<label>Key:&nbsp;</label>' +
                    '<input readonly value="' + response[i].GatewayDetailName + '" required type="text" class="form-control" placeholder="Key" />' +
                    '</div> <div class="form-group">' +
                    '<label>Value:&nbsp;</label>' +
                    '<input value="' + response[i].GatewayDetailValue + '" type="text" class="form-control" placeholder="Value" />' +
                    '</div>' +
                    '</div >');
            else
                $('#gatewayValues').append('<div class="form-inline text-center divkeyvalue forupdate"> ' +
                    '<div class="form-group">' +
                    '<a style="display:none;" class="removefiguration btn"><i class="fa fa-minus-circle"></i></a>' +
                    '<a onclick="deleteDetail(' + response[i].GatewayDetailId + ')" class="deletefiguration btn"><i class="fa fa-minus-circle"></i></a>' +
                    '</div>' +
                    '<input hidden type="text" value="' + response[i].GatewayDetailId + '">' +
                    '<div class="form-group">' +
                    '<label>Key:&nbsp;</label>' +
                    '<input value="' + response[i].GatewayDetailName + '" required type="text" class="form-control" placeholder="Key" />' +
                    '</div> <div class="form-group">' +
                    '<label>Value:&nbsp;</label>' +
                    '<input value="' + response[i].GatewayDetailValue + '" type="text" class="form-control" placeholder="Value" />' +
                    '</div>' +
                    '</div >');
        }
    }
    else {
        $('#gatewayValues').append('<div class="form-inline text-center divkeyvalue forinsert"> ' +
            '<div class="form-group">' +
            '<a class="removefiguration btn"><i class="fa fa-minus-circle"></i></a>' +
            '</div>' +
            '<div class="form-group">' +
            '<label>Key:&nbsp;</label>' +
            '<input value="" required type="text" class="form-control" placeholder="Key" />' +
            '</div> <div class="form-group">' +
            '<label>Value:&nbsp;</label>' +
            '<input value="" type="text" class="form-control" placeholder="Value" />' +
            '<div class="form-group">' +
            '<label>Is Default?<br style="line-height: 0;">' +
            '<input value="" type="checkbox" class="form-control" />' +
            '</label>' +
            '</div>' +
            '</div>' +
            '</div >');
        addRemoveListeners();
    }
}

function GetWatewayConfig(id) {
    if (id !== undefined) {
        if (id !== 5)
            sessionStorage.setItem('CurrentGatewayId', id);
        else {
            swal({
                title: "Atention!",
                text: "This is a default option, you cannot configure it, you must change it",
                icon: "warning"
            });
            return false;
        }
    }
        
    $.ajax("../Gateway/Get_Gateway_Config/" + sessionStorage.getItem('CurrentGatewayId'), {
        type: "GET",
        statusCode: {
            200: function (response) {
                console.log(response);
                appendInputs(response);
            },
            400: function (response) {
                console.log(response);
                swal({
                    title: "Ups!",
                    text: "Something went wrong, please try again later",
                    icon: "warning"
                });
            },
            404: function (response) {
                console.log(response);
                swal({
                    title: "Error!",
                    text: "Something went wrong, we're having difficulty connecting",
                    icon: "error"
                });
            }
        },
        success: function (response) {
            $('#gatewayModal').modal();
            if (response.length === 0 && document.getElementById('saveproccess') !== null) {
                document.getElementById('saveproccess').disabled = true;
                document.getElementById('saveproccess').className = 'btn btn-default';
            }
            else {
                if (document.getElementById('saveproccess') !== null) {
                    document.getElementById('saveproccess').disabled = false;
                    document.getElementById('saveproccess').className = 'btn btn-success';
                }
            }
            return true;
        },
        error: function (err) {
            console.log(err);
            return false;
        }
    });
}
