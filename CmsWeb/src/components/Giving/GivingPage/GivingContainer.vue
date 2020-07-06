<template>
    <div>
        <div class="page-header text-center" style="margin-bottom:0;">
            <h1>{{ page.PageName }}</h1>
        </div>
        <div class="panel">
            <transition :name="slideDirection" mode="out-in">
                <div class="panel-body" v-if="view === 'gifts'" key="gifts">
                    <div class="row">
                        <div class="col-sm-12">
                            <button v-if="!identity.PeopleId" @click="loadView('signin', true)" class="btn btn-link pull-right"><i class="fa fa-user"></i> Sign In</button>
                            <button v-else class="btn btn-link pull-right"><i class="fa fa-user"></i> {{ identity.Name }}</button>
                        </div>
                    </div>
                    <div v-if="pageTypes.length > 1" class="text-center" style="margin-bottom: 25px;">
                        <div aria-label="Giving Type" class="btn-group give-type text-center" role="group" style="margin: 0 auto;">
                            <button v-for="type in pageTypes" :key="type.Name" :class="[givingType == type.Name ? 'btn-primary' : '', 'btn-default', 'btn']" @click="updateType(type.Name)">{{ type.Name }}</button>
                        </div>
                    </div>
                    <transition name="fade" mode="out-in">
                        <div v-if="givingType == 'One Time'" key="onetime">
                            <transition-group name="gift">
                                <one-time-gift v-for="(gift, index) in gifts" v-model="gifts[index]" :count="gifts.length" :key="gift.key" :funds="unusedFunds" :showValidation="showValidation" @remove="removeGift(index)"></one-time-gift>
                            </transition-group>
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
                        <div v-else-if="givingType == 'Recurring'" key="recurring">
                            <transition-group name="gift">
                                <recurring-gift v-for="(gift, index) in gifts" v-model="gifts[index]" :count="gifts.length" :key="gift.key" :funds="unusedFunds" :showValidation="showValidation" :frequencies="recurringFrequencies" @remove="removeGift(index)"></recurring-gift>
                            </transition-group>
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
                        <div v-else-if="givingType == 'Pledge'" key="pledge">
                            <transition-group name="gift">
                                <one-time-gift v-for="(gift, index) in gifts" v-model="gifts[index]" :count="gifts.length" :key="gift.key" :funds="unusedFunds" :showValidation="showValidation" @remove="removeGift(index)"></one-time-gift>
                            </transition-group>
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
                    </transition>
                </div>
                <div class="panel-body" v-else-if="view === 'signin'" key="signin">
                    <giving-login v-model="identity" :SMSReady="identity.SMSReady" @next="loadView('payment')" @back="loadView('gifts')"></giving-login>
                </div>
            </transition>
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
                onKey: 0,
                identity: false,
                showValidation: false,
                slideDirection: 'slide-left'
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
            loadView(newView, skipValidation = false) {
                // setup the new view
                this.slideDirection = 'slide-left';
                if (this.view === 'signin' && newView === 'gifts')  {
                    this.slideDirection = 'slide-right';
                }
                // todo: also handle routing here?
                if (this.view === 'gifts') {
                    if (!skipValidation && !this.validateGifts()) {
                        return false;
                    }
                }
                if (newView === 'payment') {
                    this.getIdentity();
                    newView = 'gifts';
                }
                if (newView === 'signin') {
                    if (this.identity.PeopleId) {
                        // if already logged in, move to payment
                        newView = 'payment';
                    }
                }
                this.view = newView;
            },
            validateGifts() {
                let vm = this;
                let valid = true;
                vm.gifts.forEach((gift) => {
                    if (!gift.amount || gift.amount < 1) {
                        valid = false;
                    }
                    if (vm.givingType == 'Recurring' && !gift.frequency) {
                        valid = false;
                    }
                });
                vm.showValidation = !valid;
                return valid;
            },
            getIdentity() {
                let vm = this;
                axios
                    .get("/Account/Identity")
                    .then(
                        response => {
                            if (response.status === 200) {
                                vm.identity = response.data;
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
            getPageTypes() {
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
                            vm.init();
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
            },
            init() {
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
                let type = "";
                if (this.type) type = this.type.trim().toLowerCase();
                switch (type) {
                    case 'onetime':
                    case 'once':
                        this.givingType = 'One Time';
                        break;
                    case 'pledge':
                        this.givingType = 'Pledge';
                        break;
                    case 'recurring':
                        this.givingType = 'Recurring';
                        break;
                    case 'weekly':
                        gift.frequency = 1;
                        this.givingType = 'Recurring';
                        break;
                    case 'biweekly':
                        gift.frequency = 2; 
                        this.givingType = 'Recurring';
                        break;
                    case 'semimonthly':
                        gift.frequency = 3;
                        this.givingType = 'Recurring';
                        break;
                    case 'monthly':
                        gift.frequency = 4;
                        this.givingType = 'Recurring';
                        break;
                    case 'quarterly':
                        gift.frequency = 5;
                        this.givingType = 'Recurring';
                        break;
                    case 'annually':
                        gift.frequency = 6;
                        this.givingType = 'Recurring';
                        break;
                    default:
                        this.givingType = this.pageTypes[0].Name;
                }
                this.gifts.push(gift);
            }
        },
        mounted() {
            this.page = JSON.parse(this.pageProp);
            this.getIdentity();
            this.getPageTypes();
        }
    };
</script>
<style>
    .fade-enter-active,
    .fade-leave-active {
        transition-duration: 0.25s;
        transition-property: opacity;
        transition-timing-function: ease;
    }

    .fade-enter,
    .fade-leave-active {
        opacity: 0
    }

    .gift {
        transition: all 0.5s;
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
    .slide-left-enter-active,
    .slide-left-leave-active,
    .slide-right-enter-active,
    .slide-right-leave-active {
        transition-duration: 0.5s;
        transition-property: height, opacity, transform;
        transition-timing-function: cubic-bezier(0.55, 0, 0.1, 1);
        overflow: hidden;
    }

    .slide-left-enter,
    .slide-right-leave-active {
        opacity: 0;
        transform: translate(100%, 0);
    }

    .slide-left-leave-active,
    .slide-right-enter {
        opacity: 0;
        transform: translate(-100%, 0);
    }
</style>
