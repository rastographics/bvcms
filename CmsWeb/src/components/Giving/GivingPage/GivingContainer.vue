<template>
    <div class="panel panel-default">
        <div class="panel-body">
            <money-input v-model="gift.amount"></money-input>
            <nav aria-label="Page navigation" class="text-center">
                <ul class="pagination">
                    <li v-for="type in pageTypes" :key="type.Name" :class="{active: gift.type == type.Name}"><a @click="updateType(type.Name)">{{ type.Name }}</a></li>
                </ul>
            </nav>
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
                gift: {
                    type: "",
                    amount: 0.00
                },
                pageTypes: [],
                page: {
                },
            };
        },
        methods: {
            updateType(type) {
                this.gift.type = type;
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
                                vm.gift.type = vm.type || vm.pageTypes[0].Name;
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
            this.gift.amount = parseFloat(this.amount) || 0;

        }
    };
</script>
