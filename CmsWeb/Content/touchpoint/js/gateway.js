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
            this.GetGatewayDetails();
        },
        oops: (message, title) => {
            warning_swal(title || 'Warning', message || 'Something went wrong, try again later');
        },
        fail: (message, title) => {
            error_swal(title || 'Error', message || 'We are working to fix it');
        },
        success: (message, title) => {
            success_swal(title || 'Success', message || 'Configuration Saved');
        },
        GetGatewayDetails: function () {
            this.$http.get('/Gateway/GetGatewayDetails').then(
                response => {
                    if (response.status === 200) {
                        this.GatewayDetails = response.body;
                        this.GetProcesses();
                    }
                    else {
                        console.log(response);
                        this.oops();
                    }
                },
                err => {
                    console.log(err);
                    this.fail();
                }
            );
        },
        GetProcesses: function () {
            this.$http.get('/Gateway/GetProcesses').then(
                response => {
                    if (response.status === 200) {
                        for (var i in response.body) {
                            var p = response.body[i];
                            var list = [];
                            p.AcceptACH && list.push("ACH");
                            p.AcceptCredit && list.push("Credit");
                            p.AcceptDebit && list.push("Debit");
                            p.AcceptOptions = list.join(', ');
                        }
                        this.Processes = response.body;

                        var nullProcesses = this.Processes.filter(function (item) {
                            return item.GatewayAccountId === null;
                        }).length;

                        this.UseForAllShow = nullProcesses >= 2 ? true : false;

                        this.GetGateways();
                    }
                    else {
                        console.log(response);
                        this.oops();
                    }
                },
                err => {
                    console.log(err);
                    this.fail();
                }
            );
        },
        GetGateways: function () {
            this.$http.get('/Gateway/GetGateways').then(
                response => {
                    if (response.status === 200) {
                        this.Gateways = response.body;
                        this.GetGatewayAccounts();
                    }
                    else {
                        console.log(response);
                        this.oops();
                    }
                },
                err => {
                    console.log(err);
                    this.fail();
                }
            );
        },
        GetGatewayAccounts: function () {
            this.$http.get('/Gateway/GetGatewayAccounts').then(
                response => {
                    if (response.status === 200) {
                        this.GatewayAccounts = response.body;
                    }
                    else {
                        console.log(response);
                        this.oops();
                    }
                },
                err => {
                    console.log(err);
                    this.fail();
                }
            );
        },
        modalInfo: function (ProcessId, GatewayAccountId) {
            this.IsGatewayReadOnly = true;
            this.DetailValue = [];
            this.GatewayAccountId = GatewayAccountId;

            if (ProcessId !== null) {
                this.ProcessId = ProcessId;
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
            this.$http.get('/Gateway/GetGatewayTemplate/' + this.GatewayId).then(
                response => {
                    if (response.status === 200) {
                        this.Inputs = response.body;
                        this.IsGatewayReadOnly = false;
                        for (let i = 0; i < this.Inputs.length; i++)
                            this.DetailValue.push(this.Inputs[i].GatewayDetailValue);
                    }
                    else {
                        console.log(response);
                        this.oops();
                    }
                },
                err => {
                    console.log(err);
                    this.fail();
                }
            );
        },
        checkboxesFor: function (process) {
            var list = [];
            var checked = (b) => b ? 'checked="checked"' : '';
            var box = (b, l) => '<div class="checkbox-inline"><label class="control-label" for="Accept' + l + process.ProcessId +
                '"><input type="checkbox" id="Accept' + l + process.ProcessId +'" name="Accept' + l +
                '" value="true" ' + checked(b) + '> ' + l +
                ' </label></div>';
            list.push(box(process.AcceptACH, 'ACH'));
            list.push(box(process.AcceptCredit, 'Credit'));
            list.push(box(process.AcceptDebit, 'Debit'));
            return list.join('');
        },
        acceptChange: function (process) {
            this.$http.post('/Gateway/UpdateAccept', process).then(
                response => {
                    if (response.status !== 200) {
                        this.oops();
                    }
                },
                err => {
                    console.log(err);
                    this.fail();
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
            else {
                if (this.GatewayId === null)
                    this.GatewayId = 1;
                this.OnChangeGateway();
            }
                
        },
        processForm: function () {
            var IsInsert = this.IsGatewayReadOnly ? false : true;
            var GatewayAccountInputs = this.Inputs.map(function (item) {
                return item.GatewayDetailName;
            });

            if (IsInsert) {
                this.$http.post('/Gateway/InsertAccount/' + IsInsert, {
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
                            this.success();
                            $('#config-modal').modal('hide');
                        }
                        else {
                            console.log(response);
                            this.oops();
                        }
                    },
                    err => {
                        console.log(err);
                        this.fail();
                    }
                );
            }
            else {
                this.$http.post('/Gateway/InsertAccount/' + IsInsert, {
                    ProcessId: this.ProcessId,
                    GatewayAccountId: this.GatewayAccountId,
                    GatewayAccountInputs: GatewayAccountInputs,
                    GatewayAccountValues: this.DetailValue,
                    UseForAll: this.UseForAll
                }).then(
                    response => {
                        if (response.status === 200) {
                            this.myFunctionOnLoad();
                            this.success();
                            $('#config-modal').modal('hide');
                        }
                        else {
                            console.log(response);
                            this.oops();
                        }
                    },
                    err => {
                        console.log(err);
                        this.fail();
                    }
                );
            }
        },
        deleteProcess: function (ProcessId) {
            this.$http.post('/Gateway/DeleteProcessAccount', {
                ProcessId: ProcessId
            }).then(
                response => {
                    if (response.status === 200) {
                        this.myFunctionOnLoad();
                        this.success();
                    }
                    else {
                        console.log(response);
                        this.oops();
                    }
                },
                err => {
                    console.log(err);
                    this.fail();
                }
            );
        }
    },
    created: function () {
        this.myFunctionOnLoad();
    }
});
