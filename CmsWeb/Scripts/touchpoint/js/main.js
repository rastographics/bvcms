
/* standard block / unblock */
$.block = function (message) {
    if (!message)
        message = '<h1>working on it...</h1>';
    $.blockUI({
        message: message,
        overlayCSS: { opacity: 0 },
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: .5,
            color: '#fff'
        }
    });
};
$.unblock = function () {
    $.unblockUI();
};

/* helper methods */
String.prototype.startsWith = function (t, i) {
    return (t == this.substring(0, t.length));
};