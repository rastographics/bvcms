<template>
    <div class="payment-list">
        <div class="list-group">
            <div v-for="method in paymentMethods" class="list-group-item" :key="method.PaymentMethodId" @click="selectMethod(method.PaymentMethodId)">
                <div class="col-xs-1">
                    <div class="radio">
                        <input type="radio" v-model="selectedMethod" :value="method.PaymentMethodId">
                        <span class="switch"></span>
                    </div>
                </div>
                <div class="col-xs-9">
                    <strong v-if="method.PaymentMethodId == selectedMethod">{{ method.Name }}</strong>
                    <span v-else>{{ method.Name }}</span>
                </div>
                <div class="col-xs-2 text-right">
                    <a v-if="!editingPayment" @click.stop="edit(method)" class="btn btn-sm btn-default">Edit</a>
                </div>
                <transition name="expand" mode="out-in">
                    <edit-payment-method v-if="editingPayment == method.PaymentMethodId" v-model="value" :showValidation="showValidation" :paymentTypes="paymentTypes"></edit-payment-method>
                    <div class="row">
                        <div class="col-sm-6">
                            <a class="btn btn-default">Cancel</a>
                        </div>
                        <div class="col-sm-6">
                            <a class="btn btn-primary">Save</a>
                        </div>
                    </div>
                </transition>
            </div>
            <div class="list-group-item" @click="selectMethod('new')">
                <div class="col-xs-1">
                    <div class="radio">
                        <input type="radio" v-model="selectedMethod" value="new">
                        <span class="switch"></span>
                    </div>
                </div>
                <div class="col-xs-11">
                    <strong v-if="selectedMethod == 'new'">Add New</strong>
                    <span v-else>Add New</span>
                </div>
            </div>
        </div>
        <transition name="expand" mode="out-in">
            <new-payment-method v-if="selectedMethod == 'new'" v-model="value" :showValidation="showValidation" :paymentTypes="paymentTypes"></new-payment-method>
            <div class="row">
                <div class="col-sm-6">
                    <a class="btn btn-default">Cancel</a>
                </div>
                <div class="col-sm-6">
                    <a class="btn btn-primary">Save</a>
                </div>
            </div>
        </transition>
    </div>
</template>
<script>
    import axios from "axios";
    import { bus } from "touchpoint/common/bus.js";

    export default {
        props: ["value", "paymentTypes"],
        data: function () {
            return {
                showValidation: false,
                editingPayment: false,
                paymentMethods: [],
                selectedMethod: 0
            };
        },
        methods: {
            getSavedPaymentMethods() {
                let vm = this;
                axios.get("/GivingPayment/UserPaymentMethods")
                .then(
                    response => {
                        if (response.status === 200) {
                            vm.paymentMethods = response.data;
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
            selectMethod(id) {
                if (!this.editingPayment) {
                    this.selectedMethod = id;
                }
                // this.editingPayment = false;
            },
            edit(method) {
                this.editingPayment = method.PaymentMethodId;
                this.selectedMethod = method.PaymentMethodId;
            }
        },
        mounted() {
            this.getSavedPaymentMethods();
        }
    }
</script>
<style scoped>
    .payment-list {
        margin: 0 -20px;
    }
    .payment-list .list-group {
        margin-bottom: 0;
    }
    .payment-list .list-group-item {
        border-left: 0;
        border-right: 0;
        border-radius: 0;
        padding: 10px 20px;
        cursor: pointer;
    }
    .payment-list .list-group-item:last-child {
        border-bottom: 0;
    }
    .new-payment-method {
        margin: 0 25px;
        max-height: 1000px;
    }
    .expand-enter-active,
    .expand-leave-active {
        transition-duration: 0.5s;
        transition-property: max-height;
    }
    .expand-enter,
    .expand-leave-to {
        max-height: 0;
        overflow: hidden;
    }
</style>
