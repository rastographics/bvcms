$(function () {

    CKEDITOR.replace('editor', {
        height: 200,
        customConfig: '/Content/touchpoint/lib/ckeditor/js/ckeditorconfig.js'
    });
    
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

    $('body').on('click', 'a.editor', function (ev) {
        if (!$(this).attr("href"))
            return false;
        var name = $(this).attr("tb");
        ev.preventDefault();
        CKEDITOR.instances['editor'].setData($("#" + name).val());
        
        $('#editor-modal').modal('show');
        $("#save-edit").off("click").on("click", function (ev) {
            ev.preventDefault();
            var v = CKEDITOR.instances['editor'].getData();
            $("#" + name).val(v);
            $("#" + name + "_ro").html(v);
            CKEDITOR.instances['editor'].setData('');
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