import Vue from 'vue';

Vue.config.productionTip = false

// Giving Pages Components
Vue.component('giving-page-index', require('./components/Giving/GivingPageIndex/GivingPageIndex.vue').default);
Vue.component('add-giving-page', require('./components/Giving/GivingPageIndex/AddGivingPage.vue').default);
Vue.component('edit-giving-page', require('./components/Giving/GivingPageEdit/GivingPageEdit.vue').default);

// Generic Components
Vue.component('generic-slider', require('./components/GenericComponents/Slider.vue').default);

window.Vue = Vue;