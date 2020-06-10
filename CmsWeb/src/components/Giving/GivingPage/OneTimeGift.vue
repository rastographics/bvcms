<template>
    <div class="gift well">
        <button v-if="count > 1" @click="remove" type="button" class="close" aria-label="Remove"><span aria-hidden="true">&times;</span></button>
        <money-input v-model="value.amount"></money-input>
        <div class="row">
            <div class="col-sm-8 col-sm-offset-2">
                <fund-picker :value="value" :funds="funds"></fund-picker>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ["value", "count", "funds"],
        methods: {
            remove() {
                this.$emit('remove');
            },
        }
    }
</script>
