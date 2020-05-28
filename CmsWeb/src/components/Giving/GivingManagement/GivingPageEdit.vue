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
        <form @submit.prevent="saveGivingPage">
            <div class="tab-content">
                <div id="Details-tab" class="tab-pane fade active in">
                    <div class="row">
                        <div class="col-md-6">
                            <div :class="{'form-group': true, 'has-error': showValidation && !page.PageName}">
                                <label class="control-label">Page Name</label>
                                <input type="text" v-model="page.PageName" class="form-control" />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div :class="{'form-group': true, 'has-success': validUrl && !page.PageId, 'has-error': !validUrl && page.PageUrl.length && !page.PageId}">
                                <label for="pageUrl" class="control-label">Page URL <a href="#" data-toggle="popover" data-placement="right" data-trigger="focus" data-title="Page URL" data-content="The publically accessible URL for this giving page. This can't be changed later, and must be unique."><i class="fa fa-info-circle"></i></a></label>
                                <div class="input-group" v-if="page.PageId !== 0">
                                    <span class="input-group-addon disabled" id="pageUrl">{{givePrefix}}{{page.PageUrl}}</span>
                                    <input type="text" class="form-control" style="border-left-width:0;" disabled />
                                </div>
                                <div class="input-group" v-else>
                                    <span class="input-group-addon" id="pageUrl" @click="$refs.pageUrl.focus()">{{givePrefix}}</span>
                                    <input type="text" class="form-control" ref="pageUrl" v-model="page.PageUrl" aria-describedby="pageUrl" style="border-left-width:0;">
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            <div :class="{'form-group': true, 'has-error': showValidation && !pageTypes.length}">
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
                                <MultiSelect v-model="page.SkinFile"
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
                            <div :class="{'form-group': true, 'has-error': showValidation && !page.DefaultFund}">
                                <label class="control-label">Default Fund <a href="#" data-toggle="popover" data-placement="right" data-trigger="focus" data-title="Default Fund" data-content="This is the primary fund that gifts from this page will go to by default."><i class="fa fa-info-circle"></i></a></label>
                                <MultiSelect v-model="page.DefaultFund"
                                             :options="fundsList"
                                             :searchable="true"
                                             :close-on-select="true"
                                             :allowEmpty="false"
                                             trackBy="Id"
                                             label="Name"></MultiSelect>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="control-label">Fund Options <a href="#" data-toggle="popover" data-placement="right" data-trigger="focus" data-title="Fund Options" data-content="Any funds listed here will be presented as optional alternatives to the default fund for donors to choose to give to."><i class="fa fa-info-circle"></i></a></label>
                                <MultiSelect v-model="page.AvailableFunds"
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
                                <MultiSelect v-model="page.EntryPoint"
                                             :options="entryPointList"
                                             :searchable="true"
                                             :close-on-select="true"
                                             trackBy="Id"
                                             label="Name"></MultiSelect>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="control-label">URL Redirect <a href="#" data-toggle="popover" data-placement="right" data-trigger="focus" data-title="URL Redirect" data-content="If you choose to disable this giving page, you can enter a fully qualified URL here for donors to be redirected to if they try to visit this page."><i class="fa fa-info-circle"></i></a></label>
                                <input type="text" v-model="page.DisabledRedirect" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12" style="margin-bottom: 15px;">
                            <div class="pull-right">
                                <a href="/Giving" class="btn btn-default" style="margin-right: 10px;"><i class="fa fa-arrow-circle-left"></i> Back to List</a>
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
                                <label class="control-label">Thank You Message</label>
                                <input type="text" v-model="page.ThankYouText" class="form-control" />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="control-label">Email From</label>
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
                                <MultiSelect v-model="page.ConfirmEmailPledge"
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
                                <MultiSelect v-model="page.ConfirmEmailOneTime"
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
                                <MultiSelect v-model="page.ConfirmEmailRecurring"
                                             :options="confirmationEmailList"
                                             :searchable="true"
                                             :close-on-select="true"
                                             trackBy="Id"
                                             label="Name"></MultiSelect>
                            </div>
                        </div>
                        <div class="col-sm-12" style="margin-bottom: 15px;">
                            <div class="pull-right">
                                <a href="/Giving" class="btn btn-default" style="margin-right: 10px;"><i class="fa fa-arrow-circle-left"></i> Back to List</a>
                                <a class="btn btn-primary" @click="saveGivingPage">Save</a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </form>
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
                if (this.page.PageId == 0) {
                    this.page.PageUrl = this.slugify(name);
                }
            },
            "page.PageUrl": function (url) {
                if (this.page.PageId == 0) {
                    this.validUrl = false;
                    this.page.PageUrl = this.slugify(url);
                    this.verifyUrl();
                }
            }
        },
        data: function () {
            return {
                page: {
                    PageName: ""
                },
                validUrl: true,
                showValidation: false,
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
                if (!str) return '';
                return str.toLowerCase()
                    .replace(/^\s+|\s+$/g, '') // trim
                    .replace(/[^a-z0-9 -]/g, '') // remove invalid chars
                    .replace(/\s+/g, '-') // collapse whitespace and replace by -
                    .replace(/-+/g, '-'); // collapse dashes
            },
            validate() {
                var errors = [];
                this.showValidation = false;
                if (this.selectedPageTypes == 0) {
                    errors.push("Giving type is required");
                }
                if (!this.page.PageName) {
                    errors.push("Page name is required");
                }
                if (!this.page.PageUrl) {
                    errors.push("Page URL is required");
                }
                if (!this.page.DefaultFund) {
                    errors.push("Default fund is required");
                }
                if (this.page.PageId == 0 && !this.validUrl && this.page.PageUrl) {
                    errors.push("That page URL is already in use.");
                }
                if (errors.length) {
                    this.showValidation = true;
                    error_swal("Error", errors.join("\n"));
                    return false;
                }
                return true;
            },
            saveGivingPage() {
                if (this.validate()) {
                    let vm = this;
                    let url = vm.page.PageId ? "/Giving/Update" : "/Giving/Create";
                    let page = Object.assign({}, vm.page);
                    page.PageType = vm.selectedPageTypes;
                    axios.post(url, page)
                    .then(
                        response => {
                            if (response.status === 200) {
                                vm.page.PageId = response.data.PageId;
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
                let vm = this;
                axios
                    .get("/Giving/GetPageTypes")
                    .then(
                        response => {
                            if (response.status === 200) {
                                vm.pageTypeList = response.data;
                                vm.pageTypeList.forEach(function (type) {
                                    if (vm.page.PageType & type.Id) {
                                        vm.pageTypes.push(type);
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
                axios.get("/Giving/CheckUrlAvailability?url=" + url)
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
                let vm = this;
                axios.get("/Giving/GetAvailableFunds")
                .then(
                    response => {
                        if (response.status === 200) {
                            vm.fundsList = response.data;
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
                axios.get("/Giving/GetEntryPoints")
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
<style scoped>
    .has-success .input-group-addon,
    .has-error .input-group-addon {
        color: #555;
    }
    .input-group-addon {
        font-size: 13px;
        background-color: transparent;
    }
    .input-group-addon.disabled {
        cursor: not-allowed;
        background-color: #eee;
    }
</style>
