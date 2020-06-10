<template>
    <div class="gift recurring well">
        <button v-if="count > 1" @click="remove" type="button" class="close" aria-label="Remove"><span aria-hidden="true">&times;</span></button>
        <money-input v-model="value.amount"></money-input>
        <div class="row">
            <div class="col-sm-6">
                <fund-picker :value="value" :funds="funds"></fund-picker>
            </div>
            <div class="col-sm-6">
                <div class="form-group" style="margin-top:25px;">
                    <select v-model="value.frequency" class="form-control">
                        <option value="0">Select frequency</option>
                        <option v-for="frequency in frequencies" :key="frequency.Id" :value="frequency.Id">{{ frequency.Name }}</option>
                    </select>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-sm-9">
                Your first gift will process <a href="#" ref="datepicker" data-type="date" data-format="yyyy-mm-dd" data-viewformat="mm/dd/yyyy" data-title="Select date" :data-value="value.date">{{ value.date }}</a>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ["value", "count", "funds", "frequencies"],
        methods: {
            remove() {
                this.$emit('remove');
            },
        },
        mounted() {
            this.$refs.datepicker.editable({
                format: 'yyyy-mm-dd',
                viewformat: 'dd/mm/yyyy',
                datepicker: {
                    weekStart: 1
                }
            });
        }
    }
</script>
