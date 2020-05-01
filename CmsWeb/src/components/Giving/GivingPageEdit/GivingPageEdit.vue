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
                    <select
                      class="form-control"
                      id="givingPageType"
                      v-model="currentGivingPage.PageType"
                    >
                      <option
                        v-for="pageType in PageTypes"
                        v-bind:value="pageType.id"
                        :key="pageType.id"
                      >{{pageType.pageTypeName}}</option>
                    </select>
                  </div>
                </div>
                <div class="col-lg-2 col-md-2 col-sm-2"></div>
              </div>
              <div class="row">
                <div class="col-lg-5 col-md-5 col-sm-5">
                  <div class="form-group">
                    <label class="control-label">Default Fund</label>
                    <input
                      type="text"
                      id="givingDefaultFund"
                      v-model="currentGivingPage.FundId"
                      class="form-control"
                    />
                  </div>
                </div>
                <div class="col-lg-5 col-md-5 col-sm-5">
                  <div class="form-group">
                    <label class="control-label">Available Funds</label>
                    <select
                      multiple
                      class="form-control"
                      id="givingAvailableFunds"
                      v-model="FundIdArray"
                    >
                      <option
                        v-for="availableFund in AvailableFunds"
                        v-bind:value="availableFund.id"
                        :key="availableFund.id"
                      >{{availableFund.pageTypeName}}</option>
                    </select>
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
                    <select
                      class="form-control"
                      style="width:90%;"
                      id="givingEntryPointId"
                      v-model="currentGivingPage.EntryPointId"
                    >
                      <option
                        v-for="entryPoint in EntryPoints"
                        v-bind:value="entryPoint.id"
                        :key="entryPoint.id"
                      >{{entryPoint.pageTypeName}}</option>
                    </select>
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
                    <select
                      multiple
                      class="form-control"
                      id="givingOnlineNotifyPerson"
                      v-model="NotifyPersonArray"
                    >
                      <option
                        v-for="onlineNotifyPerson in OnlineNotifyPersonList"
                        v-bind:value="onlineNotifyPerson.id"
                        :key="onlineNotifyPerson.id"
                      >{{onlineNotifyPerson.pageTypeName}}</option>
                    </select>
                  </div>
                </div>
              </div>
              <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                  <div class="form-group">
                    <label class="control-label">Confirmation Email - pledge</label>
                    <select
                      class="form-control"
                      id="givingConfirmationEmailPledge"
                      v-model="currentGivingPage.ConfirmationEmailPledge"
                    >
                      <option
                        v-for="confirmationEmail in ConfirmationEmailList"
                        v-bind:value="confirmationEmail.id"
                        :key="confirmationEmail.id"
                      >{{confirmationEmail.pageTypeName}}</option>
                    </select>
                  </div>
                </div>
              </div>
              <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                  <div class="form-group">
                    <label class="control-label">Confirmation Email - One time</label>
                    <select
                      class="form-control"
                      id="givingConfirmationEmailOneTime"
                      v-model="currentGivingPage.ConfirmationEmailOneTime"
                    >
                      <option
                        v-for="confirmationEmail in ConfirmationEmailList"
                        v-bind:value="confirmationEmail.id"
                        :key="confirmationEmail.id"
                      >{{confirmationEmail.pageTypeName}}</option>
                    </select>
                  </div>
                </div>
              </div>
              <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                  <div class="form-group">
                    <label class="control-label">Confirmation Email - recurring</label>
                    <select
                      class="form-control"
                      id="givingConfirmationEmailRecurring"
                      v-model="currentGivingPage.ConfirmationEmailRecurring"
                    >
                      <option
                        v-for="confirmationEmail in ConfirmationEmailList"
                        v-bind:value="confirmationEmail.id"
                        :key="confirmationEmail.id"
                      >{{confirmationEmail.pageTypeName}}</option>
                    </select>
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
export default {
  props: ["showEditModal", "givingPageId"],
  data: function() {
    return {
      showMyModal: this.showEditModal,
      currentGivingPageId: this.givingPageId,
      currentGivingPage: null,
      PageTypes: [],
      AvailableFunds: [],
      EntryPoints: [],
      OnlineNotifyPersonList: [],
      ConfirmationEmailList: [],
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
    getPageTypes: function() {
      let vm = this;
      axios
        .get("/Giving/GetPageTypes")
        .then(
          response => {
            if (response.status === 200) {
              vm.PageTypes = response.data;
            } else {
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
    getAvailableFunds: function() {
      let vm = this;
      axios
        .get("/Giving/GetAvailableFunds")
        .then(
          response => {
            if (response.status === 200) {
              vm.AvailableFunds = response.data;
            } else {
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
    getEntryPoints: function() {
      let vm = this;
      axios
        .get("/Giving/GetEntryPoints")
        .then(
          response => {
            if (response.status === 200) {
              vm.EntryPoints = response.data;
            } else {
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
    getOnlineNotifyPersonList: function() {
      let vm = this;
      axios
        .get("/Giving/GetOnlineNotifyPersonList")
        .then(
          response => {
            if (response.status === 200) {
              vm.OnlineNotifyPersonList = response.data;
            } else {
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
    getConfirmationEmailList: function() {
      let vm = this;
      axios
        .get("/Giving/GetConfirmationEmailList")
        .then(
          response => {
            if (response.status === 200) {
              vm.ConfirmationEmailList = response.data;
            } else {
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
    saveGivingPage() {
      //alert("save giving page button worked!");
      this.showMyModal = false;
    },
    changeCurrentGivingPageEnabled() {
      this.currentGivingPage.Enabled = !this.currentGivingPage.Enabled;
    }
  },
  created() {
    this.getSelectedGivingPage();
  },
  mounted() {
    this.getPageTypes();
    this.getAvailableFunds();
    this.getEntryPoints();
    this.getOnlineNotifyPersonList();
    this.getConfirmationEmailList();
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

/* .modal {
  display: block !important;
  position: fixed;
  top: 33vh;
  left: 50%;
  transform: translate(-50%, -50%);
  z-index: 9999;

  width: 75%;
  height: 587px;
  background-color: #fff;
  display: table;

  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
  border: 1px solid rgba(0, 0, 0, 0.2);
  border-radius: 0;
  background-clip: padding-box;
  outline: 0;
} */

/* Extra small devices (phones, 600px and down) */
@media only screen and (max-width: 600px) {
  .modal {
    display: block !important;
    position: fixed;
    top: 47%;
    left: 50%;
    transform: translate(-50%, -50%);
    z-index: 9999;

    overflow-y: scroll;
    width: 90%;
    height: 90%;
    background-color: #fff;
    display: table;

    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
    border: 1px solid rgba(0, 0, 0, 0.2);
    border-radius: 0;
    background-clip: padding-box;
    outline: 0;
  }
}

/* Small devices (portrait tablets and large phones, 600px and up) */
@media only screen and (min-width: 600px) {
  .modal {
    display: block !important;
    position: fixed;
    top: 33%;
    left: 50%;
    transform: translate(-50%, -50%);
    z-index: 9999;

    overflow-y: scroll;
    width: 85%;
    height: 58%;
    background-color: #fff;
    display: table;

    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
    border: 1px solid rgba(0, 0, 0, 0.2);
    border-radius: 0;
    background-clip: padding-box;
    outline: 0;
  }
}

/* Medium devices (landscape tablets, 768px and up) */
@media only screen and (min-width: 768px) {
  .modal {
    display: block !important;
    position: fixed;
    top: 33%;
    left: 50%;
    transform: translate(-50%, -50%);
    z-index: 9999;

    overflow-y: scroll;
    width: 85%;
    height: 58%;
    background-color: #fff;
    display: table;

    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
    border: 1px solid rgba(0, 0, 0, 0.2);
    border-radius: 0;
    background-clip: padding-box;
    outline: 0;
  }
}

/* Large devices (laptops/desktops, 992px and up) */
@media only screen and (min-width: 992px) {
  .modal {
    display: block !important;
    position: fixed;
    top: 33%;
    left: 50%;
    transform: translate(-50%, -50%);
    z-index: 9999;

    overflow: hidden;
    width: 85%;
    height: 58%;
    background-color: #fff;
    display: table;

    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
    border: 1px solid rgba(0, 0, 0, 0.2);
    border-radius: 0;
    background-clip: padding-box;
    outline: 0;
  }
}

/* Extra large devices (large laptops and desktops, 1200px and up) */
@media only screen and (min-width: 1200px) {
  .modal {
    display: block !important;
    position: fixed;
    top: 33%;
    left: 50%;
    transform: translate(-50%, -50%);
    z-index: 9999;

    overflow: hidden;
    width: 75%;
    height: 58%;
    background-color: #fff;
    display: table;

    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
    border: 1px solid rgba(0, 0, 0, 0.2);
    border-radius: 0;
    background-clip: padding-box;
    outline: 0;
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