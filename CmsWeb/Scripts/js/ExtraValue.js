$(function () {
    $("a.addextravalue").live("click", function (ev) {
        ev.preventDefault();
        $("<form id='addextravalue-dialog' class='modal fade hide validate ajax form-horizontal' data-width='600' data-keyboard='false' data-backdrop='static' />")
            .load($(this).attr("href"), {}, function () {
                $(this).modal("show");
                $.AttachFormElements();
                $(this).validate({
                    highlight: function (element) {
                        $(element).closest(".control-group").addClass("error");
                    },
                    unhighlight: function (element) {
                        $(element).closest(".control-group").removeClass("error");
                    }
                });
                $(this).on('hidden', function () {
                    $(this).remove();
                });
            });
    });
});