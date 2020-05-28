<template>
    <div class="panel panel-default">
        <div class="panel-body">
            <nav aria-label="Page navigation">
                <ul class="pagination">
                    <li v-for="type in pageTypes" :class="{active: giveType == type.Name}"><a @click="updateType(type.Name)">{{ type.Name }}</a></li>
                </ul>
            </nav>
            <input class="form-control" v-model="giveAmount" />
            {{ page }}
        </div>
    </div>
</template>
<script>
    import axios from "axios";
    export default {
        props: ["pageProp", "fund", "type", "amount" ],
        data: function () {
            return {
                giveType: "",
                giveAmount: 0.00,
                pageTypes: [],
                page: {
                },
            };
        },
        methods: {
            updateType(type) {
                this.giveType = type;
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
                                vm.giveType = vm.type || vm.pageTypes[0].Name;
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

            // initialize based on params (or defaults)
            this.giveAmount = this.amount || 0;

        }
    };
</script>
