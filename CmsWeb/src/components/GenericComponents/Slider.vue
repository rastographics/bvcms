<template>
  <div
    data-toggle="toggle"
    style="width: 100px; height: 0px;"
    v-bind:class="['toggle', 'btn', mySliderValue ? 'btn-primary' : 'btn-default', mySliderValue ? 'on' : 'off']"
    @click="mySliderValue = !mySliderValue"
  >
    <input
      type="checkbox"
      data-toggle="toggle"
      data-width="100"
      v-model="mySliderValue"
      checked="mySliderValue"
    />
    <div class="toggle-group">
      <label class="btn btn-primary toggle-on">Enabled</label>
      <label class="btn btn-default active toggle-off">Disabled</label>
      <span class="toggle-handle btn btn-default"></span>
    </div>
  </div>
</template>

<script>
export default {
  props: ["sliderValue"],
  data() {
    return {
      mySliderValue: this.sliderValue
    };
  },
  watch: {
    mySliderValue() {
      this.$emit("toggleSlider");
    }
  },
  methods: {}
};
</script>

<style scoped>

</style>