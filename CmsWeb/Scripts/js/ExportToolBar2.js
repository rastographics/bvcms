$(document).ready(function () {
    if (!$.InitFunctions)
        $.InitFunctions = {};

    $.InitFunctions.TagAllCallBack = function (a) {
        $(".taguntag:visible").text("Remove");
    };

    $('#UnTagAll').live("click", function (ev) {
        ev.preventDefault();
        var $a = $(this);
        $.block();
        $.post(this.href, null, function (ret) {
            $(".taguntag:visible").text(ret);
            $('[data-toggle="dropdown"]').parent().removeClass('open');
            $.unblock();
        });
        return false;
    });
    $(document).on("click", '#AddContact', function (ev) {
        ev.preventDefault();
        var url = this.href;
        bootbox.confirm("Are you sure you want to add a contact for all these people?", function (result) {
            if (result === true) {
                $.block();
                $.post(url, null, function (ret) {
                    $.unblock();
                    if (ret < 0)
                        $.growlUI("error", "too many people to add to a contact (max 100)");
                    else if (ret == 0)
                        $.growlUI("error", "no results");
                    else
                        window.location = ret;
                });
            }
        });
        return false;
    });
    $(document).on("click", '#AddTasks', function (ev) {
        ev.preventDefault();
        var message = "Are you sure you want to add a task for all these people?";
        if (window.location.pathname.contains("/Person"))
            message = "Are you sure you want to add a task for this person?";
        var url = this.href;
        bootbox.confirm(message, function (result) {
            if (result === true) {
                $.block();
                $.post(url, null, function (ret) {
                    $.unblock();
                    if (ret > 100)
                        $.growlUI("error", "too many people to add tasks for (max 100)");
                    else if (ret == 0)
                        $.growlUI("error", "no results");
                    else
                        window.location = "/Task";
                });
            }
        });
        return false;
    });
    $.QueryString = function (q, item) {
        var r = new Object();
        $.each(q.split('&'), function () {
            var kv = this.split('=');
            r[kv[0]] = kv[1];
        });
        return r[item];
    };
    $.block = function (message) {
        if (!message)
            message = '<h1>working on it...</h1>';
        $.blockUI({
            message: message,
            overlayCSS: { opacity: 0 },
            css: {
                border: '3px',
                padding: '15px',
                backgroundColor: '#aaa',
                '-webkit-border-radius': '10px',
                '-moz-border-radius': '10px',
                opacity: .9,
                color: '#000',
                width: '500px'
            }
        });
    };
    $.unblock = function () {
        $.unblockUI();
    };
    $.navigate = function (url, data) {
        url += (url.match(/\?/) ? "&" : "?") + data;
        window.location = url;
    };
    $.DateValid = function (d, growl) {
        var reDate = /^(0?[1-9]|1[012])[\/-](0?[1-9]|[12][0-9]|3[01])[\/-]((19|20)?[0-9]{2})$/i;
        if ($.dateFormat.startsWith('d'))
            reDate = /^(0?[1-9]|[12][0-9]|3[01])[\/-](0?[1-9]|1[012])[\/-]((19|20)?[0-9]{2})$/i;
        var v = true;
        if (!reDate.test(d)) {
            if (growl == true)
                $.growlUI("error", "enter valid date");
            v = false;
        }
        return v;
    };
    $.SortableDate = function (s) {
        var dt;
        if ($.dateFormat.startsWith('d'))
            dt = new Date(s.split('/')[2], s.split('/')[1] - 1, s.split('/')[0]);
        else
            dt = new Date(s.split('/')[2], s.split('/')[0] - 1, s.split('/')[1]);
        var dt2 = dt.getFullYear() + '-' + (dt.getMonth() + 1) + '-' + dt.getDate();
        return dt2;
    };

    jQuery.fn.center = function (parent) {
        if (parent) {
            parent = this.parent();
        } else {
            parent = window;
        }
        this.css({
            "position": "absolute",
            "top": ((($(parent).height() - this.outerHeight()) / 2) + $(parent).scrollTop() + "px"),
            "left": ((($(parent).width() - this.outerWidth()) / 2) + $(parent).scrollLeft() + "px")
        });
        return this;
    };
});
function getISODateTime(d){
    // padding function
    var s = function(p){
        return (''+p).length<2?'0'+p:''+p;
    };
    
    // default parameter
    if (typeof d === 'undefined'){
        var d = new Date();
    };
    
    // return ISO datetime
    return d.getFullYear() + '-' +
        s(d.getMonth()+1) + '-' +
        s(d.getDate()) + ' ' +
        s(d.getHours()) + ':' +
        s(d.getMinutes()) + ':' +
        s(d.getSeconds());
}
if (!String.prototype.format) {
  String.prototype.format = function() {
    var args = arguments;
    return this.replace(/{(\d+)}/g, function(match, number) { 
      return typeof args[number] != 'undefined'
        ? args[number]
        : match
      ;
    });
  };
}