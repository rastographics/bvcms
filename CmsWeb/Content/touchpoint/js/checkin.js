//Vue.use(VueMask.VueMaskPlugin);
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
        adminMode: false,
        editingPerson: false,
        lastSearch: false,
        showDropOption: false,
        idleTimer: false,
        idleStage: 0,
        idleTimeout: 45000,
        profiles: [],
        families: [],
        members: [],
        classes: [],
        attendance: [],
        classData: {
            member: 0,
            showAll: 0
        },
        kiosk: {
            name: '',
            profile: 'default'
        },
        user: {
            name: '',
            password: ''
        },
        search: '',
        defaultProfile: {
            'name': 'Default',
            'BackgroundImageURL': null,
            'CampusId': 0,
            'CutoffAge': 18,
            'DisableTimer': false,
            'EarlyCheckin': 60,
            'GuestLabels': false,
            'LateCheckin': 60,
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
            if (loading && typeof($.block) === "function") {
                $.block();
            } else {
                $.unblockUI();
            }
        },
        search: function (current) {
            let vm = this;
            if (current.includes('-') && current.length === 32) {
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
        },
        formatTime: function (dt) {
            if (!dt) return '';
            var date = new Date(dt);
            var time = date.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true });
            return time;
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
            var len = this.search.replace(/\D/g, '').length;
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
        generatePayload(data, id = 0) {
            var inner = JSON.stringify(data);
            var outer = JSON.stringify({
                version: 3,
                device: 3,
                id: id,
                kiosk: this.kiosk.name,
                data: inner
            });
            var body = {
                data: outer
            };
            return Object.keys(body).map(key => key + '=' + body[key]).join('&');
        },
        cookie(name, value, days) {
            var expires;
            if (days) {
                var date = new Date();
                date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                expires = "; expires=" + date.toGMTString();
            } else {
                expires = "";
            }
            document.cookie = encodeURIComponent(name) + "=" + value + expires + "; path=/";
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
        initKeyboard(layout = 'numeric') {
            let isWindows = -1 < navigator.userAgent.indexOf('Windows');
            let isChromebook = -1 < navigator.userAgent.indexOf('CrOS');
            let vm = this;
            setTimeout(function () {
                vm.keyboard = new Keyboard({
                    onChange: (input) => {
                        vm.search = input;
                        if (vm.keyboard.options.layoutName === "shift") {
                            vm.keyboard.setOptions({
                                layoutName: "default"
                            });
                        }
                    },
                    onKeyPress: (button) => {
                        if (button === '{enter}') {
                            vm.find();
                        } else if (button === '{shift}') {
                            var currentLayout = vm.keyboard.options.layoutName;
                            let shiftToggle = currentLayout === "default" ? "shift" : "default";
                            vm.keyboard.setOptions({
                                layoutName: shiftToggle
                            });
                        }
                    },
                    onInit: () => {
                        $(".keyboard-input").focus();
                    },
                    layout: {
                        numeric: ["1 2 3", "4 5 6", "7 8 9", "{bksp} 0 {enter}"],
                        default: [
                            '` 1 2 3 4 5 6 7 8 9 0 - = {bksp}',
                            'q w e r t y u i o p [ ] \\',
                            'a s d f g h j k l ; \' {enter}',
                            'z x c v b n m , . / {shift}',
                            '{space}'
                        ],
                        shift: [
                            '~ ! @ # $ % ^ & * ( ) _ + {bksp}',
                            'Q W E R T Y U I O P { } |',
                            'A S D F G H J K L : " {enter}',
                            'Z X C V B N M < > ? {shift}',
                            '{space}'
                        ]
                    },
                    layoutName: layout,
                    display: {
                        '{bksp}': '<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" x="0px" y="0px" width="38px" viewBox="0 0 512 640" enable-background="new 0 0 512 512" style="margin-top:15px;fill:white;" xml:space="preserve"><path d="M436,136H184.3c-5.3,0-10.4,2.1-14.1,5.9l-108.3,100c-7.8,7.8-7.8,20.5,0,28.3l108.3,100c3.8,3.8,8.8,5.9,14.1,5.9H436  c11,0,20-9,20-20V156C456,145,447,136,436,136z M370.1,301.9c7.8,7.8,5.3,22.9,0,28.3c-3.9,3.9-18.6,9.7-28.3,0L296,284.3  l-45.9,45.9c-9.2,9.2-21.9,6.4-28.3,0c-7.8-7.8-7.8-20.5,0-28.3l45.9-45.9l-45.9-45.9c-7.8-7.8-7.8-20.5,0-28.3s20.5-7.8,28.3,0  l45.9,45.9l45.9-45.9c7.8-7.8,20.5-7.8,28.3,0c7.8,7.8,7.8,20.5,0,28.3L324.3,256L370.1,301.9z"/></svg>',
                        '{enter}': '<i class="fa fa-search"></i>'
                    },
                    theme: "hg-theme-default hg-layout-" + layout,
                    mergeDisplay: true,
                    autoUseTouchEvents: (!isWindows && !isChromebook), // use touch events on devices and browsers that support it
                    disableButtonHold: true,        // holding button only leads to one press
                    stopMouseDownPropagation: true, // prevent click bubble up on button press
                    disableCaretPositioning: true,  // force cursor to the end of the input (mask support)
                    preventMouseDownDefault: true   // prevent loss of focus on button press
                });
                $(".keyboard-input").on("input", function (e) {
                    vm.keyboard.setInput(e.target.value);
                    e.preventDefault();
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
            if (!['landing','login'].includes(vm.view) || vm.search.length) {
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
            this.showDropOption = false;
            if (this.view === 'landing' || this.view === 'admin') {
                this.keyboard.destroy();
            }
            if (newView === 'checkin' && !this.members.length) {
                newView = 'landing';
            }
            if (this.adminMode && newView === 'admin') {
                this.adminMode = false;
                return;
            }
            if (this.adminMode && newView === 'landing') {
                newView = 'namesearch';
            }
            this.view = newView;
            if (newView === 'landing' || newView === 'namesearch') {
                this.search = '';
                this.members = [];
                this.classes = [];
                this.families = [];
                this.attendance = [];
                this.reprintLabels = false;
                if (!this.profile || !this.kiosk.name.length) {
                    this.logout();
                    error_swal("Error", "Couldn't retrieve saved profile data. Please try logging in again.");
                    return;
                }
            }
            if (newView === 'landing' || newView === 'admin') {
                this.search = '';
                this.initKeyboard('numeric');
            }
        },
        logout() {
            localStorage.removeItem('identity');
            localStorage.removeItem('profile');
            this.cookie('Authorization', "", -1);
            this.search = '';
            this.identity = false;
            this.profile = false;
            this.adminMode = false;
            this.profiles = [];
            this.classes = [];
            this.loadView('login');
            this.getProfiles();
            window.location = "/CheckIn";
        },
        reset() {
            // called on session timeout if kiosk has been idle too long
            this.idleStage = 0;
            this.adminMode = false;
            this.loadView('landing');
            swal.close();
        },
        leaveAdminMode() {
            this.adminMode = false;
            this.loadView('landing');
        },
        useLastSearch() {
            this.search = this.lastSearch;
            this.find();
        },
        selectFamily(family) {
            this.loadAttendance(family.members);
            this.loadView('checkin');
        },
        editPerson(person) {
            this.editingPerson = person.id;
            this.getPerson(person.id);
        },
        addPerson() {
            this.editingPerson = {
                campus: 0,
                id: 0,
                familyID: 0,
                firstName: "",
                goesBy: "",
                altName: "",
                lastName: "",
                genderID: 0,
                birthday: null,
                primaryEmail: "",
                homePhone: null,
                workPhone: "",
                mobilePhone: "",
                maritalStatusID: 0,
                address: "",
                address2: "",
                city: "",
                state: "",
                zipCode: "",
                country: "",
                church: null,
                allergies: "",
                emergencyName: "",
                emergencyPhone: ""
            };
            this.loadView('editperson');
        },
        addToFamily() {
            this.editingPerson = 0;
            var hoh = this.members[0];
            this.getPerson(hoh.id);
        },
        joinClass(member) {
            this.classData.member = true;
            this.getClasses(member);
        },
        visitClass(member) {
            this.classData.member = false;
            this.getClasses(member);
        },
        tryAdminPin() {
            if (this.search === this.profile.AdminPIN) {
                this.adminMode = true;
            } else {
                warning_swal('Invalid', 'Invalid PIN.');
            }
            this.loadView('checkin');
        },
        dropClass(member, group) {
            let vm = this;
            swal({
                title: "Are you sure?",
                text: "Do you really want to remove " + member.name + " from " + group.name + "?",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-danger",
                confirmButtonText: "Yes",
                closeOnConfirm: true,
                allowOutsideClick: "true"
            }, function () {
                vm.updateMembership(member, group, true, false);
            });
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
                            if (vm.profiles.length) {
                                vm.kiosk.profile = vm.profiles[0].id;
                            }
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
                                    userId: profile.userID,
                                    genders: profile.genders,
                                    maritals: profile.maritals,
                                    states: profile.states,
                                    countries: profile.countries,
                                    campuses: profile.campuses
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
                                vm.cookie('Authorization', token);
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
        find(callback) {
            let vm = this;
            let successCallback = typeof callback === "function" ? callback : $.noop;
            var phone = vm.search.replace(/[^\d!]/g, '');

            if (vm.view === 'admin') {
                vm.tryAdminPin();
                return;
            }
            // handle special entry
            if (phone === vm.profile.Logout) {
                vm.logout();
                return false;
            }
            else if (phone === vm.profile.AdminPIN) {
                vm.adminMode = true;
                vm.loadView('namesearch');
                return false;
            }

            var isQrCode = vm.search.includes('-');
            if (isQrCode && vm.search === '666f7265-6c69-7365-2037-2f31362f3136') {
                vm.loadView('landing');
                swal({
                    title: "Test Scan",
                    text: "Your scanner is working and set up correctly!",
                    type: "success"
                });
                return false;
            }

            if (!vm.adminMode && !isQrCode && (phone.length < 4 || phone.length > 15)) {
                vm.loadView('landing');
                warning_swal('No results', 'No families found with that number, please try again.');
                return false;
            }
            var payload = vm.generatePayload({
                search: (vm.adminMode || isQrCode) ? vm.search : phone,
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
                                vm.lastSearch = vm.search;
                                vm.families = results;
                                vm.loadView('families');
                            } else if (results.length === 1) {
                                vm.lastSearch = vm.search;
                                if (vm.adminMode && !isQrCode) {
                                    // in admin mode, load the families view so we have the option to create a new family
                                    vm.families = results;
                                    vm.loadView('families');
                                    return;
                                }
                                vm.families = [];
                                var attendance = null;
                                if (response.data.argString) {
                                    // for auto populating attendance
                                    attendance = JSON.parse(response.data.argString).attendances;
                                }
                                vm.loadAttendance(results[0].members, attendance);
                                successCallback();
                                vm.loadView('checkin');
                            } else {
                                vm.loadView('landing');
                                if (vm.adminMode) {
                                    swal({
                                        title: "No results",
                                        text: "No families found",
                                        type: "warning",
                                        showCancelButton: true,
                                        confirmButtonClass: "btn-danger",
                                        confirmButtonText: "Add Person",
                                        cancelButtonText: "Search Again",
                                        closeOnConfirm: true,
                                        allowOutsideClick: "true"
                                    }, function () {
                                        vm.addPerson();
                                    });
                                } else {
                                    warning_swal('No results', 'No families found, please try again.');
                                }
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
        getPerson(id) {
            let vm = this;
            var payload = vm.generatePayload({}, id);
            vm.loading = true;
            vm.$http.post('/CheckInApiV2/GetPerson', payload, vm.apiHeaders).then(
                response => {
                    vm.loading = false;
                    if (response.status === 200) {
                        if (response.data.error === 0) {
                            var person = JSON.parse(response.data.data);
                            if (vm.editingPerson !== 0) {
                                // edit existing
                                if (person.birthday && person.birthday.length > 10) {
                                    person.birthday = person.birthday.slice(0, 10);
                                }
                                vm.editingPerson = person;
                            } else {
                                // add new to family
                                person.id = 0;
                                person.firstName = "";
                                person.goesBy = "";
                                person.altName = "";
                                person.genderID = 0;
                                person.birthday = null;
                                person.primaryEmail = "";
                                person.workPhone = "";
                                person.mobilePhone = "";
                                person.maritalStatusID = 0;
                                person.church = "";
                                person.allergies = "";
                                person.emergencyName = "";
                                person.emergencyPhone = "";
                                vm.editingPerson = person;
                            }
                            this.loadView('editperson');
                        } else {
                            if (response.data.error === -6) {
                                vm.logout();
                            }
                            warning_swal('Search Failed', response.data.data);
                        }
                    } else {
                        warning_swal('Warning!', 'Something went wrong, try again later');
                    }
                },
                err => {
                    console.log(err);
                    vm.loading = false;
                    error_swal('Error', 'Something went wrong');
                }
            );
        },
        savePerson() {
            let vm = this;
            var endpoint;
            var payload = vm.generatePayload(vm.editingPerson);
            if (vm.editingPerson.id) {
                endpoint = "/CheckInApiV2/EditPerson";
            } else {
                endpoint = "/CheckInApiV2/AddPerson";
            }
            vm.loading = true;
            vm.$http.post(endpoint, payload, vm.apiHeaders).then(
                response => {
                    vm.loading = false;
                    if (response.status === 200) {
                        if (response.data.error === 0) {
                            var results = JSON.parse(response.data.data);
                            vm.search = results.barcodeID.replace(/\"/g, "");
                            if (!vm.editingPerson.id) {
                                // go directly to visit class view after creating a person
                                vm.find(function () {
                                    vm.members.forEach(function (member) {
                                        if (member.id === results.peopleID) {
                                            vm.visitClass(member);
                                        }
                                    });
                                });
                            } else {
                                vm.find();
                            }
                        } else {
                            if (response.data.error === -6) {
                                vm.logout();
                            }
                            warning_swal('Save Failed', response.data.data);
                        }
                    } else {
                        warning_swal('Warning!', 'Something went wrong, try again later');
                    }
                },
                err => {
                    console.log(err);
                    vm.loading = false;
                    error_swal('Error', 'Something went wrong');
                }
            );
        },
        getClasses(member, showAll = 0) {
            let vm = this;
            vm.classes = [];
            vm.editingPerson = member;
            vm.classData.showAll = showAll;

            var dayID = new Date(vm.searchDay + 'T00:00').getDay();
            var payload = vm.generatePayload({
                peopleID: member.id,
                campusID: vm.profile.CampusId,
                dayID: dayID,
                showAll: showAll
            });
            vm.loading = true;
            vm.$http.post('/CheckInApiV2/GroupSearch', payload, vm.apiHeaders).then(
                response => {
                    vm.loading = false;
                    if (response.status === 200) {
                        if (response.data.error === 0) {
                            var results = JSON.parse(response.data.data);
                            if (results.length) {
                                vm.classes = results;
                                vm.loadView('classes');
                            } else {
                                warning_swal('No results', 'No classes available to join');
                            }
                        } else {
                            if (response.data.error === -6) {
                                vm.logout();
                            }
                            warning_swal('Search Failed', response.data.data);
                        }
                    } else {
                        warning_swal('Warning!', 'Something went wrong, try again later');
                    }
                },
                err => {
                    console.log(err);
                    vm.loading = false;
                    error_swal('Error', 'Something went wrong');
                }
            );
        },
        addVisitingOrgToState(person, org) {
            let vm = this;
            vm.members.forEach(function (member) {
                if (member.id === person.id) {
                    org.guest = true;
                    member.groups.push(org);
                    vm.loadClassData(member, org);
                    vm.toggleAttendance(person.id, org.id, org.date, true);
                }
            });
            vm.loadView('checkin');
        },
        updateMembership(person, org, memberStatus, join) {
            // memberStatus - true: member, false: visitor
            // join - true: join, false: drop
            let vm = this;

            if (join && !memberStatus) {
                // handling adding guests doesn't require any api call
                vm.addVisitingOrgToState(person, org);
                return;
            }
            var payload = vm.generatePayload({
                peopleID: person.id,
                orgID: org.id,
                join: join
            });
            vm.loading = true;
            vm.$http.post('/CheckInApiV2/UpdateMembership', payload, vm.apiHeaders).then(
                response => {
                    vm.loading = false;
                    if (response.status === 200) {
                        if (response.data.error === 0) {
                            var results = JSON.parse(response.data.data);
                            if (results.length) {
                                vm.search = results.replace(/\"/g, "");
                                if (join && memberStatus) {
                                    vm.find(function () {
                                        // immediately mark for check in after adding to org
                                        vm.toggleAttendance(person.id, org.id, org.date, true);
                                    });
                                } else {
                                    vm.find();
                                }
                            }
                        } else {
                            if (response.data.error === -6) {
                                vm.logout();
                            }
                            warning_swal('Membership update failed', response.data.data);
                        }
                    } else {
                        warning_swal('Warning!', 'Something went wrong, try again later');
                    }
                },
                err => {
                    console.log(err);
                    vm.loading = false;
                    error_swal('Error', 'Something went wrong');
                }
            );
        },
        loadClassData(member, group) {
            let vm = this;
            var disabled = false;
            var now = vm.timestamp();
            var start = vm.timestamp(group.date);
            // use the check in times from the class if present, otherwise fall back to the db default which is attached to the profile
            var early = group.EarlyCheckin != null ? group.EarlyCheckin : vm.profile.EarlyCheckin;
            var late = group.LateCheckin != null ? group.LateCheckin : vm.profile.LateCheckin;
            if (early && (start - early > now) && start > now) {
                disabled = true;
            }
            if (late && (start + late < now) && start < now) {
                disabled = true;
            }
            var attend = member.id + '.' + group.id + '.' + group.date;
            vm.$set(vm.attendance, attend, {
                initial: group.checkedIn ? vm.CHECKEDIN : vm.ABSENT,
                status: group.checkedIn ? vm.CHECKEDIN : vm.ABSENT,
                disabled: disabled,
                changed: false
            });
        },
        loadAttendance(members, attendances) {
            let vm = this;
            vm.attendance = [];
            vm.members = members;
            members.forEach(function (member) {
                member.groups.forEach(function (group) {
                    vm.loadClassData(member, group);
                });
            });
            if (attendances && attendances.length) {
                attendances.forEach(function (attendance) {
                    attendance.groups.forEach(function (group) {
                        var attend = attendance.peopleID + '.' + group.groupID + '.' + group.datetime;
                        var old = vm.attendance[attend];
                        if (old.status !== group.present) {
                            vm.toggleAttendance(attendance.peopleID, group.groupID, group.datetime);
                        }
                    });
                });
            }
        },
        toggleAttendance(memberId, groupId, date, quiet = false) {
            let vm = this;
            if (vm.showDropOption === memberId) return;
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
            vm.adminMode = false;
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
            vm.cookie('Authorization', identity);
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
            if (vm.view === 'landing' || vm.view === 'namesearch') {
                $(".keyboard-input").focus();
            }
        });

        $('.CheckInLoading').hide();
        $('.checkin').show();
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
