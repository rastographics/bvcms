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

            $.fn.modal.Constructor.prototype.enforceFocus = function () {
                var modalThis = this;
                $(document).on('focusin.modal', function (e) {
                    // Fix for CKEditor + Bootstrap IE issue with dropdowns on the toolbar
                    // Adding additional condition '$(e.target.parentNode).hasClass('cke_contents cke_reset')' to
                    // avoid setting focus back on the modal window.
                    if (modalThis.$element[0] !== e.target && !modalThis.$element.has(e.target).length
                        && $(e.target.parentNode).hasClass('cke_contents cke_reset')) {
                        modalThis.$element.focus();
                    }
                });
            };

            CKEDITOR.replace('editor', {
                height: 200,
                allowedContent: true,
                customConfig: '/Content/touchpoint/js/ckeditorconfig.js',
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
