new Vue({
    el: '#checkinSetup',
    data: {
        CheckinProfiles: [{}],
        CheckinProfileId: null,
        CheckinProfileSettings: {},
        ProfileName: null,
        CheckinProfile: {},
        CampusId: null,
        Campuses: [{}],
        TimeLapseList: [
            { text: '30 minutes', id: 30 },
            { text: '45 minutes', id: 45 },
            { text: '1 hour', id: 60 },
            { text: '1 1/2 hours', id: 90 },
            { text: '2 hours', id: 120 },
            { text: '3 hours', id: 180 },
            { text: '4 hours', id: 240 },
            { text: '8 hours', id: 480 },
            { text: '12 hours', id: 720 }
        ],
        DaysOfTheWeek: [
            { text: 'Sunday', id: 0 },
            { text: 'Monday', id: 1 },
            { text: 'Tuesday', id: 2 },
            { text: 'Wednesday', id: 3 },
            { text: 'Thursday', id: 4 },
            { text: 'Friday', id: 5 },
            { text: 'Saturday', id: 6 }
        ],
        SecurityTypes: [
            { text: 'None', id: 0 },
            { text: 'Per Meeting', id: 1 },
            { text: 'Per Child', id: 2 },
            { text: 'Per Family', id: 3 }
        ],
        ShowCheckConfTimes: [
            { text: 'No', id: 0 },
            { text: '2 seconds', id: 2 },
            { text: '3 seconds', id: 3 },
            { text: '5 seconds', id: 5 }
        ],
        SelectedBGImage: null,
        BGImageName: null,
        BGImageURL: null
    },
    methods: {
        init() {
            this.GetCheckinProfiles();
            this.GetCampuses();
        },
        CreateCheckinProfile() {
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
        GetCheckinProfiles() {
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
        GetCampuses() {
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
        deletePrompt(profile) {
            let vm = this;
            swal({
                title: "Are you sure?",
                text: "You're about to delete the " + profile.Name + " profile.",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-confirm",
                confirmButtonText: "Delete it",
                showLoaderOnConfirm: true,
                closeOnConfirm: false
            }, function () {
                vm.deleteProfile(profile.CheckinProfileId);
            });
        },
        deleteProfile(CheckinProfileId) {
            this.$http.delete('/CheckinSetup/DeleteProfile/' + CheckinProfileId).then(
                response => {
                    if (response.status === 200) {
                        this.init();
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
        modalInfo(CheckinProfileId) {
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
        onFileSelected(event) {
            this.SelectedBGImage = event.target.files[0];
            this.BGImageName = this.SelectedBGImage.name;
            this.BGImageURL = URL.createObjectURL(this.SelectedBGImage);
        },
        validateSettingsForm() {
            let adminPIN = this.CheckinProfileSettings.AdminPIN;
            let logoutPIN = this.CheckinProfileSettings.Logout;
            if (!adminPIN || adminPIN.length < 4 || adminPIN.length > 6 || !$.isNumeric(adminPIN)) {
                error_swal('Invalid admin code', 'Please choose a numeric admin code that is between 4 and 6 digits long.');
                return false;
            }
            if (!logoutPIN || logoutPIN.length < 4 || logoutPIN.length > 6 || !$.isNumeric(logoutPIN)) {
                error_swal('Invalid logout code', 'Please choose a numeric logout code that is between 4 and 6 digits long.');
                return false;
            }
            if (adminPIN === logoutPIN) {
                error_swal('Invalid logout code', 'The admin code and logout code can not be the same.');
                return false;
            }
            return true;
        },
        submitSettingsForm() {
            if (this.validateSettingsForm()) {
                this.CheckinProfileSettings.CampusId = this.CampusId;
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
                            this.init();
                            success_swal('Success', 'Profile Saved. If kiosks are active, log out and log in again on each kiosk for changes to take effect.');
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
        ValidateCheckBoxes() {
            if (this.CheckinProfileSettings.CampusId === null) {
                this.CampusId = -1;
            } else {
                this.CampusId = this.CheckinProfileSettings.CampusId;
            }
        },
        clearBGInput() {
            const input = this.$refs.BGupload;
            input.type = 'text';
            input.type = 'file';
        }
    },
    mounted() {
        this.init();
    }
});

$(function () {
    $('.logoutCode, .adminCode').mask("00000", { placeholder: "00000", clearIfNotMatch: true });
});
