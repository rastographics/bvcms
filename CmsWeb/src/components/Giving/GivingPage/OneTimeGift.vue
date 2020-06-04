<template>
    <div>
        <money-input v-model="value.amount"></money-input>
        <div v-if="funds && funds.length" class="form-group">
            <select class="form-control" v-model="fundId">
                <option v-for="fund in funds" :key="fund.Id" :value="fund.Id">{{ fund.Name }}</option>
            </select>
        </div>
        <a v-if="index != 0" @click="remove">x</a>
    </div>
</template>
<script>
    export default {
        props: ["value", "index", "funds"],
        data: function () {
            return {
                fundId: this.value.fund.Id
            }
        },
        watch: {
            "fundId": function (fundId) {
                let gift = this.value;
                this.funds.forEach((fund) => {
                    if (fund.Id == fundId) {
                        gift.fund = fund;
                    }
                });
                this.$emit("input", gift);
            }
        },
        methods: {
            remove() {
                this.$emit('remove');
            },
        }
    }
</script>
