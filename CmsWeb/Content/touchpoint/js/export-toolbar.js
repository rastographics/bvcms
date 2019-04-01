$(function () {

    if (!$.InitFunctions)
        $.InitFunctions = {};

    $.InitFunctions.TagAllCallBack = function (a) {
        $(".taguntag:visible").removeClass('btn-default').removeClass('btn-success');
        $(".taguntag:visible").addClass('btn-default');
        $(".taguntag:visible").html("<i class='fa fa-tag'></i> Remove");
    };

    $('body').on('click', '#singleemail', function (ev) {
        ev.preventDefault();
        var t = $(this);

        swal({
            title: "Are you sure?",
            text: "Warning, email replacement codes and <span> <br> special links will not work. <small><i><a href='https://docs.touchpointsoftware.com/BlueToolbar/toolbar.html#email-and-texting-envelope-icon'  target='blank'>Read more..</a></i></small></span>",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-success",
            confirmButtonText: "Yes, continue!",
            closeOnConfirm: false,
            html: true
        },
        function () {
            window.location = t[0].href;
        });

        return true;
    });

    $('body').on('click', '#UnTagAll', function (ev) {
        ev.preventDefault();
        var $a = $(this);
        $.block();
        $.post(this.href, null, function (ret) {
            $(".taguntag:visible").removeClass('btn-default').removeClass('btn-success');
            $(".taguntag:visible").addClass('btn-success');
            $(".taguntag:visible").html("<i class='fa fa-tag'></i> Add");
            $('[data-toggle="dropdown"]').parent().removeClass('open');
            $.unblock();
        });
        return true;
    });

    $('body').on('click', '#AddContact', function (ev) {
        ev.preventDefault();
        var url = this.href;

        swal({
            title: "Are you sure?",
            text: "This will add a contact for all listed people.",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-success",
            confirmButtonText: "Yes, add contact!",
            closeOnConfirm: false
        },
        function () {
            $.block();
            $.post(url, null, function (ret) {
                $.unblock();
                if (ret < 0)
                    swal("Error!", "Too many people to add to a contact (max 100).", "error");
                else if (ret == 0)
                    swal("Error!", "No results.", "error");
                else
                    window.location = ret;
            });
        });
        return true;
    });

    $('body').on('click', '#AddTasks', function (ev) {
        ev.preventDefault();
        var message = "This will add a task for all listed people.";
        if (window.location.pathname.indexOf("/Person") > -1)
            message = "This will add a task for this person.";
        var url = this.href;

        swal({
            title: "Are you sure?",
            text: message,
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-success",
            confirmButtonText: "Yes, add task!",
            closeOnConfirm: true
        },
        function () {
            $.block();
            $.post(url, null, function (ret) {
                $.unblock();
                if (ret > 100)
                    swal("Error!", "Too many people to add tasks for (max 100).", "error");
                else if (ret == 0)
                    swal("Error!", "No results.", "error");
                else
                    window.location = "/TaskSearch";
            });
        });
        return true;
    });

    $('button.dropdown-toggle').click(function (e) {
        $("li.hideAlt").hide();
    });

    $(document).keydown(function (e) {
        if (e.keyCode == 17 && $("ul.dropdown-menu").is(':visible')) {
            $("li.hideAlt").not(".hidy").show();
        }
    });

    $('#delete-report').click(function (e) {
        e.preventDefault();

        var a = $(this);
        var rowToDelete = a.parent().parent();
        var reportToDelete = a.data("report");
        var deleteUrl = a.data('action-url');

        swal({
            title: "Are you sure you want to delete this report? (" + reportToDelete + ")",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete it!",
            closeOnConfirm: false
        },
        function () {
            $.post(deleteUrl).done(function (data) {
                if (data === 'success') {
                    swal({
                        title: "Deleted!",
                        type: "success"
                    },
                        function () {
                            rowToDelete.remove();
                        });
                }
            });
        });
    });
});
