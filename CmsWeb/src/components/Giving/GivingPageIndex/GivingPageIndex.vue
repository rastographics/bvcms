<template>
  <div>
    <div>
      <div v-if="givingPageList.length === 0" class="box box-responsive">
        <div class="box-content">
          <div class="row hidden-xs">
            <div class="col-sm-12">
              <div class="pull-right">
                <button class="btn btn-success">
                  <i class="fa fa-plus-circle"></i> Giving Page
                </button>
              </div>
            </div>
          </div>
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
                    <a style="cursor:pointer">Skin</a>
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
                <tr></tr>
              </tbody>
            </table>
          </div>
          <button class="btn btn-success btn-block visible-xs-block">
            <i class="fa fa-plus-circle"></i> Giving Page
          </button>
        </div>
      </div>

      <div v-if="givingPageList.length > 0" class="box box-responsive">
        <div class="box-content">
          <add-giving-page
            v-bind:showAddModal="showAddModal"
            :pageTypes="PageTypes"
            :availableFunds="AvailableFunds"
            :entryPoints="EntryPoints"
            @click="showAddModal = true"
            v-on:add-givingPage="AddNewGivingPageToList"
          ></add-giving-page>
          <div class="table-responsive">
            <table class="table table-striped">
              <thead>
                <tr>
                  <th>
                    <a v-on:click="sort('codiceCliente')" style="cursor:pointer">Enabled</a>
                  </th>
                  <th>
                    <a v-on:click="sort('ragioneSociale')" style="cursor:pointer">Page Name</a>
                  </th>
                  <th>
                    <a v-on:click="sort('provincia')" style="cursor:pointer">Skin</a>
                  </th>
                  <th>
                    <a v-on:click="sort('indirizzo')" style="cursor:pointer">Page Type</a>
                  </th>
                  <th>
                    <a v-on:click="sort('provincia')" style="cursor:pointer">Default Fund</a>
                  </th>
                  <th>
                    <a v-on:click="sort('provincia')" style="cursor:pointer">Action</a>
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="items in givingPageList" :key="items.GivingPageId">
                  <td>
                    <giving-page-index-slider
                      v-bind:sliderValue="items.Enabled"
                      :givingPageId="items.GivingPageId"
                    ></giving-page-index-slider>
                  </td>
                  <td>{{ items.PageName }}</td>
                  <td>{{ items.Skin }}</td>
                  <td>{{ items.PageType }}</td>
                  <td>{{ items.DefaultFund }}</td>
                  <td>
                    <edit-giving-page
                      v-bind:showEditModal="showEditModal"
                      :givingPageId="items.GivingPageId"
                      :pageTypes="PageTypes"
                      :availableFunds="AvailableFunds"
                      :entryPoints="EntryPoints"
                      :onlineNotifyPersonList="OnlineNotifyPersonList"
                      :confirmationEmailList="ConfirmationEmailList"
                      @click="showEditModal = true"
                    ></edit-giving-page>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div class="row visible-xs-block">
            <div class="col-sm-12">
              <div style="text-align: center;">
                <!-- <add-giving-page
                  v-bind:showAddModal="showAddModal"
                  :pageTypes="PageTypes"
                  :availableFunds="AvailableFunds"
                  :entryPoints="EntryPoints"
                  @click="showAddModal = true"
                ></add-giving-page> -->
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  components: {},
  data: function() {
    return {
      givingPageList: [],
      showAddModal: false,
      showEditModal: false,
      selectedGivingPageId: null,
      selectedGivingPage: null,
      PageTypes: [],
      AvailableFunds: [],
      EntryPoints: [],
      OnlineNotifyPersonList: [],
      ConfirmationEmailList: []
    };
  },
  methods: {
    fetchGivingPages() {
      axios
        .get("/Giving/GetGivingPageList")
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
      axios
        .get("/Giving/GetGivingPage", {
          params: {
            pageId: id
          }
        })
        .then(
          response => {
            if (response.status === 200) {
              this.selectedGivingPage = response.data;
              this.showEditModal = true;
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
    hideEditModalMethod() {
      this.showEditModal = false;
    },
    getPageTypes: function() {
      axios
        .get("/Giving/GetPageTypes")
        .then(
          response => {
            if (response.status === 200) {
              this.PageTypes = response.data;
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
    AddNewGivingPageToList(newGivingPage) {
      this.givingPageList = [...this.givingPageList, newGivingPage[0]];
    }
  },
  mounted() {
    this.fetchGivingPages();
    this.getPageTypes();
    this.getAvailableFunds();
    this.getEntryPoints();
    this.getOnlineNotifyPersonList();
    this.getConfirmationEmailList();
  }
};
</script>

<style scoped>
/* Extra large devices (large laptops and desktops, 1200px and up) */
@media only screen and (min-width: 1200px) {
  .box {
    width: 60vw;
  }
}
</style>
