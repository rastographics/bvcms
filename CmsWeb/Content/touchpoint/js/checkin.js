Vue.use(VueMask.VueMaskPlugin);
Mousetrap.unbind('/');

var Keyboard = window.SimpleKeyboard.default;

new Vue({
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
        idleTimeout: 45000,
        profiles: [],
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
        defaultProfile: {
            'name': 'Default',
            'BackgroundImageURL': null,
            'CampusId': 0,
            'CutoffAge': 18,
            'DisableTimer': false,
            'EarlyCheckIn': 60,
            'GuestLabels': false,
            'LateCheckIn': 60,
            'Testing': false,
            'LocationLabels': false,
            'Logout': '12345',
            'SecurityType': 0,
            'ShowCheckinConfirmation': 5
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
            if (len < 8) {
                return '###-###########';
            } else if (len < 11) {
                return '(###) ###-#########';
            } else {
                return '###############';
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
        },
        customStyles: function () {
            if (this.view !== 'landing' || !this.profile.BackgroundImageURL) return {};
            return {
                'background': 'url(' + this.profile.BackgroundImageURL + ') no-repeat center center fixed',
                'background-size': 'cover',
                'position': 'absolute',
                'top': '0',
                'width': '100vw',
                'height': '100vh'
            };
        },
        searchDay: function () {
            // use today, unless we're in test mode
            var date = new Date();
            if (this.profile.Testing) {
                date.setDate(date.getDate() + ((7 - date.getDay()) % 7 + this.profile.TestDay) % 7);  // sunday = 0
            }
            var month = date.getMonth() + 1;
            var day = date.getDate();
            date = date.getFullYear() + '-' + (month > 9 ? '' : '0') + month + '-' + (day > 9 ? '' : '0') + day;
            return date;
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
        timestamp(dt) {
            var d = new Date(dt);
            if (isNaN(d.getTime())) {
                d = new Date();
            }
            if (this.profile.Testing) {
                return d.getHours() * 60 + d.getMinutes();
            } else {
                return d.getTime() / 1000 / 60;
            }
        },
        initKeyboard() {
            let vm = this;
            setTimeout(function () {
                vm.keyboard = new Keyboard({
                    onChange: (input) => {
                        vm.search.phone = input;
                    },
                    onKeyPress: (button) => {
                        if (button === '{enter}') {
                            vm.find();
                        }
                    },
                    onInit: () => {
                        $(".keyboard-input").focus();
                    },
                    layout: {
                        default: ["1 2 3", "4 5 6", "7 8 9", "{bksp} 0 {enter}"]
                    },
                    display: {
                        '{bksp}': '<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" x="0px" y="0px" width="38px" viewBox="0 0 512 640" enable-background="new 0 0 512 512" style="margin-top:15px;fill:white;" xml:space="preserve"><path d="M436,136H184.3c-5.3,0-10.4,2.1-14.1,5.9l-108.3,100c-7.8,7.8-7.8,20.5,0,28.3l108.3,100c3.8,3.8,8.8,5.9,14.1,5.9H436  c11,0,20-9,20-20V156C456,145,447,136,436,136z M370.1,301.9c7.8,7.8,5.3,22.9,0,28.3c-3.9,3.9-18.6,9.7-28.3,0L296,284.3  l-45.9,45.9c-9.2,9.2-21.9,6.4-28.3,0c-7.8-7.8-7.8-20.5,0-28.3l45.9-45.9l-45.9-45.9c-7.8-7.8-7.8-20.5,0-28.3s20.5-7.8,28.3,0  l45.9,45.9l45.9-45.9c7.8-7.8,20.5-7.8,28.3,0c7.8,7.8,7.8,20.5,0,28.3L324.3,256L370.1,301.9z"/></svg>',
                        '{enter}': '<i class="fa fa-search"></i>'
                    },
                    theme: "hg-theme-default hg-layout-numeric numeric-theme",
                    autoUseTouchEvents: true,       // use touch events on devices and browsers that support it
                    disableButtonHold: true,        // holding button only leads to one press
                    stopMouseDownPropagation: true, // prevent click bubble up on button press
                    disableCaretPositioning: true,  // force cursor to the end of the input (mask support)
                    preventMouseDownDefault: true   // prevent loss of focus on button press
                });
                $(".keyboard-input").on("input", function (e) {
                    vm.keyboard.setInput(e.target.value);
                });
            }, 100);
        },
        resetIdleTimer() {
            let vm = this;
            clearTimeout(vm.idleTimer);
            if (!vm.profile.DisableTimer) {
                vm.idleTimer = setTimeout(vm.handleIdle, vm.idleStage === 1 ? '7000' : vm.idleTimeout);
            }
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
                vm.resetIdleTimer();
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
        getProfiles() {
            let vm = this;
            vm.loading = true;
            vm.$http.get('/CheckInApiV2/GetProfiles')
            .then(
                response => {
                    vm.loading = false;
                    if (response.status === 200) {
                        vm.profiles = response.data;
                    }
                    else {
                        warning_swal('Couldn\'t load profiles', 'Something went wrong, try again later');
                    }
                },
                err => {
                    console.log(err);
                    vm.loading = false;
                    error_swal('Error', 'Unable to load profiles.');
                }
            );
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
                                var settings = null;
                                var profile = JSON.parse(response.data.data);
                                profile = {
                                    userName: profile.userName,
                                    userId: profile.userID
                                };
                                // load profile settings based on dropdown selection, or use the defaults
                                vm.profiles.forEach((p) => {
                                    if (vm.kiosk.profile === p.id) {
                                        settings = p;
                                    }
                                });
                                if (settings === null) {
                                    settings = vm.defaultProfile;
                                    if (vm.kiosk.profile !== 'default') {
                                        warning_swal('Couldn\'t load the chosen profile.', 'Using default settings instead. To try again, type 12345 in the phone entry to logout.');
                                    }
                                }
                                // apply settings to the profile
                                Object.assign(profile, settings);
                                localStorage.setItem('identity', token);
                                localStorage.setItem('kiosk', vm.kiosk.name);
                                localStorage.setItem('profile', JSON.stringify(profile));
                                vm.identity = token;
                                vm.profile = profile;
                                vm.loadView('landing');
                                console.log('Profile ' + vm.profile.name + ' loaded');
                                console.log(vm.profile);
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
                        console.log(err);
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
            this.search.phone = '';
            this.identity = false;
            this.profile = false;
            this.profiles = [];
            this.loadView('login');
            this.getProfiles();
        },
        reset() {
            // called on session timeout if kiosk has been idle too long
            this.idleStage = 0;
            this.loadView('landing');
            swal.close();
        },
        find() {
            let vm = this;
            var phone = vm.search.phone.replace(/\D/g, '');
            // handle special entry
            if (phone === vm.profile.Logout) {
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
                campus: vm.profile.CampusId,
                date: vm.searchDay
            });
            vm.loading = true;
            vm.$http.post('/CheckInApiV2/Search', payload, vm.apiHeaders).then(
                response => {
                    vm.loading = false;
                    if (response.status === 200) {
                        if (response.data.error === 0) {
                            var results = JSON.parse(response.data.data);
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
                    console.log(err);
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
                    var disabled = false;
                    var now = vm.timestamp();
                    var start = vm.timestamp(group.date);
                    if (vm.profile.EarlyCheckIn && (start - vm.profile.EarlyCheckIn > now) && start > now) {
                        disabled = true;
                    }
                    if (vm.profile.LateCheckIn && (start + vm.profile.LateCheckIn < now) && start < now) {
                        disabled = true;
                    }
                    var attend = member.id + '.' + group.id + '.' + group.date;
                    vm.$set(vm.attendance, attend, {
                        initial: group.checkedIn ? vm.CHECKEDIN : vm.ABSENT,
                        status: group.checkedIn ? vm.CHECKEDIN : vm.ABSENT,
                        disabled: disabled,
                        changed: false
                    });
                });
            });
        },
        toggleAttendance(memberId, groupId, date, quiet = false) {
            let vm = this;
            var attend = memberId + '.' + groupId + '.' + date;
            var old = vm.attendance[attend];
            if (old !== undefined) {
                if (!old.disabled) {
                    var status = old.status === vm.ABSENT ? vm.PRESENT : vm.ABSENT;
                    vm.$set(vm.attendance, attend, {
                        initial: old.initial,
                        status: status,
                        disabled: false,
                        changed: status !== old.initial
                    });
                } else {
                    if (!quiet) {
                        warning_swal('Can\'t check in', 'This group isn\'t available for check in right now. Please check in with a staff member.');
                    }
                }
            } else {
                // guest
                vm.$set(vm.attendance, attend, {
                    initial: vm.ABSENT,
                    status: vm.PRESENT,
                    disabled: false,
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
                    vm.toggleAttendance(attend[0], attend[1], attend[2], true);
                }
            }
            return false;
        },
        uncheckAllAttendance() {
            let vm = this;
            var keys = Object.keys(vm.attendance);
            for (var i = 0; i < keys.length; i++) {
                if (vm.attendance[keys[i]].status === vm.PRESENT) {
                    var attend = keys[i].split('.');
                    vm.toggleAttendance(attend[0], attend[1], attend[2], true);
                }
            }
            return false;
        },
        toggleReprintAll() {
            let vm = this;
            vm.reprintLabels = !vm.reprintLabels;
            var keys = Object.keys(vm.attendance);
            if (vm.reprintLabels) {
                var reprint = false;
                for (var i = 0; i < keys.length; i++) {
                    var attend = vm.attendance[keys[i]];
                    if (attend.status === vm.CHECKEDIN || attend.initial === vm.CHECKEDIN) {
                        reprint = true;
                        vm.$set(vm.attendance, keys[i], {
                            initial: vm.CHECKEDIN,
                            status: vm.PRESENT,
                            disabled: attend.disabled,
                            changed: true
                        });
                    }
                }
                if (!reprint) {
                    vm.reprintLabels = false;
                    warning_swal('Nothing to reprint', 'No labels have been printed yet.');
                }
            } else {
                for (i = 0; i < keys.length; i++) {
                    if (vm.attendance[keys[i]].initial === vm.CHECKEDIN) {
                        vm.$set(vm.attendance, keys[i], {
                            initial: vm.CHECKEDIN,
                            status: vm.CHECKEDIN,
                            disabled: vm.attendance[keys[i]].disabled,
                            changed: false
                        });
                    }
                }
            }
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
                    var peopleId = attend[0];
                    var group = {
                        groupID: attend[1],
                        present: vm.attendance[keys[i]].status === vm.PRESENT,
                        datetime: attend[2]
                    };
                    if (attendances.hasOwnProperty(peopleId)) {
                        attendances[peopleId].groups.push(group);
                    } else {
                        attendances[peopleId] = {
                            peopleID: peopleId,
                            groups: [group]
                        };
                    }
                    
                }
            }
            var payload = vm.generatePayload({
                securityLabels: vm.profile.SecurityType,
                guestLabels: vm.profile.GuestLabels,
                locationLabels: vm.profile.LocationLabels,
                nameTagAge: vm.profile.CutoffAge,
                attendances: Object.values(attendances)
            });
            vm.$http.post('/CheckInApiV2/UpdateAttend', payload, vm.apiHeaders).then(
                response => {
                    vm.loading = false;
                    if (response.status === 200) {
                        if (response.data.error === 0) {
                            if (vm.profile.ShowCheckinConfirmation) {
                                var timeout = setTimeout(function () {
                                    swal.close();
                                }, vm.profile.ShowCheckinConfirmation * 1000);
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
                                vm.loadView('landing');
                            }
                            
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
                    console.log(err);
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
        vm.getProfiles();

        // init, fetch identity, profile, and show login or landing
        var profile = localStorage.getItem('profile');
        if (profile && profile.length) {
            vm.profile = JSON.parse(profile);
            console.log('Profile loaded');
            console.log(vm.profile);
        }
        var kiosk = localStorage.getItem('kiosk');
        if (kiosk && kiosk.length) {
            vm.kiosk.name = kiosk;
            console.log('Kiosk: ' + vm.kiosk.name);
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
        var events = ['mousedown', 'scroll', 'touchstart'];
        events.forEach(function (name) {
            document.addEventListener(name, vm.resetIdleTimer, true);
        });

        // redirect focus on stray clicks to support barcode scanner
        $('.checkin').click(function () {
            if (vm.view === 'landing') {
                $(".keyboard-input").focus();
            }
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
