$(function () {

    $("#person-tabs").tabdrop();

    $.fn.editableform.buttons = '<button type="submit" class="btn btn-primary btn-sm editable-submit">' +
                                    '<i class="fa fa-fw fa-check"></i>' +
                                '</button>' +
                                '<button type="button" class="btn btn-default btn-sm editable-cancel">' +
                                    '<i class="fa fa-fw fa-times"></i>' +
                                '</button>';

    $('#family-members-section').on('hide.bs.collapse', function () {
        toggleIcons($('#family-members-collapse i'), false);
    });

    $('#family-members-section').on('show.bs.collapse', function () {
        toggleIcons($('#family-members-collapse i'), true);
    });

    $('#related-family-section').on('hide.bs.collapse', function () {
        toggleIcons($('#related-family-collapse i'), false);
    });

    $('#related-family-section').on('show.bs.collapse', function () {
        toggleIcons($('#related-family-collapse i'), true);
    });

    $('#family-picture-section').on('hide.bs.collapse', function () {
        toggleIcons($('#family-picture-collapse i'), false);
    });

    $('#family-picture-section').on('show.bs.collapse', function () {
        toggleIcons($('#family-picture-collapse i'), true);
    });

    function toggleIcons(ele, expand) {
        if (expand) {
            $(ele).removeClass("fa-chevron-circle-right").addClass('fa-chevron-circle-down');
        } else {
            $(ele).removeClass("fa-chevron-circle-down").addClass('fa-chevron-circle-right');
        }
    }

    var xs = $('.device-xs').is(':visible');
    if (xs) {
        $('#family-members-section').collapse('hide');
        $('#related-family-section').collapse('hide');
        $('#family-picture-section').collapse('hide');
    } else {
        $('#family-members-section').collapse('show');
        $('#related-family-section').collapse('show');
        $('#family-picture-section').collapse('show');
    }

    $('body').on('click', '#split', function (ev) {
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

    $('body').on('click', '#deletePerson', function (ev) {
        ev.preventDefault();
        var href = $(this).attr("href");

        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete it!",
            closeOnConfirm: false
        },
        function () {
            $.post(href, {}, function (ret) {
                window.location = ret;
            });
        });
    });

    $('body').on('click', 'a.editaddr', function (ev) {
        ev.preventDefault();
        $("<form class='modal-form validate ajax' />").load($(this).attr("href"), {}, function () {
            var f = $(this);
            $('#empty-dialog').html(f);
            $('#empty-dialog').modal("show");
            
            f.on("click", "a.clear-address", function () {
                $("#AddressLineOne").val("");
                $("#AddressLineTwo").val("");
                $("#CityName").val("");
                $("#ZipCode").val("");
                $("#BadAddress").prop('checked', false);;
                $("#StateCode_Value").val("");
                $("#StateCode_Value").trigger("chosen:updated");
                $("#ResCode_Value").val("0");
                $("#ResCode_Value").trigger("chosen:updated");
                $("#Country_Value").val("United States");
                $("#FromDt").val("");
                $("#ToDt").val("");
            });
            f.on("click", "a.close-saved-address", function () {
                $.post($(this).attr("href"), {}, function (ret) {
                    $("#profile-header").html(ret).ready(SetProfileEditable);
                });
            });

            $('#empty-dialog').on('hidden', function () {
                $('#empty-dialog').remove();
            });
        });
    });

    $('body').on('click', 'a.personal-picture, a.family-picture', function (ev) {
        ev.preventDefault();
        $("<div />")
            .load($(this).attr("href"), {}, function () {
                var div = $(this);
                var dialog = div.find("div.modal-dialog");
                var f = div.find("form");

                $('#empty-dialog').html(dialog);
                $('#empty-dialog').modal("show");
                dialog.on('hidden', function () {
                    div.remove();
                    dialog.remove();
                });

                $("#delete-picture").click(function (ev) {
                    ev.preventDefault();
                    var a = this;

                    swal({
                        title: "Are you sure?",
                        type: "warning",
                        showCancelButton: true,
                        confirmButtonClass: "btn-danger",
                        confirmButtonText: "Yes, delete it!",
                        closeOnConfirm: false
                    },
                    function () {
                        f.attr("action", a.href);
                        f.submit();
                    });

                    return false;
                });

                $("#refresh-thumbnail").click(function (ev) {
                    ev.preventDefault();
                    var a = this;
                    f.attr("action", a.href);
                    f.submit();
                    return false;
                });
            });
    });

    $('body').on('click', '#family_related a.edit', function (ev) {
        ev.preventDefault();
        $("<div />").load($(this).attr("href"), {}, function () {
            var div = $(this);
            var dialog = div.find('div.modal-dialog');
            $('#empty-dialog').html(dialog);
            $('#empty-dialog').modal("show");

            $('#empty-dialog').on('shown', function () {
                dialog.find("textarea").focus();
            });
            $('#empty-dialog').on('hidden', function () {
                $(this).remove();
            });
            $('#empty-dialog').on("click", "a.save", function (e) {
                e.preventDefault();
                $.post($(this).attr("href"), { value: dialog.find("textarea").val() }, function (ret) {
                    $("#related-families-div").html(ret);
                    $('#empty-dialog').modal("hide");
                });
            });
            $('#empty-dialog').on("click", "a.delete", function (e) {
                e.preventDefault();
                var a = $(this);

                swal({
                    title: "Are you sure?",
                    text: "This will remove the family relationship.",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonClass: "btn-danger",
                    confirmButtonText: "Yes, delete it!",
                    closeOnConfirm: true
                },
               function () {
                   $('#empty-dialog').modal("hide");
                   $.block();
                   $.post(a.attr("href"), {}, function (ret) {
                       $.unblock();
                       $("#related-families-div").html(ret);
                   });
               });
               return false;
            });
        });
    });

    $('body').on('click', 'a.membertype', function (ev) {
        ev.preventDefault();
        var $a = $(this);
        $("<div />").load(this.href, {}, function () {
            var d = $(this);
            var f = d.find("form");
            f.modal("show");
            f.on('hidden', function () {
                d.remove();
                f.remove();
                $.RebindMemberGrids();
            });
        });
    });

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
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

    $("a[href='#enrollment']").on('shown.bs.tab', function (e) {
        if ($("#current").length < 2) {
            $("a[href='#current']").click().tab("show");
        }
    });

    $("a[href='#profile']").on('shown.bs.tab', function (e) {
        var id = "#memberstatus";
        if ($(id).length < 2) {
            $("a[href='" + id + "']").click().tab("show");
            $.cookie('lasttab', id);
        }
    });

    $("a[href='#ministry']").on('shown.bs.tab', function (e) {
        var id = "#contactsreceived";
        if ($(id).length < 2) {
            $("a[href='" + id + "']").click().tab("show");
            $.cookie('lasttab', id);
        }
    });

    $("a[href='#giving']").on('shown.bs.tab', function (e) {
        var id = "#contributions";
        if ($(id).length < 2) {
            $("a[href='" + id + "']").click().tab("show");
            $.cookie('lasttab', id);
        }
    });

    $("a[href='#emails']").on('shown.bs.tab', function (e) {
        var id = "#receivedemails";
        if ($(id).length < 2) {
            $("a[href='" + id + "']").click().tab("show");
            $.cookie('lasttab', id);
        }
    });

    $("a[href='#system']").on('shown.bs.tab', function (e) {
        var id = "#user";
        if ($(id).length < 2) {
            $("a[href='" + id + "']").click().tab("show");
            $.cookie('lasttab', id);
        }
    });

    $('body').on('click', '#future', function (ev) {
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
                swal("Error!", ret.error, "error");
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
            source: [
                {
                    value: 10,
                    text: "Primary Adult"
                }, {
                    value: 20,
                    text: "Secondary Adult"
                }, {
                    value: 30,
                    text: "Child"
                }
            ],
            type: "select",
            url: "/Person2/PostData",
            name: "position"
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
        $("a.editable-bit").editable({ type: 'checklist', mode: 'inline', source: { 'True': 'True' }, emptytext: 'False' });
    };

    $('body').on('click', 'a.click-pencil', function (e) {
        e.stopPropagation();
        $(this).prev().editable('toggle');
        return false;
    });

    $('body').on('click', 'a.visibilityroles', function (e) {
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

    $('body').on('click', '#failedemails a.unblock', function (ev) {
        var address = $(this).attr("email");
        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-warning",
            confirmButtonText: "Yes, unblock it!",
            closeOnConfirm: false
        },
        function () {
            $.post("/Manage/Emails/Unblock", { email: address }, function (ret) {
                swal({ title: "Email unblocked!", type: "success" });
            });
        });
    });

    $('body').on('click', '#failedemails a.unspam', function (ev) {
        var address = $(this).attr("email");
        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-warning",
            confirmButtonText: "Yes, unspam it!",
            closeOnConfirm: false
        },
        function () {
            $.post("/Manage/Emails/Unspam", { email: address }, function (ret) {
                swal({ title: "Email unspamed!", type: "success" });
            });
        });
    });

    $.RebindMemberGrids = function() {
        $("a.refresh-current").click();
        $("a.refresh-pending").click();
    };
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
            window.location = "/Merge/{0}/{1}".format(ret.pid, ret.pid2);
            break;
    }
}

function RebindUserInfoGrid() {
    $.updateTable($('#user-tab form'));
    $("#memberDialog").dialog('close');
}

function dialogError(arg) {
    return arg;
}