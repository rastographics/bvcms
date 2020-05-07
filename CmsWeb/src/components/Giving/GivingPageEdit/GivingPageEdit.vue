<template>
  <div>
    <button class="btn btn-sm btn-default" @click="showMyModal = true">
      <i class="fa fa-pencil"></i> Edit
    </button>
    <transition name="fade" appear>
      <div class="modal-overlay" v-if="showMyModal" @click="showMyModal = false"></div>
    </transition>
    <transition name="slide" appear>
      <div class="modal" v-if="showMyModal">
        <div class="modal-header">
          <button type="button" class="close" aria-label="Close" @click="showMyModal = false">
            <span aria-hidden="true">&times;</span>
          </button>
          <h4 class="modal-title">Edit Giving Page</h4>
        </div>
        <div class="modal-body">
          <div class="row">
            <div class="col-md-8">
              <div class="row">
                <div class="col-lg-5 col-md-5 col-sm-5">
                  <div class="form-group">
                    <label class="control-label">Page Name</label>
                    <input type="text" v-model="currentPageName" class="form-control" />
                  </div>
                </div>
                <div class="col-lg-5 col-md-5 col-sm-5">
                  <div class="form-group">
                    <label class="control-label">Page Title</label>
                    <input type="text" v-model="currentPageTitle" class="form-control" />
                  </div>
                </div>
                <div class="col-lg-2 col-md-2 col-sm-2">
                  <div class="form-group">
                    <div>
                      <label class="control-label">Enabled</label>
                    </div>
                    <div>
                      <generic-slider
                        v-model="currentPageEnabled"
                        v-bind:sliderValue="currentPageEnabled"
                        v-on:toggleSlider="currentPageEnabled = !currentPageEnabled"
                      ></generic-slider>
                    </div>
                  </div>
                </div>
              </div>
              <div class="row">
                <div class="col-lg-5 col-md-5 col-sm-5">
                  <div class="form-group">
                    <label class="control-label">Shell</label>
                    <MultiSelect
                      v-model="currentPageSkin"
                      :options="shellList"
                      :searchable="true"
                      :close-on-select="true"
                      :show-labels="false"
                      :allowEmpty="true"
                      trackBy="Id"
                      :custom-label="ShellCustomLabel"
                    ></MultiSelect>
                  </div>
                </div>
                <div class="col-lg-5 col-md-5 col-sm-5">
                  <div class="form-group">
                    <label class="control-label">Page Type</label>
                    <MultiSelect
                      v-model="currentPageType"
                      :options="pageTypeList"
                      :searchable="true"
                      :close-on-select="true"
                      :show-labels="false"
                      :multiple="true"
                      :allowEmpty="false"
                      :preselectFirst="true"
                      trackBy="id"
                      :custom-label="PageTypeCustomLabel"
                    ></MultiSelect>
                  </div>
                </div>
                <div class="col-lg-2 col-md-2 col-sm-2"></div>
              </div>
              <div class="row">
                <div class="col-lg-5 col-md-5 col-sm-5">
                  <div class="form-group">
                    <label class="control-label">Default Fund</label>
                    <MultiSelect
                      v-model="currentDefaultFund"
                      :options="fundsList"
                      :searchable="true"
                      :close-on-select="true"
                      :show-labels="false"
                      :allowEmpty="true"
                      trackBy="FundId"
                      :custom-label="AvailableFundsCustomLabel"
                    ></MultiSelect>
                  </div>
                </div>
                <div class="col-lg-5 col-md-5 col-sm-5">
                  <div class="form-group">
                    <label class="control-label">Available Funds</label>
                    <MultiSelect
                      v-model="currentAvailableFunds"
                      :options="fundsList"
                      :searchable="true"
                      :close-on-select="true"
                      :show-labels="false"
                      :multiple="true"
                      trackBy="FundId"
                      :custom-label="AvailableFundsCustomLabel"
                    ></MultiSelect>
                  </div>
                </div>
                <div class="col-lg-2 col-md-2 col-sm-2"></div>
              </div>
              <div class="row">
                <div class="col-lg-9 col-md-9 col-sm-9">
                  <div class="form-group">
                    <label class="control-label">Disabled Redirect</label>
                    <input type="text" v-model="currentPageDisabledRedirect" class="form-control" />
                  </div>
                </div>
                <div class="col-lg-3 col-md-3 col-sm-3"></div>
              </div>
              <div class="row">
                <div class="col-lg-5 col-md-5 col-sm-5">
                  <div class="form-group">
                    <label class="control-label">Entry Point</label>
                    <MultiSelect
                      v-model="currentPageEntryPoint"
                      :options="entryPointList"
                      :searchable="true"
                      :close-on-select="true"
                      :show-labels="false"
                      trackBy="Id"
                      :custom-label="EntryPointCustomLabel"
                    ></MultiSelect>
                  </div>
                </div>
                <div class="col-lg-7 col-md-7 col-sm-7"></div>
              </div>
              <br />
              <button
                class="btn btn-default"
                style="border-radius:5px;"
                @click="showMyModal = false"
              >Cancel</button>
              <button
                class="btn btn-primary"
                style="border-radius:5px;margin-left: 5px;"
                @click="saveGivingPage"
              >Save</button>
            </div>
            <div class="col-md-4" style="border-left: 2px solid black;">
              <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                  <div class="form-group">
                    <label class="control-label">Top Text</label>
                    <input type="text" v-model="currentTopText" class="form-control" />
                  </div>
                </div>
              </div>
              <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                  <div class="form-group">
                    <label class="control-label">Thank you message</label>
                    <input type="text" v-model="currentThankYouText" class="form-control" />
                  </div>
                </div>
              </div>
              <div class="row">
                <div class="col-lg-10 col-md-10 col-sm-10">
                  <div class="form-group">
                    <label class="control-label">Online Notify Person</label>
                    <MultiSelect
                      v-model="currentOnlineNotifyPerson"
                      :options="onlineNotifyPersonList"
                      :searchable="true"
                      :close-on-select="true"
                      :show-labels="false"
                      trackBy="PeopleId"
                      :multiple="true"
                      :custom-label="NotifyPersonCustomLabel"
                    ></MultiSelect>
                  </div>
                </div>
              </div>
              <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                  <div class="form-group">
                    <label class="control-label">Confirmation Email - pledge</label>
                    <MultiSelect
                      v-model="currentConfirmEmailPledge"
                      :options="confirmationEmailList"
                      :searchable="true"
                      :close-on-select="true"
                      :show-labels="false"
                      trackBy="Id"
                      :custom-label="ConfirmEmailCustomLabel"
                    ></MultiSelect>
                  </div>
                </div>
              </div>
              <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                  <div class="form-group">
                    <label class="control-label">Confirmation Email - One time</label>
                    <MultiSelect
                      v-model="currentConfirmEmailOneTime"
                      :options="confirmationEmailList"
                      :searchable="true"
                      :close-on-select="true"
                      :show-labels="false"
                      trackBy="Id"
                      :custom-label="ConfirmEmailCustomLabel"
                    ></MultiSelect>
                  </div>
                </div>
              </div>
              <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                  <div class="form-group">
                    <label class="control-label">Confirmation Email - recurring</label>
                    <MultiSelect
                      v-model="currentConfirmEmailRecurring"
                      :options="confirmationEmailList"
                      :searchable="true"
                      :close-on-select="true"
                      :show-labels="false"
                      trackBy="Id"
                      :custom-label="ConfirmEmailCustomLabel"
                    ></MultiSelect>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<script>
