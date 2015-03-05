$(function () {

    function intializePopovers() {
        $('[data-toggle="popover"]').popover({ html: true });
        $('[data-toggle="popover"]').click(function (ev) {
            ev.preventDefault();
        });
    }


    $.fn.editableform.buttons = '<button type="submit" class="btn btn-primary btn-sm editable-submit">' +
                                           '<i class="fa fa-fw fa-check"></i>' +
                                       '</button>' +
                                       '<button type="button" class="btn btn-default btn-sm editable-cancel">' +
                                           '<i class="fa fa-fw fa-times"></i>' +
                                       '</button>';

    
    $("button.create").click(function(ev) {
        ev.preventDefault();
        if ($("#TagProgramId").val() === "0") {
            swal("Error!", "Target program must be selected.", "error");
            return false;
        }
        var f = $('#progform');
        var q = f.serialize();
        $.post("/Division/Create/", q, function(ret) {
            $('#results > tbody').prepend(ret);
            $('body').animate({ scrollTop: 0 }, 500);

            var row = $('#results tbody').children('tr:first');
            var bgColor = $(row).css('background-color');
            $(row).animate({ backgroundColor: '#fcf8e3' }, 1000, function () {
                $(row).animate({ backgroundColor: bgColor }, 1000);
            });
            $.initializeTable();
        });
        return false;
    });

    $('span.clickEdit').bind('keydown', function(event) {
        if (event.keyCode == 9) {
            $(this).find("input").blur();
            var i = $('.clickEdit').index(this);
            $(".clickEdit:eq(" + (i + 2) + ")").click();
            return false;
        }
    });

    $('body').on('click', 'a.taguntag', function(ev) {
        ev.preventDefault();
        var f = $('#progform');
        var q = f.serialize();
        var a = $(this);
        $.post(a.attr('href'), q, function(ret) {
            var tr = a.closest("tr");
            tr.replaceWith(ret);
            $.initializeTable();
        });
        return false;
    });

    $('body').on('click', 'a.mainprog', function(e) {
        e.preventDefault();
        var f = $('#progform');
        var q = f.serialize();
        var a = $(this);
        $.post(a.attr('href'), q, function(ret) {
            var tr = a.closest("tr");
            tr.replaceWith(ret);
            $.initializeTable();
        });
        return false;
    });

    $("body").on("click", 'a.delete', function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete it!",
            closeOnConfirm: false
        },
        function () {
            $.post("/Division/Delete/" + id, null, function (ret) {
                if (ret && ret.error)
                    swal("Error!", ret.error, "error");
                else {
                    swal({
                        title: "Deleted!",
                        type: "success"
                    },
                    function () {
                        window.location = "/Divisions/";
                    });
                }
            });
        });
    });

    $("#refresh").click(function(ev) {
        ev.preventDefault();
        $.getTable();
    });

    $('#TagProgramId').change(function () {
        $.getTable();
    });

    $('#ProgramId').change(function() {
        $.getTable();
    });

    $.getTable = function() {
        var f = $('#progform');
        var q = f.serialize();
        $.block();
        $.post("/Division/Results", q, function(ret) {
            $('#results').replaceWith(ret).ready(function() {
                $.initializeTable();
                $.unblock();
            });
        });
        return false;
    }

    $.initializeTable = function () {
        intializePopovers();

        $(".clickEdit").editable({
            mode: 'inline',
            type: 'text',
            url: "/Division/Edit/",
            params: function (params) {
                var data = {};
                data['id'] = params.pk;
                data['value'] = params.value;
                return data;
            }
        });
        $(".yesno").editable({
            mode: 'inline',
            type: 'select',
            source: [{value: 0, text: "no"}, {value: 1, text: "yes"}],
            url: "/Division/Edit/",
            params: function (params) {
                var data = {};
                data['id'] = params.pk;
                data['value'] = params.value;
                return data;
            }
        });
    }

    $.initializeTable();
});
