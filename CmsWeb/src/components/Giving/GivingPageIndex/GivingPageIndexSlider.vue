<template>
  <label class="switch">
    <input
      type="checkbox"
      v-model="mySliderValue"
      @click="saveGivingPageEnabled(mySliderValue, myGivingPageId)"
    />
    <span class="slider round"></span>
  </label>
</template>

<script>
import axios from "axios";
export default {
  props: ["sliderValue", "givingPageId"],
  data() {
    return {
      mySliderValue: this.sliderValue,
      myGivingPageId: this.givingPageId
    };
  },
  methods: {
    saveGivingPageEnabled(myValue, id) {
      axios.post("/Giving/SaveGivingPageEnabled", {
          currentValue: !myValue,
          currentGivingPageId: id
      }).then().catch(err => console.log(err));
    }
  }
};
</script>

<style scoped>
.switch {
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
}
</style>
