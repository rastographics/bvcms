<template>
    <div>
        <ul class="nav nav-tabs">
            <li class="active">
                <a href="#Details-tab" aria-controls="Details-tab" data-toggle="tab" aria-expanded="true">Details</a>
            </li>
            <li>
                <a href="#Email-tab" aria-controls="Email-tab" data-toggle="tab" aria-expanded="false">Email</a>
            </li>
        </ul>
        <div class="tab-content">
            <div id="Details-tab" class="tab-pane fade active in">
                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group">
                            <label class="control-label">Page Name</label>
                            <input type="text" v-model="page.PageName" class="form-control" />
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="form-group">
                            <label class="control-label">Page URL <a href="#" data-toggle="popover" data-placement="right" data-trigger="focus" data-title="Page URL" data-content="The publically accessible URL for this giving page. This can't be changed later, and must be unique."><i class="fa fa-info-circle"></i></a></label>
                            <div class="input-group">
                                <span class="input-group-addon" id="pageUrl" style="font-size:13px;">{{givePrefix}}</span>
                                <input type="text" class="form-control" v-model="page.PageUrl" aria-describedby="pageUrl" :disabled="page.GivingPageId">
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group">
                            <label class="control-label">Giving Types <a href="#" data-toggle="popover" data-placement="right" data-trigger="focus" data-title="Giving Type" data-content="The types of giving allowed on this page. You can allow more than one. Be sure to set the cooresponding emails for each type in the 'Email' tab."><i class="fa fa-info-circle"></i></a></label>
                            <MultiSelect v-model="pageTypes"
                                         :options="pageTypeList"
                                         :searchable="true"
                                         :close-on-select="true"
                                         :multiple="true"
                                         :allowEmpty="false"
                                         trackBy="Id"
                                         label="Name"></MultiSelect>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="form-group">
                            <label class="control-label">Page Shell <a href="#" data-toggle="popover" data-placement="right" data-trigger="focus" data-title="Page Shell" data-content="You can theme this giving page by creating an HTML content file with the keyword 'shell' and applying it here."><i class="fa fa-info-circle"></i></a></label>
                            <MultiSelect v-model="page.SkinFileId"
                                         :options="shellList"
                                         :searchable="true"
                                         :close-on-select="true"
                                         :allowEmpty="true"
                                         trackBy="Id"
                                         label="Name"></MultiSelect>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group">
                            <label class="control-label">Default Fund</label>
                            <MultiSelect v-model="page.DefaultFundId"
                                         :options="fundsList"
                                         :searchable="true"
                                         :close-on-select="true"
                                         :allowEmpty="true"
                                         trackBy="Id"
                                         label="Name"></MultiSelect>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="form-group">
                            <label class="control-label">Available Funds</label>
                            <MultiSelect v-model="selectedAvailableFunds"
                                         :options="fundsList"
                                         :searchable="true"
                                         :close-on-select="true"
                                         :multiple="true"
                                         trackBy="Id"
                                         label="Name"></MultiSelect>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group">
                            <label class="control-label">Entry Point</label>
                            <MultiSelect v-model="page.EntryPointId"
                                         :options="entryPointList"
                                         :searchable="true"
                                         :close-on-select="true"
                                         trackBy="Id"
                                         label="Name"></MultiSelect>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="form-group">
                            <label class="control-label">Disabled Redirect</label>
                            <input type="text" v-model="page.DisabledRedirect" class="form-control" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-12" style="margin-bottom: 15px;">
                        <div class="pull-right">
                            <a href="/Giving" class="btn btn-default" style="margin-right: 10px;">Cancel</a>
                            <a class="btn btn-primary" @click="saveGivingPage">Save</a>
                        </div>
                    </div>
                </div>
            </div>
            <div id="Email-tab" class="tab-pane fade">
                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group">
                            <label class="control-label">Top Text</label>
                            <input type="text" v-model="page.TopText" class="form-control" />
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="form-group">
                            <label class="control-label">Thank you message</label>
                            <input type="text" v-model="page.ThankYouText" class="form-control" />
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="form-group">
                            <label class="control-label">Online Notify Person</label>
                            <MultiSelect v-model="page.OnlineNotifyPerson"
                                         :options="onlineNotifyPersonList"
                                         :searchable="true"
                                         :close-on-select="true"
                                         trackBy="Id"
                                         label="Name"
                                         :multiple="true"></MultiSelect>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="form-group" v-if="selectedPageTypes & TYPE_PLEDGE">
                            <label class="control-label">Pledge Confirmation Email</label>
                            <MultiSelect v-model="page.ConfirmationEmailPledgeId"
                                         :options="confirmationEmailList"
                                         :searchable="true"
                                         :close-on-select="true"
                                         trackBy="Id"
                                         label="Name"></MultiSelect>
                        </div>
                    </div>
                    <div class="col-md-6" v-if="selectedPageTypes & TYPE_ONE_TIME">
                        <div class="form-group">
                            <label class="control-label">One Time Confirmation Email</label>
                            <MultiSelect v-model="page.ConfirmationEmailOneTimeId"
                                         :options="confirmationEmailList"
                                         :searchable="true"
                                         :close-on-select="true"
                                         trackBy="Id"
                                         label="Name"></MultiSelect>
                        </div>
                    </div>
                    <div class="col-md-6" v-if="selectedPageTypes & TYPE_RECURRING">
                        <div class="form-group">
                            <label class="control-label">Recurring Confirmation Email</label>
                            <MultiSelect v-model="page.ConfirmationEmailRecurringId"
                                         :options="confirmationEmailList"
                                         :searchable="true"
                                         :close-on-select="true"
                                         trackBy="Id"
                                         label="Name"></MultiSelect>
                        </div>
                    </div>
                    <div class="col-sm-12" style="margin-bottom: 15px;">
                        <div class="pull-right">
                            <a href="/Giving" class="btn btn-default" style="margin-right: 10px;">Cancel</a>
                            <a class="btn btn-primary" @click="saveGivingPage">Save</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import axios from "axios";
    import MultiSelect from "vue-multiselect";
    export default {
        props: ["pageProp", "givePrefix"],
        components: {
            MultiSelect
        },
        computed: {
            selectedPageTypes: function () {
                if (!this.pageTypes) return 0;
                return this.pageTypes.reduce(function (prev, cur) {
                    return prev + cur.Id;
                }, 0); 
            }
        },
        watch: {
            "page.PageName": function (name) {
                // suggest a url based on the title
                this.page.PageUrl = this.slugify(name);
            },
            "page.PageUrl": function (url) {
                this.validUrl = false;
                this.page.PageUrl = this.slugify(url);
                this.verifyUrl();
            }
        },
        data: function () {
            return {
                page: {
                    PageName: ""
                },
                validUrl: true,
                selectedAvailableFunds: [],
                fundsList: [],
                shellList: [],
                pageTypes: [],
                pageTypeList: [],
                entryPointList: [],
                confirmationEmailList: [],
                onlineNotifyPersonList: [],
                TYPE_PLEDGE: 1,
                TYPE_ONE_TIME: 2,
                TYPE_RECURRING: 4
            };
        },
        methods: {
            slugify(str) {
                return str.toLowerCase()
                    .replace(/^\s+|\s+$/g, '') // trim
                    .replace(/[^a-z0-9 -]/g, '') // remove invalid chars
                    .replace(/\s+/g, '-') // collapse whitespace and replace by -
                    .replace(/-+/g, '-'); // collapse dashes
            },
            validate() {
                if (this.selectedPageTypes == 0) {
                    error_swal("Error", "Choose at least one giving type");
                    return false;
                }
                if (!this.page.PageName) {
                    error_swal("Error", "Page name is required");
                    return false;
                }
                if (!this.page.PageUrl) {
                    error_swal("Error", "Page URL is required");
                    return false;
                }
                if (!this.page.DefaultFundId) {
                    error_swal("Error", "Default fund is required");
                    return false;
                }
                return true;
            },
            saveGivingPage() {
                if (this.validate()) {
                    axios.post("/Giving/Update", {
                        pageId: this.page.GivingPageId,
                        pageName: this.page.PageName,
                        pageUrl: this.page.PageUrl,
                        pageType: this.selectedPageTypes,
                        enabled: this.page.Enabled,
                        defaultFund: this.page.DefaultFundId,
                        disRedirect: this.page.DisabledRedirect,
                        //skinFile: this.page.currentPageSkin,
                        topText: this.page.TopText,
                        thankYouText: this.page.ThankYouText,
                        //onlineNotifyPerson: this.page.currentOnlineNotifyPerson,
                        confirmEmailPledge: this.page.ConfirmEmailPledge,
                        confirmEmailOneTime: this.page.ConfirmEmailOneTime,
                        confirmEmailRecurring: this.page.ConfirmEmailRecurring,
                        //campusId: this.page.currentCampusId,
                        entryPoint: this.page.EntryPointId,
                        // currentIndex: this.page.currentIndex
                        availFundsArray: this.page.currentAvailableFunds,
                    })
                    .then(
                        response => {
                            if (response.status === 200) {
                                snackbar('Giving page saved', 'success');
                            } else {
                                warning_swal("Warning!", "Error saving giving page. Please try again later");
                            }
                        },
                        err => {
                            error_swal("Error", "Error saving giving page. Please try again later");
                        }
                    )
                    .catch(error => {
                        error_swal("Error", "Error saving giving page. Please try again later");
                    });
                }
            },
            getPageTypes: function () {
                axios
                    .get("/Giving/GetPageTypes")
                    .then(
                        response => {
                            if (response.status === 200) {
                                this.pageTypeList = response.data;
                                console.log(this.pageTypeList);
                                this.pageTypeList.forEach(function (type) {
                                    if (this.selectedPageTypes & type.Id) {
                                        this.pageTypes.push(type);
                                    }
                                });
                            } else {
                                warning_swal("Warning!", "Something went wrong, try again later");
                            }
                        },
                        err => {
                            console.log(err);
                            error_swal("Fatal Error!", "We are working to fix it");
                        }
                    )
                    .catch(function (error) {
                        console.log(error);
                    });
            },
            verifyUrl: function () {
                var url = this.page.PageUrl;
                axios
                    .get("/Giving/CheckUrlAvailability?url=" + url)
                    .then(
                        response => {
                            if (response.status === 200) {
                                if (this.page.PageUrl == url && response.data.result == true) {
                                    this.validUrl = true;
                                } else {
                                    this.validUrl = false;
                                }
                            } else {
                                warning_swal("Warning!", "There was a problem checking for the URL availability. Please try again.");
                            }
                        },
                        err => {
                            console.log(err);
                            error_swal("Error", "There was a problem checking for the URL availability. Please try again.");
                        }
                    )
                    .catch(function (error) {
                        error_swal("Error", "There was a problem checking for the URL availability. Please try again.");
                    });
            },
            getAvailableFunds: function () {
                axios
                    .get("/Giving/GetAvailableFunds")
                    .then(
                        response => {
                            if (response.status === 200) {
                                this.fundsList = response.data;
                            } else {
                                warning_swal("Warning!", "Something went wrong, try again later");
                            }
                        },
                        err => {
                            console.log(err);
                            error_swal("Fatal Error!", "We are working to fix it");
                        }
                    )
                    .catch(function (error) {
                        console.log(error);
                    });
            },
            getEntryPoints: function () {
                axios
                    .get("/Giving/GetEntryPoints")
                    .then(
                        response => {
                            if (response.status === 200) {
                                this.entryPointList = response.data;
                            } else {
                                warning_swal("Warning!", "Something went wrong, try again later");
                            }
                        },
                        err => {
                            console.log(err);
                            error_swal("Fatal Error!", "We are working to fix it");
                        }
                    )
                    .catch(function (error) {
                        console.log(error);
                    });
            },
            getOnlineNotifyPersonList: function () {
                axios
                    .get("/Giving/GetOnlineNotifyPersonList")
                    .then(
                        response => {
                            if (response.status === 200) {
                                this.onlineNotifyPersonList = response.data;
                            } else {
                                warning_swal("Warning!", "Something went wrong, try again later");
                            }
                        },
                        err => {
                            console.log(err);
                            error_swal("Fatal Error!", "We are working to fix it");
                        }
                    )
                    .catch(function (error) {
                        console.log(error);
                    });
            },
            getConfirmationEmailList: function () {
                axios
                    .get("/Giving/GetConfirmationEmailList")
                    .then(
                        response => {
                            if (response.status === 200) {
                                this.confirmationEmailList = response.data;
                            } else {
                                warning_swal("Warning!", "Something went wrong, try again later");
                            }
                        },
                        err => {
                            console.log(err);
                            error_swal("Fatal Error!", "We are working to fix it");
                        }
                    )
                    .catch(function (error) {
                        console.log(error);
                    });
            },
            getShellList: function () {
                axios
                    .get("/Giving/GetShellList")
                    .then(
                        response => {
                            if (response.status === 200) {
                                this.shellList = response.data;
                            } else {
                                warning_swal("Warning!", "Something went wrong, try again later");
                            }
                        },
                        err => {
                            console.log(err);
                            error_swal("Fatal Error!", "We are working to fix it");
                        }
                    )
                    .catch(function (error) {
                        console.log(error);
                    });
            },
        },
        mounted() {
            this.page = JSON.parse(this.pageProp);
            this.page.SkinFileId = this.page.SkinFile.Id;
            this.getPageTypes();
            this.getAvailableFunds();
            this.getEntryPoints();
            this.getOnlineNotifyPersonList();
            this.getConfirmationEmailList();
            this.getShellList();
            $('[data-toggle="popover"]').popover({ container: '.tab-content' });
            $('[data-toggle="popover"]').click(function (ev) {
                ev.preventDefault();
            });
        }
    };
</script>
<style src="vue-multiselect/dist/vue-multiselect.min.css"></style>
