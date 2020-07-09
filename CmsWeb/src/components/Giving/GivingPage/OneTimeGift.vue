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
                <fund-picker :value="value" :funds="funds" @input="hideNote"></fund-picker>
                <a v-if="value.fund.AllowNotes && showNote == false" @click="showNote = true" class="notelink" style="cursor:pointer;"><i class="fa fa-plus-circle"></i> Note</a>
            </div>
        </div>
        <transition name="gift">
            <div class="row" v-if="showNote" style="margin-bottom: 15px;">
                <div class="col-sm-12 col-md-8 col-md-offset-2">
                    <div class="form-group">
                        <label class="control-label">Note (optional)</label>
                        <input class="form-control" type="text" v-model="value.note" />
                    </div>
                </div>
            </div>
        </transition>
    </div>
</template>
<script>
    export default {
        props: ["value", "count", "funds", "showValidation"],
        data: function () {
            return {
                showNote: false
            }
        },
        methods: {
            remove() {
                this.$emit('remove');
            },
            hideNote() {
                this.value.note = '';
                this.showNote = false;
            }
        }
    }
</script>
<style scoped>
    .notelink {
        position: absolute;
        top: 17px;
        right: -38px;
    }
</style>
