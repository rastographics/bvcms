<template>
    <div class="new-payment-method">
        <div class="text-center" style="margin-bottom: 25px;">
            <div aria-label="Payment Type" class="btn-group payment-type text-center" role="group">
                <button v-for="type in paymentTypes" :key="type.Id" :class="[paymentType == type.Id ? 'btn-primary' : '', 'btn-default', 'btn']" @click="updateType(type.Id)">{{ type.Name }}</button>
            </div>
        </div>
        <transition name="fade" mode="out-in">
            <div class="row" v-if="paymentType === 'card'" key="card">
                <div class="col-md-6 col-md-offset-3">
                    <div class="row well">
                        <div class="col-xs-12">
                            <div :class="{'form-group': true, 'has-error': showValidation && value.cardInfo.number.length <= 18}">
                                <label class="control-label">
                                    Card Number
                                </label>
                                <input type="text" class="form-control" v-model="value.cardInfo.number" placeholder="4111 1234 1234 1234" v-mask="'#### #### #### ####'" required />
                                <small v-if="showValidation" class="text-danger">Please enter your card number</small>
                            </div>
                        </div>
                        <div class="col-xs-6">
                            <div :class="{'form-group': true, 'has-error': showValidation && value.cardInfo.date.length != 7}">
                                <label class="control-label">
                                    Expiration Date
                                </label>
                                <input type="text" class="form-control" v-model="value.cardInfo.date" placeholder="MM / YY" v-mask="'## / ##'" required />
                                <small v-if="showValidation" class="text-danger">Please enter your expiration date</small>
                            </div>
                        </div>
                        <div class="col-xs-6">
                            <div :class="{'form-group': true, 'has-error': showValidation && value.cardInfo.cvc.length != 3}">
                                <label class="control-label">
                                    Security Code
                                </label>
                                <input type="text" class="form-control" v-model="value.cardInfo.cvc" placeholder="123" v-mask="'###'" required />
                                <small v-if="showValidation" class="text-danger">Please enter your security code</small>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row" v-if="paymentType === 'bank'" key="bank">
                <div class="col-md-10 col-md-offset-1">
                    <div class="row well">
                        <div class="col-sm-12">
                            <div :class="{'form-group': true, 'has-error': showValidation && !value.bankInfo.name.length}">
                                <label class="control-label">
                                    Bank Name
                                </label>
                                <input type="text" class="form-control" v-model="value.bankInfo.name" placeholder="Bank of America" required />
                                <small v-if="showValidation" class="text-danger">Please enter your bank name</small>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div :class="{'form-group': true, 'has-error': showValidation && value.bankInfo.routing.length != 9}">
                                <label class="control-label">
                                    Routing #
                                </label>
                                <input type="number" class="form-control" v-model="value.bankInfo.routing" placeholder="123456789" v-mask="'#########'" required />
                                <small v-if="showValidation" class="text-danger">Please enter your routing number</small>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div :class="{'form-group': true, 'has-error': showValidation && value.bankInfo.account.length <= 3}">
                                <label class="control-label">
                                    Account #
                                </label>
                                <input type="number" class="form-control" v-model="value.bankInfo.account" placeholder="1234567890" v-mask="'##########'" required />
                                <small v-if="showValidation" class="text-danger">Please enter your account number</small>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </transition>
        <div class="row">
            <div class="col-sm-12">
                <h4>Billing Address</h4>
            </div>
        </div>
        <div class="row">
            <div class="col-md-6">
                <div :class="{'form-group': true, 'has-error': showValidation && !value.billingInfo.first.length}">
                    <input type="text" class="form-control" v-model="value.billingInfo.first" placeholder="First Name" required />
                    <small v-if="showValidation" class="text-danger">Please enter your first name</small>
                </div>
            </div>
            <div class="col-md-6">
                <div :class="{'form-group': true, 'has-error': showValidation && !value.billingInfo.last.length}">
                    <input type="text" class="form-control" v-model="value.billingInfo.last" placeholder="Last Name" required />
                    <small v-if="showValidation" class="text-danger">Please enter your last name</small>
                </div>
            </div>
            <div class="col-md-6">
                <div :class="{'form-group': true, 'has-error': showValidation && !emailValid}">
                    <input type="email" class="form-control" v-model="value.billingInfo.email" placeholder="Email Address" />
                    <small v-if="showValidation" class="text-danger">Please enter your email</small>
                </div>
            </div>
            <div class="col-md-6">
                <div :class="{'form-group': true, 'has-error': showValidation && value.billingInfo.phone.length < 10}">
                    <input type="tel" class="form-control" v-model="value.billingInfo.phone" placeholder="Phone Number" />
                    <small v-if="showValidation" class="text-danger">Please enter your phone</small>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <div :class="{'form-group': true, 'has-error': showValidation && value.billingInfo.address.length < 8}">
                    <input type="text" class="form-control" v-model="value.billingInfo.address" placeholder="Address" required />
                    <small v-if="showValidation" class="text-danger">Please enter your billing address</small>
                </div>
            </div>
            <div class="col-md-6">
                <div :class="{'form-group': true, 'has-error': showValidation && !value.billingInfo.city.length}">
                    <input type="text" class="form-control" v-model="value.billingInfo.city" placeholder="City" required />
                    <small v-if="showValidation" class="text-danger">Please enter your billing city</small>
                </div>
            </div>
            <div class="col-md-6">
                <div :class="{'form-group': true, 'has-error': showValidation && !value.billingInfo.state.length}">
                    <input type="text" class="form-control" v-model="value.billingInfo.state" placeholder="State" required />
                    <small v-if="showValidation" class="text-danger">Please enter your billing state</small>
                </div>
            </div>
            <div class="col-md-6">
                <div :class="{'form-group': true, 'has-error': showValidation && !value.billingInfo.zip.length}">
                    <input type="text" class="form-control" v-model="value.billingInfo.zip" placeholder="Zip" required />
                    <small v-if="showValidation" class="text-danger">Please enter your billing zip code</small>
                </div>
            </div>
            <div class="col-md-6">
                <div :class="{'form-group': true, 'has-error': showValidation && !value.billingInfo.country.length}">
                    <input type="text" class="form-control" v-model="value.billingInfo.country" placeholder="Country" required />
                    <small v-if="showValidation" class="text-danger">Please enter your billing country</small>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import { utils } from "touchpoint/common/utils.js";
    import { bus } from "touchpoint/common/bus.js";

    export default {
        props: ["value", "showValidation", "paymentTypes"],
        data: function () {
            return {
                paymentType: "card"
            };
        },
        computed: {
            formValid: function () {
                return this.emailValid && this.paymentValid &&
                    this.value.billingInfo.first.length > 0 &&
                    this.value.billingInfo.last.length > 0 &&
                    this.value.billingInfo.phone.length >= 10 &&
                    this.value.billingInfo.address.length >= 8 &&
                    this.value.billingInfo.city.length > 0 &&
                    this.value.billingInfo.state.length > 0 &&
                    this.value.billingInfo.zip.length > 0 &&
                    this.value.billingInfo.country.length > 0;
                    
            },
            emailValid: function () {
                return utils.validateEmail(this.value.billingInfo.email);
            },
            paymentValid: function () {
                if (this.paymentType == 'card') {
                    return this.value.cardInfo.number.length > 18 &&
                        this.value.cardInfo.cvc.length == 3 &&
                        this.value.cardInfo.date.length == 7;
                } else {
                    return this.value.bankInfo.name.length &&
                        this.value.bankInfo.routing.length == 9 &&
                        this.value.bankInfo.account.length > 3;
                }
            }
        },
        watch: {
            formValid: function (value) {
                bus.$emit('paymentValidationChange', value);
            }
        },
        methods: {
            updateType(type) {
                this.paymentType = type;
                this.clearPaymentInfo();
            },
            clearPaymentInfo() {
                let payment = this.value;
                payment.cardInfo = {
                    number: "",
                    date: "",
                    cvc: ""
                };
                payment.bankInfo = {
                    name: "",
                    routing: "",
                    account: ""
                };
                this.$emit('input', payment);
            }
        }
    }
</script>
