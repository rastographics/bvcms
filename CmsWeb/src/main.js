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
Vue.component('recurring-gift', require('./components/Giving/GivingPage/RecurringGift.vue').default);
Vue.component('one-time-gift', require('./components/Giving/GivingPage/OneTimeGift.vue').default);
Vue.component('money-input', require('./components/Giving/GivingPage/MoneyInput.vue').default);
Vue.component('fund-picker', require('./components/Giving/GivingPage/FundPicker.vue').default);

// Touchpoint Common Components
Vue.component('tp-toggle', require('./components/Common/Toggle.vue').default);

window.Vue = Vue;
