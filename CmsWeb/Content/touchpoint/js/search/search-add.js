/*! 
search-add.js 1/14/2016 3:50 PM
TouchPoint Software, LLC
*/
$(function () {

    $.fn.loadWith = function (u, f) {
        var c = $(this);
        $.post(u, function (d) {
            c.replaceWith(d).ready(f);
        });
    };

    $('#addorg').click(function (e) {
        e.preventDefault();
        var url = '/AddOrganization';
        if ($('#OrganizationId').length > 0) {
            url = url + '?displayCopySettings=true';
        }

        $("<div />").load(url, {}, function () {
            var div = $(this);
            var dialog = div.find("#new-org-modal");
            $('#empty-dialog').html(dialog);
            $('#empty-dialog').modal("show");
            dialog.on('hidden', function () {
                div.remove();
                dialog.remove();
            });

        });
    });
    
    $('body').on('click', 'a.searchadd', function (ev) {
        ev.preventDefault();
        
        $("<form id='search-add' class='modal-form validate ajax' />")
             .load($(this).attr("href"), {}, function () {
                 var form = $(this);
                 $('#empty-dialog').html(form);
                 $('#empty-dialog').modal("show");

                 $.AttachFormElements();
                 $(form).validate({
                     highlight: function (element) {
                         $(element).closest(".control-group").addClass("error");
                     },
                     unhighlight: function (element) {
                         $(element).closest(".control-group").removeClass("error");
                     }
                 });
                 $('#empty-dialog').on('hidden', function () {
                     $('#empty-dialog').remove();
                 });
             });
    });

    $('#empty-dialog').on('shown.bs.modal', function () {
        $("#search-add #Name").focus();
    });

    $('body').on('keydown', '#search-add input', function (ev) {
        if (ev.keyCode === 13) {
            ev.preventDefault();
            $("#searchperson").click();
            return false;
        }
        else
            return true;
    });

    $('body').on('click', '#search-add a.clear', function (ev) {
        ev.preventDefault();
        $("#Name").val('');
        $("#Phone").val('');
        $("#Address").val('');
        $("#dob").val('');
        return false;
    });

    $('body').on('click', '#search-add a.commit', function (ev) {
        ev.preventDefault();
        var $this = $(this);
        var alreadyClicked = $this.data('clicked');
        if (alreadyClicked) {
            return false;
        }
        $this.data('clicked', true);
        var f = $this.closest("form");
        var q = f.serialize();
        var loc = $this.attr("href");
        $.post(loc, q, function (ret) {
            $('#empty-dialog').modal("hide");
            if (ret.message)
                swal({title: "Error!", text: ret.message, type: "error", html: true});
            else if (ret.from === 'Menu')
                window.location = '/Person2/' + ret.pid;
            else
                AddSelected(ret);
        });
        return false;
    });

    $('body').on('click', 'form.ajax tbody > tr a.reveal', function (ev) {
        ev.stopPropagation();
    });

    $.NotReveal = function (ev) {
        if ($(ev.target).is("a"))
            if (!$(ev.target).is('.reveal'))
                return true;
        return false;
    };

    $('body').on('click', 'form.ajax tr.section', function (ev) {
        if ($.NotReveal(ev)) return;
        ev.preventDefault();
        $ToggleShown($(this));
    });

    $('body').on('click', 'form.ajax a[rel="reveal"]', function (ev) {
        ev.preventDefault();
        $ToggleShown($(this).parents("tr"));
    });

    var $ToggleShown = function (tr) {
        if (tr.hasClass("notshown"))
            $ShowAll(tr);
        else if (tr.hasClass("shown"))
            $CollapseAll(tr);
        else
            tr.next("tr").find("div.collapse")
                .off('hidden')
                .on("hidden", function (e) { e.stopPropagation(); })
                .collapse("toggle");
    };

    var $ShowAll = function (tr) {
        tr.nextUntil("tr.section").find("div.collapse")
            .off('hidden')
            .on("hidden", function (e) { e.stopPropagation(); })
            .collapse("show");

        tr.removeClass("notshown").addClass("shown");
        tr.find("i").removeClass("fa-caret-right").addClass("fa-caret-down");
    };

    $('body').on('shown.bs.collapse', function () {
        $.resizeModalBackDrop();
    });

    $('body').on('hidden.bs.collapse', function () {
        $.resizeModalBackDrop();
    });

    var $CollapseAll = function (tr) {
        tr.nextUntil("tr.section").find("div.collapse")
            .off("hidden")
            .on("hidden", function (e) { e.stopPropagation(); })
            .collapse('hide');
        tr.removeClass("shown").addClass("notshown");
        tr.find("i").removeClass("fa-caret-down").addClass("fa-caret-right");
    };

    $('body').on('click', 'form.ajax tr.master', function (ev) {
        if ($.NotReveal(ev)) return;
        ev.preventDefault();
        $(this).next("tr").find("div.collapse")
            .off('hidden')
            .on("hidden", function (e) { e.stopPropagation(); })
            .collapse("toggle");
    });

    $('body').on('click', 'form.ajax tr.details', function (ev) {
        if ($.NotReveal(ev)) return;
        ev.preventDefault();
        ev.stopPropagation();
        $(this).find("div.collapse")
            .off("hidden")
            .on("hidden", function (e) { e.stopPropagation(); })
            .collapse('hide');
    });
});