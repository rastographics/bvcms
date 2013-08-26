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
        var state = $(this).attr("href");
        var d = $(state);
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
        return true;
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
///#source 1 1 /Scripts/jquery/jquery.hiddenposition.1.1.js
/*	jQuery HiddenPosition Plugin - easily position any DOM element, even if it's hidden
 *  Examples and documentation at: http://www.garralab.com/hiddenposition.php
 *  Copyright (C) 2012  garralab@gmail.com
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
(function($) {
    /*
     * TODO:
     * consider margin
     */
    var DEBUG = false;
    var version = '1.1';
    $.fn.hiddenPosition = function(options) {
        var opts = $.extend({}, $.fn.hiddenPosition.defaults, options);
        return this.each(function() {
            var $this = $(this);
            var o = $.metadata ? $.extend({}, opts, $this.metadata()) : opts;
            position($this,o);
        });
    };
    function position(element,options) {
        var dims = getHiddenDims(element);
        var ofDims = getHiddenDims($(options.of));
        var my = getDirections(options.my);
        var at = getDirections(options.at);
        var offset = getOffset(options.offset);
        debugObject('my',my);
        debugObject('at',at);
        debugObject('myDims',dims);
        debugObject('targetDims',ofDims);
        
        var coord = getCoordinates(ofDims);
        
        switch (at.h) {
            case 'center':
                coord.left += ofDims.width/2
                break;
            case 'right':
                coord.left += ofDims.width
                break;
            default:
                break;
        }
        switch (at.v) {
            case 'center':
                coord.top += ofDims.height/2
                break;
            case 'bottom':
                coord.top += ofDims.height
                break;
            default:
                break;
        }
        switch (my.h) {
            case 'center':
                coord.left -= dims.width/2
                break;
            case 'right':
                coord.left -= dims.width
                break;
            default:
                break;
        }
        switch (my.v) {
            case 'center':
                coord.top -= dims.height/2
                break;
            case 'bottom':
                coord.top -= dims.height
                break;
            default:
                break;
        }
        coord.top += offset.v;
        coord.left += offset.h;
        coord = checkCollisions(element,coord,dims,my,at,options);
        if (dims.position=='relative') {
            coord = transformToRelative(element,coord,dims)
        }
        if (options.using) {
            options.using(coord,element);
        } else {
            move(element,coord);
        }
    };
    function move(element,coord) {
        element.css('left',coord.left+'px')
        .css('top',coord.top+'px');
    };
    function transformToRelative(element,coord,dims) {
        debugObject("to coordinates",coord);
        debugObject("dims",dims);
        coord.top -= dims.offsetTop-dims.top;
        coord.left -= dims.offsetLeft-dims.left;
        debugObject("transformed coordinates",coord);
        return coord;
    };
    function checkCollisions(element,coord,dims,my,at,options) {
        if (options.collision.match(/flip|fit/g)) {
            debug('CHECK COLLISION!',options.collision);
            var collisions = {h:'none',v:'none'};
            var opts = options.collision.split(' ');
            if (opts.length == 1) {
                collisions.h = opts[0];
                collisions.v = opts[0];
            } else if (opts.length > 1) {
                collisions.h = opts[0];
                collisions.v = opts[1];
            }
            var viewport = null;
            if (options.viewport) {
                viewport = getHiddenDims($(options.viewport));
            } else {
                viewport = {
                    position: 'absolute',
                    top: 0,
                    left: 0,
                    offsetTop: 0,
                    offsetLeft: 0,
                    width: $(window).width(),
                    height: $(window).height(),
                    innerWidth: $(window).width(),
                    innerHeight: $(window).height()
                }
            }
            debugObject('viewport',viewport);
            
            coord = checkCollision(collisions.h,'h',element,coord,dims,viewport,my,at);
            coord = checkCollision(collisions.v,'v',element,coord,dims,viewport,my,at);
        }
        return coord;
    };
    function checkCollision(method,dir,element,coord,dims,viewport,my,at) {
        if (method != 'none') {
            var vcoord = getCoordinates(viewport);
            if (dir == 'h') {
                if (coord.left < vcoord.left) {
                    if (method == 'fit') {
                        coord.left = vcoord.left;
                    } else if (method == 'flip') {
                        if (my.h=='right' && (coord.left+dims.width>=vcoord.left)) {
                            coord.left += dims.width;
                        }
                    } 
                } else if (coord.left + dims.width > vcoord.left + viewport.innerWidth) {
                    if (method == 'fit') {
                        coord.left -= ((coord.left + dims.width) - (vcoord.left + viewport.innerWidth));
                    } else if (method == 'flip') {
                        if (my.h=='left' && (coord.left <= vcoord.left+viewport.innerWidth)) {
                            coord.left -= dims.width;
                        }
                    }
                }
            } else if (dir == 'v') {
                if (coord.top < vcoord.top) {
                    if (method == 'fit') {
                        coord.top = vcoord.top;
                    } else if (method == 'flip') {
                        if (my.v=='bottom' && (coord.top+dims.height>=vcoord.top)) {
                            coord.top += dims.height;
                        }
                    } 
                } else if (coord.top + dims.height > vcoord.top + viewport.innerHeight) {
                    if (method == 'fit') {
                        coord.top -= ((coord.top + dims.height) - (vcoord.top + viewport.innerHeight));
                    } else if (method == 'flip') {
                        if (my.v=='top' && (coord.top <= vcoord.top+viewport.innerHeight)) {
                            coord.top -= dims.height;
                        }
                    }
                }
            }
        }
        return coord;
    };
    function getCoordinates(dims) {
        var coord = {top:dims.offsetTop,left:dims.offsetLeft};
        if (dims.position=='absolute' || dims.position=='fixed') {
            if (dims.top) coord.top = dims.top;
            if (dims.left) coord.left = dims.left;
        }
        return coord;
    };
    function getOffset(option) {
        var off = {
            h:0,
            v:0
        };
        if (option) {
            var opts = option.split(' ');
            if (opts.length > 0) {
                off.h = Number(opts[0]);
            }
            if (opts.length > 1) {
                off.v = Number(opts[1]);
            }
        }
        return off;
    };
    function getDirections(option) {
        var dir = {
            h:'center',
            v:'center'
        };
        if (option) {
            var opts = option.split(' ');
            if (opts.length > 0) {
                dir = getDirection(opts[0],dir);
            }
            if (opts.length > 1) {
                dir = getDirection(opts[1],dir);
            }
        }
        return dir;
    };
    function getDirection(str,d) {
        switch (str) {
            case 'top':
                d.v = 'top';
                break;
            case 'bottom':
                d.v = 'bottom';
                break;
            case 'left':
                d.h = 'left';
                break;
            case 'right':
                d.h = 'right';
                break;
            default:
                break;
        }
        return d;
    };
    function getDims(elem) {
        var offset = $(elem).offset();
        return {
            position: $(elem).css('position'),
            top: Number($(elem).css('top').replace(/[^\d\.-]/g,'')),
            left: Number($(elem).css('left').replace(/[^\d\.-]/g,'')),
            offsetTop: offset.top,
            offsetLeft: offset.left,
            width: $(elem).outerWidth(),
            height: $(elem).outerHeight(),
            innerWidth: $(elem).innerWidth(),
            innerHeight: $(elem).innerHeight()
        };
    };
    function getHiddenDims(elems) {
        var dims = null, i = 0, offset, elem;

        while ((elem = elems[i++])) {
            var hiddenElems = $(elem).parents().andSelf().filter(':hidden');
            if ( ! hiddenElems.length ) {
                dims = getDims(elem);
            } else {
                debug('hidden');
                var backupStyle = [];
                hiddenElems.each( function() {
                    var style = $(this).attr('style');
                    style = typeof style == 'undefined'? '': style;
                    debug('style',style);
                    backupStyle.push( style );
                    $(this).attr( 'style', style + ' ; display: block !important;' );
                    debug('style',$(this).attr( 'style' ));
                });
                var left = hiddenElems.eq(0).css( 'left' );
                debug('left',left);
                hiddenElems.eq(0).css( 'left', -10000 );
                dims = getDims(elem);
                hiddenElems.eq(0).css( 'top', -10000 ).css('left',left);
                dims.offsetLeft = getDims(elem).offsetLeft;
                dims.left = getDims(elem).left;
                
                hiddenElems.each( function() {
                    $(this).attr( 'style', backupStyle.shift() );
                });
            }
            
        }

        return dims;
    };
    $.fn.hiddenPosition.defaults = {
        my: "center",
        at: "center",
        of: null,
        offset: null,
        using: null,
        collision: "flip",
        viewport: null
    };
    $.fn.hiddenPosition.getHiddenDimensions = function(element) {
        return getHiddenDims(element);
    };
    $.fn.hiddenPosition.toggleDebug = function() {
        DEBUG = !DEBUG;
    };
    function debug(log, jQueryobj) {
        try {
            if (DEBUG && window.console && window.console.log)
                window.console.log(log + ': ' + jQueryobj);
        } catch(ex) {}
    };
    function debugObject(log, jQueryobj) {
        try {
            if (!jQueryobj) jQueryobj=log;
            debug(log, jQueryobj);
            if ( DEBUG && window.console && window.console.log && ($.browser.msie || $.browser.opera) ) {
                window.console.log($.param(jQueryobj));
            } else if (DEBUG && window.console && window.console.debug) {
                window.console.debug(jQueryobj);
            }
        } catch(ex) {}
    };
})(jQuery);
