///#source 1 1 /Scripts/js/dropdown.js
$(function() {
    $.fn.dropdown = function() {
        return this.each(function() {
            $(this).click(function(ev) {
                ev.preventDefault();
                var trigger = $(this),
                    dd = $($(this).next('.dropdown-menu')),
                    isOpen = trigger.hasClass('dropdown-open');
                $("div.dropdown-menu").hide();
                $(".trigger-dropdown").removeClass("dropdown-open");
                if (isOpen)
                    return false;
                dd.show();
                dd.css({
                    left: dd.hasClass('anchor-right') ?
                        trigger.position().left - (dd.outerWidth() - trigger.outerWidth())
                        : trigger.position().left 
                });
                trigger.addClass('dropdown-open');
                return false;
            });
        });
    };
   $.fn.dropdownFocus = function() {
        return this.each(function() {
            $(this).focus(function(ev) {
                ev.preventDefault();
                var trigger = $(this),
                    dd = $($(this).next('.dropdown-menu')),
                    isOpen = trigger.hasClass('dropdown-open');
                $("div.dropdown-menu").hide();
                $(".trigger-dropdown").removeClass("dropdown-open");
                if (isOpen)
                    return false;
                dd.show();
                dd.css({
                    left: dd.hasClass('anchor-right') ?
                        trigger.position().left - (dd.outerWidth() - trigger.outerWidth())
                        : trigger.position().left 
                });
                trigger.addClass('dropdown-open');
                return false;
            });
            $(this).click(function(ev) {
                var trigger = $(this), isOpen = trigger.hasClass('dropdown-open');
                if (isOpen)
                    return false;
                return true;
            });
        });
    };
	$(function () {
		$('HTML').on('click.dropdown', hideDropdowns);
		$(window).on('resize.dropdown', hideDropdowns);
	    $(document).keydown(function(e) {
	        if (e.keyCode == 17 && $("a.trigger-dropdown").hasClass("dropdown-open")) {
	            $("li.hideAlt").toggle();
	        }
	    });
        $("li.hideAlt").hide();
	});
});
    function hideDropdowns(event) {
		var targetGroup = event ? $(event.target).parents().andSelf() : null;
		if (targetGroup && targetGroup.is('.dropdown-menu') && !targetGroup.is('A'))
		    return;
		$('div.dropdown-menu').hide();
		$('.trigger-dropdown').removeClass('dropdown-open');
        $("li.hideAlt").hide();
	};
