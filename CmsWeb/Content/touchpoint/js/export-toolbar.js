$(function () {

    $('body').on('click', 'a.dialog-options', function (ev) {
        ev.preventDefault();
        var $a = $(this);
        $("<div id='dialog-options' />").load($a.data("target"), function () {
            var div = $(this);
            var dialog = div.find("div.modal-dialog");
            var f = div.find("form");
            f.attr("action", $a[0].href);

            if ($a[0].title)
                div.find("h3.modal-title").text($a[0].title);
            
            $('#empty-dialog').html(dialog);
            $('#empty-dialog').modal("show");

            dialog.on('hidden', function () {
                div.remove();
                dialog.remove();
            });
            f.validate({
                submitHandler: function (form) {
                    if (form.method.toUpperCase() === 'GET') {
                        form.submit();
                    }
                    else {
                        var q = f.serialize();
                        $.post(form.action, q, function (ret) {
                            if ($a.data("callback")) {
                                $.InitFunctions[$a.data("callback")]($a);
                            }
                        });
                    }
                    $('#empty-dialog').modal("hide");
                },
                highlight: function (element) {
                    $(element).closest(".form-group").addClass("has-error");
                },
                unhighlight: function (element) {
                    $(element).closest(".form-group").removeClass("has-error");
                }
            });
        });
        return true;
    });

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
        bootbox.confirm(t.data("confirm"), function (ret) {
            if(ret)
                window.location = t[0].href;
        });
        return false;
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
        bootbox.confirm("Are you sure you want to add a contact for all these people?", function (result) {
            if (result === true) {
                $.block();
                $.post(url, null, function (ret) {
                    $.unblock();
                    if (ret < 0)
                        $.growlUI("error", "too many people to add to a contact (max 100)");
                    else if (ret == 0)
                        $.growlUI("error", "no results");
                    else
                        window.location = ret;
                });
            }
        });
        return false;
    });

    $('body').on('click', '#AddTasks', function (ev) {
        ev.preventDefault();
        var message = "Are you sure you want to add a task for all these people?";
        if (window.location.pathname.contains("/Person"))
            message = "Are you sure you want to add a task for this person?";
        var url = this.href;
        bootbox.confirm(message, function (result) {
            if (result === true) {
                $.block();
                $.post(url, null, function (ret) {
                    $.unblock();
                    if (ret > 100)
                        $.growlUI("error", "too many people to add tasks for (max 100)");
                    else if (ret == 0)
                        $.growlUI("error", "no results");
                    else
                        window.location = "/Task";
                });
            }
        });
        return false;
    });

});