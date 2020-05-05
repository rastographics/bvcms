import Vue from 'vue';
//import App from './App.vue'

Vue.config.productionTip = false




// Giving Pages Components
Vue.component('giving-page-index', require('./components/Giving/GivingPageIndex/GivingPageIndex.vue').default);
Vue.component('giving-page-index-slider', require('./components/Giving/GivingPageIndex/GivingPageIndexSlider.vue').default);

Vue.component('add-giving-page', require('./components/Giving/GivingPageIndex/AddGivingPage.vue').default);

Vue.component('edit-giving-page', require('./components/Giving/GivingPageEdit/GivingPageEdit.vue').default);
Vue.component('edit-giving-page-slider', require('./components/Giving/GivingPageEdit/GivingPageEditSlider.vue').default);







// Generic Components
Vue.component('users-grid', require('./components/UsersGrid/UsersGrid.vue').default);
Vue.component('generic-modal', require('./components/GenericComponents/Modal.vue').default);
Vue.component('generic-slider', require('./components/GenericComponents/Slider.vue').default);
// Vue.component('generic-touchpoint-slider', require('./components/GenericComponents/TouchpointSlider.vue').default);


Vue.component('generic-multi-select', require('./components/GenericComponents/MultiSelect.vue').default);








window.Vue = Vue;

//new Vue({
//  render: h => h(App),
//}).$mount('#app')
