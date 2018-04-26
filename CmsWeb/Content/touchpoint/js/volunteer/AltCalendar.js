$(function () {
    $('#org-main-section').on('show.bs.collapse', function () {
        toggleIcons($('#org-main-collapse i'), true);
    });

    $('#org-main-section').on('hide.bs.collapse', function () {
        toggleIcons($('#org-main-collapse i'), false);
    });

    function toggleIcons(ele, expand) {
        if (expand) {
            $(ele).removeClass('fa-chevron-circle-right').addClass('fa-chevron-circle-up');
        } else {
            $(ele).removeClass('fa-chevron-circle-up').addClass('fa-chevron-circle-right');
        }
    }

    var xs = $('.device-xs').is(':visible');
    if (xs) {
        $('#org-main-section').collapse('hide');
    }


    $.InitFunctions.popovers = function () {
        $('[data-toggle="popover"]').popover({ html: true });
        $('[data-toggle="popover"]').click(function (ev) {
            ev.preventDefault();
            $('[data-toggle="popover"]').not(this).popover('hide'); 
            $(this).popover('toggle');
        });
    };

    $(document).click(function (e) {
        if ($(e.target).parent().find('[data-toggle="popover"]').length > 0) {
            $('[data-toggle="popover"]').popover('hide');
        }
    });

    $.InitFunctions.popovers();

 
    $('body').on('click', 'a.membertype', function (ev) {
        ev.preventDefault();
        var parent = $(ev.currentTarget).parent();
        if (parent.parent().find('[data-toggle="popover"]').length > 0) {
            $('[data-toggle="popover"]').popover('hide');
        }
        var $a = $(this);
        $('<form class="modal-form ajax" />').load(this.href, {}, function () {
            var f = $(this);
            $('#empty-dialog').html(f);
            $('#empty-dialog').modal('show');
        });

        $('#empty-dialog').on('hidden', function () {
            f.remove();
            $.RebindMemberGrids();
        });
    });

    $(document).on('click', '.removeVolunteer', function (e) {
        e.preventDefault();
        $.block();
        var list = [];
        if (typeof $(this).attr('pid') != 'undefined') {
            list.push({
                source: $(this).attr("source"),
                pid: $(this).attr("pid"),
                mid: $(this).attr("mid")
            });
        }

        if (list.length === 0)
            return;
        var $info = {
            id: $('#OrgId').val(),
            sg1: $('#sg1').val(),
            sg2: $('#sg2').val(),
            target: $(this).attr('target'),
            week: '0',
            time: '1/1/1000 12:00:00 AM',
            SortByWeek: $('#SortByWeek').val(),
            list: list
        };
        $.ajax({
            url: '/Volunteers/ManageArea/',
            data: JSON.stringify($info),
            success: function (ret) {
                $.unblock();
                window.location.reload(true);
            },
            error: function (ret) {
                swal('Error!', ret, 'error');
            },
            type: 'POST',
            contentType: 'application/json, charset=utf-8',
            dataType: 'html'
        });
    });


});
