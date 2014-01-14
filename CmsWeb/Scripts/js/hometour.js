$(function () {
    var $demo, duration, remaining, tour;
    tour = new Tour();
    duration = 5000;
    remaining = duration;
    tour.addSteps([
{
  title: "Welcome to the new UI!"
  , content: "This new look is in beta, but we want you to use it. " +
      "At any time, you may click the ? link on the menu and select Use Old Look. " +
      "However we would like for you to take this brief tour where we will show you a few things about the new menu. " +
      "Click next to continue. " +
      "Once you have watched this tour, " +
      "you can <a id='tourdone' class='hide-tip red' data-hidetip='home' href='#'>click here</a> so you won't see it again."
  , backdrop: true
  , orphan: true
}, {
    title: "This is where it all begins"
  , element: "#SearchText"
  , placement: "bottom"
  , content: "At least this is where all search begins. " +
        "Start your simple search for people or organizations by clicking in this text box as before. " +
        "Not only can you enter search text, but you will see an extended menu as well. " +
        "This menu includes Find People, Organization Search and Search Builder options (Did you notice it is no longer on the main menu?). " +
        "There are additional guided tours on the Person and Search Builder pages. "
}, {
    title: "You Finance Folks..."
  , element: "#adminstop"
  , placement: "bottom"
  , content: "Are you wondering how to access all of your stuff? Look under the Admin menu where we have neatly tucked away all sorts of goodies!"
}, {
    title: "Help? You got it!"
  , element: "#helpstop"
  , placement: "left"
  , content: "Click the ? link to see the new help page. " +
        "You will still be able to get context-sensitive articles, search our documentation using Google, or click Contact Support to send us a help request. " +
        "And now you will be able to upload screenshots with your request! "
}, {
    title: "Tag, you're it!"
  , element: "#tagstop"
  , placement: "bottom"
  , content: "Notice the new tag icon where we display your active tag name. We added a drop down list of all your tags making it easy to switch tags."
}, {
    title: "A word about you"
  , element: "#nav-account"
  , placement: "bottom"
  , content: "This is your user icon and name (the name will disappear on a narrow window, by the way). We added a Use Old Look link there in case you want to switch back to the same page in the old look. " +
        "You can also reset your help tips there, too, in case you want to look at these tours again."
}, {
  title: "Want to know more?"
  ,content: "This ends this little tour. " +
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