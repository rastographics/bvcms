$(function () {

    function initializePopovers() {
        $('[data-toggle="popover"]').popover({ html: true });
        $('[data-toggle="popover"]').click(function (ev) {
            ev.preventDefault();
        });
    }
    initializePopovers();

    $('#pid').blur(function () {
        var tr, pid;
        if ($(this).val() == '')
            return false;
        if ($(this).val() == 'd') {
            tr = $('#bundle > tbody > tr:first');
            pid = $("a.pid", tr).text();
            $('#name').val($("td.name", tr).text());
            $('#checkno').val($("td.checkno", tr).text());
            $('#notes').val($("td.notes", tr).text());
            $('#amt').focus();
            $(this).val($.trim(pid));
            return true;
        }

        var q = $('#pbform').serialize();
        $.post("/PostBundle/GetNamePid/", q, function (ret) {
            if (ret.error == 'not found') {

                $.growl("No Results!", "Person id not found.", "warning");
                $('#name').focus();
                $('#pid').val('');
            }
            else {
                $('#name').val(ret.name);
                $('#pid').val(ret.PeopleId);
                $('#amt').focus();
            }
        });
    });

    $(".ui-autocomplete-input").on("autocompleteopen", function () {
        var autocomplete = $(this).data("autocomplete"),
    		menu = autocomplete.menu;
        if (!autocomplete.options.selectFirst) {
            return;
        }
        menu.activate($.Event({ type: "mouseenter" }), menu.element.children().first());
    });

    $("#name").focus(function () {
        $.enteredname = $(this).val();
    });

    $("#name").autocomplete({
        appendTo: "#SearchResults2",
        autoFocus: true,
        minLength: 3,
        source: function (request, response) {
            if ($("#moreresults").is(":checked")) {
                $.post("/PostBundle/Names2", request, function (ret) {
                    response(ret.slice(0, 30));
                }, "json");
                $("#SearchResults2 > ul").css({
                    'max-height': 400,
                    'overflow-y': 'auto'
                });
            }
            else {
                $.post("/PostBundle/Names", request, function (ret) {
                    response(ret.slice(0, 10));
                }, "json");
            }
        },
        select: function (event, ui) {
            $("#name").val(ui.item.Name);
            $("#pid").val(ui.item.Pid);
            return false;
        }
    }).data("uiAutocomplete")._renderItem = function (ul, item) {
        return $("<li>")
            .append("<a><b>" + item.Name + "</b>" + item.Spouse + "<br>" + item.Addr + item.RecentGifts + "</a>")
            .appendTo(ul);
    };

    $("#name").blur(function () {
        if ($('#pid').val() == '' && $(this).val() != '') {
            $.growl("No Results!", "Name of person not found.", "warning");
            $('#name').focus();
        }
        else if ($(this).val() != $.enteredname && $.enteredname != '') {
            var q = $('#pbform').serialize();
            $.post("/PostBundle/GetNamePid/", q, function (ret) {
                if (ret.error == 'not found') {
                    $.growl("No Results!", "Person id not found.", "warning");
                    $('#name').focus();
                    $('#pid').val('');
                }
                else {
                    $('#name').val(ret.name);
                    $('#pid').val(ret.PeopleId);
                    $('#amt').focus();
                }
            });
        }
    });

    var keyallowed = true;

    $('#notes').keypress(function (event) {
        if (keyallowed && (event.keyCode == 9 || event.keyCode == 13) && !event.shiftKey) {
            event.preventDefault();
            keyallowed = false;
            $.PostRow({ scroll: false });
        }
    });

    $('#pid').keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            $('#name').focus();
        }
    });

    $('#name').keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            $('#amt').focus();
        }
    });

    $('#amt').keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            $('#PLNT').focus();
        }
    });

    $('#PLNT').keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            $('#fund').focus();
        }
    });

    $('#fund').keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            $('#checkno').focus();
        }
    });

    $('#checkno').keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            $('#notes').focus();
        }
    });

    $('a.update').click(function (event) {
        event.preventDefault();
        $.PostRow({ scroll: true });
    });

    $("body").on("click", 'a.edit', function (ev) {
        ev.preventDefault();
        var tr = $(this).closest("tr");
        $('#editid').val(tr.attr("cid"));
        $('#pid').val($.trim($("a.pid", tr).text()));
        $('#name').val($("td.name span", tr).text());
        $('#contributiondate').val($(".date", tr).val());
        $("#gear").show();
        $('#fund').val($("td.fund", tr).attr('val'));

        var plnt = $("td.PLNT span", tr).text();
        plnt = plnt || "CN";
        $('#PLNT').val(plnt);
        
        var a = $('#amt');
        a.val($("td.amt", tr).attr("val"));
        $('#checkno').val($("td.checkno span", tr).text());
        $('#notes').val($("td.notes span", tr).text());
        tr.hide();
        if (a.val() == '0.00')
            a.val('');
        $('html,body').animate({ scrollTop: 0 }, 600);
        a.focus();
        $('button.contribution-actions').prop('disabled', true);
    });

    $("body").on("click", 'a.split', function (ev) {
        ev.preventDefault();
        $('#split-modal').modal('show');
        $('#contributionId').val($(this).closest("tr").attr('cid'));
    });

    $('#split-modal').on('shown.bs.modal', function () {
        $("#amt-split").val('').focus();
    });

    $("#split-submit").click(function (ev) {
        ev.preventDefault();

        var newamt = $("#amt-split").val();
        newamt = parseFloat(newamt);
        if (isNaN(newamt))
            return false;
        var tr = $('tr[cid=' + $('#contributionId').val() + ']');
        var q = {
            pid: $("a.pid", tr).text(),
            name: $("td.name", tr).text(),
            fund: $("td.fund", tr).attr('val'),
            pledge: $("td.fund", tr).attr('pledge'),
            amt: newamt,
            splitfrom: tr.attr("cid"),
            checkno: $("td.checkno", tr).text(),
            notes: $("td.notes", tr).text(),
            id: $("#id").val()
        };
        $.PostRow({ scroll: true, q: q });
        $('#split-modal').modal('hide');
    });

    $("body").on("click", 'a.delete', function (ev) {
        ev.preventDefault();
        var tr = $(this).closest("tr");
        $('#editid').val(tr.attr("cid"));
        var q = $('#pbform').serialize();
        
        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete it!",
            closeOnConfirm: false
        },
        function () {
            $.post("/PostBundle/DeleteRow/", q, function (ret) {
                if (ret && ret.error)
                    swal("Error!", ret.error, "error");
                else {
                    tr.remove();
                    showHideDiffRow(ret.diff);
                    $('.totalitems').text(ret.totalitems);
                    $('.difference').text(ret.difference);
                    $('.itemcount').text(ret.itemcount);
                    
                    $('#editid').val('');
                    swal({
                        title: "Deleted!",
                        type: "success"
                    });
                }
            });
        });
    });

    function showHideDiffRow(diff) {
        if (diff == 0) {
            $('tr.diffRow').hide();
        } else {
            $('tr.diffRow').show();
        }
    }

    $("body").on("click", 'a.pid', function (ev) {
        if (!$(this).hasClass('searchadd')) {
            event.preventDefault();
            var d = $('#searchDialog');
            $('iframe', d).attr("src", this.href);
            d.dialog("open");
        }
    });

    $.fn.editableform.buttons = '<button type="submit" class="btn btn-primary btn-sm editable-submit">' +
                                            '<i class="fa fa-fw fa-check"></i>' +
                                        '</button>' +
                                        '<button type="button" class="btn btn-default btn-sm editable-cancel">' +
                                            '<i class="fa fa-fw fa-times"></i>' +
                                        '</button>';
    function initializeEditable() {
        $(".clickEdit").editable({
            mode: 'popup',
            type: 'text',
            url: "/PostBundle/Edit/",
            params: function (params) {
                var data = {};
                data['id'] = params.pk;
                data['value'] = params.value;
                return data;
            },
            success: function (ret) {
                showHideDiffRow(ret.diff);
                $('.totalitems').text(ret.totalitems);
                $('.difference').text(ret.difference);
                $('.itemcount').text(ret.itemcount);
                $('#a' + ret.cid).closest('td').attr('val', ret.amt);
            }
        });

        $(".clickSelect").editable({
            mode: 'popup',
            type: 'select',
            url: "/PostBundle/Edit/",
            source: "/PostBundle/Funds/",
            params: function (params) {
                var data = {};
                data['id'] = params.pk;
                data['value'] = params.value;
                return data;
            },
            success: function (ret, value) {
                $(this).closest('td').attr('val', value);
            }
        });
    }
    initializeEditable();

    $.PostRow = function (options) {
        if (!options.q) {
            var n = parseFloat($('#amt').val());
            var plnt = $("#PLNT").val();
            if (!n > 0 && plnt != 'GK' && plnt != 'SK') {
                swal("Contribution Error!", "Cannot post. No amount specified.", "error");
                return true;
            }
            if (!isNaN(n) && n != 0 && plnt == 'GK') {
                swal("Contribution Error!", "Cannot post. Gift In Kind must be zero.", "error");
                return true;
            }
            options.q = $('#pbform').serialize();
        }
        var action = "/PostBundle/PostRow/";
        var cid = $('#editid').val();
        if (cid)
            action = "/PostBundle/UpdateRow/";
        $.post(action, options.q, function (ret) {
            if (!ret)
                return;
            if (ret.error) {
                alert(ret.error);
                return;
            }
            showHideDiffRow(ret.diff);
            $('.totalitems').text(ret.totalitems);
            $('.difference').text(ret.difference);
            $('.itemcount').text(ret.itemcount);
            var pid = $('#pid').val();
            var tr;
            if (cid) {
                tr = $('tr[cid="' + cid + '"]');
                tr.replaceWith(ret.row);
                tr = $('tr[cid="' + cid + '"]');
            }
            else if (options.q.splitfrom) {
                tr = $('tr[cid="' + options.q.splitfrom + '"]');
                $('#a' + options.q.splitfrom).text(ret.othersplitamt);
                $(ret.row).insertAfter(tr);
                tr = $('tr[cid="' + ret.cid + '"]');
            }
            else {
                $('#bundle tbody').prepend(ret.row);
                tr = $('#bundle tbody tr:first');
            }
            $('td.name', tr).tooltip({ showBody: "|" });
            $('button.contribution-actions').prop('disabled', false);
            $('#editid').val('');
            $('#entry input').val('');
            $('#fund').val($('#fundid').val());
            $('#pid').focus();
            keyallowed = true;
            var top = tr.offset().top - 360;
            if (options.scroll == true) {
                $('html,body').animate({ scrollTop: top }, 1000);
            }
            $(tr).children('td').effect("highlight", { color: '#fcf8e3' }, 3000);
            $("#gear").hide();
            initializePopovers();
            initializeEditable();
        });
    };
    $('#searchDialog').dialog({
        title: 'Search/Add Contributor',
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
    //$(".totalitems").text($("#titems").val());
    //$("#totalcount").text($("#tcount").val());


    $("#showmove").click(function (ev) {
        ev.preventDefault();
        $('#move-modal').modal('show');
    });

    $('#move-modal').on('shown.bs.modal', function () {
        $("#moveto").val('').focus();
    });

    $("#moveit").click(function (ev) {
        ev.preventDefault();
        $.post("/PostBundle/Move/" + $("#editid").val(), { moveto: $("#moveto").val() }, function (ret) {
            $('#move-modal').modal('hide');
            if (ret.status === "ok") {
                $('#editid').val('');
                $('#entry input').val('');
                $('#fund').val($('#fundid').val());
                $('#pid').focus();
                showHideDiffRow(ret.diff);
                $('.totalitems').text(ret.totalitems);
                $('.difference').text(ret.difference);
                $('.itemcount').text(ret.itemcount);
                keyallowed = true;
                swal({
                    title: "Moved!",
                    text: "Contribution was successfully moved.",
                    type: "success"
                });
                $('button.contribution-actions').prop('disabled', false);
                $("#gear").hide();
            } else {
                swal("Error!", ret.error, "error");
            }
        });
    });

    $("#movecancel").click(function (ev) {
        ev.preventDefault();
        $.unblockUI();
    });

    $("#showdate").click(function (ev) {
        ev.preventDefault();
        $('#edit-date-modal').modal('show');
    });

    $('#edit-date-modal').on('shown.bs.modal', function () {
        $("#newcontributiondate").val($("#contributiondate").val()).focus();
        $.InitializeDateElements();
    });

    $("#editdatedone").click(function (ev) {
        ev.preventDefault();
        $("#contributiondate").val($("#newcontributiondate").val());
        $('#edit-date-modal').modal('hide');
    });
});

function AddSelected(ret) {
    $('#searchDialog').dialog("close");
    var tr = $('tr[cid=' + ret.cid + ']');
    $('a.pid', tr).text(ret.pid);
    $('td.name', tr).text(ret.name);
    $('a.edit', tr).click();
}
