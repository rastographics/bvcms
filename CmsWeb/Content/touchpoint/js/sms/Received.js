$(function () {
    $('#receivedform')
        .on('click', '#clearreceived',
        function (ev) {
            ev.preventDefault();
            $('#RecdFilterStart').val(null);
            $('#RecdFilterEnd').val(null);
            $('#RecdFilterMessage').val(null);
            $('#RecdFilterGroupId').val(0);
            $('#RecdFilterSender').val(null);
        }).on('click', "a.addtotag",
        function (ev) {
            ev.preventDefault();
            var q = $('#receivedform').serialize();
            $.post("/SmsMessages/TagReceivedDialog", q,
                function (ret) {
                    $("#countpeople").html(ret);
                });
            $('#tagmessages').addClass("tabreceived");
            $('#tagmessages-modal').modal();
        }).on('keypress', 'input[type=text]',
        function (e) {
            var key = e.which;
            if (key === 13)  // the enter key code
            {
                $('#searchreceived').click();
                return false;
            }
            return true;
        }).on('click', 'a.showdetails',
        function (ev) {
            ev.preventDefault();
            $.post(this.href,
                function (ret) {
                    $('#receivedresults').replaceWith(ret);
                });
        }).on('click', 'button.backtoreceived',
        function (ev) {
            ev.preventDefault();
            var f = $(this).closest('form');
            var q = f.serialize();
            $.post("/SmsMessages/ReceivedResults",
                q,
                function (ret) {
                    $('#receivedresults').replaceWith(ret);
                });
        }).on('click', 'a.replyto',
        function (ev) {
            ev.preventDefault();
            var mid = $(this).closest("tr").attr("id");
            $.post("/SmsMessages/ReplyingTo/" + mid,
                function (ret) {
                    var o = $.parseJSON(ret);
                    $("#ReceivedId").val(o.ReceivedId);
                    $("#FromGroup").html(o.FromGroup);
                    $("#ToPerson").html(o.ToPerson);
                    $("#ToMessage").html(o.ToMessage);
                    $("#Response").html(o.Response);
                    $('#replyto-modal').modal();
                });
        }).on("click", "#gotoReply",
        function (ev) {
            ev.preventDefault();
            var sentid = $(this).data("sentid");
            FilterReplySent('#' + sentid);
        });

    $('#SendReplyForm').on('click', '#sendmessage',
        function (ev) {
            ev.preventDefault();
            var q = $("#SendReplyForm").serialize();
            $("#replyto-modal").modal("hide");
            $.post("/SmsMessages/SendReply", q,
                function (ret) {
                    $("#replymessage").val("");
                    var id = $("#ReceivedId").val();
                    var tr = $(`tr[id=${id}]`);
                    tr.addClass('repliedto');
                    snackbar(ret);
                });
        });

    $('#tagmessages-modal').on('click',
        'a.btn.tabreceived',
        function (ev) {
            ev.preventDefault();
            var q = $("#receivedform").serialize();
            q = q + "&" + $("#tagAllForm").serialize();
            $('#tagmessages').removeClass("tabreceived");
            $('#tagmessages-modal').modal("hide");
            $.post("/SmsMessages/TagReceived", q,
                function (ret) {
                    snackbar(ret);
                });
        });
});
function FilterRepliedTo(msgval) {
    $('#RecdFilterStart').val(null);
    $('#RecdFilterEnd').val(null);
    $('#RecdFilterGroupId').val(0);
    $('#RecdFilterSender').val(null);
    $('#RecdFilterMessage').val(msgval);
    $('#showReceivedTab').click();
    $('#searchreceived').click();
}

