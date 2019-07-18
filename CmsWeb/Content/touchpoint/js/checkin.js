var CheckInApp = new Vue({
    el: '#CheckInApp',
    data: {
        identity: false,
        view: false,
        profile: false,
        user: {
            name: '',
            password: ''
        },
        search: {
            phone: ''
        }
    },
    computed: {
        apiHeaders: function () {
            return {
                headers: {
                    'Authorization': this.identity,
                    'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8'
                }
            };
        }
    },
    methods: {
        generateToken() {
            if (this.user.name.length && this.user.password.length) {
                var encoded = 'Basic ' + window.btoa(this.user.name + ":" + this.user.password);
                this.user.name = '';
                this.user.password = '';
                return encoded;
            } else {
                return false;
            }
        },
        generatePayload(data) {
            var inner = JSON.stringify(data);
            var outer = JSON.stringify({
                version: 3,
                device: 3,
                data: inner
            });
            var body = {
                data: outer
            };
            return Object.keys(body).map(key => key + '=' + body[key]).join('&');
        },
        auth() {
            let vm = this;
            var token = vm.generateToken();
            if (token) {
                vm.$http.post('/CheckInApiV2/Authenticate', {}, {
                    headers: {
                        'Authorization': token
                    }
                }).then(
                    response => {
                        if (response.status === 200) {
                            if (response.data.error === 0) {
                                var profile = JSON.parse(response.data.data);
                                profile = {
                                    userName: profile.userName,
                                    userId: profile.userId
                                };
                                localStorage.setItem('identity', token);
                                localStorage.setItem('profile', JSON.stringify(profile));
                                vm.identity = token;
                                vm.profile = profile;
                                vm.view = 'landing';
                            } else {
                                // invalid creds
                                warning_swal('Login Failed', response.data.data);
                            }
                        }
                        else {
                            console.log(response);
                            warning_swal('Warning!', 'Something went wrong, try again later');
                        }
                    },
                    err => {
                        console.log(err);
                        error_swal('Error', 'Unable to connect to TouchPoint');
                    }
                );
            } else {
                warning_swal('Login Failed', 'Email and password are required');
            }
        },
        logout() {
            localStorage.removeItem('identity');
            localStorage.removeItem('profile');
            this.identity = false;
            this.profile = false;
            this.view = 'login';
        },
        find() {
            let vm = this;
            // todo: set campus from profile
            var payload = vm.generatePayload({
                search: vm.search.phone,
                campus: 0,
                date: '2019-07-21 08:00:00'
            });
            vm.$http.post('/CheckInApiV2/Search', payload, vm.apiHeaders).then(
                response => {
                    if (response.status === 200) {
                        if (response.data.error === 0) {
                            var results = JSON.parse(response.data.data);
                            console.log(JSON.parse(response.data.data));
                            vm.search.phone = '';
                            if (results.length > 1) {
                                vm.view = 'families';
                            } else if (results.length === 1) {
                                vm.view = 'checkin';
                            } else {
                                // no results
                            }
                        } else {
                            vm.search.phone = '';
                            warning_swal('Search Failed', response.data.data);
                        }
                    }
                    else {
                        vm.search.phone = '';
                        console.log(response);
                        warning_swal('Warning!', 'Something went wrong, try again later');
                    }
                },
                err => {
                    console.log(err);
                    error_swal('Error', 'Something went wrong');
                }
            );
        }
    },
    mounted() {
        // init, fetch identity, profile, and show login or landing
        var identity = localStorage.getItem('identity');
        if (identity && identity.length) {
            this.identity = identity;
            this.view = 'landing';
        } else {
            this.view = 'login';
        }
        var profile = localStorage.getItem('profile');
        if (profile && profile.length) {
            this.profile = JSON.parse(profile);
            console.log(this.profile);
        }
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
