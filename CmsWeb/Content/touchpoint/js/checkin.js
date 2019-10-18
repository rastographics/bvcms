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
        },
        'search.phone': function (current, previous) {
            let vm = this;
            if (current.includes('!') && !previous.includes('!')) {
                setTimeout(vm.find, 200);
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
                return '?X##############';
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
                                cookie('Authorization', token);
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
            cookie('Authorization', "", -1);
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
            var phone = vm.search.phone.replace(/[^\d!]/g, '');
            // handle special entry
            if (phone === vm.profile.Logout) {
                vm.logout();
                return false;
            }
            var isQrCode = phone.includes('!');
            if (isQrCode && phone === '!0000') {
                vm.loadView('landing');
                swal({
                    title: "Test Scan",
                    text: "Your scanner is working and set up correctly!",
                    type: "success"
                });
                return false;
            }

            if (!isQrCode && (phone.length < 4 || phone.length > 15)) {
                vm.loadView('landing');
                warning_swal('No results', 'No families found with that number, please try again.');
                return false;
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
                                var attendance = null;
                                if (response.data.argString) {
                                    // for auto populating attendance
                                    attendance = JSON.parse(response.data.argString).attendances;
                                }
                                vm.loadAttendance(results[0].members, attendance);
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
        loadAttendance(members, attendances) {
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
            attendances.forEach(function (attendance) {
                attendance.groups.forEach(function (group) {
                    var attend = attendance.peopleID + '.' + group.groupID + '.' + group.datetime;
                    var old = vm.attendance[attend];
                    if (old.status !== group.present) {
                        vm.toggleAttendance(attendance.peopleID, group.groupID, group.datetime);
                    }
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
        scannerTest() {
            // disable the timer for testing
            this.profile.DisableTimer = true;
            this.resetIdleTimer();
            var content = document.createElement("span");
            content.innerHTML = 'You will see a success message if it\'s working properly. Make sure you are using a 2D scanner and that the scanner is set to \'ASCII Mode\'.<img style="display: block; margin: 0px auto; width: 200px; height: 200px;" src = "data:image/jpeg;base64, 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" />';
            swal({
                title: "Scan this barcode",
                confirmButtonText: "OK",
                allowOutsideClick: "true"
            });
            $('.sweet-alert p').append(content);
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

function cookie(name, value, days) {
    var expires;

    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toGMTString();
    } else {
        expires = "";
    }
    document.cookie = encodeURIComponent(name) + "=" + value + expires + "; path=/";
}

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
