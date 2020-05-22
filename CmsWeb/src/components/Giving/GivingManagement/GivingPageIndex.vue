<template>
    <div class="box">
        <div class="box-content">
            <div class="table-responsive">
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Giving</th>
                            <th>Default Fund</th>
                            <th>Public Url</th>
                            <th>Enabled</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="page in givingPageList" :key="page.GivingPageId">
                            <td>
                                <a :href="page.EditUrl">{{ page.PageName }}</a>
                            </td>
                            <td>
                                <span v-if="!page.Enabled && page.DisabledRedirect">{{ page.PageUrl }} redirects to <a :href="page.DisabledRedirect">{{ page.DisabledRedirect }}</a></span>
                                <span v-else-if="!page.Enabled">(no redirect set)</span>
                                <a v-else :href="page.PageUrl">{{ page.PageUrl }}</a>
                            </td>
                            <td>{{ page.PageTypeString }}</td>
                            <td>
                                <span v-if="page.DefaultFund != null">{{ page.DefaultFund.FundName }}</span>
                                <span v-else>none</span>
                            </td>
                            <td>
                                <tp-toggle v-model="page.Enabled" @input="toggleEnabled(page.GivingPageId, page.Enabled)"></tp-toggle>
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
                            this.givingPageList = response.data;
                        } else {
                            warning_swal("Warning!", "Something went wrong, try again later");
                        }
                    },
                    err => {
                        error_swal("Error", "Error loading giving pages. Please try again later.");
                    }
                )
                .catch(error => error_swal("Error", "Error loading giving pages. Please try again later."));
            },
            toggleEnabled(id, value) {
                axios.post("/Giving/SaveGivingPageEnabled", {
                    value: value,
                    currentGivingPageId: id
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
            }
        },
        mounted() {
            this.fetchGivingPages();
        }
    };
</script>
