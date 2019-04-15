var multipleGateway = new Vue({
    el: '#multipleGateway',
    data: {
        GatewayDetails: [{}],
        Processes: [{}],
        Gateways: [{}],
        GatewayAccounts: [{}],
        ProcessName: '',
        GatewayAccountId: null,
        ProcessId: null,
        AccountName: '',
        GatewayId: null,
        Inputs: [{}],
        DetailValue: [],
        IsGatewayReadOnly: false,
        UseForAll: false,
        UseForAllShow: true
    },
    methods: {
        myFunctionOnLoad: function () {
            this.$http.get('../Gateway/GetGatewayDetails').then(
                response => {
                    if (response.status === 200) {
                        console.log(response.body);
                        this.GatewayDetails = response.body;
                        this.GetProcesses();
                    }
                    else {
                        console.log(response);
                        warning_swal('Ups!', 'Something went wrong, try again later');
                    }
                },
                err => {
                    console.log(err);
                    error_swal('Fatal Error!', 'We are working to fix it');
                }
            );
        },
        GetProcesses: function () {
            this.$http.get('../Gateway/GetProcesses').then(
                response => {
                    if (response.status === 200) {
                        console.log(response.body);
                        this.Processes = response.body;

                        var nullProcesses = this.Processes.filter(function (item) {
                            return item.GatewayAccountId === null;
                        }).length;

                        this.UseForAllShow = nullProcesses >= 2 ? true : false;

                        this.GetGateways();
                    }
                    else {
                        console.log(response);
                        warning_swal('Ups!', 'Something went wrong, try again later');
                    }
                },
                err => {
                    console.log(err);
                    error_swal('Fatal Error!', 'We are working to fix it');
                }
            );
        },
        GetGateways: function () {
            this.$http.get('../Gateway/GetGateways').then(
                response => {
                    if (response.status === 200) {
                        console.log(response.body);
                        this.Gateways = response.body;
                        this.GetGatewayAccounts();
                    }
                    else {
                        console.log(response);
                        warning_swal('Ups!', 'Something went wrong, try again later');
                    }
                },
                err => {
                    console.log(err);
                    error_swal('Fatal Error!', 'We are working to fix it');
                }
            );
        },
        GetGatewayAccounts: function () {
            this.$http.get('../Gateway/GetGatewayAccounts').then(
                response => {
                    if (response.status === 200) {
                        console.log(response.body);
                        this.GatewayAccounts = response.body;
                    }
                    else {
                        console.log(response);
                        warning_swal('Ups!', 'Something went wrong, try again later');
                    }
                },
                err => {
                    console.log(err);
                    error_swal('Fatal Error!', 'We are working to fix it');
                }
            );
        },
        modalInfo: function (ProcessId, GatewayAccountId) {
            this.IsGatewayReadOnly = true;
            this.DetailValue = [];
            this.GatewayAccountId = GatewayAccountId;

            if (ProcessId !== null) {
                this.ProcessId = ProcessId;
                console.log(this.ProcessId);
                var res = this.Processes.filter(function (item) {
                    return item.ProcessId === ProcessId;
                })[0];

                this.ProcessName = res.ProcessName;
                this.GatewayId = res.GatewayId;
                this.AccountName = res.GatewayAccountName;

                if (GatewayAccountId !== null) {
                    this.Inputs = this.GatewayDetails.filter(function (item) {
                        return item.GatewayAccountId === GatewayAccountId;
                    });

                    for (let i = 0; i < this.Inputs.length; i++)
                        this.DetailValue.push(this.Inputs[i].GatewayDetailValue);
                }
                else
                    this.Inputs = [{}];
                
                $('#config-modal').modal();
            }
            else {
                this.Inputs = this.GatewayDetails.filter(function (item) {
                    return item.GatewayAccountId === GatewayAccountId;
                });

                for (let i = 0; i < this.Inputs.length; i++)
                    this.DetailValue.push(this.Inputs[i].GatewayDetailValue);
            }
        },
        OnChangeGateway: function () {
            this.DetailValue = [];
            this.$http.get('../Gateway/GetGatewayTemplate/' + this.GatewayId).then(
                response => {
                    if (response.status === 200) {
                        this.Inputs = response.body;
                        this.IsGatewayReadOnly = false;
                        for (let i = 0; i < this.Inputs.length; i++)
                            this.DetailValue.push(this.Inputs[i].GatewayDetailValue);
                    }
                    else {
                        console.log(response);
                        warning_swal('Ups!', 'Something went wrong, try again later');
                    }
                },
                err => {
                    console.log(err);
                    error_swal('Fatal Error!', 'We are working to fix it');
                }
            );
        },
        checkAccount: function () {
            var res = this.GatewayAccounts.filter(function (item) {
                return item.GatewayAccountName === event.target.value;
            });
            if (res.length > 0) {
                this.GatewayId = res[0].GatewayId;
                this.modalInfo(null, res[0].GatewayAccountId);
            }
            else
                this.OnChangeGateway();
        },
        processForm: function () {
            var IsInsert = this.IsGatewayReadOnly ? false : true;
            var GatewayAccountInputs = this.Inputs.map(function (item) {
                return item.GatewayDetailName;
            });

            if (IsInsert) {
                this.$http.post('../Gateway/InsertAccount/' + IsInsert, {
                    ProcessId: this.ProcessId,
                    GatewayAccountName: this.AccountName,
                    GatewayId: this.GatewayId,
                    GatewayAccountInputs: GatewayAccountInputs,
                    GatewayAccountValues: this.DetailValue,
                    UseForAll: this.UseForAll
                }).then(
                    response => {
                        if (response.status === 200) {
                            this.myFunctionOnLoad();
                            success_swal('Success', 'Configuration Saved');
                            $('#config-modal').modal('hide');
                        }
                        else {
                            console.log(response);
                            warning_swal('Ups!', 'Something went wrong, try again later');
                        }
                    },
                    err => {
                        console.log(err);
                        error_swal('Fatal Error!', 'We are working to fix it');
                    }
                );
            }
            else {
                this.$http.post('../Gateway/InsertAccount/' + IsInsert, {
                    ProcessId: this.ProcessId,
                    GatewayAccountId: this.GatewayAccountId,
                    GatewayAccountInputs: GatewayAccountInputs,
                    GatewayAccountValues: this.DetailValue,
                    UseForAll: this.UseForAll
                }).then(
                    response => {
                        if (response.status === 200) {
                            this.myFunctionOnLoad();
                            success_swal('Success', 'Configuration Saved');
                            $('#config-modal').modal('hide');
                        }
                        else {
                            console.log(response);
                            warning_swal('Ups!', 'Something went wrong, try again later');
                        }
                    },
                    err => {
                        console.log(err);
                        error_swal('Fatal Error!', 'We are working to fix it');
                    }
                );
            }
        },
        deleteProcess: function (ProcessId) {
            this.$http.post('../Gateway/DeleteProcessAccount', {
                ProcessId: ProcessId
            }).then(
                response => {
                    if (response.status === 200) {
                        this.myFunctionOnLoad();
                        success_swal('Success', 'Configuration Saved');
                    }
                    else {
                        console.log(response);
                        warning_swal('Ups!', 'Something went wrong, try again later');
                    }
                },
                err => {
                    console.log(err);
                    error_swal('Fatal Error!', 'We are working to fix it');
                }
            );
        }
    },
    created: function () {
        this.myFunctionOnLoad();
    }
});

function error_swal(title, message) {
    swal({
        type: "error",
        title: title,
        text: message
    });
}

function warning_swal(title, message) {
    swal({
        type: "info",
        title: title,
        text: message
    });
}

function success_swal(title, message) {
    swal({
        type: "success",
        title: title,
        text: message
    });
}