///#source 1 1 /Scripts/js/Pager.js
$(document).ready(function () {
    $.gotoPage = function (e, pg) {
        var f = $(e).closest('form');
        $("#Page", f).val(pg);
        return $.getTable(f);
    };
    $.setPageSize = function (e) {
        var f = $(e).closest('form');
        $('#Page', f).val(1);
        $("#PageSize", f).val($(e).val());
        return $.getTable(f);
    };
    $.getTable = function (f) {
        var q;
        if (f)
            q = f.serialize();
        $.post(f.attr('action'), q, function (ret) {
            $(f).html(ret).ready(function () {
                $('table.grid > tbody > tr:even', f).addClass('alt');
                $("a.trigger-dropdown", f).dropdown();
                $('.bt').button();
                $(".datepicker").datepicker();
            });
        });
        return false;
    };
    $('table.grid > thead a.sortable').live("click", function () {
        var f = $(this).closest("form");
        var newsort = $(this).text();
        var sort = $("#Sort", f);
        var dir = $("#Direction", f);
        if ($(sort).val() == newsort && $(dir).val() == 'asc')
            $(dir).val('desc');
        else
            $(dir).val('asc');
        $(sort).val(newsort);
        $.getTable(f);
        return false;
    });
    $.showTable = function (f) {
        if ($('table.grid', f).size() == 0)
            $.getTable(f);
        return false;
    };
    $.updateTable = function (f) {
        if ($('table.grid', f).size() > 0)
            $.getTable(f);
        return false;
    };
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

///#source 1 1 /Scripts/js/ExportToolBar.js
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
        $('div.dropdown-menu').hide();
        var d = $("#ChooseFormat");
        d.dialog("open");

        var sep = ev.target.href.search(/\?/) == -1 ? "?" : "&";
        $("#cmdOK").unbind("click").click(function (ev2) {
            ev2.preventDefault();
            var url = ev.target.href
                + sep + "titles=" + $('#UseTitle')[0].checked
                + "&format=" + $('input[name=format]:checked').val()
                + "&usephone=" + $('#UsePhone')[0].checked
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
        $('div.dropdown-menu').hide();
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
        $('div.dropdown-menu').hide();
        $.block();
        $.post(this.href, null, function (ret) {
            $(".taguntag:visible").text(ret);
            $.unblock();
        });
        return false;
    });
    $('body').on("click", '#AddContact', function (ev) {
        $('div.dropdown-menu').hide();
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
        $('div.dropdown-menu').hide();
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



///#source 1 1 /Scripts/js/headermenu.js
$(document).ready(function () {
    $("a.trigger-dropdown").dropdown();

    $('#SearchText').each(function () {
        var searchterm = "";
        $(this).autocomplete({
            appendTo: "#SearchResults",
            position: { my: "right top", at: "right bottom", of: $("#SearchText") },
            minLength: 3,
            autoFocus: true,
            open: function () {
                $("#SearchResults > ul").css("z-index", 1002);
            },
            close: function (event) {
                var thisval = $(this).val();
                if (searchterm !== thisval && thisval !== "") {
                    return $("#SearchText").autocomplete("search");
                }
                $("#SearchText").val('');
            },
            source: function (request, response) {
                if (request.term === '---')
                    response([
                        { id: -1, line1: "People Search" },
                        { id: -2, line1: "Search Builder" },
                        { id: -3, line1: "Organization Search" }
                    ]);
                else {
                    searchterm = request.term;
                    $.post("/Home/Names", request, function (ret) {
                        response(ret.slice(0, 15));
                    }, "json");
                }
            },
            select: function (event, ui) {
                var thisval = $(this).val();
                if (searchterm !== thisval && thisval !== "") {
                    return false;
                }
                if (ui.item.id === -1)
                    window.location = "/PeopleSearch?name=" + searchterm;
                else if (ui.item.id === -2)
                    window.location = "/QueryBuilder/Main";
                else if (ui.item.id === -3)
                    window.location = "/OrgSearch";
                else
                    window.location = (ui.item.isOrg ? "/Organization/Index/" : "/Person/Index/") + ui.item.id;
                return true;
            },
            focus: function (ev) {
                ev.preventDefault();
            }
        }).data("uiAutocomplete")._renderItem = function (ul, item) {
            if (item.id === 0)
                return $("<li>").append("<hr/>").appendTo(ul);
            var li = "<a><b>" + (item.isOrg ? "Org: " : "") + item.line1 + "</b>";
            if (item.id > 0)
                li += "<br>" + (item.isOrg ? "Div: " : "") + item.line2;
            li += "</a>";
            return $("<li>")
                .append(li)
                .appendTo(ul);
        };
    });

    $("a.tutorial").click(function (ev) {
        ev.preventDefault();
        startTutorial($(this).attr("href"));
    });
    $('#AddDialog').dialog({
        bgiframe: true,
        autoOpen: false,
        width: 750,
        height: 700,
        modal: true,
        overlay: {
            opacity: 0.5,
            background: "black"
        }, close: function () {
            $('iframe', this).attr("src", "");
        }
    });
    $('#addpeople').click(function (e) {
        e.preventDefault();
        var d = $('#AddDialog');
        $('iframe', d).attr("src", "/SearchAdd?type=addpeople");
        d.dialog("option", "title", "Add People");
        d.dialog("open");
    });
    $('#addorg').click(function (e) {
        e.preventDefault();
        var d = $('#AddDialog');
        $('iframe', d).attr("src", "/AddOrganization");
        d.dialog("option", "title", "Add Organization");
        d.dialog("open");
    });
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
    $('#SearchText').each(function () {
        $(this).addClass('text-label');
        $(this).focus(function () {
            if (this.value === '' || this.value === $(this).attr('default')) {
                this.value = '';
                $(this).removeClass('text-label');
                $(this).autocomplete("search", "---");
            }
        });
        $(this).blur(function () {
            if (this.value === '' && $(this).attr('default')) {
                this.value = $(this).attr('default');
                $(this).addClass('text-label');
            }
        });
    });
});
function CloseAddDialog() {
    $("#AddDialog").dialog("close");
    window.location = "/Person/Current";
}
function CloseAddOrgDialog(id) {
    $("#AddDialog").dialog("close");
    window.location = "/Organization/Index/" + id;
}
///#source 1 1 /Scripts/js/Support.js
var mustSearch = 1;

$(function () {
    $(document).on("click", "#supportfeedback,#emailsupport", null, supportOpen);
    $(document).on("keyup", "#gsc-i-id1", null, searchRequested);
    $(document).on("click", ".gsc-search-button", null, searchDone);
    $(document).on("click", "#showSupportForm", null, supportShowForm);
    $(document).on("click", "#sendSupport", null, supportSend);
    $(document).on("click", "#cancelSupport", null, supportClose);
    $(document).on("click", "[supportHelp]", null, supportHelpOpen);
    $(document).on("click", "[supportCancelHelp]", null, supportHelpClose);

    $("#supportDialog").dialog({ autoOpen: false, resizable: false, width: 800, height: 500, modal: true, dialogClass: "no-title" });
    $("#supportHelp").dialog({ autoOpen: false, resizable: false, width: 500, height: "auto", modal: true, dialogClass: "no-title" });
});

function searchRequested(e) {
    if (e.keyCode == 13)
        searchDone(e);
}

function searchDone(e) {
    mustSearch--;
    if (mustSearch <= 0) {
        $("#showSupportForm").button( "option", "disabled", false );
    }
}

function supportShowForm(e) {
    $("#reqBody").val("");

    $("#supportInstructions").css("display", "none");
    $("#supportForm").css("display", "block");
}

function supportOpen(e) {
    mustSearch = 1;

    $("#showSupportForm").button("option", "disabled", true);
    $("#supportInstructions").css("display", "block");
    $("#supportForm").css("display", "none");

    $("#supportDialog").dialog("open");
}

function supportClose(e) {
    $("#supportDialog").dialog("close");
}

function supportSend(e) {
    if( $("#supportUrgency")[0].selectedIndex == 0 )
    {
        alert( "Please select an urgency before submitting your support request" );
        return;
    }

    $("#supportLastSearch").val($("#gsc-i-id1").val());

    var theURL = "/Home/SendSupportRequest";
    var thePost = $(this).closest("form").serialize();
    $.ajax(theURL, { data: thePost, type: "POST", success: supportSentComplete });
}

function supportSentComplete(data, status, xhr) {
    if (data == "OK") {
        alert("Your request has been submitted!");
        $("#supportDialog").dialog("close");
    }
    else {
        alert("There was an error submitting your support request, please try again.");
    }
}

function supportHelpOpen(e) {
    var contentID = $(this).attr("contentID");
    $("#supportHelp").html($("#" + contentID).html());
    $("#supportHelp").dialog("open");
}

function supportHelpClose(e) {
    $("#supportHelp").dialog("close");
}
