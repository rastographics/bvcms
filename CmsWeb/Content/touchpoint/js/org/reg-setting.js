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

        if ($('#editor').data('fa.editable')) {
            $('#editor').froalaEditable('destroy');
        }
        $('#editor').froalaEditable({
            inlineMode: false,
            spellcheck: true,
            useFileName: false,
            zIndex: 2501,
            height: 200,
            theme: 'custom',
            buttons: ['bold', 'italic', 'underline', 'fontSize', 'fontFamily', 'color', 'removeFormat', 'sep', 'formatBlock', 'align', 'insertOrderedList', 'insertUnorderedList', 'outdent', 'indent', 'sep', 'createLink', 'specialLink', 'sep', 'insertImage', 'uploadFile', 'table', 'undo', 'redo', 'html', 'fullscreen'],
            imageUploadURL: '/Account/FroalaUpload',
            fileUploadURL: '/Account/FroalaUpload'
        });
        
        $('#editor').froalaEditable('setHTML', $("#" + name).val());
        $('#editor-modal').modal('show');

        $("#save-edit").off("click").on("click", function (ev) {
            ev.preventDefault();

            var v = $('#editor').froalaEditable('getHTML');
            $("#" + name).val(v);
            $("#" + name + "_ro").html(v);

            $('#editor').froalaEditable('setHTML', '');
            $('#editor-modal').modal('hide');
            return false;
        });
        return false;
    });

});