import axios from "axios";
import MultiSelect from "vue-multiselect";
export default {
  props: {
    showEditModal: Boolean,
    pageId: Number,
    pageName: String,
    pageTitle: String,
    pageEnabled: Boolean,
    pageSkin: Object,
    pageType: Array,
    defaultFund: Object,
    availableFunds: Array,
    pageDisabledRedirect: String,
    pageEntryPoint: Object,
    capusId: Object,
    topText: String,
    thankYouText: String,
    onlineNotifyPerson: Array,
    confirmEmailPledge: Object,
    confirmEmailOneTime: Object,
    confirmEmailRecurring: Object,

    pageTypeList: Array,
    fundsList: Array,
    entryPointList: Array,
    onlineNotifyPersonList: Array,
    confirmationEmailList: Array,
    shellList: Array
  },
  components: {
    MultiSelect
  },
  data: function() {
    return {
      showMyModal: this.showEditModal,
      currentPageId: this.pageId,
      currentPageName: this.pageName,
      currentPageTitle: this.pageTitle,
      currentPageEnabled: this.pageEnabled,
      currentPageSkin: this.pageSkin,
      currentPageType: this.pageType,
      currentDefaultFund: this.defaultFund,
      currentAvailableFunds: this.availableFunds,
      currentPageDisabledRedirect: this.pageDisabledRedirect,
      currentPageEntryPoint: this.pageEntryPoint,
      currentCampusId: this.capusId,
      currentTopText: this.topText,
      currentThankYouText: this.thankYouText,
      currentOnlineNotifyPerson: this.onlineNotifyPerson,
      currentConfirmEmailPledge: this.confirmEmailPledge,
      currentConfirmEmailOneTime: this.confirmEmailOneTime,
      currentConfirmEmailRecurring: this.confirmEmailRecurring,
      tester: null
    };
  },
  methods: {
    PageTypeCustomLabel({ pageTypeName }) {
      return `${pageTypeName}`;
    },
    AvailableFundsCustomLabel({ FundName }) {
      return `${FundName}`;
    },
    EntryPointCustomLabel({ Description }) {
      return `${Description}`;
    },
    NotifyPersonCustomLabel({ Name }) {
      return `${Name}`;
    },
    ConfirmEmailCustomLabel({ Title }) {
      return `${Title}`;
    },
    ShellCustomLabel({ Title }) {
      return `${Title}`;
    },
    saveGivingPage() {
      axios
        .post("/Giving/UpdateGivingPage", {
          pageId: this.currentPageId,
          pageName: this.currentPageName,
          pageTitle: this.currentPageTitle,
          pageType: this.currentPageType,
          enabled: this.currentPageEnabled,
          defaultFund: this.currentDefaultFund,
          availFundsArray: this.currentAvailableFunds,
          disRedirect: this.currentPageDisabledRedirect,
          skinFile: this.currentPageSkin,
          topText: this.currentTopText,
          thankYouText: this.currentThankYouText,
          onlineNotifyPerson: this.currentOnlineNotifyPerson,
          confirmEmailPledge: this.currentConfirmEmailPledge,
          confirmEmailOneTime: this.currentConfirmEmailOneTime,
          confirmEmailRecurring: this.currentConfirmEmailRecurring,
          campusId: this.currentCampusId,
          entryPoint: this.currentPageEntryPoint
        })
        .then(
          response => {
            if (response.status === 200) {
              this.tester = response.data;
              //this.$emit('updatePage', response.data);
              // this.$emit('updatePage', [this.currentPageId, this.currentPageName, this.currentPageTitle, this.currentPageEnabled]);
            } else {
              warning_swal("Warning!", "Something went wrong, try again later");
            }
          },
          err => {
            console.log(err);
            error_swal("Fatal Error!", "We are working to fix it");
          }
        )
        .catch(function(error) {
          console.log(error);
        });
      this.showMyModal = false;
    }
  }
};
</script>

