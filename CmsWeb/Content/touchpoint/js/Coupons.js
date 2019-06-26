var ListApp = new Vue({
    el: '#ListApp',
    data: {
        Coupons: [{}],
        Users: [{}],
        OnlineRegs: [{}],
        CouponStatus: [{}],
        name: null,
        couponcode: null,
        amount: null,
        useridfilter: null,
        regidfilter: null,
        startdateIso: null,
        enddateIso: null,
        usedfilter: null
    },
    methods: {
        myFunctionOnLoad: function () {
            this.$http.post('/Coupon/GetCoupons').then(
                response => {
                    if (response.status === 200) {
                        this.Coupons = response.body;
                        this.GetUsers();
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
        GetUsers: function () {
            this.$http.get('/Coupon/GetUsers').then(
                response => {
                    if (response.status === 200) {
                        this.Users = response.body;
                        this.GetOnlineRegs();
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
        GetOnlineRegs: function () {
            this.$http.get('/Coupon/GetOnlineRegs').then(
                response => {
                    if (response.status === 200) {
                        this.OnlineRegs = response.body;
                        this.GetCouponStatus();
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
        GetCouponStatus: function () {
            this.$http.get('/Coupon/GetCouponStatus').then(
                response => {
                    if (response.status === 200) {
                        this.CouponStatus = response.body;
                        $("#regidfilter").select2();
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
        FilterCoupon: function () {
            this.$http.post('/Coupon/GetCoupons', {
                name: this.name,
                useridfilter: this.useridfilter,
                regidfilter: this.regidfilter,
                startdate: startdateIso.value,
                enddate: enddateIso.value,
                usedfilter: this.usedfilter
            }).then(
                response => {
                    if (response.status === 200) {
                        this.Coupons = response.body;
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
        ResetFilter: function () {
            this.useridfilter = null;
            regidfilter.value = null;
            this.regidfilter = null;
            this.name = null;
            regidfilter.value = "";
            this.regidfilter = null;
            this.startdateIso = null;
            startdateIso.value = "";
            this.enddateIso = null;
            enddateIso.value = "";
            usedfilter.value = "";
            this.usedfilter = null;
        },
        DownloadExcel: function () {
            var url = '../Coupon/DownloadExcel/';
            var params = '{0}/{1}/{2}/{3}/{4}/{5}'.format(this.name, this.useridfilter === null ? 0 : this.useridfilter, this.regidfilter, startdateIso.value.length === 0 ? null : startdateIso.value, enddateIso.value.length === 0 ? null : enddateIso.value, this.usedfilter);
            var iframe = document.createElement("iframe");
            iframe.setAttribute("src", url + params);
            iframe.setAttribute("style", "display: none");
            document.body.appendChild(iframe);
        },
        CreateCoupon: function () {
            if (this.regidfilter === null) {
                swal({
                    type: 'info',
                    title: 'Warning',
                    text: 'Please select a Registration Type'
                });
            }
            else {
                this.$http.post('/Coupon/Create', {
                    regid: this.regidfilter,
                    name: this.name,
                    amount: this.amount,
                    couponcode: this.couponcode
                }).then(
                    response => {
                        if (response.status === 200) {
                            if (response.body[0].Message === 'Ok') {
                                swal({
                                    title: "Success",
                                    text: "Coupon created successfully",
                                    type: "success",
                                    showCancelButton: true,
                                    cancelButtonText: 'Close',
                                    confirmButtonClass: "btn-warning",
                                    confirmButtonText: "Go to list",
                                    closeOnConfirm: true
                                }, function () {
                                    location.replace('../Coupon/List');
                                });
                            }
                            else {
                                warning_swal('Warning!', 'This coupon already exists');
                            }
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
        CancelCoupon: function (Code) {
            swal({
                title: "Are you sure?",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-warning",
                confirmButtonText: "Yes, cancel it!",
                closeOnConfirm: true
            }, function () {
                $.block();
                ListApp.$http.post('/Coupon/Cancel', {
                    id: Code
                }).then(
                    response => {
                        if (response.status === 200) {
                            ListApp.myFunctionOnLoad();
                            $.unblock();
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
              });
        },
        ShowCreate: function () {
            document.getElementById('ListContent').hidden = true;
            document.getElementById('CreateContent').hidden = false;
        }
    },
    created: function () {
        this.myFunctionOnLoad();
    }
});

var CreateApp = new Vue({
    el: '#CreateApp',
    data: {
        Coupons: [{}],
        Users: [{}],
        OnlineRegs: [{}],
        CouponStatus: [{}],
        name: null,
        couponcode: null,
        amount: null,
        useridfilter: null,
        regidfilter: null,
        startdateIso: null,
        enddateIso: null,
        usedfilter: null
    },
    methods: {
        myFunctionOnLoad: function () {
            this.$http.post('/Coupon/GetCoupons').then(
                response => {
                    if (response.status === 200) {
                        this.Coupons = response.body;
                        this.GetUsers();
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
        GetUsers: function () {
            this.$http.get('/Coupon/GetUsers').then(
                response => {
                    if (response.status === 200) {
                        this.Users = response.body;
                        this.GetOnlineRegs();
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
        GetOnlineRegs: function () {
            this.$http.get('/Coupon/GetOnlineRegs').then(
                response => {
                    if (response.status === 200) {
                        this.OnlineRegs = response.body;
                        this.GetCouponStatus();
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
        GetCouponStatus: function () {
            this.$http.get('/Coupon/GetCouponStatus').then(
                response => {
                    if (response.status === 200) {
                        this.CouponStatus = response.body;
                        $("#regidSearch").select2();
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
        FilterCoupon: function () {
            this.$http.post('/Coupon/GetCoupons', {
                name: this.name,
                useridfilter: this.useridfilter,
                regidfilter: this.regidfilter,
                startdate: startdateIso.value,
                enddate: enddateIso.value,
                usedfilter: this.usedfilter
            }).then(
                response => {
                    if (response.status === 200) {
                        this.Coupons = response.body;
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
        ResetFilter: function () {
            this.useridfilter = null;
            regidfilter.value = null;
            this.regidfilter = null;
            this.name = null;
            regidfilter.value = "";
            this.regidfilter = null;
            this.startdateIso = null;
            startdateIso.value = "";
            this.enddateIso = null;
            enddateIso.value = "";
            usedfilter.value = "";
            this.usedfilter = null;
        },
        DownloadExcel: function () {
            var url = '../Coupon/DownloadExcel/';
            var params = '{0}/{1}/{2}/{3}/{4}/{5}'.format(this.name, this.useridfilter === null ? 0 : this.useridfilter, this.regidfilter, startdateIso.value.length === 0 ? null : startdateIso.value, enddateIso.value.length === 0 ? null : enddateIso.value, this.usedfilter);
            var iframe = document.createElement("iframe");
            iframe.setAttribute("src", url + params);
            iframe.setAttribute("style", "display: none");
            document.body.appendChild(iframe);
        },
        CreateCoupon: function () {
            if (this.regidfilter === null) {
                swal({
                    type: 'info',
                    title: 'Warning',
                    text: 'Please select a Registration Type'
                });
            }
            else {
                this.$http.post('/Coupon/Create', {
                    regid: this.regidfilter,
                    name: this.name,
                    amount: this.amount,
                    couponcode: this.couponcode
                }).then(
                    response => {
                        if (response.status === 200) {
                            if (response.body[0].Message === 'Ok') {
                                swal({
                                    title: "Success",
                                    text: "Coupon created successfully",
                                    type: "success",
                                    showCancelButton: true,
                                    cancelButtonText: 'Close',
                                    confirmButtonClass: "btn-warning",
                                    confirmButtonText: "Go to list",
                                    closeOnConfirm: true
                                }, function () {
                                    location.replace('../Coupon/List');
                                });
                            }
                            else {
                                warning_swal('Warning!', 'This coupon already exists');
                            }
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
        CancelCoupon: function (Code) {
            swal({
                title: "Are you sure?",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-warning",
                confirmButtonText: "Yes, cancel it!",
                closeOnConfirm: true
            }, function () {
                $.block();
                ListApp.$http.post('/Coupon/Cancel', {
                    id: Code
                }).then(
                    response => {
                        if (response.status === 200) {
                            ListApp.myFunctionOnLoad();
                            $.unblock();
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
            });
        },
        ShowList: function () {
            document.getElementById('CreateContent').hidden = true;
            document.getElementById('ListContent').hidden = false;
        }
    },
    created: function () {
        this.myFunctionOnLoad();
    }
});

function formatMoney(n, c, d, t) {
    c = isNaN(c = Math.abs(c)) ? 2 : c,
    d = d === undefined ? "." : d,
    t = t === undefined ? "," : t,
    s = n < 0 ? "-" : "",
    i = String(parseInt(n = Math.abs(Number(n) || 0).toFixed(c))),
    j = (j = i.length) > 3 ? j % 3 : 0;

    return "$" + s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
}

function IsCanceled(Used, Code, Canceled) {
    return ((Used === null || (Code !== undefined && Code.length !== 12)) && Canceled === null);
}

function IsCanceledOrUsed(Used, Canceled) {
    return Used !== null ? Used : (Canceled !== null ? 'canceled' : 'not used');
}

function SetSearchOrgId (e) {
    ListApp.regidfilter = (e.value || e.options[e.selectedIndex].value);
}

function SetCreateOrgId(e) {
    CreateApp.regidfilter = (e.value || e.options[e.selectedIndex].value);
}

function SetUsedfilter(e) {
    ListApp.usedfilter = (e.value || e.options[e.selectedIndex].value);
}

String.prototype.format = function () {
    a = this;
    for (k in arguments) {
        a = a.replace("{" + k + "}", arguments[k]);
    }
    return a;
};

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
