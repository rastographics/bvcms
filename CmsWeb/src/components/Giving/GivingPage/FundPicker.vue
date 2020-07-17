<template>
    <div v-if="fundOptions && fundOptions.length" class="form-group">
        <select class="form-control" @change="updateFund($event.target.value)">
            <option v-for="fund in fundOptions" :key="fund.Id" :value="fund.Id" :selected="fund.Id == value.fund.Id">{{ fund.Name }}</option>
        </select>
    </div>
</template>
<script>
    export default {
        props: ["value", "funds"],
        computed: {
            fundOptions: function () {
                return this.funds.concat(this.value.fund);
            }
        },
        methods: {
            updateFund(id) {
                let gift = this.value;
                this.fundOptions.forEach((fund) => {
                    if (fund.Id == id) {
                        gift.fund = fund;
                    }
                });
                this.$emit("input", gift);
            }
        }
    }
</script>
