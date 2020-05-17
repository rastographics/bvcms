$(document).ready(function () {
    $('#sent').on('click',
        '#clearsent',
        function (ev) {
            ev.preventDefault();
            $('#SentFilterStart').val(null);
            $('#SentFilterEnd').val(null);
            $('#SentFilterTitle').val(null);
            $('#SentFilterGroupId').val(0);
            $('#SentFilterPeopleId').val(null);
        });

    $('#sent').on('change',
        '#SentFilterGroupId',
        function (ev) {
            ev.preventDefault();
            var f = $(this).closest('form');
            var q = f.serialize();
            $.post("/SmsMessages/SentFilterGroupIdChanged",
                q,
                function (ret) {
                    $("#SentFilterGroupMembers").html(ret);
                });
        });

    $('#sent').on("click",
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
    $('#sent').on('click',
        'a.showdetails',
        function (ev) {
            ev.preventDefault();
            var url = this.href;
            $.post(this.href,
                function (ret) {
                    $('#sentresults').html(ret);
                });
        });
    $('#sent').on('click',
        'button.backtosent',
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

    $('#sentform').on('click', "a.addtotag",
        function (ev) {
            ev.preventDefault();
            var q = $('#sentform').serialize();
            $.post("/SmsMessages/TagSentDialog", q,
                function (ret) {
                    $("#countpeople").html(ret);
                });
            $('#tagmessages').addClass("tabsent");
            $('#tagmessages-modal').modal();
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
