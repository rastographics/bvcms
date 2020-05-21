<template>
    <div id="multipleGateway">
        <div class="col-lg-9">
            <div class="box box-responsive">
                <div class="box-content">
                    <div class="table-responsive">
                        <table id="settings" class="table table-striped">
                            <thead>
                                <tr>
                                    <th style="width: 150px;">
                                        Payment Process
                                    </th>
                                    <th style="width: 150px;">
                                        Account Name
                                    </th>
                                    <th style="width: 150px;">

                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="process in Processes" :key="process.ProcessId">
                                    <td>
                                        {{process.ProcessName}}
                                    </td>
                                    <td>
                                        <a v-on:click="modalInfo(process.ProcessId, process.GatewayAccountId)" :class="[process.GatewayAccountName !== null ? 'blue-avaiable' : 'red-empty']" href="#">
                                            {{process.GatewayAccountName || "Empty"}}
                                        </a>
                                    </td>
                                    <td style="width: 50px;">
                                        <a v-if="process.GatewayAccountName !== null" v-on:click="deleteProcess(process.ProcessId)" href="#" class="btn btn-sm btn-danger deleteprocess">
                                            <i class="fa fa-trash"></i>&nbsp;Delete
                                        </a>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>

        <div class="modal fade" tabindex="-1" role="dialog" id="config-modal">
            <div class="modal-dialog">
                <div class="modal-content">
                    <form class="multiplegateway-form" v-on:submit.prevent="processForm">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">×</span></button>
                            <h4 class="modal-title">{{ProcessName}}</h4>
                        </div>
                        <div class="modal-body">
                            <div class="row">
                                <div class="col-md-3">
                                    <label>Account Name</label>
                                </div>
                                <div class="col-md-6">
                                    <input v-on:change="checkAccount" v-model="AccountName" type="text" class="form-control" list="gtAccounts" />
                                    <datalist id="gtAccounts">
                                        <option v-for="account in GatewayAccounts" :key="account.GatewayAccountName" :value="account.GatewayAccountName"></option>
                                    </datalist>
                                    <small v-show="!IsGatewayReadOnly">*This Configuration will be save as a new account</small>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-3">
                                    <label>Gateway Type</label>
                                </div>
                                <div class="col-md-6">
                                    <select class="form-control" v-model="GatewayId" v-on:change="OnChangeGateway" v-bind:disabled="IsGatewayReadOnly">
                                        <option v-for="gateway in Gateways" :key="gateway.GatewayId" :value="gateway.GatewayId">{{gateway.GatewayName}}</option>
                                    </select>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-md-offset-3">
                                    <label v-show="UseForAllShow" class="label-gateway">
                                        <input v-model="UseForAll" class="" type="checkbox" />
                                        &nbsp;Use for all payment processes
                                    </label>
                                </div>
                            </div>
                            <hr />
                            <div v-for="(input, index) in Inputs" :key="input.GatewayDetailName">
                                <div class="row">
                                    <div class="col-md-3">
                                        <label>{{input.GatewayDetailName}}</label>
                                    </div>
                                    <div class="col-md-6">
                                        <input v-if="input.IsBoolean" type="checkbox" v-model="DetailValue[index]" true-value="true" false-value="false" />
                                        <input required v-else class="form-control" v-model="DetailValue[index]" />
                                    </div>
                                </div>
                                <br />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <input type="submit" value="Save" class="btn btn-primary">
                            <input type="button" value="Cancel" class="btn btn-default" data-dismiss="modal">
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
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
            }
        },
        methods: {
            myFunctionOnLoad: function () {
                this.GetGatewayDetails();
            },
            GetGatewayDetails: function () {
                this.$http.get('/Gateway/GetGatewayDetails').then(
                    response => {
                        if (response.status === 200) {
                            console.log(response);
                            this.GatewayDetails = response.data;
                            this.GetProcesses();
                        }
                        else {
                            console.log(response);
                            warning_swal('Warning!', 'Something went wrong, try again later');
                        }
                    },
                    err => {
                        console.log(err);
                        error_swal('Fatal Error!', 'We are working to fix it');
                    }
                );
            },
            GetProcesses: function () {
                this.$http.get('/Gateway/GetProcesses').then(
                    response => {
                        if (response.status === 200) {
                            this.Processes = response.data;

                            if (this.Processes) {
                                var nullProcesses = this.Processes.filter(function (item) {
                                    return item.GatewayAccountId === null;
                                }).length;
                            }

                            this.UseForAllShow = nullProcesses >= 2 ? true : false;

                            this.GetGateways();
                        }
                        else {
                            console.log(response);
                            warning_swal('Warning!', 'Something went wrong, try again later');
                        }
                    },
                    err => {
                        console.log(err);
                        error_swal('Fatal Error!', 'We are working to fix it');
                    }
                );
            },
            GetGateways: function () {
                this.$http.get('/Gateway/GetGateways').then(
                    response => {
                        if (response.status === 200) {
                            this.Gateways = response.data;
                            this.GetGatewayAccounts();
                        }
                        else {
                            console.log(response);
                            warning_swal('Warning!', 'Something went wrong, try again later');
                        }
                    },
                    err => {
                        console.log(err);
                        error_swal('Fatal Error!', 'We are working to fix it');
                    }
                );
            },
            GetGatewayAccounts: function () {
                this.$http.get('/Gateway/GetGatewayAccounts').then(
                    response => {
                        if (response.status === 200) {
                            this.GatewayAccounts = response.data;
                        }
                        else {
                            console.log(response);
                            warning_swal('Warning!', 'Something went wrong, try again later');
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
                            this.Inputs = response.data;
                            this.IsGatewayReadOnly = false;
                            for (let i = 0; i < this.Inputs.length; i++)
                                this.DetailValue.push(this.Inputs[i].GatewayDetailValue);
                        }
                        else {
                            console.log(response);
                            warning_swal('Warning!', 'Something went wrong, try again later');
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
                                success_swal('Success', 'Configuration Saved');
                                $('#config-modal').modal('hide');
                            }
                            else {
                                console.log(response);
                                warning_swal('Warning!', 'Something went wrong, try again later');
                            }
                        },
                        err => {
                            console.log(err);
                            error_swal('Fatal Error!', 'We are working to fix it');
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
                                success_swal('Success', 'Configuration Saved');
                                $('#config-modal').modal('hide');
                            }
                            else {
                                console.log(response);
                                warning_swal('Warning!', 'Something went wrong, try again later');
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
                this.$http.post('/Gateway/DeleteProcessAccount', {
                    ProcessId: ProcessId
                }).then(
                    response => {
                        if (response.status === 200) {
                            this.myFunctionOnLoad();
                            success_swal('Success', 'Configuration Saved');
                        }
                        else {
                            console.log(response);
                            warning_swal('Warning!', 'Something went wrong, try again later');
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
    }
</script>
