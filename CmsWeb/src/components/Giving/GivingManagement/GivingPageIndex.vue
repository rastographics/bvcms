<template>
  <div>
    <div>
      <div class="box box-responsive">
        <div class="box-content">
          <add-giving-page
            v-bind:showAddModal="showAddModal"
            :pageTypes="PageTypeList"
            :availableFunds="AvailableFunds"
            :entryPoints="EntryPoints"
            :shellList="ShellList"
            @click="showAddModal = true"
            v-on:add-givingPage="AddNewGivingPageToList"
          ></add-giving-page>
          <div class="table-responsive">
            <table class="table table-striped">
              <thead>
                <tr>
                  <th>
                    <a style="cursor:pointer">Enabled</a>
                  </th>
                  <th>
                    <a style="cursor:pointer">Page Name</a>
                  </th>
                  <th>
                    <a style="cursor:pointer">Shell</a>
                  </th>
                  <th>
                    <a style="cursor:pointer">Page Type</a>
                  </th>
                  <th>
                    <a style="cursor:pointer">Default Fund</a>
                  </th>
                  <th>
                    <a style="cursor:pointer">Action</a>
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(items, index) in givingPageList" :key="items.GivingPageId">
                  <td>
                    <tp-toggle
                        v-model="items.Enabled"
                        @input="toggleIndexSlider(items.GivingPageId, items.Enabled)"
                    ></tp-toggle>
                  </td>
                  <td>{{ items.PageName }}</td>
                  <td v-if="items.SkinFile != null">{{ items.SkinFile.Title }}</td>
                  <td v-else></td>
                  <td>{{ items.PageTypeString }}</td>
                  <td v-if="items.DefaultFund != null">{{ items.DefaultFund.FundName }}</td>
                  <td v-else></td>
                  <td>
                    <edit-giving-page
                      v-bind:showEditModal="showEditModal"
                      :pageId="items.GivingPageId"
                      :pageName="items.PageName"
                      :pageTitle="items.PageTitle"
                      :pageEnabled="items.Enabled"
                      :pageSkin="items.SkinFile"
                      :pageType="items.PageType"
                      :defaultFund="items.DefaultFund"
                      :availableFunds="items.AvailableFunds"
                      :pageDisabledRedirect="items.DisabledRedirect"
                      :pageEntryPoint="items.EntryPoint"
                      :campusId="items.EntryPoint"
                      :topText="items.TopText"
                      :thankYouText="items.ThankYouText"
                      :onlineNotifyPerson="items.OnlineNotifyPerson"
                      :confirmEmailPledge="items.ConfirmEmailPledge"
                      :confirmEmailOneTime="items.ConfirmEmailOneTime"
                      :confirmEmailRecurring="items.ConfirmEmailRecurring"
                      :pageTypeList="PageTypeList"
                      :fundsList="AvailableFunds"
                      :entryPointList="EntryPoints"
                      :onlineNotifyPersonList="OnlineNotifyPersonList"
                      :confirmationEmailList="ConfirmationEmailList"
                      :shellList="ShellList"
                      :currentIndex="index"
                      v-on:updatePage="UpdateCurrentGivingPage"
                    ></edit-giving-page>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  data: function() {
    return {
      givingPageList: [],
      showAddModal: false,
      showEditModal: false,
      PageTypeList: [],
      AvailableFunds: [],
      EntryPoints: [],
      OnlineNotifyPersonList: [],
      ConfirmationEmailList: [],
      ShellList: [],
      myCurrentIndex: null,
      testee: null
    };
  },
  methods: {
    fetchGivingPages() {
      axios
        .get("/Giving/List")
        .then(
          response => {
            if (response.status === 200) {
              this.givingPageList = response.data;
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
    showEditModalMethod(id) {
      this.showEditModal = true;
    },
    hideEditModalMethod() {
      this.showEditModal = false;
    },
    getPageTypes: function() {
      axios
        .get("/Giving/GetPageTypes")
        .then(
          response => {
            if (response.status === 200) {
              this.PageTypeList = response.data;
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
    getAvailableFunds: function() {
      axios
        .get("/Giving/GetAvailableFunds")
        .then(
          response => {
            if (response.status === 200) {
              this.AvailableFunds = response.data;
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
    getEntryPoints: function() {
      axios
        .get("/Giving/GetEntryPoints")
        .then(
          response => {
            if (response.status === 200) {
              this.EntryPoints = response.data;
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
    getOnlineNotifyPersonList: function() {
      axios
        .get("/Giving/GetOnlineNotifyPersonList")
        .then(
          response => {
            if (response.status === 200) {
              this.OnlineNotifyPersonList = response.data;
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
    getConfirmationEmailList: function() {
      axios
        .get("/Giving/GetConfirmationEmailList")
        .then(
          response => {
            if (response.status === 200) {
              this.ConfirmationEmailList = response.data;
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
    getShellList: function() {
      axios
        .get("/Giving/GetShellList")
        .then(
          response => {
            if (response.status === 200) {
              this.ShellList = response.data;
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
    AddNewGivingPageToList(newGivingPage) {
      this.givingPageList = [...this.givingPageList, newGivingPage[0]];
    },
    toggleIndexSlider(id, value) {
      axios
        .post("/Giving/SaveGivingPageEnabled", {
          value: value,
          currentGivingPageId: id
        })
        .then(res => snackbar("Giving page " + (value ? "enabled" : "disabled"), "success"))
        .catch(err => console.log(err));
    },
    UpdateCurrentGivingPage(updatedGivingPage) {
      this.testee = updatedGivingPage[0];
      let myUpdatedPage = updatedGivingPage[0];
      this.givingPageList.splice(myUpdatedPage.CurrentIndex, 1, myUpdatedPage);
      //alert(myUpdatedPage.CurrentIndex);
    }
  },
  mounted() {
    this.fetchGivingPages();
    this.getPageTypes();
    this.getAvailableFunds();
    this.getEntryPoints();
    this.getOnlineNotifyPersonList();
    this.getConfirmationEmailList();
    this.getShellList();
  }
};
</script>

<style src="vue-multiselect/dist/vue-multiselect.min.css"></style>

<style scoped>
/* Extra large devices (large laptops and desktops, 1200px and up) */
@media only screen and (min-width: 1200px) {
  .box {
    width: 60vw;
  }
}
</style>
