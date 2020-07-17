<template>
    <div class="card-input">
        <input type="text" class="form-control" :value="value" @input="update($event.target.value)" :placeholder="placeholder" v-mask="mask" required />
        <svg v-if="type == 'visa'" class="card-icon"><use xlink:href="#visa-light"></use></svg>
        <svg v-else-if="type == 'mastercard'" class="card-icon"><use xlink:href="#mc-dark"></use></svg>
        <svg v-else-if="type == 'amex'" class="card-icon"><use xlink:href="#amex-dark"></use></svg>
        <svg v-else-if="type == 'discover'" class="card-icon"><use xlink:href="#discover-light"></use></svg>
        <span id="svgIncludes"></span>
    </div>
</template>
<script>
    export default {
        props: ['value', 'type'],
        computed: {
            placeholder: function () {
                if (this.type == 'amex') {
                    return '3333 123456 90000';
                } else {
                    return '4111 1234 1234 1234';
                }
            },
            mask: function () {
                if (this.type == 'amex') {
                    return '#### ###### #####';
                } else {
                    return '#### #### #### ####';
                }
            }
        },
        methods: {
            update(value) {
                this.$emit('input', value);
            }
        },
        mounted() {
            $('#svgIncludes').load("/Content/touchpoint/img/card-icons.svg");
        }
    }
</script>
<style scoped>
    #svgIncludes {
        display: none;
    }
    .card-input {
        position: relative;
    }
    .card-icon {
        display: inline-block;
        vertical-align: middle;
        position: absolute;
        width: 45px;
        height: 30px;
        top: 6px;
        right: 5px;
    }
</style>
