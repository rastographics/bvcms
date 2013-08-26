$(function () {
    $('#SearchText').each(function () {
        var imap;
        var typeahead = $(this).typeahead({
            minLength: 3,
            items: 15,
            highlighter: function (item) {
                var o = imap[item];
                var content = "<a>" + (o.isOrg ? "Org: " : "") + o.line1;
                if (o.id > 0)
                    content += "<br>" + (o.isOrg ? "Div: " : "") + o.line2;
                content += "</a>";
                return $("<div>").append(content);
            },
            sorter: function (items) {
                return items;
            },
            matcher: function (item) {
                return true;
            },
            updater: function (obj) {
                var i = imap[obj];
                if (i.id === -1)
                    window.location = "/PeopleSearch?name=" + this.query;
                else if (i.id === -2)
                    window.location = "/Query/";
                else if (i.id === -3)
                    window.location = "/SavedQuery2";
                else if (i.id === -4)
                    window.location = "/OrgSearch";
                else
                    window.location = (i.isOrg ? "/Organization/Index/" : "/Person2/") + i.id;
                return "";
            },
            source: function (query, process) {
                if (query === '---') {
                    data = [
                        { order: "001", id: -1, line1: "People Search" },
                        { order: "002", id: -2, line1: "Search Builder" },
                        { order: "003", id: -3, line1: "Saved Searches" },
                        { order: "004", id: -4, line1: "Organization Search" }
                    ];
                    imap = {};
                    var strings = data.map(function (item) {
                        imap[item.order] = item;
                        return item.order;
                    });
                    return process(strings);
                }
                return $.ajax({
                    url: '/Home/Names2',
                    type: 'post',
                    data: { query: query },
                    dataType: 'json',
                    success: function (data) {
                        imap = {};
                        var strings = data.map(function (item) {
                            imap[item.order] = item;
                            return item.order;
                        });
                        return process(strings);
                    }
                });
            }
        });
        var ta = $(this).data("typeahead");
        ta.show = function() {
            var pos = $.extend({}, this.$element.position(), {
                height: this.$element[0].offsetHeight,
                width: this.$element[0].offsetWidth
            });
            this.$menu
                .insertAfter(this.$element)
                .css({
                    width: 300,
                    top: pos.top + pos.height + 11,
                    left: pos.left - 200 + pos.width
                }).show();
            this.shown = true;
            return this;
        };
        ta.render = function (items) {
            var that = this;
            items = $(items).map(function (i, item) {
                var elements = [];
                var o = imap[item];
                i = $(that.options.item).attr('data-value', item);
                if (o.id === 0)
                    elements.push($("<li/>").addClass("divider")[0]);
                else {
                    i.find('a').html(that.highlighter(item));
                    elements.push(i[0]);
                }
                return elements;
            });
            items.first().addClass('active');
            this.$menu.html(items);
            return this;
        };
        ta.next = function (event) {
            var active = this.$menu.find('.active').removeClass('active'), next = active.next();
            if (!next.length)
                next = $(this.$menu.find('li')[0]);
            if (next.hasClass("divider"))
                next = next.next();
            next.addClass('active');
        };
        ta.prev = function (event) {
            var active = this.$menu.find('.active').removeClass('active'), prev = active.prev();
            if (!prev.length)
                prev = this.$menu.find('li').last();
            if (prev.hasClass("divider"))
                prev = prev.prev();
            prev.addClass('active');
        };
        $(this).focus(function () {
            if (this.value === '' || this.value === $(this).attr('placeholder')) {
                this.value = '';
                ta.source('---', $.proxy(ta.process, ta));
            }
        });
        $(this).blur(function () {
            if ($(this).attr('placeholder')) {
                this.value = $(this).attr('placeholder');
            }
        });
    });

    $("a.tutorial").click(function (ev) {
        ev.preventDefault();
        startTutorial($(this).attr("href"));
    });
    //	$('#AddDialog').dialog({
    //		bgiframe: true,
    //		autoOpen: false,
    //		width: 750,
    //		height: 700,
    //		modal: true,
    //		overlay: {
    //			opacity: 0.5,
    //			background: "black"
    //		}, close: function () {
    //			$('iframe', this).attr("src", "");
    //		}
    //	});
    //	$('#addpeople').click(function (e) {
    //		e.preventDefault();
    //		var d = $('#AddDialog');
    //		$('iframe', d).attr("src", "/SearchAdd?type=addpeople");
    //		d.dialog("option", "title", "Add People");
    //		d.dialog("open");
    //	});
    //	$('#addorg').click(function (e) {
    //		e.preventDefault();
    //		var d = $('#AddDialog');
    //		$('iframe', d).attr("src", "/AddOrganization");
    //		d.dialog("option", "title", "Add Organization");
    //		d.dialog("open");
    //	});
    $('#cleartag').click(function (e) {
        e.preventDefault();
        if (confirm("are you sure you want to empty the active tag?"))
            $.post("/Tags/ClearTag", {}, function () {
                window.location.reload();
            });
    });
    $('.warntip').tooltip({
        delay: 150,
        showBody: "|",
        showURL: false
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
    $.navigate = function (url, data) {
        url += (url.match(/\?/) ? "&" : "?") + data;
        window.location = url;
    };
    $.DateValid = function (d, growl) {
        var reDate = /^(0?[1-9]|1[012])[\/-](0?[1-9]|[12][0-9]|3[01])[\/-]((19|20)?[0-9]{2})$/i;
        if ($.dtoptions.format.startsWith('d'))
            reDate = /^(0?[1-9]|[12][0-9]|3[01])[\/-](0?[1-9]|1[012])[\/-]((19|20)?[0-9]{2})$/i;
        var v = true;
        if (!reDate.test(d)) {
            if (growl == true)
                $.growlUI("error", "enter valid date");
            v = false;
        }
        return v;
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
    $.fn.alert = function (message) {
        this.html('<div class="alert"><a class="close" data-dismiss="alert">×</a><span>' + message + '</span></div>');
    };
});

function dimOff() {
    $("#darkLayer").hide();
}
function dimOn() {
    $("#darkLayer").show();
}
String.prototype.startsWith = function (t, i) {
    return (t == this.substring(0, t.length));
};
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
