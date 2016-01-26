$(function () {
    var $demo, duration, remaining, tour;
    tour = new Tour();
    duration = 5000;
    remaining = duration;
    tour.addSteps([
{
  title: "Welcome to the Person Page!"
  , content: "In this tour, we will show you a few things to help you get acclimated. " +
      "Click next to continue. " +
      "Once you have watched this tour, " +
      "you can <a id='tourdone' class='hide-tip red' data-hidetip='person' href='#'>click here</a> so you won't see it again."
  , backdrop: true
  , orphan: true
}, {
  title: "Edit an Address"
  , element: "a.edit.editaddr:first"
  , placement: "right"
  , content: "Click the pencil icon. " +
      "The pencil is used frequently in the new UI to indicate edit in place of the Edit Button. " +
      "A dialog box will appear. This dialog is also where you can add a personal address."
}, {
  title: "We got Badges!"
  , element: "li.badges span:first"
  , placement: "bottom"
  , content: "These are status flags presented as badges on a person's record. " +
      "The green ones are updated every night. The blue ones are for displaying and editing campus and family position."
}, {
  title: "The blue toolbar" // only for access users
  , element: '#bluetoolbarstop'
  , placement: "bottom"
  ,content: "Lots of things to do here." +
      "This is where you will email, run reports/exports and do other management functions."
}, {
  title: "The blue toolbar" // for mydata users
  , element: '#mydatabluetoolbarstop'
  , placement: "bottom"
  ,content: "There is a User button which will allow you to logout, change password, and reset help tips. " +
      "The ? takes you to the help page. You can contact your church there. "
}, {
  title: "Famliy Sidebar"
  ,element: "#family-div"
  , placement: "left"
  ,content: "This is where the family shows. " +
      "The current family member will have a blue bar and a white background. " +
      "Click on another family member to bring up their page."
}, {
  title: "Add to Family"
  ,element: "#family-div a.searchadd"
  , placement: "left"
  ,content: "Click the + to add a new family member."
}, {
  title: "Related Families"
  ,element: "#related-families-div"
  , placement: "left"
  ,content: "Related families show here. " +
      "Click the + to add a related family. " +
      "For an Existing related family, click the pencil to edit the desription. " +
      "Click the family name to go to the head of that family."
}, {
  title: "Family Photo"
  ,element: "#family-photo-div"
  , placement: "left"
  ,content: "You can have a family photo in addtion to the personal photo! " +
      "Click the + to upload a photo. " +
      "Click the picture to edit or delete an Existing photo."
}, {
  title: "Stuff in the Tabs"
  , element: "#involvementstop"
  , placement: "bottom"
  ,content: 
      "Involvement is for memberships past and present. " +
      "Profile is for Church Membership and " +
      "Extra Values are found there as well. "
}, {
    title: "More Stuff in the Tabs" // for access users
  , element: "#ministrystop"
  , placement: "bottom"
  ,content: "Ministry has Contacts and Tasks sub-tabs. " +
      "If you are on your own record or have Finance role, " +
      "there will be a Giving tab which also contains links for online giving. " +
      "There is also an Emails tab with sub-tabs for sent/received, etc. " +
      "The System tab is  where you find users, changes, and duplicates."
}, {
    title: "More Stuff in the Tabs" // for mydata users
  , element: "#mydatagiving"
  , placement: "bottom"
  ,content: "<p>Your Giving tab contains a list of your contributions, " +
      "a tab for generating statements, " +
      "and a tab with links for managing your recurring giving or making a one-time gift. " +
        "By the way, only you and a Finance person will ever be able to see this information.</p>" +
        "<p>The emails tab will be a list of emails you have received, both regular and confirmations (transactional). " +
        "There is also a tab to manage your email opt-outs and see your failed emails." +
        "</p>"
}, {
  title: "Want to know more?"
  ,content: "This ends this little tour. " +
      "The next time it starts, you can tell the system that you don't need to see it anymore. " +
      "Be sure to check out our <a href='http://www.youtube.com/bvcmscom' target='_blank'>" +
      "the videos on our YouTube channel.</a>"
  ,backdrop: true
  ,orphan: true
}
    ]);
    tour.init();
    tour.restart();
    $("html").smoothScroll();
    $("#tourdone").live("click", function (ev) {
        ev.preventDefault();
        tour.end();
    });
});