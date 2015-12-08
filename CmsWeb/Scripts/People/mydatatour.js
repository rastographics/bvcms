$(function () {
    var $demo, duration, remaining, tour;
    tour = new Tour();
    duration = 5000;
    remaining = duration;
    tour.addSteps([
{
  title: "Welcome to your profile page!"
  , content: "In this tour, we will show you a few things to help you get acclimated. " +
      "Click next to continue. " +
      "Once you have watched this tour, " +
      "you can <a id='tourdone' class='hide-tip red' data-hidetip='mydata' href='/'>click here</a> so you won't see it again."
  , backdrop: true
  , orphan: true
}, {
  title: "Edit an Address"
  , element: "a.edit.editaddr:first"
  , placement: "right"
  , content: "Click the pencil icon. " +
      "The pencil is used to indicate edit in place of the Edit Button. " +
      "A dialog box will appear allowing you to change your address. "
}, {
  title: "We got Badges!"
  , element: "li.badges span:first"
  , placement: "bottom"
  , content: "These are status flags presented as badges on your record. " +
      "The green ones are updated every night. The blue ones are for displaying campus and family position."

}, {
  title: "The blue toolbar"
  , element: '#mydatabluetoolbarstop'
  , placement: "bottom"
  ,content: "There is a User button which will allow you to logout, change your password, and reset the help tip. " +
      "The ? takes you to the help page. You can contact your church there. "
}, {
  title: "Famliy Sidebar"
  ,element: "#family-div"
  , placement: "left"
  ,content: "If you have other family members, this is where they will display. " +
      "The current family member you are viewing will have a blue bar and a white background. " +
      "Click on another family member to bring up their page."
}, {
  title: "Family Photo"
  ,element: "#family-photo-div"
  , placement: "left"
  ,content: "You can have a family photo in addtion to your personal photo! " +
      "Click the + to upload a photo. " +
      "Click the picture to edit or delete an Existing photo."
}, {
  title: "Stuff in the Tabs"
  , element: "#involvementstop"
  , placement: "bottom"
  ,content: "You can edit your information on the Personal tab. " +
      "The Involvement tab shows all of your current and past memberships. " +
      "The Profile tab shows your Church Membership information. " 
}, {
    title: "More Stuff in the Tabs"
  , element: "#givingstop"
  , placement: "bottom"
  ,content: "<p>Your Giving tab contains a list of your contributions, " +
      "a tab for generating statements, " +
      "and a tab with links for managing your recurring giving or making a one-time gift. " +
        "By the way, only you and a Finance person will ever be able to see this information.</p>" +
        "<p>The emails tab will be a list of emails you have received: regular, confirmations, and other transactional emails. " +
        "There is also a tab to manage your email opt-outs (unsubscribe) and see failure notices about emails that did not reach you." +
        "</p>"
}, {
  title: "That's all folks"
  ,content: "This ends this little tour. " +
      "The next time it starts, you can tell the system that you don't need to see the tour anymore. "
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