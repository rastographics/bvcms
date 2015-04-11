$(function () {
    //-------------------------------------------
    // Standard ---------------------------------
    //-------------------------------------------
    $("a.extravalue").live("click", function (ev) {
        ev.preventDefault();
        var $a = $(this);
        $("<div />")
            .load($(this).attr("href"), {}, function () {
                var d = $(this);
                var f = d.find("form");
                f.modal("show");
                $.AttachFormElements();
                $(this).validate({
                    highlight: function (element) {
                        $(element).closest(".control-group").addClass("error");
                    },
                    unhighlight: function (element) {
                        $(element).closest(".control-group").removeClass("error");
                    }
                });
                f.on('hidden', function () {
                    d.remove();
                    f.remove();
                });
                var showHide = function () {
                    $("#ExtraValueBitPrefix").parent().parent().addClass('hide');
                    $("#ExtraValueCheckboxes").parent().parent().addClass('hide');
                    $("#ExtraValueCodes").parent().parent().addClass('hide');
                    $("#ExtraValueLink").parent().parent().addClass('hide');
                    switch ($("#ExtraValueType_Value").val()) {
                        case "Code":
                            $("#ExtraValueCodes").parent().parent().removeClass('hide');
                            break;
                        case "Bits":
                            $("#ExtraValueBitPrefix").parent().parent().removeClass('hide');
                            $("#ExtraValueCheckboxes").parent().parent().removeClass('hide');
                            break;
                        case "Link":
                            $("#ExtraValueLink").parent().parent().removeClass('hide');
                            break;
                        case "Codes":
                            $("#ExtraValueCodes").parent().parent().removeClass('hide');
                            break;
                    }
                };
                $("#ExtraValueType_Value").live("change", showHide);
            });
        $.InitFunctions.StandardExtraValueDialogCallback = function () {
            if ($("#StandardExtraValueError").length === 0) {
                $("#extravalue-dialog").modal("hide");
                var a = $a.closest("form").find("a.ajax-refresh");
                if (a.length > 0)
                    a.click();
            }
        };
        $.InitFunctions.EditStandardExtraValueDialogCallback = function () {
            if ($("#EditStandardExtraValueError").length == 0) {
                $("#editextravalue-dialog").modal("hide");
                var a = $a.closest("form").find("a.ajax-refresh");
                if (a.length > 0)
                    a.click();
            }
        };
        $.InitFunctions.DeleteStandardCallback = function (a) {
            $(a.data("rowid")).remove();
            $("#extravalue-dialog").modal("hide");
            var a = $a.closest("form").find("a.ajax-refresh");
            if (a.length > 0)
                a.click();
        };
        $("#CloseListStandard").live("click", function (e) {
            e.preventDefault();
            $.InitFunctions.StandardExtraValueDialogCallback();
        });
    });

    //-------------------------------------------
    // AdHoc ------------------------------------
    //-------------------------------------------
    $("a.adhoc-extravalue").live("click", function (ev) {
        ev.preventDefault();
        var $a = $(this);
        $("<div />")
            .load($a.attr("href"), {}, function () {
                var d = $(this);
                var f = d.find("form");
                f.modal("show");
                $.AttachFormElements();
                f.validate({
                    highlight: function (element) {
                        $(element).closest(".control-group").addClass("error");
                    },
                    unhighlight: function (element) {
                        $(element).closest(".control-group").removeClass("error");
                    }
                });
                f.on('hidden', function () {
                    d.remove();
                    f.remove();
                });
                var showHide = function () {
                    $("#ExtraValueTextBox").parent().parent().addClass('hide');
                    $("#ExtraValueTextArea").parent().parent().addClass('hide');
                    $("#ExtraValueCheckbox").parent().parent().addClass('hide');
                    $("#ExtraValueDate").parent().parent().parent().addClass('hide');
                    $("#ExtraValueInteger").parent().parent().addClass('hide');

                    switch ($("#AdhocExtraValueType_Value").val()) {
                        case "Code":
                            $("#ExtraValueTextBox").parent().parent().removeClass('hide');
                            break;
                        case "Text":
                            $("#ExtraValueTextArea").parent().parent().removeClass('hide');
                            break;
                        case "Bit":
                            $("#ExtraValueCheckbox").parent().parent().removeClass('hide');
                            break;
                        case "Date":
                            $("#ExtraValueDate").parent().parent().parent().removeClass('hide');
                            break;
                        case "Int":
                            $("#ExtraValueInteger").parent().parent().removeClass('hide');
                            break;
                    }
                };
                $("#AdhocExtraValueType_Value").live("change", showHide);
            });
        $.InitFunctions.AdhocDialogCallback = function () {
            if ($("#ExtraValueError").length == 0) {
                $("#extravalue-dialog").modal("hide");
                var a = $a.closest("form").find("a.ajax.reload");
                if (a.length > 0)
                    a.click();
            }
        };
    });
    $.InitFunctions.ExtraEditable = function () {
        $.fn.editabletypes.abstractinput.prototype.value2input = function (value) {
            this.$input.val((value || "").toString());
        };
        $("a.click-Code").editable({ mode: 'inline' });
        $('a.click-Text').editable({ mode: 'inline' });
        $('a.click-Int').editable({ mode: 'inline' });
        $('a.click-Text2,a.click-Data').editable({
            type: 'textarea',
            mode: 'inline',
            inputclass: 'width100',
            showbuttons: 'bottom'
        }).on('shown', function (e, editable) {
            editable.input.$input.closest("span.editable-inline").css("width", "100%");
            editable.input.$input.closest("div.editable-input").css("width", "100%");
        });
        $("a.click-Code-Select").editable({ type: "select", mode: 'inline' });
        $('a.click-Bits').editable({ type: "checklist", mode: 'inline' });
        $("a.click-Date").editable({ type: 'date', mode: 'inline', format: $.dtoptions.format });
        $("a.click-Bit").editable({ type: 'checklist', mode: 'inline', source: { 'True': 'True' }, emptytext: 'False' });
    };
});