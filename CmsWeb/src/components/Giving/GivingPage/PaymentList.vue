<template>
    <div class="payment-list">
        <div class="list-group">
            <div v-for="method in paymentMethods" class="list-group-item row" :key="method.Id">
                <div class="col-xs-2">
                    <input type="radio" v-model="selectedMethod" :value="method.Id">
                </div>
                <div class="col-xs-10">
                    <p>{{ method.Name }}</p>
                </div>
            </div>
            <div class="list-group-item row">
                <div class="col-xs-2">
                    <input type="radio" v-model="selectedMethod" value="new">
                </div>
                <div class="col-xs-10">
                    <p>Add New</p>
                </div>
            </div>
        </div>
        <new-payment-method v-if="selectedMethod == 'new'" v-model="value" :showValidation="showValidation" :paymentTypes="paymentTypes"></new-payment-method>
    </div>
</template>
<script>
    export default {
        props: ["value", "paymentTypes"],
        data: function () {
            return {
                showValidation: false,
                paymentMethods: [],
                selectedMethod: 0
            };
        }
    }
</script>
