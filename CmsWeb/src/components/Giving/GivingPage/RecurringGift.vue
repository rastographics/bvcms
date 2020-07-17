<template>
    <div class="gift recurring well">
        <button v-if="count > 1" @click="remove" type="button" class="close" aria-label="Remove"><span aria-hidden="true">&times;</span></button>
        <div class="row">
            <div class="col-sm-12">
                <div :class="{'form-group': true, 'text-center': true, 'has-error': showValidation && value.amount < 1}">
                    <money-input v-model="value.amount"></money-input>
                    <small v-if="showValidation" class="text-danger">Please enter an amount</small>
                </div>
            </div>
            <div class="col-sm-12 col-md-8 col-md-offset-2">
                <fund-picker :value="value" :funds="funds" @input="showEndDate = false"></fund-picker>
            </div>
        </div>
        <div class="row" style="margin-bottom: 15px;">
            <div v-if="frequencies.length < 5" class="col-sm-12">
                <div :class="{'btn-group': true, 'btn-group-justified': true, 'has-error': showValidation && !value.frequency}" aria-label="Giving Frequency" role="group">
                    <div class="btn-group" role="group" v-for="frequency in frequencies" :key="frequency.Id">
                        <button :class="{'btn-primary': value.frequency == frequency.Id, btn: true, 'btn-default': true}" @click="setFrequency(frequency.Id)">{{ frequency.Name }}</button>
                    </div>
                </div>
                <small v-if="showValidation && !value.frequency" class="text-danger text-center">Please choose a frequency</small>
            </div>
            <div v-else class="col-sm-12 col-md-8 col-md-offset-2">
                <div :class="{'form-group': true, 'has-error': showValidation && !value.frequency}">
                    <select class="form-control" :value="value.frequency" @input="setFrequency($event.target.value)">
                        <option value="0">Select frequency</option>
                        <option v-for="frequency in frequencies" :key="frequency.Id" :value="frequency.Id">{{ frequency.Name }}</option>
                    </select>
                    <small v-if="showValidation" class="text-danger">Please choose a frequency</small>
                </div>
            </div>
            <div class="col-sm-12 text-center" v-if="value.frequency" ref="giftText" style="margin-top: 16px; font-size: 13px;">
                <template v-if="value.frequency == 3">
                    On the
                    <a data-type="select" :data-value="value.d1" ref="d1" class="editlink">{{ getOrdinal(value.d1) }}</a> and
                    <a data-type="select" :data-value="value.d2" ref="d2" class="editlink">{{ getOrdinal(value.d2) }}</a> of each month starting
                    <a data-type="date" :data-value="value.date" ref="startdate" class="datepicker"></a>{{ givingToday ? ' (today)' : '' }}
                </template>
                <template v-else>
                    {{ giftText }} <a data-type="date" :data-value="value.date" ref="startdate" class="datepicker"></a>{{ givingToday ? ' (today)' : '' }}
                </template>
                <span v-if="showEndDate">and ending <a data-type="date" :data-value="value.enddate" class="datepicker" ref="enddate"></a></span>
                <span v-else-if="value.fund.AllowEndDate">and ending <a @click="showEndDate = true" class="editlink">when cancelled</a></span>
            </div>
        </div>
    </div>
