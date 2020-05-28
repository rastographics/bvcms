import Vue from 'vue';
import axios from 'axios';
Vue.prototype.$http = axios;
Vue.config.productionTip = false;

// CheckIn
Vue.component('checkin-setup', require('./components/CheckIn/CheckinSetup.vue').default);

// Multiple Gateways
Vue.component('gateway-management', require('./components/Setup/Gateway/Manage.vue').default);

// Giving Management
Vue.component('giving-page-index', require('./components/Giving/GivingManagement/GivingPageIndex.vue').default);
Vue.component('edit-giving-page', require('./components/Giving/GivingManagement/GivingPageEdit.vue').default);

// Giving Frontend
Vue.component('giving-container', require('./components/Giving/GivingPage/GivingContainer.vue').default);

// Touchpoint Common Components
Vue.component('tp-toggle', require('./components/Common/Toggle.vue').default);

window.Vue = Vue;
