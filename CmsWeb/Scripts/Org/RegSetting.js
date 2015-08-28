$(function () {

    $.InitFunctions.SettingFormsInit = function (f) {
        $('a.notifylist').SearchUsers({
            UpdateShared: function (topid, topid0, ele) {
                $.post("/Org/UpdateNotifyIds", {
                    id: $("#OrganizationId").val(),
                    topid: topid,
                    field: ele.data("field")
                }, function (ret) {
                    ele.html(ret);
                });
            }
        });
    };

    var xsDevice = $('.device-xs').is(':visible');
    var smDevice = $('.device-sm').is(':visible');

    $('body').on('click', 'a.editor', function (ev) {
        if (!$(this).attr("href"))
            return false;
        var name = $(this).attr("tb");
        ev.preventDefault();

        if (!xsDevice && !smDevice) {
            if (CKEDITOR.instances['editor'])
                CKEDITOR.instances['editor'].destroy();

            CKEDITOR.env.isCompatible = true;
            CKEDITOR.plugins.addExternal('specialLink', '/content/touchpoint/lib/ckeditor/plugins/specialLink/', 'plugin.js');

            CKEDITOR.replace('editor', {
                height: 200,
                allowedContent: true,
                customConfig: '/scripts/js/ckeditorconfig.js',
                extraPlugins: 'specialLink'
            });
        }
        if (xsDevice || smDevice) {
            $('#editor').val($("#" + name).val());
        } else {
            CKEDITOR.instances['editor'].setData($("#" + name).val());
        }

        $('#editor-modal').modal('show');

        $("#save-edit").off("click").on("click", function (ev) {
            ev.preventDefault();

            var v;
            if (xsDevice || smDevice) {
                v = $('#editor').val();
            } else {
                v = CKEDITOR.instances['editor'].getData();
            }

            $("#" + name).val(v);
            $("#" + name + "_ro").html(v);

            if (xsDevice || smDevice) {
                $('#editor').val('');
            } else {
                CKEDITOR.instances['editor'].setData('');
            }

            $('#editor-modal').modal('hide');
            return false;
        });
        return false;
    });

});

CKEDITOR.on('dialogDefinition', function (ev) {
    var dialogName = ev.data.name;
    var dialogDefinition = ev.data.definition;
    if (dialogName == 'link') {
        var advancedTab = dialogDefinition.getContents('advanced');
	    advancedTab.label = "SpecialLinks";
        advancedTab.remove('advCSSClasses');
        advancedTab.remove('advCharset');
        advancedTab.remove('advContentType');
        advancedTab.remove('advStyles');
        advancedTab.remove('advAccessKey');
        advancedTab.remove('advName');
        advancedTab.remove('advId');
        advancedTab.remove('advTabIndex');

        var relField = advancedTab.get('advRel');
        relField.label = "Sub-Group";
        var titleField = advancedTab.get('advTitle');
        titleField.label = "Message";
        var idField = advancedTab.get('advLangCode');
        idField.label = "OrgId/MeetingId";
        var langdirField = advancedTab.get('advLangDir');
        langdirField.label = "Confirmation";
	    langdirField.items[1][0] = "Yes, send confirmation";
	    langdirField.items[2][0] = "No, do not send confirmation";
    }
});