</template>
<script>
    import { utils } from "touchpoint/common/utils.js";

    export default {
        props: ["value", "count", "funds", "frequencies", "showValidation"],
        data: function () {
            return {
                showEndDate: false
            }
        },
        computed: {
            giftText: function () {
                const days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
                var parts = this.value.date.split('-');
                let date = new Date(parts[0], parts[1] - 1, parts[2]); 
                switch (this.value.frequency) {
                    case 1: // weekly
                        return "Every " + days[date.getDay()] + " starting";
                    case 2: // biweekly
                        return "Every other " + days[date.getDay()] + " starting";
                    case 4: // monthly
                        return "On the " + this.getOrdinal(date.getDate()) + " each month starting";
                    case 5: // quarterly
                        return "Every 3 months starting";
                    case 6: // annually
                        return "Once a year starting";
                    default:
                        return "";
                }
            },
            givingToday: function () {
                var today = new Date();
                var parts = this.value.date.split('-');
                var selected = new Date(parts[0], parts[1] - 1, parts[2]); 
                return selected.getDate() == today.getDate() &&
                    selected.getMonth() == today.getMonth() &&
                    selected.getFullYear() == today.getFullYear();
            },
            dayOptions: function () {
                var dates = [];
                for (var i = 1; i <= 31; i++) {
                    dates.push({
                        value: i,
                        text: this.getOrdinal(i)
                    });
                }
                return dates;
            },
            valid: function () {
                return this.value.amount >= 1 && this.value.frequency > 0;
            }
        },
        watch: {
            showEndDate: function (show) {
                let vm = this;
                setTimeout(vm.initEditables, 100);
                if (show) {
                    var year = vm.value.date.slice(0, 4);
                    var nextyear = parseInt(year) + 1;
                    vm.value.enddate = vm.value.date.replace(year, nextyear);
                } else {
                    $(vm.$refs['enddate']).editable('destroy');
                    vm.value.enddate = null;
                }
            },
            valid: function (value) {
                this.value.valid = value;
                this.$emit('input', this.value);
            }
        },
        methods: {
            remove() {
                this.$emit('remove');
            },
            getOrdinal(n) {
                return utils.getOrdinal(n);
            },
            setFrequency(f) {
                let vm = this;
                // set up semi monthly values
                if (f == 3) {
                    var parts = vm.value.date.split('-');
                    let date = new Date(parts[0], parts[1] - 1, parts[2]);
                    var d1, d2;
                    if (date.getDate() > 15) {
                        d2 = date.getDate();
                        d1 = d2 - 15;
                    } else {
                        d1 = date.getDate();
                        d2 = d1 + 15;
                    }
                    vm.value.d1 = d1.toString();
                    vm.value.d2 = d2.toString();
                } else if (vm.value.frequency == 3) {
                    vm.value.d1 = null;
                    vm.value.d2 = null;
                }
                // clear editables
                vm.value.frequency = 0;
                // update frequency
                setTimeout(function () {
                    vm.value.frequency = parseInt(f);
                }, 100);
                // init editables
                setTimeout(vm.initEditables, 200);
            },
            initEditables() {
                let vm = this;
                $(vm.$refs['startdate']).editable({
                    mode: 'popup',
                    type: 'date',
                    clear: false,
                    savenochange: true,
                    showbuttons: false,
                    value: vm.value.date,
                    format: 'yyyy-mm-dd',
                    viewformat: 'm/d/yyyy',
                    datepicker: {
                        autoclose: true,
                        startDate: new Date(),
                        endDate: new Date(new Date().setFullYear(new Date().getFullYear() + 1)),
                        maxViewMode: 1
                    }
                }).on('save', function (e, params) {
                    vm.value.date = params.submitValue;
                    vm.$emit('input', vm.value);
                });
                if ($(vm.$refs['enddate'])) {
                    $(vm.$refs['enddate']).editable({
                        mode: 'popup',
                        type: 'date',
                        savenochange: true,
                        showbuttons: false,
                        value: vm.value.enddate,
                        format: 'yyyy-mm-dd',
                        viewformat: 'm/d/yyyy',
                        datepicker: {
                            autoclose: true,
                            startDate: new Date(vm.value.date),
                            maxViewMode: 1
                        }
                    }).on('save', function (e, params) {
                        vm.value.enddate = params.submitValue;
                        vm.$emit('input', vm.value);
                        if (!params.submitValue) {
                            vm.showEndDate = false;
                        }
                    });
                }
                if ($(vm.$refs['d1'])) {
                    $(vm.$refs['d1']).editable({
                        mode: 'inline',
                        type: 'select',
                        showbuttons: false,
                        source: vm.dayOptions,
                        value: vm.value.d1
                    }).on('save', function (e, params) {
                        vm.value.d1 = params.submitValue;
                        vm.$emit('input', vm.value);
                    });
                }
                if ($(vm.$refs['d2'])) {
                    $(vm.$refs['d2']).editable({
                        mode: 'inline',
                        type: 'select',
                        showbuttons: false,
                        source: vm.dayOptions,
                        value: vm.value.d2
                    }).on('save', function (e, params) {
                        vm.value.d2 = params.submitValue;
                        vm.$emit('input', vm.value);
                    });
                }
            }
        },
        mounted() {
            this.initEditables();
            this.value.valid = this.valid;
        }
    }
</script>
<style scoped>
    .btn-group .btn {
        padding: 8px 0px;
    }
    .btn-group.has-error .btn {
        border-top-color: #a94442;
        border-bottom-color: #a94442;
    }
    .btn-group.has-error > .btn-group:first-child:not(:last-child) > .btn:last-child {
        border-left-color: #a94442;
    }
    .btn-group.has-error > .btn-group:last-child:not(:first-child) > .btn:first-child {
        border-right-color: #a94442;
    }
    .editlink,
    .datepicker {
        border-bottom: dashed 1px #003f72;
        padding: 4px 0px;
        cursor: pointer;
    }
</style>
