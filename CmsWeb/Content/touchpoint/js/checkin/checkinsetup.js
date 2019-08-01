var multipleGateway = new Vue({
    el: '#checkinSetup',
    data: {
        CheckinProfiles: [{}],
        CheckinProfileId: null,
        CheckinProfileSettings: {},
        ProfileName: null,
        CheckinProfile: {},
        CampusId: null,
        Campuses: [{}],
        TimeLapseList: [{}],
        DaysOfTheWeek: [{}],
        SecurityTypes: [{}],
        ShowCheckConfTimes: [{}],
        EarlyCheck: null,
        LateCheck: null,
        SelectedBGImage: null,
        BGImageName: null,
        BGImageURL: null
    },
    methods: {
        myFunctionOnLoad: function () {
            this.GetCheckinProfiles();
            this.GetCampuses();
            this.SetTimeLapseList();
            this.SetDaysOfTheWeek();
            this.SetSecurityTypes();
            this.SetShowCheckConfTimes();
        },
        CreateCheckinProfile: function () {
            this.$http.get('/CheckinSetup/CreateCheckinProfile').then(
                response => {
                    if (response.status === 200) {
                        this.CheckinProfile = response.body;
                        this.CheckinProfileId = this.CheckinProfile.CheckinProfileId;
                        this.CheckinProfileSettings = this.CheckinProfile.CheckinProfileSettings;
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
        GetCampuses: function () {
            this.$http.get('/CheckinSetup/GetCampuses').then(
                response => {
                    if (response.status === 200) {
                        this.Campuses = response.body;
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
        deleteProfile: function (CheckinProfileId) {
            this.$http.delete('/CheckinSetup/DeleteProfile/' + CheckinProfileId).then(
                response => {
                    if (response.status === 200) {
                        this.myFunctionOnLoad();
                        success_swal('Success', 'Profile Deleted');
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
        modalInfo: function (CheckinProfileId) {
            if (CheckinProfileId !== 0) {
                this.CheckinProfileId = CheckinProfileId;
                this.CheckinProfile = this.CheckinProfiles.filter(function (item) {
                    return item.CheckinProfileId === CheckinProfileId;
                })[0];
                this.CheckinProfileSettings = this.CheckinProfile.CheckinProfileSettings;
            }
            this.BGImageURL = this.CheckinProfileSettings.BackgroundImageURL;
            this.BGImageName = this.CheckinProfileSettings.BackgroundImageName;
            this.ValidateCheckBoxes();
            this.clearBGInput();
            $('#config-modal').modal();
        },
        onFileSelected: function (event) {
            this.SelectedBGImage = event.target.files[0];
            this.BGImageName = this.SelectedBGImage.name;
            this.BGImageURL = URL.createObjectURL(this.SelectedBGImage);
        },
        settingsForm: function () {
            this.CheckinProfileSettings.CampusId = this.CampusId;
            this.CheckinProfileSettings.EarlyCheckin = this.EarlyCheck;
            this.CheckinProfileSettings.LateCheckin = this.LateCheck;
            const jsonD = {
                CheckinProfileId: this.CheckinProfileId,
                Name: this.CheckinProfile.Name,
                CheckinProfileSettings: this.CheckinProfileSettings
            };
            const formData = new FormData();
            if (this.SelectedBGImage !== null) {
                formData.append('bgImage', this.SelectedBGImage);
            }
            formData.append('jsonD', JSON.stringify(jsonD));
            this.$http.post('/CheckinSetup/InsertCheckinProfile', formData, {
                CheckinProfileId: this.CheckinProfileId,
                Name: this.CheckinProfile.Name,
                CheckinProfileSettings: this.CheckinProfileSettings
            }).then(
                response => {
                    if (response.status === 201) {
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
        },
        ValidateCheckBoxes: function () {
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
        SetShowCheckConfTimes: function () {
            this.ShowCheckConfTimes = [
                { text: 'No', id: 0 },
                { text: '2 seconds', id: 2 },
                { text: '3 seconds', id: 3 },
                { text: '5 seconds', id: 5 }
            ]
        },
        clearBGInput: function () {
            const input = this.$refs.BGupload;
            input.type = 'text';
            input.type = 'file';
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

$(function () {
    $('.logoutCode').mask("00000", { placeholder: "00000", clearIfNotMatch: true });
});
