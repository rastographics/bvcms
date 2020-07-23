<template>
    <div class="payment-container">
        <h4>Payment Method</h4>
        <div v-if="identity.PeopleId">
            <payment-list v-model="value" :paymentTypes="paymentTypes"></payment-list>
        </div>
        <div v-else>
            <edit-payment-method v-model="value" :showValidation="showValidation" :paymentTypes="paymentTypes"></edit-payment-method>
        </div>
    </div>
</template>
<script>
    export default {
        props: ["value", "identity", "showValidation"],
        data: function () {
            return {
                paymentTypes: [
                    {
                        Id: "card",
                        Name: "Debit/Credit Card"
                    },
                    {
                        Id: "bank",
                        Name: "Bank Account"
                    }
                ]
            };
        },
        mounted() {
            if (!this.value.length) { 
                this.$emit('input', {
                    cardInfo: {
                        number: "",
                        date: "",
                        cvc: ""
                    },
                    bankInfo: {
                        name: "",
                        routing: "",
                        account: ""
                    },
                    billingInfo: {
                        first: "",
                        last: "",
                        email: "",
                        phone: "",
                        address: "",
                        city: "",
                        state: "",
                        zip: "",
                        country: ""
                    }
                });
            } 
        }
    }
</script>
<style scoped>
    .payment-container {
        margin: 25px 0;
        border: 1px solid #ddd;
        border-radius: 4px;
        padding: 20px 20px 0;
    }
</style>
