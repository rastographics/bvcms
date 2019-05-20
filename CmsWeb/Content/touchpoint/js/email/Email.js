$(function () {
    $('#Recipients').select2();
    $('#Recipients').select2("readonly", true);

    $(".Send").click(function () {
        $.block();
        $('#Body').text(CKEDITOR.instances["Body"].getData());
        var q = $(this).closest('form');
        if ($(this).attr('data-prompt') === 'True') {
            var count = $("#Count").val();
            swal({
                title: "Are you sure?",
                text: "You're about to send an email to " + count + " people.",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-confirm",
                confirmButtonText: "Yes, send it!",
                showLoaderOnConfirm: true,
                closeOnConfirm: false
            }, function () { sendEmail(q); });
        } else {
            sendEmail(q);
        }        
    });

    function sendEmail(q) {
        $.post('/Email/QueueEmails', q.serialize(), function (ret) {
            if (ret && ret.error) {
                $.unblock();
                swal("Error!", ret.error, "error");
            } else {
                if (ret === "timeout") {
                    swal("Session Timeout!", 'Your session timed out. Please copy your email content and start over.', "error");
                    return;
                }
                var taskid = ret.id;
                if (taskid === 0) {
                    $.unblock();
                    swal("Success!", ret.content, "success");
                    $('button.Send', q).prop('disabled', true);
                } else {
                    $("#send-actions").remove();
                    var intervalid = window.setInterval(function () {
                        $.post('/Email/TaskProgress/' + taskid, null, function (ret) {
                            $.unblock();
                            if (ret && ret.error) {
                                swal("Error!", ret.error, "error");
                            } else {
                                if (ret.title == 'Email has completed.') {
                                    swal(ret.title, ret.message, "success");
                                    window.clearInterval(intervalid);
                                    $('button.Send', q).prop('disabled', true);
                                } else {
                                    swal({
                                        title: ret.title,
                                        text: ret.message,
                                        imageUrl: '/Content/touchpoint/img/spinner.gif'
                                    });
                                }
                            }
                        });
                    }, 3000);
                }
            }
        });
    }

    $(".TestSend").click(function () {
        $.block();
        $('#Body').text(CKEDITOR.instances["Body"].getData());
        var q = $(this).closest('form').serialize();

        $.post('/Email/TestEmail', q, function (ret) {
            $.unblock();
            if (ret && ret.error) {
                swal("Error!", ret.error, "error");
            } else {
                if (ret == "timeout") {
                    swal("Session Timeout!", 'Your session timed out. Please copy your email content and start over.', "error");
                    return;
                }
                swal("Success!", ret, "success");
            }
        });
    });

    $('body').on('click', '#CreateVoteTag', function (ev) {
        ev.preventDefault();
        CKEDITOR.instances["votetagcontent"].updateElement();
        var q = $(this).closest('form').serialize();
        $.post('/Email/CreateVoteTag', q, function (ret) {
            CKEDITOR.instances["votetagcontent"].setData(ret, function () {
                CKEDITOR.instances["votetagcontent"].setMode("source");
            });
        });
    });
});
