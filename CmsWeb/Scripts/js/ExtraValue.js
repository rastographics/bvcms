$(function () {
    $("a.extravalue").live("click", function (ev) {
        ev.preventDefault();
        var $a = $(this);
        $("<form id='extravalue-dialog' class='modal fade hide validate ajax form-horizontal' data-width='600' />")
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
                var showHide = function () {
                    $("#ExtraValueCodes").prop('disabled', true);
                    $("#ExtraValueBitPrefix").prop('disabled', true);
                    $("#ExtraValueBitPrefix").parent().parent().addClass('disabled');
                    $("#ExtraValueCodes").parent().parent().addClass('disabled');
                    switch ($("#ExtraValueType_Value").val()) {
                        case "Code": // code
                            $("#ExtraValueCodes").prop('disabled', false);
                            $("#ExtraValueCodes").parent().parent().removeClass('disabled');
                            break;
                        case "Bits": // bits
                            $("#ExtraValueBitPrefix").prop('disabled', false);
                            $("#ExtraValueCodes").prop('disabled', false);
                            $("#ExtraValueBitPrefix").parent().parent().removeClass('disabled');
                            $("#ExtraValueCodes").parent().parent().removeClass('disabled');
                            break;
                    }
                };
                $(this).on("change", "#ExtraValueType_Value", showHide);
            });
        $.InitFunctions.StandardExtraValueDialogCallback = function () {
            if ($("#StandardExtraValueError").length == 0) {
                $("#extravalue-dialog").modal("hide");
                $a.closest("form.ajax").find("a.ajax-refresh").click();
            }
        };
        $.InitFunctions.DeleteStandardCallback = function (a) {
            $(a.data("rowid")).remove();
        };
        $("#CloseListStandard").live("click", function(e) {
            e.preventDefault();
            $.InitFunctions.StandardExtraValueDialogCallback();
        });
    });
    $.InitFunctions.ExtraEditable = function () {
        $("a.click-Code").editable({ mode: 'inline' });
        $('a.click-Text').editable({ mode: 'inline' });
        $('a.click-Text2,a.click-Data').editable({ type: 'textarea', mode: 'inline' });
        $("a.click-Code-Select").editable({ type: "select", mode: 'inline' });
        $('a.click-Bits').editable({ type: "checklist", mode: 'inline' });
        $("a.click-Date").editable({ type: 'date', mode: 'inline', format: $.dtoptions.format });
        $("a.click-Bit").editable({ type: 'checklist', mode: 'inline', source: { 'True': 'True' }, emptytext: 'False' });
    };
});