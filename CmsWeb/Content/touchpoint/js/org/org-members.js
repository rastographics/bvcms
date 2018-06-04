$(function () {

    $.RefreshPage = function () {
        var f = $('#form');
        var q = f.serialize();
        $.block();
        $.post("/OrgMembers/List", q, function (ret) {
            $(f).html(ret).ready(function () {
                $.unblock();
                // Enable the popover help
                $('[data-toggle="popover"]').popover({ html: true });
            });
        });
    };
     $.InitFunctions.ReloadPeople = function () {
        $.RefreshPage();
    };

    $(document).on('keyup keypress', 'form input[type="text"]', function (e) {
        if (e.keyCode == 13) {
            e.preventDefault();
            return false;
        }
    });
    $('body').on('click', '#refresh', $.RefreshPage);
    $('body').on('change', '#ProgId', $.RefreshPage);
    $('body').on('change', '#SourceDivId', $.RefreshPage);
    $('body').on('change', '#SourceId', $.RefreshPage);
    $('body').on('change', '#TargetDivId', $.RefreshPage);
    $('body').on('change', '#TargetId', $.RefreshPage);
    $('body').on('change', '#MembersOnly', $.RefreshPage);
    $('body').on('change', '#Grades', $.RefreshPage);

    $('body').on('click', '#move', function (e) {
        e.preventDefault();
        var f = $('#form');
        var q = f.serialize();
        $.block();
        $.post("/OrgMembers/Move", q, function (ret) {
            if (ret.startsWith("!")) {
                $.unblock();
                swal({
                    title: "Oops...",
                    text: ret.substr(1),
                    type: "error"
                });
            }
            else {
                $(f).html(ret).ready(function () {
                    $.unblock();
                    swal({
                        title: "Completed!",
                        text: "Move completed.",
                        type: "success"
                    });
                });
            }
            // Enable the popover help
            $('[data-toggle="popover"]').popover({ html: true });
        });
    });

    $('body').on('click', '#SelectAll', function () {
        if ($(this).is(':checked')) {
            $("#list input[name='List']").prop('checked', true);
        } else {
            $("#list input[name='List']").prop('checked', false);
        }
    });

    var lastChecked = null;
    $(document).on("click", "#list input[name='List']", null, function (e) {
        if (e.shiftKey && lastChecked !== null) {
            var start = $("#list input[name='List']").index(this);
            var end = $("#list input[name='List']").index(lastChecked);
            $("#list input[name='List']").slice(Math.min(start, end), Math.max(start, end) + 1).prop("checked", true);
        }
        lastChecked = this;
    });

    $('body').on('click', 'a.EmailNotices', function (e) {
        e.preventDefault();
        var f = $("#form");
        var q = f.serialize();
        $.post("/OrgMembers/EmailNotices", q, function (ret) {
            $(f).html(ret).ready(function () {
                swal({
                    title: "Completed!",
                    text: "Email notices sent.",
                    type: "success"
                });
                // Enable the popover help
                $('[data-toggle="popover"]').popover({ html: true });
            });
        });
        return false;
    });

    $('body').on('click', 'a.ResetMoved', function (e) {
        e.preventDefault();
        var f = $("#form");
        var q = f.serialize();
        $.post("/OrgMembers/ResetMoved", q, function (ret) {
            $(f).html(ret).ready(function () {
                swal({
                    title: "Completed!",
                    text: "Email notices reset.",
                    type: "success"
                });
                // Enable the popover help
                $('[data-toggle="popover"]').popover({ html: true });
            });
        });
        return false;
    });

    $("#form").submit(function () {
        return false;
    });

    $('body').on('click', '#list a.sort', function (ev) {
        var newsort = $(this).text();
        var oldsort = $("#Sort").val();
        $("#Sort").val(newsort);
        var dir = $("#Dir").val();
        if (oldsort == newsort && dir == 'asc')
            $("#Dir").val('desc');
        else
            $("#Dir").val('asc');
        $.RefreshPage();
    });

    $('body').on('click', 'a.selectsg', function (ev) {
        ev.preventDefault();
        var t = $(this).text();
        var sg = $("#SmallGroup").val();
        switch (t) {
            case "Match All":
                if (!sg.match(/^ALL/i)) {
                    sg = "All:" + sg;
                }
                break;
            default:
                if (sg && !sg.match(/^ALL:$/i)) {
                    sg = sg + ';';
                }
                sg = sg + t;
                break;
        }
        $("#SmallGroup").val(sg);
        $("a.selectsg .fa-minus").hide();
        return false;
    });

    $('body').on('click', 'a.selectgrades', function (ev) {
        ev.preventDefault();
        var t = $(this).text();
        var g = $("#Grades").val();
        if (g)
            g = g + ';';
        g = g + t;
        $("#Grades").val(g);
        $("a.selectgrades .fa-minus").hide();
        return false;
    });

    $('[data-toggle="popover"]').popover({ html: true });

    $('body').on('change', '#ChangeMemberType', function () {
        if ($(this).is(':checked')) {
            $("#member-types").show();
        } else {
            $("#member-types").hide();
        }
    });
});
