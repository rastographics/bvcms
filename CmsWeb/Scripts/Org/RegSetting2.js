$(function () {

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
        customConfig: '/Content/touchpoint/js/ckeditorconfig.js'
    });
    $("body").on("click", 'ul.enablesort div.newitem > a', function (ev) {
        if (!$(this).attr("href"))
            return false;
        ev.preventDefault();
        var a = $(this);
        var f = a.closest("form");
        $.post(a.attr("href"), null, function (ret) {
            a.parent().prev().append(ret);
            a.parent().prev().find(".tip").tooltip({ opacity: 0, showBody: "|" });
            $.initDatePicker(f);
        });
    });

    $.regsettingeditclick = function (f) {
        $(".tip", f).tooltip({ opacity: 0, showBody: "|" });
        $("ul.enablesort.sort, ul.enablesort ul.sort", f).sortable();
        $("ul.noedit input", f).attr("disabled", "disabled");
        $("ul.noedit textarea", f).attr("disabled", "disabled");
        $("ul.noedit select", f).attr("disabled", "disabled");
        $("ul.noedit a", f).not('[target="otherorg"]').removeAttr("href");
        $("ul.noedit a", f).not('[target="otherorg"]').css("color", "grey");
        $("ul.noedit a", f).not('[target="otherorg"]').unbind("click");
        $('ul.edit a.notifylist').SearchUsers({
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
    $.regsettingeditclick();
    $("a.editor").live("click", function (ev) {
        if (!$(this).attr("href"))
            return false;
        var name = $(this).attr("tb");
        ev.preventDefault();
        CKEDITOR.instances['editor'].setData($("#" + name).val());
        dimOn();
        $("#EditorDialog").center().show();
        $("#saveedit").off("click").on("click", function (ev) {
            ev.preventDefault();
            var v = CKEDITOR.instances['editor'].getData();
            $("#" + name).val(v);
            $("#" + name + "_ro").html(v);
            CKEDITOR.instances['editor'].setData('');
            $('#EditorDialog').hide("close");
            dimOff();
            return false;
        });
        return false;
    });
    $("#canceledit").live("click", function (ev) {
        ev.preventDefault();
        $('#EditorDialog').hide("close");
        dimOff();
        return false;
    });
});