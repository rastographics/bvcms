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
                <div>
                  <label class="control-label">Enabled</label>
                </div>
                <div>
                  <tp-toggle
                    v-model="newGivingEnabled"
                    v-on:changed="toggleNewGivingEnabled()"
                  ></tp-toggle>
                </div>
              </div>
            </div>
          </div>
          <div class="row">
            <div class="col-lg-5 col-md-5 col-sm-5">
              <div class="form-group">
                <label class="control-label">Shell</label>
                <MultiSelect
                  v-model="newGivingSkinFile"
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
                  v-model="newGivingPageType"
                  :options="pageTypes"
                  :searchable="true"
                  :close-on-select="true"
                  :show-labels="false"
                  :multiple="true"
                  :allowEmpty="false"
                  :preselectFirst="true"
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
                  placeholder="Pick at least one"
                  v-model="newDefaultFund"
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
                  select-label
                  v-model="newGivingFundsArray"
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
                <input type="text" v-model="newGivingDisabledRedirect" class="form-control" />
              </div>
            </div>
            <div class="col-lg-3 col-md-3 col-sm-3"></div>
          </div>
          <div class="row">
            <div class="col-lg-5 col-md-5 col-sm-5">
              <div class="form-group">
                <label class="control-label">Entry Point</label>
                <MultiSelect
                  v-model="newGivingEntryPoint"
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
import MultiSelect from "vue-multiselect";
export default {
  props: {
    showAddModal: Boolean,
    pageTypes: Array,
    availableFunds: Array,
    entryPoints: Array,
    shellList: Array
  },
  components: {
    MultiSelect
  },
  data: function() {
    return {
      showMyModal: this.showAddModal,
      newGivingPageName: "",
      newGivingPageTitle: "",
      newGivingEnabled: false,
      newGivingSkinFile: null,
      newGivingPageType: null,
      newDefaultFund: null,
      newGivingFundsArray: [],
      newGivingDisabledRedirect: "",
      newGivingEntryPoint: null
    };
  },
  methods: {
    createNewGivingPage() {
      this.showMyModal = false;
      axios
        .post("/Giving/CreateNew", {
          pageName: this.newGivingPageName,
          pageTitle: this.newGivingPageTitle,
          enabled: this.newGivingEnabled,
          pageType: this.newGivingPageType,
          defaultFund: this.newDefaultFund,
          skinFile: this.newGivingSkinFile,
          availFundsArray: this.newGivingFundsArray,
          disRedirect: this.newGivingDisabledRedirect,
          entryPoint: this.newGivingEntryPoint
        })
        .then(
          response => {
            if (response.status === 200) {
              this.$emit("add-givingPage", response.data);
              this.newGivingPageName = "";
              this.newGivingPageTitle = "";
              this.newGivingEnabled = false;
              this.newGivingSkinFile = null;
              this.newGivingPageType = null;
              this.newDefaultFund = null;
              this.newGivingFundsArray = null;
              this.newGivingDisabledRedirect = "";
              this.newGivingEntryPoint = null;
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
    },
    toggleNewGivingEnabled() {
      this.newGivingEnabled = !this.newGivingEnabled;
    },
    AvailableFundsCustomLabel({ FundName }) {
      return `${FundName}`;
    },
    EntryPointCustomLabel({ Description }) {
      return `${Description}`;
    },
    ShellCustomLabel({ Title }) {
      return `${Title}`;
    },
    customLabel({ pageTypeName }) {
      return `${pageTypeName}`;
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
  top: 31%;
  left: 50%;
  transform: translate(-50%, -50%);
  z-index: 9999;

  overflow-y: auto;
  width: 750px;
  height: 550px;
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
    top: 48%;
    left: 50%;
    width: 750px;
    height: 90%;
  }
}
@media only screen and (max-width: 600px) {
  .modal {
    top: 47%;
    left: 50%;
    width: 90%;
    height: 90%;
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
  transform: translateY(-100vh) translateX(-50%);
}
</style>
