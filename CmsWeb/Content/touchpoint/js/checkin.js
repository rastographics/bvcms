Vue.use(VueMask.VueMaskPlugin);

var Keyboard = window.SimpleKeyboard.default;

var CheckInApp = new Vue({
    el: '#CheckInApp',
    data: {
        view: false,
        identity: false,
        profile: false,
        loading: false,
        keyboard: false,
        reprintLabels: false,
        idleTimer: false,
        idleStage: 0,
        idleTimeout: 60000,
        families: [],
        members: [],
        attendance: [],
        kiosk: {
            name: '',
            profile: 'default'
        },
        user: {
            name: '',
            password: ''
        },
        search: {
            phone: ''
        },
        ABSENT: 0,
        PRESENT: 1,
        CHECKEDIN: 2
    },
    watch: {
        loading: function (loading) {
            if (loading) {
                $.block();
            } else {
                $.unblockUI();
            }
        }
    },
    filters: {
        formatDate: function (dt) {
            if (!dt) return '';
            var date = new Date(dt);
            var time = date.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true });
            return time + ' - ' + $.datepicker.formatDate("m/d/y", date);
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
        },
        phoneMask: function () {
            var len = this.search.phone.replace(/\D/g, '').length;
            if (len > 4 && len < 11) {
                return '(###) ###-####?#?#?#?#?#';
            } else {
                return '##########?#?#?#?#?#';
            }
        },
        attendanceUpdated: function () {
            var keys = Object.keys(this.attendance);
            for (var i = 0; i < keys.length; i++) {
                if (this.attendance[keys[i]].changed === true) {
                    return true;
                }
            }
            return false;
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
                kiosk: this.kiosk.name,
                data: inner
            });
            var body = {
                data: outer
            };
            return Object.keys(body).map(key => key + '=' + body[key]).join('&');
        },
        initKeyboard() {
            let vm = this;
            setTimeout(function () {
                vm.keyboard = new Keyboard({
                    onChange: function (input) {
                        vm.search.phone = input;
                    },
                    onKeyPress: function (button) {
                        if (button === '{enter}') {
                            vm.find();
                        }
                    },
                    layout: {
                        default: ["1 2 3", "4 5 6", "7 8 9", "{bksp} 0 {enter}"]
                    },
                    display: {
                        '{bksp}': 'delete',
                        '{enter}': 'go'
                    },
                    theme: "hg-theme-default hg-layout-numeric numeric-theme"
                });
                $(".keyboard-input").on("input", function(e) {
                    vm.keyboard.setInput(e.target.value);
                });
            }, 100);
        },
        resetIdleTimer() {
            let vm = this;
            clearTimeout(vm.idleTimer);
            vm.idleTimer = setTimeout(vm.handleIdle, vm.idleTimeout);
        },
        handleIdle() {
            let vm = this;
            if (!['landing','login'].includes(vm.view) || vm.search.phone.length) {
                vm.idleStage++;
                vm.resetIdleTimer();
                if (vm.idleStage === 1) {
                    vm.showIdleAlert();
                } else if (vm.idleStage === 2) {
                    vm.reset();
                }
            }
        },
        showIdleAlert() {
            let vm = this;
            swal({
                title: "Are you still there?",
                text: "If you need more time, please press 'Yes' so that your progress isn't lost.",
                type: "warning",
                showCancelButton: false,
                confirmButtonClass: "btn-success",
                confirmButtonText: "Yes",
                cancelButtonText: "No"
            }, function () {
                vm.idleStage = 0;
            });
        },
        portraitStyles(member) {
            return {
                'background-image': 'url(' + member.picture + ')',
                'background-position': member.pictureX + '% ' + member.pictureY + '%'
            };
        },
        loadView(newView) {
            // cleanup and prep for view swap
            if (this.view === 'landing') {
                this.keyboard.destroy();
            }
            this.view = newView;
            if (newView === 'landing') {
                this.search.phone = '';
                this.members = [];
                this.families = [];
                this.attendance = [];
                this.reprintLabels = false;
                this.initKeyboard();
                if (!this.profile || !this.kiosk.name.length) {
                    this.logout();
                    error_swal("Error", "Couldn't retrieve saved profile data. Please try logging in again.");
                }
            }
        },
        auth() {
            let vm = this;
            var token = vm.generateToken();
            if (token) {
                vm.loading = true;
                vm.$http.post('/CheckInApiV2/Authenticate', {}, {
                    headers: {
                        'Authorization': token
                    }
                }).then(
                    response => {
                        vm.loading = false;
                        if (response.status === 200) {
                            if (response.data.error === 0) {
                                // todo: load profile based on login dropdown selection (vm.kiosk.profile)
                                var profile = JSON.parse(response.data.data);
                                profile = {
                                    userName: profile.userName,
                                    userId: profile.userID
                                };
                                localStorage.setItem('identity', token);
                                localStorage.setItem('kiosk', vm.kiosk.name);
                                localStorage.setItem('profile', JSON.stringify(profile));
                                vm.identity = token;
                                vm.profile = profile;
                                vm.loadView('landing');
                            } else {
                                // invalid creds
                                warning_swal('Login Failed', response.data.data);
                            }
                        }
                        else {
                            warning_swal('Warning!', 'Something went wrong, try again later');
                        }
                    },
                    err => {
                        vm.loading = false;
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
            this.loadView('login');
        },
        reset() {
            // called on session timeout if kiosk has been idle too long
            this.idleStage = 0;
            this.loadView('landing');
            swal.close();
        },
        find() {
            let vm = this;
            // todo: set campus and date from profile
            var phone = vm.search.phone.replace(/\D/g, '');
            // todo: remove, debug only
            vm.profile.logoutKey = '12345';
            // handle special entry
            if (phone === vm.profile.logoutKey) {
                vm.logout();
                return;
            }
            if (phone.length < 4 || phone.length > 15) {
                vm.loadView('landing');
                warning_swal('No results', 'No families found with that number, please try again.');
                return;
            }
            var payload = vm.generatePayload({
                search: phone,
                campus: 0,
                date: '2019-08-11 08:00:00' // todo: remove, debug only
            });
            vm.loading = true;
            vm.$http.post('/CheckInApiV2/Search', payload, vm.apiHeaders).then(
                response => {
                    vm.loading = false;
                    if (response.status === 200) {
                        if (response.data.error === 0) {
                            var results = JSON.parse(response.data.data);
                            console.log(results);
                            if (results.length > 1) {
                                vm.families = results;
                                vm.loadView('families');
                            } else if (results.length === 1) {
                                vm.families = [];
                                vm.loadAttendance(results[0].members);
                                vm.loadView('checkin');
                            } else {
                                vm.loadView('landing');
                                warning_swal('No results', 'No families found with that number, please try again.');
                            }
                        } else {
                            if (response.data.error === -6) {
                                vm.logout();
                            } else {
                                vm.loadView('landing');
                            }
                            warning_swal('Search Failed', response.data.data);
                        }
                    } else {
                        vm.loadView('landing');
                        warning_swal('Warning!', 'Something went wrong, try again later');
                    }
                },
                err => {
                    vm.loading = false;
                    vm.loadView('landing');
                    error_swal('Error', 'Something went wrong');
                }
            );
        },
        selectFamily(family) {
            this.loadAttendance(family.members);
            this.loadView('checkin');
        },
        loadAttendance(members) {
            let vm = this;
            vm.attendance = [];
            vm.members = members;
            members.forEach(function (member) {
                member.groups.forEach(function (group) {
                    var attend = member.id + '.' + group.id;
                    vm.$set(vm.attendance, attend, {
                        initial: group.checkedIn ? vm.CHECKEDIN : vm.ABSENT,
                        status: group.checkedIn ? vm.CHECKEDIN : vm.ABSENT,
                        changed: false
                    });
                });
            });
        },
        toggleAttendance(memberId, groupId) {
            let vm = this;
            var attend = memberId + '.' + groupId;
            var old = vm.attendance[attend];
            if (old !== undefined) {
                var status = old.status === vm.ABSENT ? vm.PRESENT : vm.ABSENT;
                vm.$set(vm.attendance, attend, {
                    initial: old.initial,
                    status: status,
                    changed: status !== old.initial
                });
            } else {
                // guest
                vm.$set(vm.attendance, attend, {
                    initial: vm.ABSENT,
                    status: vm.PRESENT,
                    changed: true
                });
            }
        },
        checkAllAttendance() {
            let vm = this;
            var keys = Object.keys(vm.attendance);
            for (var i = 0; i < keys.length; i++) {
                if (vm.attendance[keys[i]].status === vm.ABSENT) {
                    var attend = keys[i].split('.');
                    vm.toggleAttendance(attend[0], attend[1]);
                }
            }
            return false;
        },
        updateAttendance() {
            let vm = this;
            if (!vm.attendanceUpdated) {
                vm.loadView('landing');
                return;
            }
            vm.loading = true;
            var attendances = [];
            var keys = Object.keys(vm.attendance);
            for (var i = 0; i < keys.length; i++) {
                if (vm.attendance[keys[i]].changed) {
                    var attend = keys[i].split('.');
                    var bundle = {
                        peopleID: attend[0],
                        groups: [{
                            groupId: attend[1],
                            present: vm.attendance[keys[i]].status === vm.PRESENT
                        }]
                    };
                    attendances.push(bundle);
                }
            }
            var payload = vm.generatePayload({
                securityLabels: 0,
                guestLabels: true,
                locationLabels: true,
                nameTagAge: 18,
                attendances: attendances
            });
            vm.$http.post('/CheckInApiV2/UpdateAttend', payload, vm.apiHeaders).then(
                response => {
                    vm.loading = false;
                    if (response.status === 200) {
                        if (response.data.error === 0) {
                            var results = JSON.parse(response.data.data);
                            // todo: use profile settings for confirmation dialog
                            var timeout = setTimeout(function () {
                                swal.close();
                            }, 3000);
                            console.log(results);
                            vm.loadView('landing');
                            swal({
                                title: "You're all checked in",
                                text: "Don't forget your name tags",
                                type: "success",
                                showCancelButton: false,
                                confirmButtonClass: "btn-success",
                                confirmButtonText: "OK"
                            }, function () {
                                clearTimeout(timeout);
                                vm.loadView('landing');
                            });
                        } else {
                            if (response.data.error === -6) {
                                vm.logout();
                            } else {
                                vm.loadView('landing');
                            }
                            warning_swal('Checkin Failed', response.data.data);
                        }
                    } else {
                        vm.loadView('landing');
                        warning_swal('Warning!', 'Something went wrong, try again later');
                    }
                },
                err => {
                    vm.loading = false;
                    vm.loadView('landing');
                    error_swal('Error', 'Something went wrong');
                }
            );
        },
        postBarcode() {
            // todo: post barcode id to self check in endpoint
        }
    },
    mounted() {
        let vm = this;
        // init, fetch identity, profile, and show login or landing
        var profile = localStorage.getItem('profile');
        if (profile && profile.length) {
            vm.profile = JSON.parse(profile);
        }
        var kiosk = localStorage.getItem('kiosk');
        if (kiosk && kiosk.length) {
            vm.kiosk.name = kiosk;
        }
        var identity = localStorage.getItem('identity');
        if (identity && identity.length) {
            vm.identity = identity;
            vm.loadView('landing');
        } else {
            vm.loadView('login');
        }

        // event handlers to support idle behavior
        window.addEventListener('load', vm.resetIdleTimer, true);
        var events = ['mousedown', 'mousemove', 'keypress', 'scroll', 'touchstart'];
        events.forEach(function (name) {
            document.addEventListener(name, vm.resetIdleTimer, true);
        });
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
