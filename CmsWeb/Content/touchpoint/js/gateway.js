$(".addnew").click(function () {
    document.getElementsByClassName("noprocess")[0].hidden = true;
    document.getElementsByClassName("noprocess")[1].hidden = false;
});

$(".more-configs").click(function () {
    document.getElementsByClassName("noprocess")[0].hidden = true;
    document.getElementsByClassName("noprocess")[1].hidden = false;
});

$('.cancel').click(function () {
    document.getElementsByClassName("noprocess")[0].hidden = false;
    document.getElementsByClassName("noprocess")[1].hidden = true;
    document.getElementById("AddGatewaySettings").reset();
});

function editprocess(GatewaySettingId, proccessId, gatewayId) {
    document.getElementById('GatewaySettingId').value = GatewaySettingId;
    document.getElementById('secproccessId').value = proccessId;
    document.getElementById('secgatewaylist').value = gatewayId;

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
            i = i + 1;
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
    console.log(sessionStorage.getItem('CurrentGatewayId'));

    $.ajax("../Gateway/Get_Wateway_Config/" + this.value, {
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
                $('#gatewayModal').modal();
                $('#gatewayValues').html('');
            }
        },
        error: function (err) {
            console.log(err);
        }
    });
});

$('.getconfig').click(function () {
    sessionStorage.setItem('CurrentGatewayId', $('#gatewaylist').val());
    GetWatewayConfig();
});

function deleteDetail(id) {
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
                    '<button class="removefiguration btn"><i class="fa fa-minus-circle"></i></button>' +
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
            else
                $('#gatewayValues').append('<div class="form-inline text-center divkeyvalue forupdate"> ' +
                    '<div class="form-group">' +
                    '<button style="display:none;" class="removefiguration btn"><i class="fa fa-minus-circle"></i></button>' +
                    '<button onclick="deleteDetail(' + response[i].GatewayDetailId + ')" class="deletefiguration btn"><i class="fa fa-minus-circle"></i></button>' +
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
            '<button class="removefiguration btn"><i class="fa fa-minus-circle"></i></button>' +
            '</div>' +
            '<div class="form-group">' +
            '<label>Key:&nbsp;</label>' +
            '<input value="" required type="text" class="form-control" placeholder="Key" />' +
            '</div> <div class="form-group">' +
            '<label>Value:&nbsp;</label>' +
            '<input value="" type="text" class="form-control" placeholder="Value" />' +
            '</div>' +
            '</div >');
        addRemoveListeners();
    }
}

function GetWatewayConfig() {
    $.ajax("../Gateway/Get_Wateway_Config/" + sessionStorage.getItem('CurrentGatewayId'), {
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
        success: function () {
            $('#gatewayModal').modal();
        },
        error: function (err) {
            console.log(err);
        }
    });
}
