<template>
    <div class="gift well">
        <button v-if="count > 1" @click="remove" type="button" class="close" aria-label="Remove"><span aria-hidden="true">&times;</span></button>
        <div class="row">
            <div class="col-sm-12">
                <div :class="{'form-group': true, 'text-center': true, 'has-error': showValidation && value.amount < 1}">
                    <money-input v-model="value.amount"></money-input>
                    <small v-if="showValidation && value.amount < 1" class="text-danger">Please enter an amount</small>
                </div>
            </div>
            <div class="col-sm-12 col-md-8 col-md-offset-2">
                <fund-picker :value="value" :funds="funds"></fund-picker>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ["value", "count", "funds", "showValidation"],
        methods: {
            remove() {
                this.$emit('remove');
            },
        }
    }
</script>
