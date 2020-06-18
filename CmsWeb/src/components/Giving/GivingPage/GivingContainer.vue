<template>
    <div>
        <div class="page-header text-center">
            <h1>{{ page.PageName }}</h1>
        </div>
        <div class="panel">
            <div class="panel-body">
                <nav aria-label="Page navigation" class="text-center">
                    <ul class="pagination">
                        <li v-for="type in pageTypes" :key="type.Name" :class="{active: givingType == type.Name}"><a @click="updateType(type.Name)" style="cursor: pointer">{{ type.Name }}</a></li>
                    </ul>
                </nav>
                <div v-if="givingType == 'One Time'" class="gifts">
                    <transition-group name="gift">
                        <one-time-gift v-for="(gift, index) in gifts" v-model="gifts[index]" :count="gifts.length" :key="index" :funds="unusedFunds" @remove="removeGift(index)"></one-time-gift>
                    </transition-group>
                    <a v-if="unusedFunds.length" @click="addGift" class="btn btn-sm btn-default pull-right">Add Gift</a>
                </div>
                <div v-else-if="givingType == 'Recurring'">
                    Recurring Giving
                </div>
                <div v-else-if="givingType == 'Pledge'">
                    Pledge
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import axios from "axios";

    export default {
        props: ["pageProp", "fund", "type", "amount" ],
        data: function () {
            return {
                givingType: "",
                gifts: [],
                pageTypes: [],
                page: {
                },
            };
        },
        computed: {
            unusedFunds: function () {
                if (!this.page.AvailableFunds) return [];
                return this.page.AvailableFunds.filter(fund => {
                    return !this.gifts.some(gift => gift.fund.Id === fund.Id);
                });
            }
        },
        methods: {
            updateType(type) {
                this.givingType = type;
            },
            addGift() {
                this.gifts.push({
                    amount: 0.00,
                    fund: this.unusedFunds[0]
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
                                vm.pageTypeList = response.data;
                                vm.pageTypeList.forEach(function (type) {
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
        },
        mounted() {
            this.page = JSON.parse(this.pageProp);
            this.getPageTypes();

            let gift = {
                amount: 0.00,
                fund: {}
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
    .gift .money-input {
        height: 46px;
    }
    .gift .form-group, .gift .money-input {
        transition: all 1s;
    }
</style>
