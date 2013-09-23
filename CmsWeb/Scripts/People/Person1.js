$(function () {
    $("#split").live("click", function (ev) {
        ev.preventDefault();
        var href = $(this).attr("href");
        bootbox.confirm("Are you sure you want to split this person into their own family?", function (result) {
            if (result === true) {
                $.post(href, {}, function (ret) {
                    window.location = ret;
                });
            }
        });
    });
    $("#deletePerson").live("click", function (ev) {
        ev.preventDefault();
        var href = $(this).attr("href");
        bootbox.confirm("Are you sure you want to delete this record?", function (result) {
            if (result === true) {
                $.post(href, {}, function (ret) {
                    window.location = ret;
                });
            }
        });
    });
    $("#search-add a.commit").live("click", function (ev) {
        ev.preventDefault();
        var f = $(this).closest("form");
        var q = f.serialize();
        var loc = $(this).attr("href");
        var modal = f.closest("div.modal");
        $.post(loc, q, function (ret) {
            modal.modal("hide");
            if (ret.message) {
                alert(ret.message);
            } else
                switch (ret.from) {
                    case 'RelatedFamily':
                        $("#related-families-div").load('/Person2/RelatedFamilies/' + ret.pid, {}, function () {
                            $(ret.key).click();
                        });
                        break;
                    case 'Family':
                        $("#family-div").load('/Person2/FamilyGrid/' + ret.pid, {});
                        break;
                    case 'Menu':
                        window.location = '/Person2/' + ret.pid;
                        break;
                }
        });
        return false;
    });
    $("a.editaddr").live("click", function (ev) {
        ev.preventDefault();
        $("#edit-address").css({ "margin-top": "", "top": "" })
            .load($(this).attr("href"), {}, function () {
                var modal = $(this);
                modal.modal("show");
                modal.on('hidden', function () {
                    $(this).empty();
                });
                modal.on("click", "a.close-saved-address", function () {
                    $.post($(this).attr("href"), {}, function (ret) {
                        $("#profile-header").html(ret).ready(SetProfileEditable);
                    });
                });
            });
    });
    $("#profile-actions a.manageUser").live("click", function (ev) {
        ev.preventDefault();
        $("<div class='modal fade hide' data-width='760' />").load($(this).attr("href"), {}, function () {
            var modal = $(this);
            modal.modal("show");
            modal.on('hidden', function () {
                $(this).remove();
            });
            modal.on("click", "a.save", function (e) {
                e.preventDefault();
                var q = modal.find("form").serialize();
                $.post($(this).attr("href"), q, function (ret) {
                    $("#profile-actions").html(ret);
                    modal.modal("hide");
                });
            });
            modal.on("click", "a.delete", function (e) {
                e.preventDefault();
                var a = $(this);
                bootbox.confirm(a.data("prompt"), function (result) {
                    if (result === true)
                        $.post(a.attr("href"), {}, function (ret) {
                            $("#profile-actions").html(ret);
                            modal.modal("hide");
                        });
                });
                return false;
            });
        });
    });
    $("#family_related a.edit").live("click", function (ev) {
        ev.preventDefault();
        $("<div class='modal fade hide' />").load($(this).attr("href"), {}, function () {
            var modal = $(this);
            modal.modal("show");
            modal.on('shown', function () {
                modal.find("textarea").focus();
            });
            modal.on('hidden', function () {
                $(this).remove();
            });
            modal.on("click", "a.save", function (e) {
                e.preventDefault();
                $.post($(this).attr("href"), { value: modal.find("textarea").val() }, function (ret) {
                    $("#related-families-div").html(ret);
                    modal.modal("hide");
                });
            });
            modal.on("click", "a.delete", function (e) {
                e.preventDefault();
                var a = $(this);
                bootbox.confirm("Are you sure you want to remove this relationship?", function (result) {
                    if (result === true)
                        $.post(a.attr("href"), {}, function (ret) {
                            $("#related-families-div").html(ret);
                            modal.modal("hide");
                        });
                });
                return false;
            });
        });
    });
    $('#moveperson').click(function (ev) {
        ev.preventDefault();
        var d = $('#dialogbox');
        $('iframe', d).attr("src", this.href);
        d.dialog("option", "title", "Merge To Person");
        d.dialog("open");
        return false;
    });
    $("form.ajax a.membertype").live("click", function (ev) {
        ev.preventDefault();
        $("#member-dialog").css({ 'margin-top': '', 'top': '' })
            .load($(this).attr("href"), {}, function () {
                $(this).modal("show");
                $(this).on('hidden', function () {
                    $(this).empty();
                });
            });
    });
    $('a[data-toggle="tab"]').on('shown', function (e) {
        $.cookie('lasttab', $(e.target).attr('href'));
    });
    $("a[href='#enrollment']").on('shown', function (e) {
        if ($("#current").length < 2) {
            $("a[href='#current']").click().tab("show");
        }
    });
    $("a[href='#profile']").on('shown', function (e) {
        var id = "#memberstatus";
        if ($(id).length < 2) {
            $("a[href='" + id + "']").click().tab("show");
            $.cookie('lasttab', id);
        }
    });
    $("a[href='#ministry']").on('shown', function (e) {
        var id = "#contactsreceived";
        if ($(id).length < 2) {
            $("a[href='" + id + "']").click().tab("show");
            $.cookie('lasttab', id);
        }
    });
    $("a[href='#giving']").on('shown', function (e) {
        var id = "#contributions";
        if ($(id).length < 2) {
            $("a[href='" + id + "']").click().tab("show");
            $.cookie('lasttab', id);
        }
    });
    $("a[href='#emails']").on('shown', function (e) {
        var id = "#receivedemails";
        if ($(id).length < 2) {
            $("a[href='" + id + "']").click().tab("show");
            $.cookie('lasttab', id);
        }
    });
    $("a[href='#system']").on('shown', function (e) {
        var id = "#user";
        if ($(id).length < 2) {
            $("a[href='" + id + "']").click().tab("show");
            $.cookie('lasttab', id);
        }
    });
    $.validator.addMethod("ValidDate", function (value, element, params) {
        var v = $.DateValid(value);
        return this.optional(element) || v;
    }, "Please enter valid date");
    $('#future').live("click", function (ev) {
        ev.preventDefault();
        var d = $(this).closest('div.loaded');
        var q = d.find("form").serialize();
        $.post($("#FutureLink").val(), q, function (ret) {
            d.html(ret);
        });
    });
    $('#addrf').validate();
    $('#addrp').validate();
    $('#basic').validate();
    $("body").on("change", '.atck', function (ev) {
        var ck = $(this);
        $.post("/Meeting/MarkAttendance/", {
            MeetingId: $(this).attr("mid"),
            PeopleId: $(this).attr("pid"),
            Present: ck.is(':checked')
        }, function (ret) {
            if (ret.error) {
                ck.attr("checked", !ck.is(':checked'));
                alert(ret.error);
            } else {
                var f = ck.closest('form');
                var q = f.serialize();
                $.post($(f).attr("action"), q, function (ret) {
                    $(f).html(ret);
                });
            }
        });
    });
    $("body").on("click", 'a.deleteextra', function (ev) {
        ev.preventDefault();
        bootbox.confirm("Are you sure?", function (result) {
            if (result === true)
                $.post("/Person/DeleteExtra/" + $("#PeopleId").val(), { field: $(this).attr("field") }, function (ret) {
                    if (ret.startsWith("error"))
                        alert(ret);
                    else {
                        $.getTable($("#extras-tab form"));
                        $.extraEditable('#extravalues');
                    }
                });
            return false;
        });
    });
    $("#changes").on('click', 'a.reverse', function (ev) {
        ev.preventDefault();
        var a = $(this);
        bootbox.confirm("Are you sure?", function (result) {
            if (result === true) {
                $.post(a.attr("href"), {
                    field: a.data("field"),
                    value: a.data("value"),
                    pf: a.data("pf")
                }, function (ret) {
                    $("#changes").html(ret);
                });
            }
        });
        return false;
    });
    $("#tasks,#contacts").on("click", 'a.add-task-contact', function (ev) {
        ev.preventDefault();
        var link = $(this).attr("href");
        bootbox.confirm("Are you sure?", function (result) {
            if (result === true) {
                $.post(link, null, function (ret) {
                    window.location = ret;
                });
            }
        });
        return false;
    });
    $("#addoptoutemail").live('click', function (ev) {
        ev.preventDefault();
        $.post($(this).attr("href"), { email: $("#optoutemail").val() }, function (ret) {
            $("#optouts").html(ret);
        });
        return false;
    });
    $('#optouts').on("click", 'a.deloptout', function (ev) {
        ev.preventDefault();
        var href = $(this).attr("href");
        bootbox.confirm('Are you sure you want to delete optout?', function (result) {
            if (result === true)
                $.post(href, {}, function (ret) {
                    if (ret.startsWith("ok"))
                        $.growlUI("failed", ret);
                    else {
                        $("#optouts").html(ret);
                        $.growlUI("Success", "OptOut deleted");
                    }
                });
        });
    });
    //    $("#failedemails").on("click", "a.unblock").click(function (ev) {
    //        if (confirm('Are you sure you want unblock this email?')) {
    //            $.post("/Person2/EmailUnblock", { email: $(this).attr("email") }, function (ret) {
    //                $.growlUI("email unblocked", ret);
    //            });
    //    });
    //    $("#failedemails").on("click", "a.unspam").click(function (ev) {
    //        if (confirm("are you sure?"))
    //            $.post("/Person2/EmailUnspam", { email: $(this).attr("email") }, function (ret) {
    //                $.growlUI("email unspamed", ret);
    //            });
    //    });
    $('#vtab>ul>li').click(function () {
        $('#vtab>ul>li').removeClass('selected');
        $(this).addClass('selected');
        var index = $('#vtab>ul>li').index($(this));
        $('#vtab>div').hide().eq(index).show();
    });
    $('body').on('click', function (e) {
        $('[rel=popover]').each(function () {
            //the 'is' for buttons that trigger popups
            //the 'has' for icons within a button that triggers a popup
            if (!$(this).is(e.target) && $(this).has(e.target).length === 0 && $('.popover').has(e.target).length === 0) {
                $(this).popover('hide');
            }
        });
    });
    var getMap = function (opts) {
        var src = "https://maps.googleapis.com/maps/api/staticmap?",
            params = $.extend({
                center: 'New York, NY',
                size: '128x128',
                sensor: false
            }, opts),
            query = [];

        $.each(params, function (k, v) {
            query.push(k + '=' + encodeURIComponent(v));
        });

        src += query.join('&');
        return '<img src="' + src + '" /><br><a href="https://www.google.com/maps/?q=' + opts.center + '" rel="external" target="_blank">View in Google Maps</a>\
      <br><a href="http://www.bing.com/maps/?q=' + opts.center + '" rel="external" target="_blank">View in Bing Maps</a>';
    };
    var SetProfileEditable = function () {
        $('[class="popover-map"]').each(function () {
            var $this = $(this);
            $this.data('html', true).data('content', getMap({ center: $this.text() }));
            $this.popover();
        });
        $('#PositionInFamily').editable({
            source: [{
                value: 10,
                text: "Primary Adult"
            }, {
                value: 20,
                text: "Secondary Adult"
            }, {
                value: 30,
                text: "Child"
            }],
            type: "select",
            url: "/Person2/PostData",
            name: "position"
            //        success: function (data) {
            //            $("#family-div").load('/Person2/FamilyGrid/' + $("#position").data("pk"), {});
            //        }
        });
        $('#Campus').editable({
            source: "/Person2/Campuses",
            type: "select",
            url: "/Person2/PostData",
            name: "campus"
        });
    };
    SetProfileEditable();
    var lastTab = $.cookie('lasttab');
    if (lastTab) {
        var tlink = $("a[href='" + lastTab + "']");
        var tabparent = tlink.closest("ul").data("tabparent");
        if (tabparent) {
            $("a[href='#" + tabparent + "']").click().tab("show");
        }
        $.cookie('lasttab', tlink.attr("href"));
        tlink.click().tab("show");
    }
});



function RebindMemberGrids() {
    $.updateTable($('#current-tab form'));
    $.updateTable($('#pending-tab form'));
    $("#memberDialog").dialog('close');
}
function RebindUserInfoGrid() {
    $.updateTable($('#user-tab form'));
    $("#memberDialog").dialog('close');
}
function AddSelected(ret) {
    window.location = "/Merge?PeopleId1=" + $("#PeopleId").val() + "&PeopleId2=" + ret.pid;
}
function dialogError(arg) {
    return arg;
}