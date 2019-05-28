$(document).ready(function () {
    $(".numeric").keypress(function (event) {
        // Backspace, tab, enter, end, home, left, right
        // We don't support the del key in Opera because del == . == 46.
        var controlKeys = [8, 9, 13, 35, 36, 37, 39];
        // IE doesn't support indexOf
        var isControlKey = controlKeys.join(",").match(new RegExp(event.which));
        // Some browsers just don't raise events for control keys. Easy.
        // e.g. Safari backspace.
        if (!event.which || // Control keys in most browsers. e.g. Firefox tab is 0
            (49 <= event.which && event.which <= 57) || // Always 1 through 9
            (48 == event.which && $(this).attr("value")) || // No 0 first digit
            (48 == event.which && ($(this).val() > 0)) || // allows 0.
            (46 == event.which && !($(this).val().includes("."))) || //allowe .
            isControlKey) { // Opera assigns values for control keys.
            return;
        } else {
            event.preventDefault();
        }
    });
});

$("body").on("click", 'button.editpledge', function (ev) {
    ev.preventDefault();
    var a = $(this);
    $("#editpledgeurl").val(a.attr("href"));
    $("#currentpledge").val(a.attr("amount"));
    $('#editPledge-modal').modal();
    return false;
});
