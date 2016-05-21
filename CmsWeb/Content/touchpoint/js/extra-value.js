$(function () {
    //-------------------------------------------
    // Standard ---------------------------------
    //-------------------------------------------
    $('body').on('click', 'a.extravalue', function (ev) {
        ev.preventDefault();
        var $a = $(this);
        $("<form id='extravalue-dialog' class='modal-form validate ajax' />")
            .load($(this).attr("href"), {}, function () {
                var form = $(this);
                $('#empty-dialog').html(form);
                $('#empty-dialog').modal("show");

                $.AttachFormElements();
                $(this).validate({
                    highlight: function (element) {
                        $(element).closest(".form-group").addClass("error");
                    },
                    unhighlight: function (element) {
                        $(element).closest(".form-group").removeClass("error");
                    }
                });
                $('#empty-dialog').on('hidden', function () {
                    $('#empty-dialog').remove();
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
                $('body').on('change', '#ExtraValueType_Value', showHide);
            });
        $.InitFunctions.StandardExtraValueDialogCallback = function () {
            if ($("#StandardExtraValueError").length === 0) {
                $("#empty-dialog").modal("hide");
                var a = $a.closest("form").find("a.ajax-refresh");
                if (a.length > 0)
                    a.click();
            }
        };
        $.InitFunctions.EditStandardExtraValueDialogCallback = function () {
            if ($("#EditStandardExtraValueError").length == 0) {
                var a = $a.closest("form").find("a.ajax-refresh");
                if (a.length > 0)
                    a.click();
            }
        };
        $('body').on('click', '#CloseListStandard', function (e) {
            e.preventDefault();
            $.InitFunctions.StandardExtraValueDialogCallback();
        });
    });

    $('body').on('click', 'a.delete-extra-value', function (ev) {
        ev.preventDefault();

        var title = $(this).attr('title');
        var url = $(this).attr('href');
        var rowId = $(this).data("rowid");
        bootbox.dialog({
            title: title,
            message: '<div class="checkbox"><label class="control-label"><input type="checkbox" name="removedata" id="removedata" /> Remove data too?</label></div><span class="help-block">Checking this box will remove all associated data too.</span>',
            buttons: {
                cancel: {
                    label: "Cancel",
                    className: "btn-default"
                },
                remove: {
                    label: "Delete",
                    className: "btn-danger",
                    callback: function () {
                        var removeData = $('#removedata').is(':checked');
                        $.post(url + '&removedata=' + removeData, null, function () {
                            $(rowId).remove();
                        });
                    }
                }
            }
        });
    });


    //-------------------------------------------
    // AdHoc ------------------------------------
    //-------------------------------------------
    $('body').on('click', 'a.adhoc-extravalue', function (ev) {
        ev.preventDefault();
        var $a = $(this);
        $("<form id='extravalue-dialog' class='modal-form validate ajax' />")
            .load($a.attr("href"), {}, function () {
                var form = $(this);
                $('#empty-dialog').html(form);
                $('#empty-dialog').modal("show");

                $.AttachFormElements();
                form.validate({
                    highlight: function (element) {
                        $(element).closest(".form-group").addClass("error");
                    },
                    unhighlight: function (element) {
                        $(element).closest(".form-group").removeClass("error");
                    }
                });
                $('#empty-dialog').on('hidden', function () {
                    $('#empty-dialog').remove();
                });
            });
        $.InitFunctions.AdhocDialogCallback = function () {
            if ($("#ExtraValueError").length === 0) {
                $("#empty-dialog").modal("hide");
                var a = $a.closest("form").find("a.ajax.reload");
                if (a.length > 0)
                    a.click();
            }
        };
    });
    var showHideExtraValueTypes = function () {
        $("#ExtraValueTextBox").parent().parent().addClass('hide');
        $("#ExtraValueTextArea").parent().parent().addClass('hide');
        $("#ExtraValueCheckbox").parent().parent().parent().addClass('hide');
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
                $("#ExtraValueCheckbox").parent().parent().parent().removeClass('hide');
                break;
            case "Date":
                $("#ExtraValueDate").parent().parent().parent().removeClass('hide');
                break;
            case "Int":
                $("#ExtraValueInteger").parent().parent().removeClass('hide');
                break;
        }
    };
    $('body').on('change', '#AdhocExtraValueType_Value', showHideExtraValueTypes);

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
        $("a.click-Date").editable({ type: 'date', mode: 'popup', format: 'mm/dd/yyyy', viewformat: 'mm/dd/yyyy', datepicker: { weekStart: 0 }, });
        $("a.click-Bit").editable({ type: 'checklist', mode: 'inline', source: { 'True': 'True' }, emptytext: 'False' });
    };
});
