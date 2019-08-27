(function () {
    var lastTimeout = 0;
    window.snackbar = function (msg, type, delay) {
        type = type || 'success';
        delay = delay || 2000;
        var el = $('.snackbar');
        el.text(msg)
            .removeClass('success error')
            .addClass(type + ' visible');
        clearTimeout(lastTimeout);
        lastTimeout = setTimeout(function () {
            el.removeClass('visible');
        }, delay);
    }
})();
