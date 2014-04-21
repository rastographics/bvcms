$(function () {
    $('#tabs').tabs();
    $("#tabs > ul > li > a").click(function() {
        var x = $(this).attr("href").substring(1);
        return clickTab(x);
    });

    var c = $.cookie("tasklast");
    $('#tasks > thead a.sortable').click(function(ev) {
        $("#Sort").val($(this).text());
        refreshList();
    });
    $('#dialogbox').dialog({
        title: 'Search Dialog',
        bgiframe: true,
        autoOpen: false,
        width: 700,
        height: 630,
        modal: true,
        overlay: {
            opacity: 0.5,
            background: "black"
        }, close: function () {
            $('iframe', this).attr("src", "");
        }
    });
    $('body').on("click", '#changeowner', function (ev) {
        ev.preventDefault();
        var d = $('#dialogbox');
        $('iframe', d).attr("src", this.href);
        d.dialog("option", "title", "Change Owner");
        d.dialog("open");
        return false;
    });
    $('body').on("click", '#delegate', function (ev) {
        ev.preventDefault();
        var d = $('#dialogbox');
        $('iframe', d).attr("src", this.href);
        d.dialog("option", "title", "Delegate task");
        d.dialog("open");
        return false;
    });
    $('body').on("click", '#changeabout', function (ev) {
        ev.preventDefault();
        var d = $('#dialogbox');
        $('iframe', d).attr("src", this.href);
        d.dialog("option", "title", "Make Task About");
        d.dialog("open");
        return false;
    });
    var refreshList = function(ev) {
        if (ev)
            ev.preventDefault();
        var q = $('#form').formSerialize2();
        $.navigate("/Task/List", q);
    };
    var clickTab = function(tab) {
        if (tab)
            $('#CurTab').val(tab);
        $.cookie('CurTaskTab', tab, { expires: 360 });
        refreshList();
        return false;
    };

    var selectTab = function(tab) {
        if (tab)
            $('#CurTab').val(tab);
        var index = $('#tabs a[href="#' + $('#CurTab').val() + '"]').parent().index();
        $('#tabs').tabs('option', "active", index);
    };

    var stripeList = function() {
        $('#tasks > tbody > tr:even').addClass('alt');
    };
    var deleteList = function(qs) {
        $.post('/Task/Action/', qs, function(ret) {
            var a = ret.split("<!---------->");
            $('#tabs').html(a[0]);
            $("#actions").html(a[1]);
            $('#tasks > tbody').html(a[2]).ready(stripeList);
        });
    };
    
    SearchClicked = refreshList;
    selectTab($.cookie("CurTaskTab"));
    stripeList();

    $("#actions").change(function() {
        var v = $('#actions option:selected').val();
        var ai = $(".actionitem:checked").getCheckboxVal().join(",");
        var qs = "option=" + v + "&curtab=" + $('#CurTab').val() + "&items=" + ai;
        $('#actions').attr('selectedIndex', 0);
        if (ai == "")
            return;
        switch (v) {
            case '':
            case '-':
                return;
            case 'delegate':
                var d = $('#dialogbox');
                $('iframe', d).attr("src", "/SearchAdd/1?type=taskdelegate2");
                d.dialog("option", "title", "Delegate tasks");
                d.dialog("open");
                return;
            case 'sharelist':
                $.growlUI("error", "not implemented yet");
                return;
            case 'deletelist':
                if (confirm('Are you sure you want to delete the list?')) {
                    $.block();
                    deleteList(qs);
                    $.unblock();
                }
                return;
            default:
                $.block();
                $.post('/Task/Action/', qs, function(ret) {
                    $('#tasks > tbody').html(ret).ready(stripeList);
                    $.unblock();
                });
                return;
        }
    });

    $("#ListName").keypress(function(e) {
        var key = window.event ? e.keyCode : e.which;
        if (key != 13)
            return true;
        addListClick();
        return false;
    });


    var addListClick = function(ev) {
        if (ev)
            ev.preventDefault();
        var qs = "ListName=" + $("#ListName").val();
        $.post('/Task/AddList/', qs, function(ret) {
            var a = ret.split("<!---------->");
            $('#tabs').html(a[0]);
            $("#actions").html(a[1]);
            $('#ListName').val('');
        });
        return false;
    };
    
    $("#AddList").click(addListClick);

    $("#TaskDesc").keypress(function(e) {
        var key = window.event ? e.keyCode : e.which;
        if (key != 13)
            return true;
        addTaskClick();
        return false;
    });

    var addTaskClick = function(ev) {
        if (ev)
            ev.preventDefault();
        var desc = $("#TaskDesc").val().replace(/\s/g, "");
        if (!desc || desc.length == 0)
            return;
        var qs = "TaskDesc=" + $("#TaskDesc").val() + "&CurTab=" + $("#CurTab").val();
        $.post('/Task/AddTask/', qs, function(ret) {
            $("#nomatch").remove();
            var alt = !($('#tasks > tbody tr:visible:first').hasClass("alt") || false);
            $('#tasks > tbody tr:first').before(ret);
            if (alt)
                $('#tasks > tbody tr:first').addClass("alt");
            $('#TaskDesc').val('');
        });
    };
    
    $("#AddTask").click(addTaskClick);
    $("#OwnerOnly").click(refreshList);
    $("#StatusId").change(refreshList);

    $("body").on("click", 'a.showdetail', function (ev) {
        ev.preventDefault();
        var id = $(this).attr("task");
        var drid = $("#TaskId").val();
        if (drid) {
            $.post("/Task/Detail/" + id + "/Row/" + drid, function(ret) {
                var a = ret.split("<!---------->");
                $('#r' + drid).html(a[0]);
                $('#r' + drid).removeClass("detailrow");
                $('#r' + id).addClass("detailrow").html(a[1]);
            });
        } else {
            $.post('/Task/Detail/' + id, function(ret) {
                $('#r' + id).addClass("detailrow");
                $('#r' + id).html(ret);
            });
        }
    });

    $("body").on("click", '.deselect', function (ev) {
        ev.preventDefault();
        var id = $("#TaskId").val();
        $.post('/Task/Columns/' + id, function(ret) {
            $('#r' + id).removeClass("detailrow").html(ret);
        });
        return false;
    });

    $("body").on("click", 'a.complete', function (ev) {
        ev.preventDefault();
        var id = $(this).attr("task");
        $.post('/Task/SetComplete/' + id, null, function(ret) {
            $('#r' + id).removeClass('detailrow').html(ret);
        });
        return false;
    });

    $("body").on("click", 'a.accept', function (ev) {
        ev.preventDefault();
        var id = $(this).attr("task");
        $.post('/Task/Accept/' + id, null, function(ret) {
            $('#r' + id).html(ret);
        });
        return false;
    });
    $("#dialogbox2").dialog({
        overlay: { background: "#000", opacity: 0.8 },
        bgiframe: true,
        modal: true,
        autoOpen: false,
        closeOnEscape: true,
        width: 700,
        height: 525,
        position: 'top',
        close: function(event, ui) {
            $('#dialogbox2').empty();
            SearchClicked = refreshList;
        }
    });
    $("body").on("click", 'a.completewcontact', function (ev) {
        ev.preventDefault();
        var taskid = $('#TaskId').val();
        $.post('/Task/CompleteWithContact/' + taskid, null, function(ret) {
            window.location = "/Contact/" + ret.ContactId + "?edit=true";
        });
        return false;
    });
    $("body").on("click", 'input.edit', function (ev) {
        ev.preventDefault();
        var id = $("#TaskId").val();
        $.post('/Task/Edit/' + id, function(ret) {
            $('#r' + id).html(ret);
            $(".datepicker").jqdatepicker();
        });
    });
    $("body").on("click", 'input.update', function (ev) {
        ev.preventDefault();
        var id = $("#TaskId").val();
        var qs = $("#Edit").formSerialize2();
        $.post('/Task/Update/' + id, qs, function(ret) {
            $('#r' + id).html(ret);
        }, "html");
        return false;
    });
});

