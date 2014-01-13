$(function () {
    var $demo, duration, remaining, tour;
    tour = new Tour();
    duration = 5000;
    remaining = duration;
    tour.addSteps([
{
  title: "Welcome to the new Search Builder Page!"
  , content: "This page is in beta, but we want you to use it. " +
      "This guided tour will point a few things out to you. " +
      "Click next to continue. " +
      "Once you have watched this tour, " +
      "you can <a id='tourdone' class='hide-tip' data-hidetip='query' href='#'>click here</a> so you won't see it again."
  , backdrop: true
  , orphan: true
}, {
    title: "This is where it all begins"
  , element: "#SearchText"
  , placement: "bottom"
  , content: "At least this is where all search begins. " +
        "Start your searches by clicking in this text box. " +
        "You get a menu which allows you to do simple searches for a person or organization " +
        "or get to this page with the last search or start a new search. " +
        "You can go to view all Saved Searches. " +
        "The last five named searches you have used are shown at the bottom of the list."
}, {
  title: "Name it and claim it..."
  , element: "#SaveAs"
  , placement: "bottom"
  , content: "Or give it away... " +
      "You always get a copy of any query you want to look at (we call it the scratchpad). " +
      "As you build your searches, they are saved automatically to this scratchpad. " +
      "You can give your search a name using this button. " +
      "You can even make it public or give it to another user. " +
      "Once you save your query with a name, you will be working with another copy in your scratchpad."
}, {
  title: "How conditions are grouped together"
  , element: "#conditions > ul > li:first > header > select"
  , placement: "bottom"
  , content: "This grey bar is the header for one or more selecting conditions. " +
      "The one I am pointing to is the top most group and will always be there. " +
      "You can add other groups as needed. " +
      "You can change how the conditions are grouped (All or Any) by clicking on the dropdown."
}, {
  title: "Add a new condition or group of conditions"
  , element: "a.addnewgroup:first"
  , placement: "right"
  , content: "Click here to a new group or a new condition. " +
      "For a condition, a dialog box will popup allowing you to select and edit the parameters of the condition."
}, {
    title: "Just a cog in the machine"
  , element: "#conditions div.dropdown:first"
  , placement: "right"
  , content: "Click the little gear next to a group or a condition to " +
        "get a dropdown menu with actions like cut, copy, paste, and delete. " +
        "This is how you will edit, copy and move conditions and groups around, even to a new query. " +
        "Only the actions that make sense at that point will be shown."
}, {
  title: "Edit a condition"
  , element: "ul.conditions li.condition:first"
  , placement: "bottom"
  , content: "Click the text of a condition to edit it."
  , orphan: true
}, {
  title: "It's not easy being green"
  , element: "div.btn-page-actions"
  , placement: "left"
  , content: "This is the new Blue Toolbar you will use to email, text, get reports, exports, " +
      "and do other things with the resulting list of people."
}, {
  title: ""
  , element: ""
  , placement: ""
  , content: ""
}, {
  title: "Want to know more?"
  ,content: "This ends this little show. " +
      "The next time it starts, you can tell the system that you don't need to see it anymore. " +
      "Be sure to <a href='http://www.youtube.com/bvcmscom' target='_blank'>" +
      "watch the videos we will be doing to introduce the new UI.</a>"
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