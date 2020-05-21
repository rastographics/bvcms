<template>
    <div
    data-toggle="toggle"
    :class="['toggle', 'btn', value ? 'btn-primary' : 'btn-default', value ? 'on' : 'off']"
    :style="{'width': `${width}px`}"
    @click="update"
    >
    <input
        type="checkbox"
        data-toggle="toggle"
        :data-width="width"
        :value="value"
    />
    <div class="toggle-group">
        <label class="btn btn-primary toggle-on">{{ enabledText }}</label>
        <label class="btn btn-default active toggle-off">{{ disabledText }}</label>
        <span class="toggle-handle btn btn-default"></span>
    </div>
    </div>
</template>

<script>
    export default {
        props: {
            value: {},
            width: {
                default: 100
            },
            enabledText: {
                default: 'Enabled'
            },
            disabledText: {
                default: 'Disabled'
            }
        },
        methods: {
            update() {
                this.$emit("input", !this.value);
            }
        }
    }
</script>
