<template>
  <div>
    <div class="row">
      <div class="col-sm-12">
        <div class="pull-right">
          <button class="btn btn-success hidden-xs" @click="showMyModal = true">
            <i class="fa fa-plus-circle"></i> Giving Page
          </button>
        </div>
      </div>
    </div>
    <button
      class="btn btn-success btn-block visible-xs-block hidden-sm hidden-md hidden-lg"
      @click="showMyModal = true"
    >
      <i class="fa fa-plus-circle"></i> Giving Page
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
          <h4 class="modal-title">Add Giving Page</h4>
        </div>
        <div class="modal-body">
          <div class="row">
            <div class="col-lg-5 col-md-5 col-sm-5">
              <div class="form-group">
                <label class="control-label">Page Name</label>
                <input type="text" v-model="newGivingPageName" class="form-control" />
              </div>
            </div>
            <div class="col-lg-5 col-md-5 col-sm-5">
              <div class="form-group">
                <label class="control-label">Page Title</label>
                <input type="text" v-model="newGivingPageTitle" class="form-control" />
              </div>
            </div>
            <div class="col-lg-2 col-md-2 col-sm-2">
              <div class="form-group">
                <label class="control-label">Enabled</label>
                <generic-slider v-model="newGivingEnabled"></generic-slider>
              </div>
            </div>
          </div>
          <div class="row">
            <div class="col-lg-5 col-md-5 col-sm-5">
              <div class="form-group">
                <label class="control-label">Skin</label>
                <input type="text" v-model="newGivingSkinFile" class="form-control" />
              </div>
            </div>
            <div class="col-lg-5 col-md-5 col-sm-5">
              <div class="form-group">
                <label class="control-label">Page Type</label>
                <select class="form-control" v-model="newGivingPageType">
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
                <input type="text" v-model="newGivingFundId" class="form-control" />
              </div>
            </div>
            <div class="col-lg-5 col-md-5 col-sm-5">
              <div class="form-group">
                <label class="control-label">Available Funds</label>
                <select multiple class="form-control" v-model="newGivingFundIdArray">
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
                <input type="text" v-model="newGivingDisabledRedirect" class="form-control" />
              </div>
            </div>
            <div class="col-lg-3 col-md-3 col-sm-3"></div>
          </div>
          <div class="row">
            <div class="col-lg-5 col-md-5 col-sm-5">
              <div class="form-group">
                <label class="control-label">Entry Point</label>
                <select class="form-control" style="width:90%;" v-model="newGivingEntryPointId">
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
        </div>
        <div class="modal-footer">
          <button
            class="btn btn-default"
            style="border-radius:5px;"
            @click="showMyModal = false"
          >Cancel</button>
          <button
            class="btn btn-primary"
            style="border-radius:5px;"
            @click="createNewGivingPage"
          >Save</button>
        </div>
      </div>
    </transition>
  </div>
</template>

<script>
import axios from "axios";
export default {
  props: ["showAddModal"],
  data: function() {
    return {
      showMyModal: this.showAddModal,
      PageTypes: [],
      AvailableFunds: [],
      EntryPoints: [],
      newGivingPageName: "",
      newGivingPageTitle: "",
      newGivingEnabled: false,
      newGivingSkinFile: "",
      newGivingPageType: 0,
      newGivingFundId: 0,
      newGivingFundIdArray: [],
      newGivingDisabledRedirect: "",
      newGivingEntryPointId: 0
    };
  },
  methods: {
    createNewGivingPage() {
      //alert("create new giving page button worked!");
      this.showMyModal = false;
      // $.ajax({
      //   type: "POST",
      //   dataType: "json",
      //   url: "/Giving/CreateNewGivingPage",
      //   data: {
      //     pageName: this.newGivingPageName,
      //     pageTitle: this.newGivingPageTitle,
      //     enabled: this.newGivingEnabled,
      //     skin: this.newGivingSkinFile,
      //     pageType: this.newGivingPageType,
      //     fundId: this.newGivingFundId,
      //     fundArray: this.newGivingFundIdArray,
      //     disabledRedirect: this.newGivingDisabledRedirect,
      //     entryPointId: this.newGivingEntryPointId
      //   },
      //   success: function(response) {
      //     //var asd = 1;
      //     //alert(response);
      //   },
      //   error: function(response) {
      //     var asd = 1;
      //     alert("fail");
      //   }
      // });
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
              alert("Warning! Something went wrong, try again later");
              // warning_swal("Warning!", "Something went wrong, try again later");
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
              alert("Warning! Something went wrong, try again later");
              // warning_swal("Warning!", "Something went wrong, try again later");
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
              alert("Warning! Something went wrong, try again later");
              // warning_swal("Warning!", "Something went wrong, try again later");
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
    }
  },
  mounted() {
    this.getPageTypes();
    this.getAvailableFunds();
    this.getEntryPoints();
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
  top: 33vh;
  left: 50%;
  transform: translate(-50%, -50%);
  z-index: 9999;

  width: 600px;
  height: 587px;
  background-color: #fff;
  display: table;

  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
  border: 1px solid rgba(0, 0, 0, 0.2);
  border-radius: 0;
  background-clip: padding-box;
  outline: 0;
}

@media only screen and (max-width: 768px) {
  .modal {
    display: block !important;
    position: fixed;
    top: 33vh;
    left: 50%;
    transform: translate(-50%, -50%);
    z-index: 9999;

    overflow-y: scroll;
    width: 600px;
    height: 587px;
    background-color: #fff;
    display: table;

    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
    border: 1px solid rgba(0, 0, 0, 0.2);
    border-radius: 0;
    background-clip: padding-box;
    outline: 0;
  }
}

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
/* @media only screen and (min-width: 600px) {
  .modal {
    display: block !important;
    position: fixed;
    top: 33vh;
    left: 50%;
    transform: translate(-50%, -50%);
    z-index: 9999;

    overflow-y: scroll;
    width: 600px;
    height: 587px;
    background-color: #fff;
    display: table;

    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
    border: 1px solid rgba(0, 0, 0, 0.2);
    border-radius: 0;
    background-clip: padding-box;
    outline: 0;
  }
} */

/* Medium devices (landscape tablets, 768px and up) */
/* @media only screen and (min-width: 768px) {
  .modal {
    position: fixed;
    top: 32vh;
    left: 50%;
    transform: translate(-50%, -50%);
    z-index: 9999;

    width: 600px;
    background-color: #fff;
    display: table;

    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
    border: 1px solid rgba(0, 0, 0, 0.2);
    border-radius: 0;
    background-clip: padding-box;
    outline: 0;
  }
} */

/* Large devices (laptops/desktops, 992px and up) */
/* @media only screen and (min-width: 992px) {
  .modal {
    position: fixed;
    top: 41vh;
    left: 50%;
    transform: translate(-50%, -50%);
    z-index: 9999;

    width: 600px;
    background-color: #fff;
    display: table;

    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
    border: 1px solid rgba(0, 0, 0, 0.2);
    border-radius: 0;
    background-clip: padding-box;
    outline: 0;
  }
} */

/* Extra large devices (large laptops and desktops, 1200px and up) */
/* @media only screen and (min-width: 1200px) {
  .modal {
    position: fixed;
    top: 32vh;
    left: 50%;
    transform: translate(-50%, -50%);
    z-index: 9999;

    width: 600px;
    background-color: #fff;
    display: table;

    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
    border: 1px solid rgba(0, 0, 0, 0.2);
    border-radius: 0;
    background-clip: padding-box;
    outline: 0;
  }
} */

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
  transform: translateY(-100vh) translateX(-50%);
}
</style>