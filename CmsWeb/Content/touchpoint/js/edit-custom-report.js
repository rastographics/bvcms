(function () {
    $("ul.sortable").sortable();
    var $form = $('#edit-custom-report-form');
    $form.validate();

    $('#ReportName').rules('add', {
        required: true,
        pattern: /^[A-Za-z0-9 ]+$/,
        messages: {
            required: "The report name is required.",
            pattern: "The report name can only contain alphanumeric characters. (a-z, 0-9)"
        }
    });

    var lastChecked = null;
    $('#run-report').click(function (e) {
        e.preventDefault();

        if ($form.valid()) {
            $.post($form.attr('action'), $form.serialize()).then(function () {
                $('#OriginalReportName').val($('#ReportName').val());
                var runLink = $('#run-report-link').attr('href').replace('___', encodeURIComponent($('#ReportName').val()));
                window.location = runLink;
            });
        }
    });

    $('input[type="checkbox"]').click(function (e) {
        if (e.shiftKey && lastChecked !== null) {
            var start = $('input[type="checkbox"]').index(this);
            var end = $('input[type="checkbox"]').index(lastChecked);
            $('input[type="checkbox"]').slice(Math.min(start, end), Math.max(start, end) + 1).prop("checked", true);
        }

        lastChecked = this;
    });
})();
