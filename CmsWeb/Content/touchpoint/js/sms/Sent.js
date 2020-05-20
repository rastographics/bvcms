$(document).ready(function () {
    $('#sentform').on('click', '#clearsent',
        function (ev) {
            ev.preventDefault();
            $('#SentFilterStart').val(null);
            $('#SentFilterEnd').val(null);
            $('#SentFilterTitle').val(null);
            $('#SentFilterGroupId').val(0);
            $('#SentFilterPeopleId').val(null);
        }).on('change', '#SentFilterGroupId',
        function (ev) {
            ev.preventDefault();
            var f = $(this).closest('form');
            var q = f.serialize();
            $.post("/SmsMessages/SentFilterGroupIdChanged",
                q,
                function (ret) {
                    $("#SentFilterGroupMembers").html(ret);
                });
        }).on('click', "a.addtotag",
        function (ev) {
            ev.preventDefault();
            var q = $('#sentform').serialize();
            $.post("/SmsMessages/TagSentDialog", q,
                function (ret) {
                    $("#countpeople").html(ret);
                });
            $('#tagmessages').addClass("tabsent");
            $('#tagmessages-modal').modal();
        }).on('keypress', '', 'input[type=text]',
        function (e) {
            var key = e.which;
            if (key == 13)  // the enter key code
            {
                $('#searchsent').click();
                return false;
            }
        });

    $('#sentresults').on('click', 'a.showdetails',
        function (ev) {
            ev.preventDefault();
            $.post(this.href,
                function (ret) {
                    $('#sentresults').html(ret);
                });
        }).on('click', 'button.backtosent',
        function (ev) {
            ev.preventDefault();
            var f = $(this).closest('form');
            var q = f.serialize();
            $.post("/SmsMessages/SentResults",
                q,
                function (ret) {
                    $('#sentresults').html(ret);
                });
        });

    $('#tagmessages-modal').on('click',
        'a.btn.tabsent',
        function (ev) {
            ev.preventDefault();
            var q = $("#sentform").serialize();
            q = q + "&" + $("#tagAllForm").serialize();
            $('#tagmessages').removeClass("tabsent");
            $('#tagmessages-modal').modal("hide");
            $.post("/SmsMessages/TagSent", q,
                function (ret) {
                    snackbar(ret);
                });
        });
});
