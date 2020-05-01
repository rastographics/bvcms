<template>
  <div id="app">
    <!--<img alt="Vue logo" src="./assets/logo.png">-->
    <!--<AddGivingPage></AddGivingPage>-->
    <!--<UsersGrid></UsersGrid>-->
    <GivingPageIndex></GivingPageIndex>
  </div>
</template>

<script>
    //import UsersGrid from './components/UsersGrid/UsersGrid.vue';
    //import AddGivingPage from './components/Giving/AddGivingPage.vue';
    import GivingPageIndex from './components/Giving/GivingPageIndex.vue';

export default {
  name: 'App',
  components: {
      //UsersGrid,
      //AddGivingPage,
      GivingPageIndex,
  }
}
</script>

<style>
    /*#app {
      font-family: Avenir, Helvetica, Arial, sans-serif;
      -webkit-font-smoothing: antialiased;
      -moz-osx-font-smoothing: grayscale;
      text-align: center;
      color: #2c3e50;
      margin-top: 60px;
    }*/
</style>
