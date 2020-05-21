<template>
    <div id="checkinSetup">
        <div class="col-lg-9">
            <div class="box box-responsive">
                <div class="box-content">
                    <div class="table-responsive">
                        <table id="profiles" class="table table-striped">
                            <thead>
                                <tr>
                                    <th style="width: 150px;">
                                        Check-In Profiles
                                    </th>
                                    <th style="width: 150px;">
                                        <a class="btn btn-success" v-on:click="CreateCheckinProfile()" href="#">
                                            <span class="glyphicon glyphicon-plus"></span>&nbsp;Add
                                        </a>
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="profile in CheckinProfiles" :key="profile.CheckinProfileId">
                                    <td>
                                        <a v-on:click="modalInfo(profile.CheckinProfileId)" class="blue-avaiable" href="#">
                                            {{profile.Name}}
                                        </a>
                                    </td>
                                    <td style="width: 50px;">
                                        <a v-on:click="deletePrompt(profile)" href="#" class="btn btn-sm btn-danger deleteprofile">
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
                    <form class="profilesettings-form" v-on:submit.prevent="submitSettingsForm">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">×</span></button>
                            <h4 v-if="CheckinProfile.Name === null" class="modal-title">New Check-In Profile</h4>
                            <h4 v-else class="modal-title">{{CheckinProfile.Name}}</h4>
                        </div>
                        <div class="modal-body">
                            <div class="row">
                                <div class="col-md-3">
                                    <label>Profile Name</label>
                                </div>
                                <div class="col-md-6">
                                    <input required v-model="CheckinProfile.Name" type="text" class="form-control" />
                                </div>
                            </div>
                            <br />
                            <ul class="nav nav-tabs">
                                <li class="active">
                                    <a href="#general" data-toggle="tab">General</a>
                                </li>
                                <li>
                                    <a href="#testing" data-toggle="tab">Testing</a>
                                </li>
                                <li>
                                    <a href="#security" data-toggle="tab">Security</a>
                                </li>
                                <li>
                                    <a href="#printing" data-toggle="tab">Printing</a>
                                </li>
                                <li>
                                    <a href="#design" data-toggle="tab">Design</a>
                                </li>
                            </ul>
                            <div class="tab-content">
                                <!---------------General Tab---------------------->
                                <div class="tab-pane active" id="general">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <label>Campus</label>
                                            <select class="form-control" v-model="CampusId">
                                                <option value="-1">All Campuses</option>
                                                <option v-for="campus in Campuses" :key="campus.Id" :value="campus.Id">{{campus.Description}}</option>
                                            </select>
                                        </div>
                                        <div class="col-md-6">
                                            <label>Logout Code</label>
                                            <input v-model="CheckinProfileSettings.Logout" type="number" min="00000" max="99999" class="form-control logoutCode" />
                                        </div>
                                    </div>
                                    <br />
                                    <div class="row">
                                        <div class="col-md-6">
                                            <label>Admin Code</label>
                                            <input v-model="CheckinProfileSettings.AdminPIN" type="number" min="00000" max="99999" class="form-control adminCode" />
                                        </div>
                                    </div>
                                    <br />
                                </div>
                                <!--------------------------Testing Tab--------------------------------->
                                <div class="tab-pane" id="testing">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <input v-model="CheckinProfileSettings.Testing" type="checkbox" />
                                            &nbsp;Enable Testing
                                        </div>
                                    </div>
                                    <br />
                                    <div class="row">
                                        <div class="col-md-6">
                                            <label v-if="CheckinProfileSettings.Testing === true">Test a specific day</label>
                                            <select v-if="CheckinProfileSettings.Testing === true" v-model="CheckinProfileSettings.TestDay" class="form-control">
                                                <option v-for="day in DaysOfTheWeek" :key="day.id" :value="day.id">{{day.text}}</option>
                                            </select>
                                        </div>
                                    </div>
                                    <br />
                                    <div class="row">
                                        <div class="col-md-10">
                                            <input v-model="CheckinProfileSettings.DisableTimer" type="checkbox" />
                                            &nbsp;Disable <i>'Are you still there'</i> prompt
                                        </div>
                                    </div>
                                    <br />
                                </div>
                                <!---------------------Security---------------------------------------------->
                                <div class="tab-pane" id="security">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <label>Show Check-In Confirmation</label>
                                            <select v-model="CheckinProfileSettings.ShowCheckinConfirmation" class="form-control">
                                                <option v-for="time in ShowCheckConfTimes" :key="time.id" :value="time.id">{{time.text}}</option>
                                            </select>
                                        </div>
                                    </div>
                                    <br />
                                </div>
                                <!---------------------Printing---------------------------------------------->
                                <div class="tab-pane" id="printing">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <input v-model="CheckinProfileSettings.Guest" type="checkbox" />
                                            &nbsp;Show Guest Label
                                        </div>
                                        <div class="col-md-6">
                                            <input v-model="CheckinProfileSettings.Location" type="checkbox" />
                                            &nbsp;Show Location Label
                                        </div>
                                    </div>
                                    <br />
                                    <div class="row">
                                        <div class="col-md-6">
                                            <label>Child Label Cutoff Age</label>
                                            <number-input v-model="CheckinProfileSettings.CutoffAge" :min="1" :max="99" inline controls class="form-control"></number-input>
                                        </div>
                                        <div class="col-md-6">
                                            <label>Security Label</label>
                                            <select class="form-control" v-model="CheckinProfileSettings.SecurityType">
                                                <option v-for="type in SecurityTypes" :key="type.id" :value="type.id">{{type.text}}</option>
                                            </select>
                                        </div>
                                    </div>
                                </div>
                                <!-------------------------------------Design---------------------------------->
                                <div class="tab-pane" id="design">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <label>Background Image</label>
                                            <input id="backgroundImage " type="file" ref="BGupload" v-on:change="onFileSelected" accept=".jpg,.jpeg,.png,image/jpeg,image/png" />
                                        </div>
                                    </div>
                                    <br />
                                    <div class="row">
                                        <div class="col-md-6">
                                            <div id="previewImage">
                                                <img v-if="BGImageURL" style="max-width: 30%;max-height: 30%;" :src="BGImageURL" />
                                            </div>
                                        </div>
                                    </div>
                                    <br />
                                    <div class="row">
                                        <div class="col-md-6">
                                            <p v-if="BGImageName">{{BGImageName}}</p>
                                        </div>
                                    </div>
                                    <br />
                                </div>
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
    import VueNumberInput from '@chenfengyuan/vue-number-input';
    export default {
        data: function () {
            return {
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
            }
        },
        components: {
           'number-input': VueNumberInput
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
                            this.CheckinProfile = response.data;
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
                            this.CheckinProfiles = response.data;
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
                            this.Campuses = response.data;
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
    }
</script>
