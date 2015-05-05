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

    $('body').on('click', 'a.editor', function (ev) {
        if (!$(this).attr("href"))
            return false;
        var name = $(this).attr("tb");
        ev.preventDefault();

        $("#" + name).froalaEditable({
            inlineMode: false,
            height: 200,
            theme: 'custom',
            buttons: ['bold', 'italic', 'underline', 'fontFamily', 'sep', 'formatBlock', 'align', 'insertOrderedList', 'insertUnorderedList', 'outdent', 'indent', 'sep', 'createLink', 'specialLink', 'sep', 'insertImage', 'table', 'html', 'fullscreen'],
            imageUploadURL: '/Account/FroalaUpload'
        });
        $("#" + name + "_ro").hide();
        $(this).hide();
        return false;
    });

});