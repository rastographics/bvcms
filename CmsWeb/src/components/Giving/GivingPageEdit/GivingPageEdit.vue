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
                    <input type="text" v-model="currentGivingPage.PageName" class="form-control" />
                  </div>
                </div>
                <div class="col-lg-5 col-md-5 col-sm-5">
                  <div class="form-group">
                    <label class="control-label">Page Title</label>
                    <input
                      type="text"
                      id="givingPageTitle"
                      v-model="currentGivingPage.PageTitle"
                      class="form-control"
                    />
                  </div>
                </div>
                <div class="col-lg-2 col-md-2 col-sm-2">
                  <div class="form-group">
                    <label class="control-label">Enabled</label>
                    <edit-giving-page-slider
                      v-model="currentGivingPage.Enabled"
                      v-bind:sliderValue="currentGivingPage.Enabled"
                      @changeValue="changeCurrentGivingPageEnabled"
                    ></edit-giving-page-slider>
                  </div>
                </div>
              </div>
              <div class="row">
                <div class="col-lg-5 col-md-5 col-sm-5">
                  <div class="form-group">
                    <label class="control-label">Skin</label>
                    <input
                      type="text"
                      id="givingSkin"
                      v-model="currentGivingPage.SkinFile"
                      class="form-control"
                    />
                  </div>
                </div>
                <div class="col-lg-5 col-md-5 col-sm-5">
                  <div class="form-group">
                    <label class="control-label">Page Type</label>
                    <MultiSelect
                      v-model="currentGivingPage.PageType"
                      :options="pageTypes"
                      :searchable="true"
                      :close-on-select="true"
                      :show-labels="false"
                      trackBy="id"
                      :custom-label="customLabel"
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
                      v-model="currentGivingPage.FundId"
                      :options="availableFunds"
                      :searchable="true"
                      :close-on-select="true"
                      :show-labels="false"
                      trackBy="FundId"
                      :custom-label="AvailableFundsCustomLabel"
                    ></MultiSelect>
                  </div>
                </div>
                <div class="col-lg-5 col-md-5 col-sm-5">
                  <div class="form-group">
                    <label class="control-label">Available Funds</label>
                    <MultiSelect
                      v-model="FundIdArray"
                      :options="availableFunds"
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
                    <input
                      type="text"
                      id="givingDisabledRedirect"
                      v-model="currentGivingPage.DisabledRedirect"
                      class="form-control"
                    />
                  </div>
                </div>
                <div class="col-lg-3 col-md-3 col-sm-3"></div>
              </div>
              <div class="row">
                <div class="col-lg-5 col-md-5 col-sm-5">
                  <div class="form-group">
                    <label class="control-label">Entry Point</label>
                    <MultiSelect
                      v-model="currentGivingPage.EntryPointId"
                      :options="entryPoints"
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
                    <input
                      type="text"
                      id="givingTopText"
                      v-model="currentGivingPage.TopText"
                      class="form-control"
                    />
                  </div>
                </div>
              </div>
              <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                  <div class="form-group">
                    <label class="control-label">Thank you message</label>
                    <input
                      type="text"
                      id="givingThankYouText"
                      v-model="currentGivingPage.ThankYouText"
                      class="form-control"
                    />
                  </div>
                </div>
              </div>
              <div class="row">
                <div class="col-lg-10 col-md-10 col-sm-10">
                  <div class="form-group">
                    <label class="control-label">Online Notify Person</label>
                    <MultiSelect
                      v-model="NotifyPersonArray"
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
                      v-model="currentGivingPage.ConfirmationEmailPledge"
                      :options="confirmationEmailList"
                      :searchable="true"
                      :close-on-select="true"
                      :show-labels="false"
                      trackBy="PeopleId"
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
                      v-model="currentGivingPage.ConfirmationEmailOneTime"
                      :options="confirmationEmailList"
                      :searchable="true"
                      :close-on-select="true"
                      :show-labels="false"
                      trackBy="PeopleId"
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
                      v-model="currentGivingPage.ConfirmationEmailRecurring"
                      :options="confirmationEmailList"
                      :searchable="true"
                      :close-on-select="true"
                      :show-labels="false"
                      trackBy="PeopleId"
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
    givingPageId: Number,
    pageTypes: Array,
    availableFunds: Array,
    entryPoints: Array,
    onlineNotifyPersonList: Array,
    confirmationEmailList: Array
  },
  components: {
    MultiSelect
  },
  data: function() {
    return {
      showMyModal: this.showEditModal,
      currentGivingPageId: this.givingPageId,
      currentGivingPage: null,
      // PageTypes: this.pageTypes,
      // AvailableFunds: this.availableFunds,
      // EntryPoints: this.entryPoints,
      // OnlineNotifyPersonList: this.onlineNotifyPersonList,
      // ConfirmationEmailList: this.confirmationEmailList,
      FundIdArray: [],
      NotifyPersonArray: []
    };
  },
  methods: {
    getSelectedGivingPage() {
      axios
        .get("/Giving/GetGivingPage", {
          params: {
            pageId: this.currentGivingPageId
          }
        })
        .then(
          response => {
            if (response.status === 200) {
              this.currentGivingPage = response.data;
            } else {
              alert("Warning! Something went wrong, try again later");
              //   warning_swal("Warning!", "Something went wrong, try again later");
            }
          },
          err => {
            alert("Fatal Error! We are working to fix it");
            console.log(err);
            // error_swal("Fatal Error!", "We are working to fix it");
          }
        )
        .catch(function(error) {
          console.log(error);
        });
    },
    AvailableFundsCustomLabel({ FundName }) {
      return `${FundName}`;
    },
    EntryPointCustomLabel({ Description }) {
      return `${Description}`;
    },
    NotifyPersonCustomLabel({Name}) {
      return `${Name}`;
    },
    ConfirmEmailCustomLabel({EmailAddress}) {
      return `${EmailAddress}`;
    },
    customLabel({ pageTypeName }) {
      return `${pageTypeName}`;
    },
    saveGivingPage() {
      //alert("save giving page button worked!");
      this.showMyModal = false;
    },
    changeCurrentGivingPageEnabled() {
      this.currentGivingPage.Enabled = !this.currentGivingPage.Enabled;
    }
  },
  created() {},
  mounted() {
    // this.getSelectedGivingPage();
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
  width: 100vw;
  height: 100vh;
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
    top: 47%;
    width: 90%;
    height: 90%;
  }
}
@media only screen and (min-width: 600px) {
  .modal {
    top: 33%;
    width: 85%;
    height: 58%;
  }
}
@media only screen and (min-width: 768px) {
  .modal {
    top: 33%;
    width: 85%;
    height: 58%;
  }
}
@media only screen and (min-width: 992px) {
  .modal {
    top: 33%;
    width: 85%;
    height: 58%;
  }
}
@media only screen and (min-width: 1200px) {
  .modal {
    top: 33%;
    width: 75%;
    height: 58%;
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