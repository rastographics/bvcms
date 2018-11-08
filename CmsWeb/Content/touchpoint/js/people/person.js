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

    $('body').on('click', '#promoteToHeadOfHousehold', function (ev) {
        ev.preventDefault();
        var href = $(this).attr("href");
        bootbox.confirm("Are you sure you want to make this person the head of the household?", function (result) {
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

    $('body').on('click', 'a.personal-picture', function (ev) {
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

                $('#edit-thumbnail').Jcrop({
                    applyFilters: ['constrain', 'extent', 'backoff', 'ratio', 'round'],
                    aspectRatio: 1,
                    setSelect: [0, 0, 999, 999],
                    dragbars: [ ],
                    borders: [],
                    allowSelect: false,
                    canDelete: false,
                    canDrag: true,
                    canResize: false
                }, function() {
                    interface_load(this);
                });

                function interface_load(jcropApi) {
                    $('#edit-thumbnail').on('load', function () {
                        var s = jcropApi.getSelection();
                        var w = this.width;
                        var h = this.height;
                        var x = $('#xPos').val();
                        var y = $('#yPos').val();
                        var xPos = 0;
                        var yPos = 0;

                        if (x === '' && y === '') {
                            xPos = (w - s.w) * 50 / 100;
                        } else {
                            xPos = (w - s.w) * x / 100;
                            yPos = (h - s.h) * y / 100;
                        }
                        jcropApi.animateTo([xPos, yPos, 999, 999]);
                    });
                }

                $('button.jcrop-box').click(function(e) {
                    e.preventDefault();
                });

                var container = $('#edit-thumbnail').Jcrop('api').container;
                container.on('cropstart', function() {
                    $('#save-crop').show();
                });

                container.on('cropend', function (e, s, c) {
                    e.preventDefault();
                    var imgWidth = $('#edit-thumbnail').width();
                    var imgHeight = $('#edit-thumbnail').height();
                    var xPos = (c.x / (imgWidth - c.w)) * 100;
                    var yPos = (c.y / (imgHeight - c.h)) * 100;

                    if (isNaN(xPos)) {
                        xPos = 0;
                    }
                    if (isNaN(yPos)) {
                        yPos = 0;
                    }
                    $('#xPos').val(Math.round(xPos));
                    $('#yPos').val(Math.round(yPos));
                    return false;
                });

                $('#save-crop').click(function(ev) {
                    ev.preventDefault();
                    var a = this;
                    f.attr("action", a.href);
                    f.submit();
                    return false;
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
                        closeOnConfirm: true
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

    $('body').on('click', 'a.family-picture', function (ev) {
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
                        closeOnConfirm: true
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
        $("<form class='modal-form validate ajax' />").load(this.href, {}, function () {
            var f = $(this);
            $('#empty-dialog').html(f);
            $('#empty-dialog').modal("show");

            $(".clickEdit", f).editable({
                mode: 'popup',
                type: 'textarea',
                url: "/OrgMemberDialog/EditQuestion/",
                params: function (params) {
                    var data = {};
                    data['id'] = params.pk;
                    data['value'] = params.value;
                    return data;
                }
            });
            $('#empty-dialog').on('hidden', function () {
                f.remove();
                $.RebindMemberGrids();
            });
        });
    });

    var subLink = null;

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        e.preventDefault();
        var tab = $(e.target).attr('href').replace("#", "#tab-");
        window.location.hash = tab;
        $.cookie('lasttab', tab);

        switch (tab) {
            case '#tab-enrollment':
                if (subLink) {
                    if (subLink.attr("href") !== '#') {
                        $.cookie('lasttab', tlink.attr("href"));
                        subLink.click().tab("show");
                        subLink = null;
                    }
                } else {
                    var id = "#current";
                    if ($(id).length < 2) {
                        $("a[href='" + id + "']").click().tab("show");
                        $.cookie('lasttab', id);
                    }
                }
                break;
            case '#tab-profile':
                if (subLink) {
                    if (subLink.attr("href") !== '#') {
                        $.cookie('lasttab', tlink.attr("href"));
                        subLink.click().tab("show");
                        subLink = null;
                    }
                } else {
                    var id = "#memberstatus";
                    if ($(id).length < 2) {
                        $("a[href='" + id + "']").click().tab("show");
                        $.cookie('lasttab', id);
                    }
                }
                break;
            case '#tab-ministry':
                if (subLink) {
                    if (subLink.attr("href") !== '#') {
                        $.cookie('lasttab', tlink.attr("href"));
                        subLink.click().tab("show");
                        subLink = null;
                    }
                } else {
                    var id = "#contactsreceived";
                    if ($(id).length < 2) {
                        $("a[href='" + id + "']").click().tab("show");
                        $.cookie('lasttab', id);
                    }
                }
                break;
            case '#tab-giving':
                if (subLink) {
                    if (subLink.attr("href") !== '#') {
                        $.cookie('lasttab', tlink.attr("href"));
                        subLink.click().tab("show");
                        subLink = null;
                    }
                } else {
                    var id = "#contributions";
                    if ($(id).length < 2) {
                        $("a[href='" + id + "']").click().tab("show");
                        $.cookie('lasttab', id);
                    }
                }
                break;
            case '#tab-emails':
                if (subLink) {
                    if (subLink.attr("href") !== '#') {
                        $.cookie('lasttab', tlink.attr("href"));
                        subLink.click().tab("show");
                        subLink = null;
                    }
                } else {
                    var id = "#receivedemails";
                    if ($(id).length < 2) {
                        $("a[href='" + id + "']").click().tab("show");
                        $.cookie('lasttab', id);
                    }
                }
                break;
            case '#tab-system':
                if (subLink) {
                    if (subLink.attr("href") !== '#') {
                        $.cookie('lasttab', tlink.attr("href"));
                        subLink.click().tab("show");
                        subLink = null;
                    }
                } else {
                    var id = "#user";
                    if ($(id).length < 2) {
                        $("a[href='" + id + "']").click().tab("show");
                        $.cookie('lasttab', id);
                    }
                }
                break;
        }
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
            subLink = tlink;
            $("a[href='#" + tabparent + "']").click().tab("show");
        } else {
            if (tlink.attr("href") !== '#') {
                $.cookie('lasttab', tlink.attr("href"));
                tlink.click().tab("show");
            }
        }
    }

    $('body').on('click', '#future', function (ev) {
        ev.preventDefault();
        var d = $(this).closest('div.loaded');
        var q = d.find("form").serialize();
        $.post($("#FutureLink").val(), q, function (ret) {
            d.html(ret);
        });
    });

//    $('#addrf').validate();
//
//    $('#addrp').validate();

    //$('#basic').validate();

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
            source: "/Person2/FamilyPositions",
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
        $("a.editable-bit").editable({ type: 'checklist', mode: 'popup', source: { 'True': 'True' }, emptytext: 'False' });
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
                swal({ title: ret, type: "success" });
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
                swal({ title: ret, type: "success" });
            });
        });
    });

    $('body').on('click', ".leave-org", function (e) {
        var elem = this;
        e.preventDefault();
        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-warning",
            confirmButtonText: "Yes, unsubscribe!",
            closeOnConfirm: false
        },
        function() {
            $.post("/OrgMemberDialog/Drop",
            {
                OrgId: $(elem).data("orgid"),
                PeopleId: $(elem).data("personid"),
                RemoveFromEnrollmentHistory: false,
                Group: "Member"
            }, function(ret) {
                swal({ title: "Unsubscribed!", type: "success" });
                $(elem).parents("tr").remove();
            });
        });
    });

    $.RebindMemberGrids = function() {
        $("a.refresh-current").click();
        $("a.refresh-pending").click();
    };

    $('body').on('click', '#role-list input[name="role"]', function (ev) {
        var roles = $('#role-list input[name="role"]:checked');
        var arr = $.map(roles, function (a) {
            return a.value;
        });
        var checkinonly = arr.length === 1 && arr[0].toLowerCase() === 'checkin';
        if (checkinonly) {
            $('#myDataUserRole').prop('checked', false);
            return;
        }
        var anyRolesChecked = arr.length > 0;
        if (anyRolesChecked) {
            $('#myDataUserRole').prop('checked', false);
            $('#role-list input[value="Access"]').prop('checked', true);
        } else {
            $('#myDataUserRole').prop('checked', true);
        }
        var viewVolApp = $('#role-list input[value="ViewVolunteerApplication"]');
        var appRev = $('#role-list input[value="ApplicationReview"]');
        if ($(this).attr('value') == appRev.attr('value')) {    
            viewVolApp.prop('checked', false);
        }
        if ($(this).attr('value') == viewVolApp.attr('value')) {
            appRev.prop('checked', false);
        }
    });

    $('body').on('click', '#myDataUserRole', function (ev) {
        var anyRolesChecked = $('#role-list input[name="role"]').is(':checked');
        if (anyRolesChecked) {
            swal({
                title: "Are you sure?",
                text: "This will uncheck all other roles for this user.",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-warning",
                confirmButtonText: "Yes, continue!",
                closeOnConfirm: true
            },
            function (isConfirm) {
                if (isConfirm) {
                    $('#role-list input[name="role"]').prop('checked', false);

                } else {
                    $('#myDataUserRole').prop('checked', false);
                }
            });
        } else {
            $('#role-list input[name="role"]').prop('checked', false);
            $('#myDataUserRole').prop('checked', true);
        }
    });
    
    $(".customstatements").click(function (ev) {
        ev.preventDefault();
        $('#statement-href').val(this.href);
        $('#customstatements-modal').modal('show');
        return true;
    });
    $('#customstatements-modal').on('hidden.bs.modal', function () {
        $("#attdetail2").off("click");
    });

    $('#customstatements-modal').on('shown.bs.modal', function () {
        $('#attdetail2').on("click", function (ev2) {
            ev2.preventDefault();
            $('#customstatements-modal').modal('hide');
            var args = "?dt1=" + $('#MeetingDate1').val() + "&dt2=" + $('#MeetingDate2').val();
            $("#orgsearchform").attr("action", $('#meetings-daterange-action').val() + args);
            $("#orgsearchform").submit();
            return false;
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
            window.location = "/Merge/{0}/{1}".format(ret.pid, ret.pid2);
            break;
    }
}
