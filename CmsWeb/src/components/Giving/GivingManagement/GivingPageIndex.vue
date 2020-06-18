<template>
    <div class="box">
        <div class="box-title-btn">
            <div class="box-tools">
                <a href="/Giving/New" class="btn btn-success"><i class="fa fa-plus-circle"></i> Add Giving Page</a>
            </div>
        </div>
        <div class="box-content">
            <div class="table-responsive">
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Giving</th>
                            <th>Default Fund</th>
                            <th>Public Url</th>
                            <th class="text-center">Enabled</th>
                            <th class="text-center">Default</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="page in givingPageList" :key="page.GivingPageId">
                            <td>
                                <a :href="page.EditUrl">{{ page.PageName }}</a>
                            </td>
                            <td>{{ page.PageTypeString }}</td>
                            <td>
                                <span v-if="page.DefaultFund != null">{{ page.DefaultFund.Name }}</span>
                                <span v-else>none</span>
                            </td>
                            <td>
                                <span v-if="!page.Enabled && page.DisabledRedirect">/Give/{{ page.PageUrl }} redirects to <a target="_blank" :href="page.DisabledRedirect">{{ page.DisabledRedirect }}</a></span>
                                <span v-else-if="!page.Enabled">(no redirect set)</span>
                                <a v-else target="_blank" :href="'/Give/' + page.PageUrl">{{ givePrefix }}{{ page.PageUrl }}</a>
                            </td>
                            <td class="text-center">
                                <tp-toggle v-model="page.Enabled" @input="toggleEnabled(page.PageId, page.Enabled)"></tp-toggle>
                            </td>
                            <td class="text-center">
                                <tp-toggle v-model="page.DefaultPage" @input="toggleDefault(page.PageId, page.DefaultPage)"></tp-toggle>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</template>

<script>
    import axios from "axios";

    export default {
        props: ["givePrefix"],
        data: function () {
            return {
                givingPageList: []
            };
        },
        methods: {
            fetchGivingPages() {
                axios.get("/Giving/List")
                .then(
                    response => {
                        if (response.status === 200) {
                            response.data.forEach(page => page.PageTypeString = "");
                            this.givingPageList = response.data;
                            this.getPageTypes();
                        } else {
                            warning_swal("Warning!", "Something went wrong, try again later");
                        }
                    },
                    err => {
                        error_swal("Error", "Error loading giving pages. Please try again later.");
                    }
                ).catch(error => error_swal("Error", "Error loading giving pages. Please try again later."));
            },
            toggleEnabled(id, value) {
                axios.post("/Giving/SaveGivingPageEnabled", {
                    value: value,
                    PageId: id
                })
                .then(
                    response => {
                        if (response.status === 200) {
                            snackbar(response.data.PageName + " has been " + (response.data.Enabled ? "enabled" : "disabled"), "success");
                        } else {
                            snackbar("Error updating giving page status.", "error");
                            this.fetchGivingPages();
                        }
                    }
                )
                .catch(error => {
                    snackbar("Error updating giving page status.", "error");
                    this.fetchGivingPages();
                });
            },
            toggleDefault(id, value) {
                axios.post("/Giving/SetGivingDefaultPage", {
                    value: value,
                    PageId: id
                })
                .then(
                    response => {
                        if (response.status === 200) {
                            if(response.data.UpdateStatus === true) {
                                snackbar(response.data.PageName + " has been " + (response.data.DefaultPage ? "set as default page" : "unset as default page"), "success");
                            } else {
                                snackbar(response.data.PageName + " cannot be set as default because " + response.data.CurrentDefaultPage + " is the current default page. Only one default page at a time.", "error");
                                this.click();
                            }
                        } else {
                            snackbar("Error updating giving page status.", "error");
                            this.fetchGivingPages();
                        }
                    }
                )
                .catch(error => {
                    snackbar("Error updating giving page status.", "error");
                    this.fetchGivingPages();
                });
            },
            getPageTypes: function () {
                let vm = this;
                axios
                    .get("/Giving/GetPageTypes")
                    .then(
                        response => {
                            if (response.status === 200) {
                                let pageTypeList = response.data;
                                vm.givingPageList.forEach(function (page) {
                                    let pageTypes = [];
                                    pageTypeList.forEach(function (type) {
                                        if (page.PageType & type.Id) {
                                            pageTypes.push(type.Name);
                                        }
                                    });
                                    page.PageTypeString = pageTypes.join(', ');
                                })
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
            this.fetchGivingPages();
        }
    };
</script>
