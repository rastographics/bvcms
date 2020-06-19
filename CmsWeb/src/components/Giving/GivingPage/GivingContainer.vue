<template>
    <div>
        <div class="page-header text-center">
            <h1>{{ page.PageName }}</h1>
        </div>
        <div class="panel">
            <div class="panel-body" v-if="view === 'gifts'">
                <div v-if="pageTypes.length > 1" class="text-center" style="margin-bottom: 25px;">
                    <div aria-label="Giving Type" class="btn-group give-type text-center" role="group"  style="margin: 0 auto;">
                        <button v-for="type in pageTypes" :key="type.Name" :class="[givingType == type.Name ? 'btn-primary' : '', 'btn-default', 'btn']" @click="updateType(type.Name)">{{ type.Name }}</button>
                    </div>
                </div>
                <div v-if="givingType == 'One Time'">
                    <transition-group name="gift">
                        <one-time-gift v-for="(gift, index) in gifts" v-model="gifts[index]" :count="gifts.length" :key="gift.key" :funds="unusedFunds" @remove="removeGift(index)"></one-time-gift>
                    </transition-group>
                </div>
                <div v-else-if="givingType == 'Recurring'">
                    <transition-group name="gift">
                        <recurring-gift v-for="(gift, index) in gifts" v-model="gifts[index]" :count="gifts.length" :key="gift.key" :funds="unusedFunds" :frequencies="recurringFrequencies" @remove="removeGift(index)"></recurring-gift>
                    </transition-group>
                </div>
                <div v-else-if="givingType == 'Pledge'">
                    <transition-group name="gift">
                        <one-time-gift v-for="(gift, index) in gifts" v-model="gifts[index]" :count="gifts.length" :key="gift.key" :funds="unusedFunds" @remove="removeGift(index)"></one-time-gift>
                    </transition-group>
                </div>
                <div class="row">
                    <div class="col-md-6">
                        <button v-if="unusedFunds.length" @click="addGift" class="btn-block btn btn-default">
                            <i class="fa fa-plus-circle"></i> Add Gift
                        </button>
                    </div>
                    <div class="col-md-6">
                        <button @click="loadView('signin')" class="btn-block btn btn-primary">
                            Next
                        </button>
                    </div>
                </div>
            </div>
            <div class="panel-body" v-if="view === 'signin'">
                Easy login
                <div class="row">
                    <div class="col-md-6">
                        <button @click="loadView('gifts')" class="btn-block btn btn-default">
                            Back
                        </button>
                    </div>
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
                recurringFrequencies: [],
                givingType: "",
                gifts: [],
                pageTypes: [],
                page: {},
                view: "gifts",
                onKey: 0
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
                    key: this.onKey,
                    amount: 0.00,
                    frequency: 0,
                    fund: this.unusedFunds[0],
                    date: this.today
                });
                this.onKey++;
            },
            removeGift(index) {
                this.gifts.splice(index, 1);
            },
            loadView(newView) {
                // setup the new view
                // todo: also handle routing here?
                if (this.view === 'gifts') {
                    // todo: validate gifts? (or maybe during payment flow?)
                }
                if (newView === 'signin') {
                    // todo: if already signed in, move to payment view
                }
                this.view = newView;
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
                                vm.getGivingFrequencies();
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

            let gift = {
                key: this.onKey,
                amount: 0.00,
                frequency: 0,
                fund: {},
                date: this.today
            };
            this.onKey++;
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
