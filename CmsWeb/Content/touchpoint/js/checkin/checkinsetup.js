var multipleGateway = new Vue({
    el: '#checkinSetup',
    data: {
        CheckinProfiles: [{}],
        CheckinProfileId: null,
        CheckinProfileSettings: [{}],
        ProfileName: null,
        CheckinProfile: [{}],
        CampusId: null,
        Campuses: [{}],
        TimeLapseList: [{}],
        DaysOfTheWeek: [{}],
        SecurityTypes: [{}],
        EarlyCheck: null,
        LateCheck: null,
        isNew: false
    },
    methods: {
        myFunctionOnLoad: function () {
            this.GetCheckinProfiles();
            this.GetCampuses();
            this.SetTimeLapseList();
            this.SetDaysOfTheWeek();
            this.SetSecurityTypes();
        },
        CreateCheckinProfile: function () {
            this.$http.get('/CheckinSetup/CreateCheckinProfile').then(
                response => {
                    if (response.status === 200) {
                        this.CheckinProfile = response.body;
                        this.CheckinProfileSettings = this.CheckinProfile.CheckinProfileSettings;
                        this.isNew = true;
                        this.modalInfo(0);
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
        GetCheckinProfiles: function () {
            this.$http.get('/CheckinSetup/GetCheckinProfiles').then(
                response => {
                    if (response.status === 200) {
                        this.CheckinProfiles = response.body;
                        console.log(this.CheckinProfiles);
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
        GetCheckinProfilesSettings: function () {
            this.$http.get('/CheckinSetup/GetCheckinProfileSettings/' + this.CheckinProfileId).then(
                response => {
                    if (response.status === 200) {
                        this.CheckingProfileSettings = response.body;
                        this.ProfileName = this.CheckinProfiles.filter(function (item) {
                            return item.CheckingProfileId === this.CheckinProfileId;
                        })[0].Name;
                    }
                    else {
                        console.log(response);
                        warning_swal('Warning!', 'Something went wrong, try again later');
                        this.GetCheckinProfilesSettings = [{}];
                    }
                },
                err => {
                    console.log(err);
                    error_swal('Fatal Error!', 'We are working to fix it');
                    this.GetCheckinProfilesSettings = [{}];
                }
            );
        },
        GetCampuses: function () {
            this.$http.get('/CheckinSetup/GetCampuses').then(
                response => {
                    if (response.status === 200) {
                        this.Campuses = response.body;
                        console.log(this.Campuses);
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
        SetTimeLapseList: function () {
            this.TimeLapseList = [
                { text: '30 minutes', id: 30 },
                { text: '45 minutes', id: 45 },
                { text: '1 hour', id: 60 },
                { text: '1 1/2 hours', id: 90 },
                { text: '2 hours', id: 120 },
                { text: '3 hours', id: 180 },
                { text: '4 hours', id: 240 },
                { text: '8 hours', id: 480 },
                { text: '12 hours', id: 720 }
            ]
        },
        SetDaysOfTheWeek: function () {
            this.DaysOfTheWeek = [
                { text: 'Sunday', id: 0 },
                { text: 'Monday', id: 1 },
                { text: 'Tuesday', id: 2 },
                { text: 'Wednesday', id: 3 },
                { text: 'Thursday', id: 4 },
                { text: 'Friday', id: 5 },
                { text: 'Saturday', id: 6 }
            ]
        },
        SetSecurityTypes: function () {
            this.SecurityTypes = [
                { text: 'None', id: 0 },
                { text: 'Per Meeting', id: 1 },
                { text: 'Per Child', id: 2 },
                { text: 'Per Family', id: 3 }
            ]
        },
        modalInfo: function (CheckinProfileId) {
            this.DetailValue = [];
            console.log(CheckinProfileId);
            this.CheckinProfileId = CheckinProfileId;
            console.log(this.CheckinProfileId);
            if (this.CheckinProfileId !== 0) {
                this.CheckinProfile = this.CheckinProfiles.filter(function (item) {
                    return item.CheckingProfileId === this.CheckinProfileId;
                })[0];
                this.isNew = false;
            }
            console.log(this.CheckinProfile);
            this.ValidateCheckBoxes();
            $('#config-modal').modal();
        },
        ValidateCheckBoxes: function () {
            console.log(this.CheckinProfileSettings);
            if (this.CheckinProfileSettings.CampusId === null) {
                this.CampusId = -1;
            } else {
                this.CampusId = this.CheckinProfileSettings.CampusId;
            }
            if (this.CheckinProfileSettings.EarlyCheckin === null) {
                this.EarlyCheck = -1;
            } else {
                this.EarlyCheck = this.CheckinProfileSettings.EarlyCheckin;
            }
            if (this.CheckinProfileSettings.LateCheckin === null) {
                this.LateCheck = -1;
            } else {
                this.LateCheck = this.CheckinProfileSettings.LateCheckin;
            }
        },
        settingsForm: function () {
            this.CheckinProfileSettings.CampusId = this.CampusId;
            this.CheckinProfileSettings.EarlyCheckin = this.EarlyCheck;
            this.CheckinProfileSettings.LateCheckin = this.LateCheck;
            this.$http.post('/CheckinSetup/InsertCheckinProfile', {
                CheckinProfileId: this.CheckinProfileId,
                Name: this.CheckinProfile.Name,
                CheckinProfileSettings: this.CheckinProfileSettings
            }).then(
                response => {
                    if (response.status === 200) {
                        this.myFunctionOnLoad();
                        success_swal('Success', 'Profile Saved');
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
        //    deleteProcess: function (ProcessId) {
        //        this.$http.post('/Gateway/DeleteProcessAccount', {
        //            ProcessId: ProcessId
        //        }).then(
        //            response => {
        //                if (response.status === 200) {
        //                    this.myFunctionOnLoad();
        //                    success_swal('Success', 'Configuration Saved');
        //                }
        //                else {
        //                    console.log(response);
        //                    warning_swal('Warning!', 'Something went wrong, try again later');
        //                }
        //            },
        //            err => {
        //                console.log(err);
        //                error_swal('Fatal Error!', 'We are working to fix it');
        //            }
        //        );
        //    }
        //},
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
