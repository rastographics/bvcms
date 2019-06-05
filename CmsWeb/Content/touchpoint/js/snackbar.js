function snackbar(msg, type = 'success', delay = '2000') {
    var el = $('.snackbar');
    el.text(msg);
    el.addClass(type + ' visible');
    setTimeout(function () {
        el.removeClass('visible');
    }, delay);
}
