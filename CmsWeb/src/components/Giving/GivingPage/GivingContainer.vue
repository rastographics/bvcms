<template>
    <div>
        <div class="page-header text-center">
            <h1>{{ page.PageName }}</h1>
        </div>
        <div class="panel">
            <div class="panel-body">
                <nav aria-label="Page navigation" class="text-center">
                    <ul class="pagination">
                        <li v-for="type in pageTypes" :key="type.Name" :class="{active: givingType == type.Name}"><a @click="updateType(type.Name)">{{ type.Name }}</a></li>
                    </ul>
                </nav>
                <div v-if="givingType == 'One Time'" class="gifts">
                    <transition-group name="gift">
                        <one-time-gift v-for="(gift, index) in gifts" v-model="gifts[index]" :count="gifts.length" :key="index" :funds="unusedFunds" @remove="removeGift(index)"></one-time-gift>
                    </transition-group>
                    <a v-if="unusedFunds.length" @click="addGift" class="btn btn-sm btn-default pull-right"><i class="fa fa-plus-circle"></i> Add Gift</a>
                </div>
                <div v-else-if="givingType == 'Recurring'">
                    <transition-group name="gift">
                        <recurring-gift v-for="(gift, index) in gifts" v-model="gifts[index]" :count="gifts.length" :key="index" :funds="unusedFunds" :frequencies="recurringFrequencies" @remove="removeGift(index)"></recurring-gift>
                    </transition-group>
                    <a v-if="unusedFunds.length" @click="addGift" class="btn btn-sm btn-default pull-right"><i class="fa fa-plus-circle"></i> Add Gift</a>
                </div>
                <div v-else-if="givingType == 'Pledge'">
                    Pledge
                </div>
            </div>
            <pre>{{ gifts }}</pre>
        </div>
    </div>
</template>
<script>
    import axios from "axios";

    export default {
        props: ["pageProp", "fund", "type", "amount" ],
        data: function () {
            return {
                recurringFrequencies: [],
                givingType: "",
                gifts: [],
                pageTypes: [],
                page: {}
            };
        },
        computed: {
            unusedFunds: function () {
                if (!this.page.AvailableFunds) return [];
                return this.page.AvailableFunds.filter(fund => {
                    return !this.gifts.some(gift => gift.fund.Id === fund.Id);
                });
            },
            today: function () {
                var today = new Date();
                return today.getFullYear() + '-' + String(today.getMonth() + 1).padStart(2, '0') + '-' + String(today.getDate()).padStart(2, '0');
            }
        },
        methods: {
            updateType(type) {
                this.givingType = type;
            },
            addGift() {
                this.gifts.push({
                    amount: 0.00,
                    frequency: 0,
                    fund: this.unusedFunds[0],
                    date: this.today
                });
            },
            removeGift(index) {
                this.gifts.splice(index, 1);
            },
            getPageTypes: function () {
                let vm = this;
                axios
                    .get("/Giving/GetPageTypes")
                    .then(
                        response => {
                            if (response.status === 200) {
                                let pageTypeList = response.data;
                                pageTypeList.forEach(function (type) {
                                    if (vm.page.PageType & type.Id) {
                                        vm.pageTypes.push(type);
                                    }
                                });
                                vm.givingType = vm.type || vm.pageTypes[0].Name;
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
            getGivingFrequencies: function () {
                let vm = this;
                axios.get("/Giving/GetGivingFrequencies")
                .then(
                    response => {
                        if (response.status === 200) {
                            vm.recurringFrequencies = response.data;
                        } else {
                            warning_swal("Warning", "Error getting giving frequencies.");
                        }
                    },
                    err => {
                        error_swal("Error", "Error getting giving frequencies.");
                    }
                )
                .catch(function (error) {
                    console.log(error);
                });
            }
        },
        mounted() {
            this.page = JSON.parse(this.pageProp);
            this.getPageTypes();
            this.getGivingFrequencies();

            let gift = {
                amount: 0.00,
                frequency: 0,
                fund: {},
                date: this.today
            };
            // initialize based on params (or defaults)
            gift.amount = parseFloat(this.amount) || 0;
            gift.fund = this.page.DefaultFund;
            this.page.AvailableFunds.push(this.page.DefaultFund);
            if (this.fund) {
                this.page.AvailableFunds.forEach((fund) => {
                    if (this.fund == fund.Id) {
                        gift.fund = fund;
                    }
                });
            }
            this.gifts.push(gift);
        }
    };
</script>
<style>
    .gift {
        transition: all 1s;
        height: 159px;
    }
    .gift.recurring {
        height: 219px;
    }
    .gift-enter,
    .gift-leave-to {
        opacity: 0;
    }
    .gift-enter {
        transform: translateY(30%);
    }
    .gift-leave-to {
        transform: translateY(30%);
    }
    .well.gift-leave-to {
        height: 0px;
        min-height: 0;
        padding: 0;
        margin: 0;
    }
    .gift-leave-to .form-group,
    .gift-leave-to .money-input,
    .gift-leave-to .close {
        height: 0;
        padding: 0;
        opacity: 0;
        margin: 0;
        font-size: 0;
    }
    .gift .form-group {
        height: 74px;
    }
    .gift.recurring .form-group {
        height: 30px;
    }
    .gift .money-input {
        height: 46px;
    }
    .gift .form-group, .gift .money-input {
        transition: all 1s;
    }
</style>
