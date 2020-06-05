<template>
    <div class="gift well">
        <button v-if="count > 1" @click="remove" type="button" class="close" aria-label="Remove"><span aria-hidden="true">&times;</span></button>
        <money-input v-model="value.amount"></money-input>
        <div v-if="fundOptions && fundOptions.length" class="form-group" style="margin-top:25px;">
            <select class="form-control" @change="updateFund($event)">
                <option v-for="fund in fundOptions" :key="fund.Id" :value="fund.Id" :selected="fund.Id == value.fund.Id">{{ fund.Name }}</option>
            </select>
        </div>
    </div>
</template>
<script>
    export default {
        props: ["value", "count", "funds"],
        computed: {
            fundOptions: function () {
                return this.funds.concat(this.value.fund);
            }
        },
        methods: {
            updateFund(event) {
                let id = event.target.value;
                let gift = this.value;
                this.fundOptions.forEach((fund) => {
                    if (fund.Id == id) {
                        gift.fund = fund;
                    }
                });
                this.$emit("input", gift);
            },
            remove() {
                this.$emit('remove');
            },
        }
    }
</script>
