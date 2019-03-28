var multipleGateway = new Vue({
    el: '#multipleGateway',
    data: {
        GatewayDetails: [{}],
        Processes: [{}],
        Gateways: [{}],
        GatewayAccounts: [{}],
        ProcessName: '',
        AccountName: '',
        GatewayId: null,
        Inputs: [{}],
        DetailValue: [],
        IsGatewayReadOnly: false
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

            if (ProcessId !== null) {
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
            var Action = this.IsGatewayReadOnly ? 'For Update' : 'For Insert';

            console.log(Action);
            for (let i = 0; i < this.Inputs.length; i++) {
                if (this.DetailValue[i].length === 0)
                    console.log(this.Inputs[i].GatewayDetailName + ': ""');
                else
                    console.log(this.Inputs[i].GatewayDetailName + ': ' + this.DetailValue[i]);
            }
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
