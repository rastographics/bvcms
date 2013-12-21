$(function () {
    $('#delete').live("click", function (ev) {
        ev.preventDefault();
        var href = $(this).attr("href");
        if (confirm('Are you sure you want to delete?')) {
            $.block("deleting bundle");
            $.post(href, null, function (ret) {
                if (!ret.startsWith("/")) {
                    $.block(ret);
                    $('.blockOverlay').attr('title', 'Click to unblock').click($.unblock);
                }
                else {
                    $.block("Bundle Deleted, click screen to return to bundles");
                    $('.blockOverlay').attr('title', 'Click to unblock').click(function () {
                        $.unblock();
                        window.location = ret;
                    });
                }
            });
        }
        return false;
    });

    $("a.displayedit").live('click', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        f.validate();
        if (f.valid()) {
            var q = f.serialize();
            $.post($(this).attr('href'), q, function(ret) {
                if (ret.message) {
                    $.block(ret.message);
                    $('.blockOverlay').attr('title', 'Click to unblock').click(function() {
                        $.unblock();
                        window.location = ret.location;
                    });
                } else
                    $(f).html(ret).ready(function() {
                        $.SetValidation();
                    });
            });
        }
        return false;
    });
    $.validator.setDefaults({
        highlight: function (input) {
            $(input).addClass("ui-state-highlight");
        },
        unhighlight: function (input) {
            $(input).removeClass("ui-state-highlight");
        }
    });
    $.SetValidation = function() {
        $("#bundleform").validate({
            rules: {
                "Bundle.ContributionDate": { date: true, required: true },
                "Bundle.DepositDate": { date: true },
                "Bundle.TotalCash": { number: true },
                "Bundle.TotalChecks": { number: true },
                "Bundle.TotalEnvelopes": { number: true },
            }
        });
        $("input.datepicker").datepicker();
        $("a.bt").button();
    };
    $.SetValidation();
});