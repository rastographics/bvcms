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
/* .switch {
  position: relative;
  display: inline-block;
  width: 54px;
  height: 24px;
}

.switch input {
  opacity: 0;
  width: 0;
  height: 0;
}

.slider {
  position: absolute;
  cursor: pointer;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  background-color: #ddd;
  -webkit-transition: 0.4s;
  transition: 0.4s;
}

.slider:before {
  position: absolute;
  content: "";
  height: 20px;
  width: 20px;
  left: 4px;
  top: 2px;
  bottom: 0px;
  background-color: white;
  -webkit-transition: 0.4s;
  transition: 0.4s;
}

input:checked + .slider {
  background-color: #003f72;
}

input:checked + .slider:before {
  -webkit-transform: translateX(26px);
  -ms-transform: translateX(26px);
  transform: translateX(26px);
  top: 2px;
  bottom: 0px;
}

input:focus + .slider {
  box-shadow: 0 0 1px #2196f3;
}

.slider.round {
  border-radius: 34px;
}

.slider.round:before {
  border-radius: 50%;
} */
</style>