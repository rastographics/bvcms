$(document).ready(function () {

    function initializePopovers() {
        $('[data-toggle="popover"]').popover({ html: true });
        $('[data-toggle="popover"]').click(function (ev) {
            ev.preventDefault();
        });
    }

    $('#name').focus();
    initializePopovers();

    $('body').on('click', '#resultsTable > thead a.sortable', function (ev) {
        ev.preventDefault();
        var newsort = $(this).attr('data-sortby');
        var sort = $("#Sort");
        var dir = $("#Direction");
        if ($(sort).val() == newsort && $(dir).val() == 'asc')
            $(dir).val('desc');
        else
            $(dir).val('asc');
        $(sort).val(newsort);

        var f = $('#results').closest('form');
        $.getTable(f);
        return false;
    });

    $("body").on("click", '.filterbatch', function (e) {
        e.preventDefault();
        $("#name").val($(this).text());
        $('#filter').click();
    });

    $("body").on("click", '.filterbatchdesc', function (e) {
        e.preventDefault();
        $("#description").val($(this).text());
        $('#filter').click();
    });

    $("body").on("click", '.filtertransaction', function (e) {
        e.preventDefault();
        $("#name").val($(this).attr("originalid"));
        $('#filter').click();
    });

    $.gotoPage = function (e, pg) {
        $("#Page").val(pg);
        var f = $('#results').closest('form');
        $.getTable(f);
        return false;
    };

    $.setPageSize = function (e) {
        $('#Page').val(1);
        $("#PageSize").val($(e).val());
        var f = $('#results').closest('form');
        $.getTable(f);
        return false;
    };

    $.getTable = function (f) {
        var q = null;
        if (f)
            q = f.serialize();
        $.block();
        $.post("/Transactions/List", q, function (ret) {
            $('#results').html(ret);
            initializePopovers();
            $.unblock();
        });
        return false;
    };

    $('body').on('click', '#filter', function (ev) {
        ev.preventDefault();
        var f = $('#results').closest('form');
        $('#Page', f).val(1);
        $.getTable(f);
        $('#page-header h2').text('Transactions');
        return false;
    });

    $('body').on('click', '.report', function (ev) {
        ev.preventDefault();
        var header = $(this).text();
        var sdt = $('#startdt').val();
        var edt = $('#enddt').val();
        if (!sdt || !edt) {
            swal("Error!", "Must set a date range.", "error");
            return false;
        }
        $.block();
        var q = $('#results').closest('form').serialize();
        $.post($(this).attr("href"), q, function (ret) {
            $('#page-header h2').text(header);
            $('#results').html(ret);
            initializePopovers();
            $.unblock();
        });
        return true;
    });

    $('body').on('click', '#export', function (ev) {
        ev.preventDefault();
        $('#filter-dropdown').dropdown('toggle');
        var f = $('#results').closest('form');
        f.attr("action", "/Transactions/Export");
        f.submit();
        f.attr("action", "/Transactions/List");
        return false;
    });

    $("body").on("click", 'a.void', function (ev) {
        ev.preventDefault();
        var a = $(this);
        var f = $('#results').closest('form');
        var q = f.serialize();

        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, void it!",
            closeOnConfirm: false
        },
        function () {
            $.post(a.attr("href"), q, function (ret) {
                if (ret.substring(5, 0) == "error")
                    swal("Error!", ret, "error");
                else {
                    swal({
                        title: "Transaction Voided!",
                        type: "success"
                    },
                    function () {
                        $("#results").html(ret);
                        initializePopovers();
                    });
                }
            });
        });
        return false;
    });
    $("body").on("click", 'a.deleteadj', function (ev) {
        ev.preventDefault();
        var a = $(this);
        var f = $('#results').closest('form');
        var q = f.serialize();

        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete it!",
            closeOnConfirm: false
        },
        function () {
            $.post(a.attr("href"), q, function (ret) {
                swal({
                    title: "Deleted!",
                    type: "success"
                },
                function () {
                    $("#results").html(ret);
                    initializePopovers();
                });
            });
        });
        return false;
    });
    $("body").on("click", 'a.deletegsa', function (ev) {
        ev.preventDefault();
        var a = $(this);
        var f = $('#results').closest('form');
        var q = f.serialize();

        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete the goer sender amount!",
            closeOnConfirm: false
        },
        function () {
            $.post(a.attr("href"), q, function (ret) {
                swal({
                    title: "Goer Sender Amount Deleted!",
                    type: "success"
                },
                function () {
                    $("#goersupporters").html(ret);
                    initializePopovers();
                });
            });
        });
        return false;
    });

    $("body").on("click", 'a.credit', function (ev) {
        ev.preventDefault();
        var a = $(this);
        $("#crediturl").val(a.attr("href"));
        $('#credit-modal').modal();
        return false;
    });

    $('#credit-modal').on('shown.bs.modal', function () {
        $("#credit-amt").val('').focus();
    });

    $("#post-credit").click(function (ev) {
        ev.preventDefault();
        var f = $('#results').closest('form');
        var q = f.serialize();
        var amt = parseFloat($("#credit-amt").val());

        $('#credit-modal').modal('hide');
        $.block();
        if (isNaN(amt))
            return false;
        q += "&amt=" + amt;

        $.post($("#crediturl").val(), q, function (ret) {
            $.unblock();
            if (ret.substring(5, 0) == "error")
                swal("Error!", ret, "error");
            else {
                swal("Transaction Refunded!", "", "success");
                $("#results").html(ret);
                initializePopovers();
            }
        });
        return false;
    });

    $("body").on("click", 'a.setpar', function (ev) {
        ev.preventDefault();
        var a = $(this);
        var f = $('#results').closest('form');
        var q = f.serialize();
        var parid = prompt("ParentId", "");
        q += "&parid=" + parid;
        $.post(a.attr("href"), q, function (ret) {
            $('#results').html(ret);
            initializePopovers();
        });
        return false;
    });
    $("body").on("click", 'a.assigngoer', function (ev) {
        ev.preventDefault();
        var href = this.href;
        var f = $('#results').closest('form');
        var q = f.serialize();

        bootbox.prompt("Enter a Goer's PeopleId", function (result) {
            if (result !== null) {
                href += '/' + result;
                $.post(href, q, function (ret) {
                    $('#goersupporters').html(ret);
                    initializePopovers();
                    swal("Assigned!", "GoerId now = " + result, "success");
                });
            }
        });
    });

    $("body").on("click", 'a.adjust', function (ev) {
        ev.preventDefault();
        var a = $(this);
        $("#voidurl").val(a.attr("href"));
        $('#adjust-modal').modal();
        return false;
    });

    $('#adjust-modal').on('shown.bs.modal', function () {
        $("#amt").val('').focus();
        $("#desc").val('');
    });

    $("#post").click(function (ev) {
        ev.preventDefault();
        $('#adjust-modal').modal('hide');
        $.block();
        var amt = parseFloat($("#amt").val());
        if (isNaN(amt)) {
            $.unblock();
            return false;
        }
        var q = $("#form").serialize();
        q += "&amt=" + amt;
        q += "&desc=" + $("#desc").val();
        $.post($("#voidurl").val(), q, function (ret) {
            $.unblock();
            if (ret.substring(5, 0) == "error")
                swal("Error!", ret, "error");
            else {
                swal("Transaction Adjusted!", "", "success");
                $("#results").html(ret);
                initializePopovers();
            }
        });
        return false;
    });

    $("form").on("keypress", 'input', function (e) {
        if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
            $('#filter').click();
            return false;
        }
        return true;
    });

});