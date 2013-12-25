$(function () {
    $("#search-add a.commit").live("click", function (ev) {
        ev.preventDefault();
        var f = $(this).closest("form");
        var q = f.serialize();
        var loc = $(this).attr("href");
        $.post(loc, q, function (ret) {
            f.modal("hide");
            if (ret.message) {
                alert(ret.message);
            } else
                switch (ret.from) {
                    case 'contributor':
                        AddSelected(ret);
                        break;
                    case 'Menu':
                        window.location = '/Person2/' + ret.pid;
                        break;
                }
        });
        return false;
    });
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
                $.growlUI("PeopleId", "Not Found");
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
    $(".bt").button();
    $('td.name').tooltip({ showBody: "|" });
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
            $.growlUI("Name", "Not Found");
            $('#name').focus();
        }
        else if ($(this).val() != $.enteredname && $.enteredname != '') {
            var q = $('#pbform').serialize();
            $.post("/PostBundle/GetNamePid/", q, function (ret) {
                if (ret.error == 'not found') {
                    $.growlUI("PeopleId", "Not Found");
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

    $.Stripe = function () {
        $('#bundle tbody tr').removeClass('alt');
        $('#bundle tbody tr:even').addClass('alt');
    };
    $.Stripe();

    var keyallowed = true;

    $('#notes').keydown(function (event) {
        if (keyallowed && (event.keyCode == 9 || event.keyCode == 13) && !event.shiftKey) {
            event.preventDefault();
            keyallowed = false;
            $.PostRow({ scroll: false });
        }
    });
    $('#pid').keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            if (!$.browser.msie)
                $('#pid').blur();
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
    $('a.edit').live("click", function (ev) {
        ev.preventDefault();
        var tr = $(this).closest("tr");
        $('#editid').val(tr.attr("cid"));
        $('#pid').val($.trim($("a.pid", tr).text()));
        $('#name').val($("td.name", tr).text());
        $('#contributiondate').val($(".date", tr).val());
        $("#gear").show();
        $('#fund').val($("td.fund", tr).attr('val'));

        var plnt = $("td.PLNT", tr).text();
        $("#entry .PLNT").html(plnt);
        plnt = plnt || "CN";
        $("input[name=PLNT][value=" + plnt + "]").prop('checked', true);

        var a = $('#amt');
        a.val($("td.amt", tr).attr("val"));
        $('#checkno').val($("td.checkno", tr).text());
        $('#notes').val($("td.notes", tr).text());
        tr.hide();
        if (a.val() == '0.00')
            a.val('');
        $('html,body').animate({ scrollTop: 0 }, 600);
        a.focus();
        $('a.edit').hide();
        $('a.split').hide();
        $('a.delete').hide();
        $.Stripe();
    });
    $("input[name='PLNT']").change(function () {
        var v = $(this).val();
        if (v == 'CN')
            v = "";
        $("#entry .PLNT").text(v);
    });
    $('a.split').live("click", function (ev) {
        ev.preventDefault();
        var newamt = prompt("Amount to split out", "");
        newamt = parseFloat(newamt);
        if (isNaN(newamt))
            return false;
        var tr = $(this).closest("tr");

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
    });
    $('a.delete').live("click", function (ev) {
        ev.preventDefault();
        if (confirm("are you sure?")) {
            var tr = $(this).closest("tr");
            $('#editid').val(tr.attr("cid"));
            var q = $('#pbform').serialize();
            $.post("/PostBundle/DeleteRow/", q, function (ret) {
                tr.remove();
                $('#totalitems').text(ret.totalitems);
                $('#itemcount').text(ret.itemcount);
                $('#editid').val('');
                $.Stripe();
            });
        }
    });
    $('a.pid').live("click", function (event) {
        if (!$(this).hasClass('searchadd')) {
            event.preventDefault();
            var d = $('#searchDialog');
            $('iframe', d).attr("src", this.href);
            d.dialog("open");
        }
    });
    $('#bundle').bind('mousedown', function (e) {
        if ($(e.target).hasClass("clickEdit")) {
            $(e.target).editable(function (value, settings) {
                $.post("/PostBundle/Edit/", { id: e.target.id, value: value }, function (ret) {
                    $('#totalitems').text(ret.totalitems);
                    $('#itemcount').text(ret.itemcount);
                    $('#a' + ret.cid).text(ret.amt);
                });
                return (value);
            }, {
                indicator: "<img src='/images/loading.gif'>",
                tooltip: "Click to edit...",
                style: 'display: inline',
                width: '4em',
                height: 25
            });
        }
        else if ($(e.target).hasClass("clickSelect")) {
            $(e.target).editable("/PostBundle/Edit/", {
                indicator: '<img src="/Content/images/loading.gif">',
                loadtype: 'post',
                loadurl: "/PostBundle/Funds/",
                type: "select",
                submit: "OK",
                style: 'display: inline'
            });
        }
    });
    $.PostRow = function (options) {
        $("#gear").hide();
        if (!options.q) {
            var n = parseFloat($('#amt').val());
            var plnt = $("#entry .PLNT").text();
            if (!n > 0 && plnt != 'GK') {
                $.growlUI("Contribution", "Cannot post, No Amount");
                return;
            }
            if (!isNaN(n) && n != 0 && plnt == 'GK') {
                $.growlUI("Contribution", "Cannot post, Gift In Kind must be zero");
                return;
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
            $('#totalitems').text(ret.totalitems);
            $('#itemcount').text(ret.itemcount);
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
            $('a.edit').show();
            $('a.split').show();
            $('a.delete').show();
            $(".bt", tr).button();
            $('#editid').val('');
            $('#entry input').val('');
            $('#fund').val($('#fundid').val());
            $.Stripe();
            $('#pid').focus();
            keyallowed = true;
            var top = tr.offset().top - 360;
            if (options.scroll == true) {
                $('html,body').animate({ scrollTop: top }, 1000);
            }
            tr.effect("highlight", {}, 3000);
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
    $("#totalitems").text($("#titems").val());
    $("#totalcount").text($("#tcount").val());


    $("#showmove").click(function (ev) {
        ev.preventDefault();
        $.blockUI({ message: $('#movebundle') });
    });
    $("#moveit").click(function (ev) {
        ev.preventDefault();
        $.post("/PostBundle/Move/" + $("#editid").val(), { moveto: $("#moveto").val() }, function (ret) {
            $.unblockUI();
            if (ret.status === "ok") {
                $('#editid').val('');
                $('#entry input').val('');
                $('#fund').val($('#fundid').val());
                $('#pid').focus();
                $('#totalitems').text(ret.totalitems);
                $('#itemcount').text(ret.itemcount);
                keyallowed = true;
                $.growlUI("Contribution", "Contribution Moved");
            } else {
                $.growlUI("Move To Bundle", ret);
            }
        });
    });
    $("#movecancel").click(function (ev) {
        ev.preventDefault();
        $.unblockUI();
    });

    $("#showdate").click(function (ev) {
        ev.preventDefault();
        $("#contributiondate").datepicker();
        $.blockUI({ message: $('#editdate') });
    });
    $("#editdatedone").click(function (ev) {
        ev.preventDefault();
        $.unblockUI();
    });
});
function AddSelected(ret) {
    $('#searchDialog').dialog("close");
    var tr = $('tr[cid=' + ret.cid + ']');
    $('a.pid', tr).text(ret.pid);
    $('td.name', tr).text(ret.name);
    $('a.edit', tr).click();
}

