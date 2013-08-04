///#source 1 1 /Scripts/js/Pager3.js
$(function () {
    //$.getTable = function (d) {
    //    var q = {};
    //    if (d.hasClass("loaded"))
    //        q = d.find("form").serialize();
    //    $.ajax({
    //        type: 'POST',
    //        url: d.data("action"),
    //        data: q,
    //        success: function (data, status) {
    //            d.html(data);
    //            d.addClass("loaded");
    //        }
    //    });
    //    return false;
    //};
    //$('table.grid > thead a.sortable').live("click", function () {
    //    var d = $(this).closest("div.loaded");
    //    var newsort = $(this).text();
    //    var sort = $("#Sort", d);
    //    var dir = $("#Direction", d);
    //    if ($(sort).val() == newsort && $(dir).val() == 'asc')
    //        $(dir).val('desc');
    //    else
    //        $(dir).val('asc');
    //    $(sort).val(newsort);
    //    $.getTable(d);
    //    return false;
    //});
    //$.showTable = function (d) {
    //    if (!d.hasClass("loaded"))
    //        $.getTable(d);
    //    return false;
    //};
    //$.updateTable = function (d) {
    //    if (!d.hasClass("loaded"))
    //        $.getTable(f);
    //    return false;
    //};
    $("body").on("click", "input[name='toggletarget']", function (ev) {
        if ($('a.target[target="people"]').length == 0) {
            $("a.target").attr("target", "people");
            $("input[name='toggletarget']").attr("checked", true);
        } else {
            $("a.target").removeAttr("target");
            $("input[name='toggletarget']").removeAttr("checked");
        }
    });
});

