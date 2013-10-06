(function($) {

$.fn.getCheckboxVal = function() {
    var vals = [];
    var i = 0;
    this.each(function() {
        vals[i++] = $(this).val();
    });
    return vals;
};
$.fieldValue2 = function(el) {
    var n = el.name, t = el.type, tag = el.tagName.toLowerCase();
    if (!n || el.disabled || t == 'reset' || t == 'button' || t == 'submit' || t == 'image' ||
        ((t == 'checkbox' || t == 'radio') && !el.checked) ||
        (tag == 'select' && el.selectedIndex == -1))
        return null;
    else if (tag == 'select') {
        var index = el.selectedIndex;
        if (index < 0) return null;
        for (var i = 0; i < el.options.length; i++) {
            var op = el.options[i];
            if (op.selected) {
                var v = op.value;
                if (!v)
                    v = (op.attributes && op.attributes['value'] && !(op.attributes['value'].specified)) ? op.text : op.value;
                return v;
            }
        }
    }
    else if (tag == 'input' && t == 'checkbox' && !el.value) {
        return el.checked;
    }
    return el.value;
};
$.fn.formToArray2 = function() {
    var a = new Object();
    if (this.length == 0)
        return a;
    var form = this[0];
    var els = form.elements;
    if (!els)
        return a;
    for (var i = 0; i < els.length; i++) {
        var el = els[i];
        var n = el.name;
        if (!n)
            continue;
        var v = $.fieldValue2(el);
        if (v !== null && typeof v != 'undefined' && v != '')
            if (!a[n]) // only add one
            a[n] = v;
    }
    return a;
};
$.fn.formSerialize2 = function() {
    return $.param(this.formToArray2());
};
//$.populateForm = function(qs) {
//    var qa = new Array();
//    var p = qs.split("&");
//    for (i in p) {
//        var a = p[i].split("=");
//        $("input[name='" + a[0] + "'], select[name='" + a[0] + "']").val(a[1]);
//    }
//};
$.QueryString = function(q, item) {
    var r = new Object();
    $.each(q.split('&'), function() {
        var kv = this.split('=');
        r[kv[0]] = kv[1];
    });
    return r[item];
};
$.block = function() {
    $.blockUI({ message: 'working on it...<img src="/Content/images/loading.gif"/>' });
};
$.unblock = function() {
    $.unblockUI({ fadeOut: 150 });
};
$.navigate = function(url, data) {
    url += (url.match(/\?/) ? "&" : "?") + data;
    window.location = url;
};
})(jQuery);

