$(function () {
    // begin initialize ReplyWords tab when document loaded
    if (window.location.hash) {
        var hash = window.location.hash;
        window.location.hash = "";
        FilterRepliedTo(hash);
    }
    $("#replywords #GroupId").click();
    // end initialize ReplyWords

    $("#replywords").on("change",
        "#GroupId",
        (function (ev) {
            ev.preventDefault();
            var f = $(this).closest('form');
            var q = f.serialize();
            $.post("/SmsMessages/ReplyWordsGroupChanged",
                q,
                function (ret) {
                    $("#replywordslist").replaceWith(ret);
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
                    $("#replywordslist").replaceWith(ret);
                });
        });
    $("#replywords").on("change",
        "input.replyword",
        function (ev) {
            ev.preventDefault();
            var word = $(this).val();
            var filterwords = "stop|stopall|unsubscribe|cancel|end|quit|start|yes|unstop|help|info";
            var regex = new RegExp( filterwords, "i");
            var isfound = regex.test( word );
            if (isfound)
                error_swal("Reply Word", word + " replyword not allowed");
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
                    $("#replywordslist").replaceWith(ret);
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