///#source 1 1 /Scripts/js/headermenu2.js
$(function () {
    $('#SearchText').each(function () {
        var imap;
        var typeahead = $(this).typeahead({
            minLength: 3,
            items: 15,
            highlighter: function (item) {
                var o = imap[item];
                var content = "<a><b>" + (o.isOrg ? "Org: " : "") + o.line1 + "</b>";
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
                    window.location = "/QueryBuilder/Main";
                else if (i.id === -3)
                    window.location = "/OrgSearch";
                else
                    window.location = (i.isOrg ? "/Organization/Index/" : "/Person/Index/") + i.id;
                return "";
            },
            source: function (query, process) {
                if (query === '---') {
                    data = [
                        { order: "001", id: -1, line1: "People Search" },
                        { order: "002", id: -2, line1: "Advanced Search" },
                        { order: "003", id: -3, line1: "Organization Search" }
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
            if (this.value === '' || this.value === $(this).attr('default')) {
                this.value = '';
                ta.source('---', $.proxy(ta.process, ta));
            }
        });
        $(this).blur(function () {
            if (this.value === '' && $(this).attr('default')) {
                this.value = $(this).attr('default');
            }
        });
    });
    $("#AdminMenuToggle").click(function(ev) {
        ev.preventDefault();
        ev.stopPropagation();
        var menu = $("#AdminMenu");
        if (menu.is(":visible"))
            menu.hide();
        else
            menu.show();
    });
    $("body").on("click", function(ev) {
        if($(ev.target).not("#AdminMenuToggle"))
            $("#AdminMenu").hide();
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

///#source 1 1 /Scripts/js/form-ajax.js
$(function () {
    $.AttachFormElements = function () {
        $("form.ajax input.ajax-typeahead").typeahead({
            minLength: 3,
            source: function (query, process) {
                return $.ajax({
                    url: $(this.$element[0]).data("link"),
                    type: 'post',
                    data: { query: query },
                    dataType: 'json',
                    success: function (jsonResult) {
                        return typeof jsonResult == 'undefined' ? false : process(jsonResult);
                    }
                });
            }
        });
        $("form.ajax .date").datepicker();
        $("form.ajax select").chosen();
    };
    $("div.modal form.ajax").live("submit", function (event) {
        event.preventDefault();
        var $form = $(this);
        var $target = $form.closest("div.modal");
        $.ajax({
            type: 'POST',
            url: $form.attr('action'),
            data: $form.serialize(),
            success: function (data, status) {
                //$target.removeClass("fade");
                $target.html(data).ready(function () {
                    var top = ($(window).height() - $target.height()) / 2;
                    if (top < 10)
                        top = 10;
                    $target.css({ 'margin-top': top, 'top': '0' });
                    $.AttachFormElements();
                });
            }
        });
        return false;
    });
    $("ul.nav-tabs a.ajax").live("click", function(event) {
        var d = $($(this).attr("href"));
        if(!d.hasClass("loaded"))
            $.ajax({
                type: 'POST',
                url: d.data("link"),
                data: {},
                success: function (data, status) {
                    d.html(data);
                    d.addClass("loaded");
                }
            });
        return false;
    });
    $("form.ajax a.ajax").live("click", function (event) {
        event.preventDefault();
        var $this = $(this);
        var $form = $this.closest("form.ajax");
        var $modal = $form.closest("div.modal");
        var url = $this.data("link");
        if (typeof url === 'undefined')
            url = $this[0].href;
        var data = $form.serialize();
        $.ajax({
            type: 'POST',
            url: url,
            data: data,
            success: function (data, status) {
                if ($modal.length > 0) {
                    //$modal.removeClass("fade");
                    $modal.html(data).ready(function () {
                        var top = ($(window).height() - $modal.height()) / 2;
                        if (top < 10)
                            top = 10;
                        $modal.css({ 'margin-top': top, 'top': '0' });
                        $.AttachFormElements();
                    });
                } else {
                    $form.html(data).ready(function () {
                        $.AttachFormElements();
                    });
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
        return false;
    });
    $.ajaxSetup({
        beforeSend: function () {
            $("#loading-indicator").css({
                'position': 'absolute',
                'left': $(window).width() / 2,
                'top': $(window).height() / 2,
                'z-index': 2000
            }).show();
        },
        complete: function () {
            $("#loading-indicator").hide();
        }
    });
});

///#source 1 1 /Scripts/Search/SearchAdd.js
$(function () {
    $("a.searchadd").live("click", function (ev) {
        ev.preventDefault();
        $("<div id='search-add' class='modal fade hide' data-width='600' data-keyboard='false' data-backdrop='static' />")
            .load($(this).attr("href"), {}, function () {
                $(this).modal("show");
                $(this).on('hidden', function () {
                    $(this).remove();
                });
            });
    });
    $("#search-add a.clear").live('click', function (ev) {
        ev.preventDefault();
        $("#name").val('');
        $("#phone").val('');
        $("#address").val('');
        $("#dob").val('');
        return false;
    });

    $("form.ajax tbody > tr a.reveal").live("click", function (e) {
        e.stopPropagation();
    });
    $.NotReveal = function (ev) {
        if ($(ev.target).is("a"))
            if (!$(ev.target).is('.reveal'))
                return true;
        return false;
    };
    $("form.ajax tr.section").live("click", function (ev) {
        if ($.NotReveal(ev)) return;
        ev.preventDefault();
        $ToggleShown($(this));
    });
    $('form.ajax a[rel="reveal"]').live("click", function (ev) {
        ev.preventDefault();
        $ToggleShown($(this).parents("tr"));
    });
    var $ToggleShown = function(tr) {
        if (tr.hasClass("notshown"))
            $ShowAll(tr);
        else if (tr.hasClass("shown"))
            $CollapseAll(tr);
        else 
            tr.next("tr").find("div.collapse")
                .off('hidden')
                .on("hidden", function (e) { e.stopPropagation(); })
                .collapse("toggle");
    };
    var $ShowAll = function (tr) {
        tr.nextUntil("tr.section").find("div.collapse")
            .off('hidden')
            .on("hidden", function (e) { e.stopPropagation(); })
            .collapse("show");
        tr.removeClass("notshown").addClass("shown");
        tr.find("i").removeClass("icon-caret-right").addClass("icon-caret-down");
    };
    var $CollapseAll = function (tr) {
        tr.nextUntil("tr.section").find("div.collapse")
            .off("hidden")
            .on("hidden", function (e) { e.stopPropagation(); })
            .collapse('hide');
        tr.removeClass("shown").addClass("notshown");
        tr.find("i").removeClass("icon-caret-down").addClass("icon-caret-right");
    };
    $("form.ajax tr.master").live("click", function (ev) {
        if ($.NotReveal(ev)) return;
        ev.preventDefault();
        $(this).next("tr").find("div.collapse")
            .off('hidden')
            .on("hidden", function (e) { e.stopPropagation(); })
            .collapse("toggle");
    });
    $("form.ajax tr.details").live("click", function (ev) {
        if ($.NotReveal(ev)) return;
        ev.preventDefault();
        ev.stopPropagation();
        $(this).find("div.collapse")
            .off("hidden")
            .on("hidden", function (e) { e.stopPropagation(); })
            .collapse('hide');
    });
});
