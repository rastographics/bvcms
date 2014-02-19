$(function () {
    var $demo, duration, remaining, tour;
    tour = new Tour();
    duration = 5000;
    remaining = duration;
    tour.addSteps([
{
  title: "Welcome to Search Builder!"
  , content:"This tour will introduce you to Search Builder. " +
      "Click next to continue. " +
      "Once you have watched this tour, " +
      "you can <a id='tourdone' class='hide-tip red' data-hidetip='query' href='#'>click here</a> so you won't see it again."
  , backdrop: true
  , orphan: true
}, {
    title: "This is where it all begins"
  , element: "#SearchText"
  , placement: "bottom"
  , content: "At least this is where all search begins. " +
        "Click in this text box to see the menu. " +
        "Notice you can select Search Builder (to run a copy of your last search), New Search or Saved Searches. " +
        "The last five saved searches you have used are shown at the bottom of the list."
}, {
  title: "Name it and claim it..."
  , element: "#SaveAs"
  , placement: "bottom"
  , content: "You always get a copy of any query you want to look at (we call it the scratchpad). " +
      "As you add and edit Conditions, the search is automatically run and saved to the scratchpad. " +
    "You can give your search a name using this button. " +
      "You can even make it public. " +
      "Once you save your query with a name, you will be working with another copy in your scratchpad."
}, {
  title: "How conditions are grouped together"
  , element: "#conditions > ul > li:first > header > select"
  , placement: "bottom"
  , content: "This grey bar is the header for a group of one or more conditions. " +
      "The one I am pointing to is the top-most group and will always be there. " +
      "You can add other groups below as needed. " +
      "A group can be All True, Any True or All False. Click the dropdown to change this."
}, {
  title: "Add a new condition or group of conditions"
  , element: "a.addnewgroup:first"
  , placement: "right"
  , content: "Click here to add a new group or a new condition. " +
      "For a condition, a popup dialog box will appear allowing you to select and edit the parameters of the condition."
}, {
    title: "Just a cog in the machine"
  , element: "#conditions div.dropdown:first"
  , placement: "right"
  , content: "Click the little gear next to a group or a condition to " +
        "get a dropdown menu with actions such as cut, copy, paste, and delete. " +
        "This is how you will edit, copy and move conditions and groups around. You can even move them to a new query. " +
        "Only the actions that make sense at that point will be shown."
}, {
  title: "Edit a condition"
  , element: "ul.conditions li.condition:first"
  , placement: "bottom"
  , content: "Click the text of a condition to edit it."
  , orphan: true
}, {
  title: "What can I do with this?"
  , element: "div.btn-page-actions"
  , placement: "left"
  , content: "With the Blue Toolbar you can email, text, get reports, exports, " +
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
      "Be sure to check out our <a href='http://www.youtube.com/bvcmscom' target='_blank'>" +
      "YouTube channel.</a>"
  , backdrop: true
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