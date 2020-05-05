<template>
  <div style="margin-top:5px;">
    <input
      type="checkbox"
      name="myToggle"
      class="toggle-status"
      v-model="mySliderValue"
      data-toggle="toggle"
      data-on="Enabled"
      data-off="Disabled"
      data-width="100"
      checked="mySliderValue"
      onclick="myalert()"
      @click="saveGivingPageEnabled(mySliderValue, myGivingPageId)"
      @mousedown="saveGivingPageEnabled(mySliderValue, myGivingPageId)"
      @mouseup="saveGivingPageEnabled(mySliderValue, myGivingPageId)"
      @change="saveGivingPageEnabled(mySliderValue, myGivingPageId)"
    />
    <!-- <label class="switch">
      <input type="checkbox" v-model="mySliderValue" @change="saveGivingPageEnabled(mySliderValue, myGivingPageId)" />
      <span class="slider round">hello</span>
    </label> -->
  </div>
</template>

<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.2/jquery.min.js"></script>

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
      alert("success");
      // axios
      //   .post("/Giving/SaveGivingPageEnabled", {
      //     currentValue: myValue,
      //     currentGivingPageId: id
      //   })
      //   .then()
      //   .catch(err => console.log(err));
    },
    myalert() {
      alert("success");
    }
  },
  mounted() {
    const plugin = document.createElement("script");
    plugin.setAttribute(
      "src",
      "https://gitcdn.github.io/bootstrap-toggle/2.2.2/js/bootstrap-toggle.min.js"
    );
    plugin.async = true;
    document.head.appendChild(plugin);
  }
};
$('.toggle-status').change(function () {
            alert("success");
        });
</script>



<style scoped>
/* .checkbox label .toggle,.checkbox-inline .toggle{margin-left:-20px;margin-right:5px}
.toggle{position:relative;overflow:hidden}
.toggle input[type=checkbox]{display:none}
.toggle-group{position:absolute;width:200%;top:0;bottom:0;left:0;transition:left .35s;-webkit-transition:left .35s;-moz-user-select:none;-webkit-user-select:none}
.toggle.off .toggle-group{left:-100%}
.toggle-on{position:absolute;top:0;bottom:0;left:0;right:50%;margin:0;border:0;border-radius:0}
.toggle-off{position:absolute;top:0;bottom:0;left:50%;right:0;margin:0;border:0;border-radius:0}
.toggle-handle{position:relative;margin:0 auto;padding-top:0;padding-bottom:0;height:100%;width:0;border-width:0 1px}
.toggle.btn{min-width:59px;min-height:34px}
.toggle-on.btn{padding-right:24px}
.toggle-off.btn{padding-left:24px}
.toggle.btn-lg{min-width:79px;min-height:45px}
.toggle-on.btn-lg{padding-right:31px}
.toggle-off.btn-lg{padding-left:31px}
.toggle-handle.btn-lg{width:40px}
.toggle.btn-sm{min-width:50px;min-height:30px}
.toggle-on.btn-sm{padding-right:20px}
.toggle-off.btn-sm{padding-left:20px}
.toggle.btn-xs{min-width:35px;min-height:22px}
.toggle-on.btn-xs{padding-right:12px}
.toggle-off.btn-xs{padding-left:12px} */
/* .switch {
  position: relative;
  display: inline-block;
  width: 100px;
  height: 32px;
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
  height: 30px;
  width: 12px;
  left: 1px;
  top: 1px;
  bottom: 0px;
  background-color: white;
  -webkit-transition: 0.4s;
  transition: 0.4s;
}

input:checked + .slider {
  background-color: #003f72;
}

input:checked + .slider:before {
  -webkit-transform: translateX(86px);
  -ms-transform: translateX(86px);
  transform: translateX(86px);
  top: 1px;
  bottom: 0px;
}

input:focus + .slider {
  box-shadow: 0 0 1px #2196f3;
}


.slider.round:before {
  cursor: pointer;
  border-radius: 0px;
} */
</style>