<style scoped>
.modal-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 9998;
  width: 100%;
  height: 100%;
}

.modal {
  display: block !important;
  position: fixed;
  left: 50%;
  transform: translate(-50%, -50%);
  z-index: 9999;

  overflow-y: auto;
  background-color: #fff;
  display: table;

  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
  border: 1px solid rgba(0, 0, 0, 0.2);
  border-radius: 0;
  background-clip: padding-box;
  outline: 0;
}

@media only screen and (max-width: 600px) {
  .modal {
    top: 49%;
    width: 90%;
    height: 90%;
  }
}
@media only screen and (min-width: 600px) {
  .modal {
    top: 49%;
    width: 90%;
    height: 90%;
  }
}
@media only screen and (min-width: 768px) {
  .modal {
    top: 49%;
    width: 90%;
    height: 90%;
  }
}
@media only screen and (min-width: 992px) {
  .modal {
    top: 41%;
    width: 90%;
    height: 75%;
  }
}
@media only screen and (min-width: 1200px) {
  .modal {
    top: 41%;
    width: 90%;
    height: 75%;
  }
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.5s;
}

.fade-enter,
.fade-leave-to {
  opacity: 0;
}

.slide-enter-active,
.slide-leave-active {
  transition: transform 0.5s;
}

.slide-enter,
.slide-leave-to {
  transform: translateY(-50%) translateX(100vw);
}
</style>