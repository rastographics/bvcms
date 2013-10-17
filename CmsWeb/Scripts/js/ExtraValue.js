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
                    switch ($("#ExtraValueType_Value").val()) {
                        case "Code":
                            $("#ExtraValueCodes").parent().parent().removeClass('hide');
                            break;
                        case "Bits":
                            $("#ExtraValueBitPrefix").parent().parent().removeClass('hide');
                            $("#ExtraValueCheckboxes").parent().parent().removeClass('hide');
                            break;
                        case "Codes":
                            $("#ExtraValueCodes").parent().parent().removeClass('hide');
                            break;
                    }
                };
                $("#ExtraValueType_Value").live("change", showHide);
            });
        $.InitFunctions.StandardExtraValueDialogCallback = function () {
            if ($("#StandardExtraValueError").length == 0) {
                $("#extravalue-dialog").modal("hide");
                var a = $a.closest("form").find("a.ajax-refresh");
                if(a.length > 0)
                    a.click();
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
                $("#AdhocExtraValueType_Value").click(showHide);
            });
        $.InitFunctions.AdhocDialogCallback = function () {
            if ($("#ExtraValueErrmr").length == 0) {
                $("#extravalue-dialog").modal("hide");
                var a = $a.closest("form").find("a.ajax.reload");
                if(a.length > 0)
                    a.click();
            }
        };
    });
    $.InitFunctions.AdhocExtraEditable = function () {
        $("a.click-Code").editable({ mode: 'inline' });
        $('a.click-Text').editable({ mode: 'inline' });
        $('a.click-Text2,a.click-Data').editable({ type: 'textarea', mode: 'inline' });
        $("a.click-Code-Select").editable({ type: "select", mode: 'inline' });
        $('a.click-Bits').editable({ type: "checklist", mode: 'inline' });
        $("a.click-Date").editable({ type: 'date', mode: 'inline', format: $.dtoptions.format });
        $("a.click-Bit").editable({ type: 'checklist', mode: 'inline', source: { 'True': 'True' }, emptytext: 'False' });
    };
});