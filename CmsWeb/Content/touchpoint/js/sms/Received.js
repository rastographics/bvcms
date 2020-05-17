$(document).ready(function () {
    $('#received').on('click',
        '#clearreceived',
        function (ev) {
            ev.preventDefault();
            $('#RecdFilterStart').val(null);
            $('#RecdFilterEnd').val(null);
            $('#RecdFilterMessage').val(null);
            $('#RecdFilterGroupId').val(0);
            $('#RecdFilterSender').val(null);
        });

    $('#received').on("click",
        'a.sortable',
        function () {
            var newsort = $(this).text();
            var sort = $("#Sort");
            var dir = $("#Direction");

            if ($(sort).val() == newsort && $(dir).val() == 'asc')
                $(dir).val('desc');
            else
                $(dir).val('asc');

            $(sort).val(newsort);
            $("#form").submit();
            return false;
        });
    $('#received').on('click',
        'a.showdetails',
        function (ev) {
            ev.preventDefault();
            $.post(this.href,
                function (ret) {
                    $('#receivedresults').html(ret);
                });
        });
    $('#received').on('click',
        'button.backtoreceived',
        function (ev) {
            ev.preventDefault();
            var f = $(this).closest('form');
            var q = f.serialize();
            $.post("/SmsMessages/ReceivedResults",
                q,
                function (ret) {
                    $('#receivedresults').html(ret);
                });
        });

    $('#received').on('click',
        'a.replyto',
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
        });
    $('#sendmessage').on('click',
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

    $('#receivedform').on('click',
        "a.addtotag",
        function (ev) {
            ev.preventDefault();
            var q = $('#receivedform').serialize();
            $.post("/SmsMessages/TagReceivedDialog", q,
                function (ret) {
                    $("#countpeople").html(ret);
                });
            $('#tagmessages').addClass("tabreceived");
            $('#tagmessages-modal').modal();
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
