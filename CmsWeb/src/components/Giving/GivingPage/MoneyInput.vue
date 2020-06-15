<template>
    <div :class="{'money-input': true, 'focus' : focus}">
        <currency-input :value="value"
                        @input="update($event)"
                        @focus="focus = true"
                        @blur="focus = false"
                        currency="USD"
                        locale="en-US"
                        :distraction-free="false"
                        :auto-decimal-mode="true"
                        :allow-negative="false"></currency-input>
    </div>
</template>
<script>
    import { CurrencyInput } from 'vue-currency-input';

    export default {
        props: ['value'],
        data: function () {
            return {
                focus: false
            };
        },
        components: { CurrencyInput },
        methods: {
            update(value) {
                this.$emit("input", value);
            }
        }
    }
</script>

<style scoped>
    .money-input {
        margin: 0 auto;
        width: 200px;
        text-align: center;
        position: relative;
    }

    .money-input input {
        border: 0;
        border-bottom: 1px solid #333;
        font-size: 30px;
        width: 100%;
        text-align: center;
        background-color: transparent;
    }

    .money-input:before {
        content: "";
        position: absolute;
        width: 100%;
        height: 2px;
        bottom: 0;
        left: 0;
        background-color: #337ab7;
        visibility: hidden;
        transform: scaleX(0);
        transition: all 0.3s ease-in-out 0s;
    }

    .money-input input:focus {
        outline: none;
    }

    .money-input.focus:before {
        visibility: visible;
        transform: scaleX(1);
    }
</style>
