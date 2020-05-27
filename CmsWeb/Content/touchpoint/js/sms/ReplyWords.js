$(function () {
    $("#replywords").on("change",
        "#GroupId",
        (function (ev) {
            ev.preventDefault();
            var f = $(this).closest('form');
            var q = f.serialize();
            $.post("/SmsMessages/ReplyWordsGroupChanged",
                q,
                function (ret) {
                    $("#ReplyWordsList").html(ret);
                });
        }));
    $("#replywords").on("change",
        ".Action",
        function (ev) {
            ev.preventDefault();
            var f = $(this).closest('form');
            var q = f.serialize();
            $.post("/SmsMessages/ReplyWordActionChanged",
                q,
                function (ret) {
                    $("#ReplyWordsList").html(ret);
                });
        });
    $('#replywords').on("click",
        ".AddReplyWord",
        function (ev) {
            ev.preventDefault();
            var f = $(this).closest('form');
            var q = f.serialize();
            $.post("/SmsMessages/AddReplyWord",
                q,
                function (ret) {
                    $("#ReplyWordsList").html(ret);
                });
        });
    $('#replywords').on("click",
        ".deleteaction",
        function (ev) {
            ev.preventDefault();
            var tr = $(this).closest('div.row');
            tr.remove();
        });
    $("#replywords").on("click",
        ".SaveReplyWord",
        function (ev) {
            ev.preventDefault();
            var f = $(this).closest('form');
            var q = f.serialize();
            $.post("/SmsMessages/SaveReplyWords",
                q,
                function (ret) {
                    $("#replywords").html(ret);
                    snackbar("Saved");
                });
        });
});
