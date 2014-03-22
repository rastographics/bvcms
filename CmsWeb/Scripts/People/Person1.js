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
    $("a.editaddr").live("click", function (ev) {
        ev.preventDefault();
        $("#edit-address").css({ "margin-top": "", "top": "" })
            .load($(this).attr("href"), {}, function () {
                var modal = $(this);
                modal.modal("show");
                modal.on('hidden', function () {
                    $(this).empty();
                });
                modal.on("click", "a.clear-address", function () {
                    $("#AddressLineOne").val("");
                    $("#AddressLineTwo").val("");
                    $("#CityName").val("");
                    $("#ZipCode").val("");
                    $("#BadAddress").prop('checked', false); ;
                    $("#StateCode_Value").val("");
                    $("#StateCode_Value").trigger("chosen:updated");
                    $("#ResCode_Value").val("0");
                    $("#ResCode_Value").trigger("chosen:updated");
                    $("#Country_Value").val("United States");
                    $("#FromDt").val("");
                    $("#ToDt").val("");
                });
                modal.on("click", "a.close-saved-address", function () {
                    $.post($(this).attr("href"), {}, function (ret) {
                        $("#profile-header").html(ret).ready(SetProfileEditable);
                    });
                });
            });
    });
    $("a.personal-picture, a.family-picture").live("click", function (ev) {
        ev.preventDefault();
        $("<div />")
            .load($(this).attr("href"), {}, function () {
                var d = $(this);
                var f = d.find("form");
                f.modal("show");
                f.on('hidden', function () {
                    d.remove();
                    f.remove();
                });
                $("#delete-picture").click(function (ev) {
                    ev.preventDefault();
                    var a = this;
                    bootbox.confirm("Are you sure you want to delete this picture?", function (result) {
                        if (result === true) {
                            f.attr("action", a.href);
                            f.submit();
                        }
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
        e.preventDefault();
        var tab = $(e.target).attr('href').replace("#", "#tab-");;
        window.location.hash = tab;
        $.cookie('lasttab', tab);
        return false;
    });
    var lastTab = $.cookie('lasttab');
    if (window.location.hash) {
        lastTab = window.location.hash;
    }
    if (lastTab) {
        var tlink = $("a[href='" + lastTab.replace("tab-", "") + "']");
        var tabparent = tlink.closest("ul").data("tabparent");
        if (tabparent) {
            $("a[href='#" + tabparent + "']").click().tab("show");
        }
        if (tlink.attr("href") !== '#') {
            $.cookie('lasttab', tlink.attr("href"));
            tlink.click().tab("show");
        }
    }
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
            }
        });
    });
    $('#vtab>ul>li').click(function () {
        $('#vtab>ul>li').removeClass('selected');
        $(this).addClass('selected');
        var index = $('#vtab>ul>li').index($(this));
        $('#vtab>div').hide().eq(index).show();
    });
    var SetProfileEditable = function () {
        $(".popover-map").dropdown();
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
    $.InitFunctions.Editable = function () {
        $("a.editable").editable();
    };
    $('a.click-pencil').live("click", function (e) {
        e.stopPropagation();
        $(this).prev().editable('toggle');
        return false;
    });
    $('a.visibilityroles').live("click", function (e) {
        e.preventDefault();
        $(this).editable('toggle');
        return false;
    });
    $.InitFunctions.MemberDocsEditable = function () {
        $("#memberdocs-form a.editable").editable({
            placement: "right",
            showbuttons: "bottom"
        });
    };
    $("#failedemails a.unblock").live("click", function (ev) {
        if (confirm("are you sure?"))
            $.post("/Manage/Emails/Unblock", { email: $(this).attr("email") }, function (ret) {
                $.growlUI("email unblocked", ret);
            });
    });
    $("#failedemails a.unspam").live("click", function (ev) {
        if (confirm("are you sure?"))
            $.post("/Manage/Emails/Unspam", { email: $(this).attr("email") }, function (ret) {
                $.growlUI("email unspamed", ret);
            });
    });
});

function AddSelected(ret) {
    switch (ret.from) {
        case 'RelatedFamily':
            $("#related-families-div").loadWith('/Person2/RelatedFamilies/' + ret.pid, function () {
                $(ret.key).click();
            });
            break;
        case 'Family':
            $("#family-div").loadWith('/Person2/FamilyMembers/' + ret.pid);
            break;
        case 'MergeTo':
            window.location = "/Manage/Merge?PeopleId1=" + ret.pid + "&PeopleId2=" + ret.pid2;
            break;
    }
}

function RebindMemberGrids() {
    $("#refresh-current").click();
    $("#refresh-pending").click();
}
function RebindUserInfoGrid() {
    $.updateTable($('#user-tab form'));
    $("#memberDialog").dialog('close');
}
function dialogError(arg) {
    return arg;
}