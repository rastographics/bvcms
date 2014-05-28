$(document).ready(function () {
    $("#ChooseFormat").dialog({
        overlay: { background: "#000", opacity: 0.3 },
        bgiframe: true,
        modal: true,
        autoOpen: false,
        closeOnEscape: true,
        width: 400
    });
    $("#TagAllDialog").dialog({
        overlay: { background: "#000", opacity: 0.3 },
        bgiframe: true,
        modal: true,
        autoOpen: false,
        closeOnEscape: true
    });
    $(".ChooseFormat").live("click", function (ev) {
        ev.preventDefault();
        $('div.dropdown2-menu').hide();
        var d = $("#ChooseFormat");
        $("#UseMailFlags").prop('checked', $(this).hasClass("UseMailFlags"));
        d.dialog("open");

        var sep = ev.target.href.search(/\?/) == -1 ? "?" : "&";
        $("#cmdOK").unbind("click").click(function (ev2) {
            ev2.preventDefault();
            var url = ev.target.href
                + sep + "titles=" + $('#UseTitle')[0].checked
                + "&format=" + $('input[name=format]:checked').val()
                + "&usephone=" + $('#UsePhone')[0].checked
                + "&usemailflags=" + $('#UseMailFlags')[0].checked
                + "&sortzip=" + $('#SortZip')[0].checked
				+ "&skipNum=" + $('#SkipNum').val();
            $("#ChooseFormat").dialog("close");
            window.open(url);
            return false;
        });
        return false;
    });
    $('#TagAll').live("click", function (ev) {
        ev.preventDefault();
        var action = this.href;
        $('div.dropdown2-menu').hide();
        var d = $("#TagAllDialog").dialog("open");
        $("#TagAllRun").click(function (ev) {
            if ($("#tagalltagname").val().length === 0) {
                alert("need tagname");
                return false;
            }
            ev.preventDefault();
            d.dialog("close");
            var q = $(this).closest("form").serialize();
            $.block();
            $.post(action, q, function (ret) {
                if (ret === "Manage")
                    window.location = "/Tags";
                else {
                    $(".taguntag:visible").text(ret);
                    $.unblock();
                }
            });
            return false;
        });
        return false;
    });
    $('#UnTagAll').live("click", function (ev) {
        ev.preventDefault();
        $('div.dropdown2-menu').hide();
        $.block();
        $.post(this.href, null, function (ret) {
            $(".taguntag:visible").text(ret);
            $.unblock();
        });
        return false;
    });
    $('body').on("click", '#AddContact', function (ev) {
        $('div.dropdown2-menu').hide();
        ev.preventDefault();
        if (!confirm("Are you sure you want to add a contact for all these people?"))
            return false;
        $.block();
        $.post(this.href, null, function (ret) {
            $.unblock();
            if (ret < 0)
                $.growlUI("error", "too many people to add to a contact (max 100)");
            else if (ret == 0)
                $.growlUI("error", "no results");
            else
                window.location = ret;
        });
        return false;
    });
    $('body').on("click", '#AddTasks', function (ev) {
        ev.preventDefault();
        $('div.dropdown2-menu').hide();
        if (!confirm("Are you sure you want to add a task for each of these people?"))
            return false;
        $.block();
        $.post(this.href, null, function (ret) {
            $.unblock();
            if (ret < 0)
                $.growlUI("error", "too many people to add tasks for (max 100)");
            else if (ret == 0)
                $.growlUI("error", "no results");
            else
                window.location = "/Task";
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
    var $di3 = {
        overlay: { background: "#000", opacity: 0.3 },
        bgiframe: true,
        modal: true,
        autoOpen: false,
        closeOnEscape: true
    };
    $("#ExportStartEnd").dialog($di3);
    $("#SetExtraValues").dialog($di3);
    $("body").on("click", '.ChooseStartEnd', function (ev) {
        ev.preventDefault();
        var d = $("#ExportStartEnd");
        d.dialog("open");
        $("#ExportStartEndRun").unbind("click").click(function (ev2) {
            ev2.preventDefault();
            var url = ev.target.href;
            url = url.appendQuery("start=" + $("#startdt", d).val());
            url = url.appendQuery("end=" + $("#enddt", d).val());
            $("#ExportStartEnd").dialog("close");
            window.open(url);
            return false;
        });
        return false;
    });
    $("body").on("click", 'a.ChooseExtraValues', function (ev) {
        ev.preventDefault();
        hideDropdowns();
        var d = $("#SetExtraValues");
        d.dialog('option', 'title', $(this).text().trim());
        d.dialog("open");
        $("#SetExtraValuesRun").unbind("click").click(function (ev2) {
            ev2.preventDefault();
            var q = { field: $("#field", d).val(), value: $("#value", d).val() };
            $.block();
            $.post(ev.target.href, q, function (ret) {
                $.unblock();
                alert(ret);
                $("#SetExtraValues").dialog("close");
            });
            return false;
        });
        return false;
    });
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
function dimOff() {
    $("#darkLayer").hide();
}
function dimOn() {
    $("#darkLayer").show();
}
if (typeof String.prototype.startsWith != 'function') {
  String.prototype.startsWith = function (str){
    return this.slice(0, str.length) == str;
  };
}
String.prototype.appendQuery = function (q) {
    if (this && this.length > 0)
        if (this.contains("&") || this.contains("?"))
            return this + '&' + q;
        else
            return this + '?' + q;
    return q;
};
String.prototype.contains = function (it) {
    return this.indexOf(it) != -1;
};
String.prototype.endsWith = function (t, i) {
    return (t == this.substring(this.length - t.length));
};
String.prototype.addCommas = function () {
    var x = this.split('.');
    var x1 = x[0];
    var x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
};
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


