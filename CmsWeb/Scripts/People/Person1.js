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
            }
            else
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
        $("<div class='modal fade hide' />").load($(this).attr("href"), {}, function () {
            var modal = $(this);
            modal.modal("show");
            modal.on('hidden', function () {
                $(this).remove();
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

    $('a.deloptout').live("click", function (ev) {
        ev.preventDefault();
        var href = $(this).attr("href");
        if (confirm('Are you sure you want to delete?')) {
            $.post(href, {}, function (ret) {
                if (ret !== "ok")
                    $.growlUI("failed", ret);
                else {
                    $.updateTable($('#user-tab form'));
                    $.growlUI("Success", "OptOut deleted");
                }
            });
        }
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
        $("<div class='modal fade hide' />").load($(this).attr("href"), {}, function () {
            $(this).modal("show");
            $(this).on('hidden', function () {
                $(this).remove();
            });
        });
    });

    $('a[data-toggle="tab"]').on('shown', function (e) {
        $.cookie('lasttab', $(e.target).attr('href'));
    });
    var lastTab = $.cookie('lasttab');
    if (lastTab) {
        var tlink = $("a[href='" + lastTab + "']");
        var tabparent = tlink.closest("ul").data("tabparent");
        if (tabparent) {
            $("a[href='#" + tabparent + "']").click().tab("show");
        }
        tlink.click().tab("show");
    }
    $("a[href='#enrollment']").on('shown', function (e) {
        if ($("#current").length < 2) {
            $("a[href='#current']").click().tab("show");
        }
    });
    $("a[href='#membership']").on('shown', function (e) {
        if ($("#status").length < 2) {
            $("a[href='#status']").click().tab("show");
            $.cookie('lasttab', "#status");
        }
    });
    //$("#contacts-link").click(function () {
    //        $("#contacts-tab").each(function () {
    //            $.showTable($(this));
    //        });
    //    });
    //    $("#member-link").click(function () {
    //        var f = $("#memberdisplay");
    //        if ($("table", f).size() === 0) {
    //            $.post(f.attr('action'), null, function (ret) {
    //                $(f).html(ret).ready(function () {
    //                    $.UpdateForSection(f);
    //                });
    //            });
    //            $.showTable($("#extras-tab form"));
    //            $.extraEditable('#extravalues');
    //        }
    //    });
    //    $("#system-link").click(function () {
    //        $.showTable($("#user-tab"));
    //    });
    //    $("#changes-link").click(function () {
    //        $.showTable($("#changes-tab"));
    //    });
    //    $("#volunteer-link").click(function () {
    //        $.showTable($("#volunteer-tab"));
    //    });
    //    $("#duplicates-link").click(function () {
    //        $.showTable($("#duplicates-tab"));
    //    });
    //    $("#optouts-link").click(function () {
    //        $.showTable($("#optouts-tab"));
    //    });
    //    $("#recreg-link").click(function (ev) {
    //        ev.preventDefault();
    //        var f = $('#recreg-tab');
    //        if ($('table', f).size() > 0)
    //            return false;
    //        var q = f.serialize();
    //        $.post(f.attr('action'), q, function (ret) {
    //            $(f).html(ret);
    //            $(".bt", f).button();
    //        });
    //        return false;
    //    });

    $('#future').live("click", function (ev) {
        ev.preventDefault();
        var d = $(this).closest('div.loaded');
        var q = d.find("form").serialize();
        $.post($("#FutureLink").val(), q, function (ret) {
            d.html(ret);
        });
    });

    $.validator.addMethod("date2", function (value, element, params) {
        var v = $.DateValid(value);
        return this.optional(element) || v;
    }, $.format("Please enter valid date"));

    $.validator.setDefaults({
        highlight: function (input) {
            $(input).addClass("ui-state-highlight");
        },
        unhighlight: function (input) {
            $(input).removeClass("ui-state-highlight");
        },
        rules: {
            "NickName": { maxlength: 15 },
            "Title": { maxlength: 10 },
            "First": { maxlength: 25 },
            "Middle": { maxlength: 15 },
            "Last": { maxlength: 100, required: true },
            "Suffix": { maxlength: 10 },
            "AltName": { maxlength: 100 },
            "Maiden": { maxlength: 20 },
            "HomePhone": { maxlength: 20 },
            "CellPhone": { maxlength: 20 },
            "WorkPhone": { maxlength: 20 },
            "EmailAddress": { maxlength: 150 },
            "School": { maxlength: 60 },
            "Employer": { maxlength: 60 },
            "Occupation": { maxlength: 60 },
            "WeddingDate": { date2: true },
            "DeceasedDate": { date2: true },
            "Grade": { number: true },
            "Address1": { maxlength: 40 },
            "Address2": { maxlength: 40 },
            "City": { maxlength: 30 },
            "Zip": { maxlength: 15 },
            "FromDt": { date2: true },
            "ToDt": { date2: true },
            "DecisionDate": { date2: true },
            "JoinDate": { date2: true },
            "BaptismDate": { date2: true },
            "BaptismSchedDate": { date2: true },
            "DropDate": { date2: true },
            "NewMemberClassDate": { date2: true }
        }
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
            }
            else {
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
        if (confirm("are you sure?"))
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
    $("form").on('click', 'a.reverse', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        $.post("/Person/Reverse", {
            id: $("#PeopleId").val(),
            field: $(this).attr("field"),
            value: $(this).attr("value"),
            pf: $(this).attr("pf")
        }, function (ret) {
            $(f).html(ret);
        });
    });
    $('#vtab>ul>li').click(function () {
        $('#vtab>ul>li').removeClass('selected');
        $(this).addClass('selected');
        var index = $('#vtab>ul>li').index($(this));
        $('#vtab>div').hide().eq(index).show();
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
        $('#FamilyPosition').editable({
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
    $('body').on('click', function (e) {
        $('[rel=popover]').each(function () {
            //the 'is' for buttons that trigger popups
            //the 'has' for icons within a button that triggers a popup
            if (!$(this).is(e.target) && $(this).has(e.target).length === 0 && $('.popover').has(e.target).length === 0) {
                $(this).popover('hide');
            }
        });
    });

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