var queryString = "";
function ChangePage() { }
function SearchClicked() { }
function SelectPerson(id) { }

function ActOnPerson(action, peopleid) {
    var taskid = $('#TaskId').val();
    $.post(action + taskid + "?peopleid=" + peopleid, null, function(ret) {
        $('#r' + taskid).html(ret);
    });
    $('#dialogbox').dialog("close");
}
$.fn.initPager = function() {
    this.each(function() {
        $(".pagination", this).pagination($("#Count", this).val(), {
            items_per_page: $("#PageSize", this).val(),
            num_display_entries: 5,
            num_edge_entries: 1,
            current_page: 0,
            callback: ChangePage
        });
        $('#NumItems', this).text($('#Count', this).val().addCommas() + " items");
    });
    return this;
};
function AddSelected(ret) {
    ActOnPerson(ret.url, ret.pid);
}
function AddSelected2(ret) {
    var ai = $(".actionitem:checked").getCheckboxVal().join(",");
    var qs = "items=" + ai;
    $.block();
    $.post('/Task/DelegateAll/' + ret.pid, qs, function (ret) {
        $('#tasks > tbody').html(ret).ready(stripeList);
        $('#dialogbox').dialog("close");
        $.unblock();
    });
}
    var GotoPage = function(pg) {
        var q = $('#form').formSerialize2();
        q = q.appendQuery("Page=" + pg);
        $.navigate("/Task/List", q);
    };
    var SetPageSize = function(sz) {
        var q = $('#form').formSerialize2();
        q = q.appendQuery("PageSize=" + sz);
        $.navigate("/Task/List", q);
    };