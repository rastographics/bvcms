$(function () {

    $.AttachFormElements = function () {
        if ($("form.ajax input.ajax-typeahead").length > 0) {
            $("form.ajax input.ajax-typeahead").typeahead({
                minLength: 3,
                source: function (query, process) {
                    return $.post($("input:focus").data("link") + "?query=" + query, function (data) {
                        return process(data);
                    });
                }
            });
        }
        if ($('.multi-select').length > 0) {
            $('.multi-select').multiselect({
                maxHeight: 200
            });
        }
        $.InitializeDateElements();
    };

    $('body').on('click', 'ul.nav-tabs a.tabajax, ul.nav-tabs a.ajax, a.ajax.ui-tabs-anchor, ul.nav-pills a.ajax, a.ajax.ui-tabs-anchor', function (event) {
        var $this = $(this);
        var alreadyClicked = $this.data('clicked');
        if (alreadyClicked) {
            return true;
        }
        $this.data('clicked', true);
        var state = $this.attr("href") || $this.data("target");
        var d = $(state);
        var $form = d.find("form.ajax");
        var postdata = $form.serialize();
        var url = d.data("link");

        if (url.length > 0) {
            if (!d.hasClass("loaded")) {
                $.ajax({
                    type: 'POST',
                    url: url,
                    data: postdata,
                    beforeSend: function () {
                        $.block();
                    },
                    complete: function () {
                        $.unblock();
                    },
                    success: function (data, status) {
                        $.unblock();
                        d.addClass("loaded");
                        $('select.nav-select-pills').val(state);
                        d.html(data).ready(function () {
                            if (d.data("init")) {
                                var temp = d.data("init").split(",");
                                for (var i in temp)
                                    if (temp.hasOwnProperty(i))
                                        $.InitFunctions[temp[i]]();
                            }
                            if (d.data("init2"))
                                $.InitFunctions[d.data("init2")]();
                            var $form2 = d.find("form.ajax");
                            if ($form2.length > 0)
                                $form = $form2;
                            if ($form.data("init")) {
                                var t = $form.data("init").split(",");
                                for (var ii in t)
                                    if (t.hasOwnProperty(ii))
                                        $.InitFunctions[t[ii]]();
                            }
                            if ($form.data("init2"))
                                $.InitFunctions[$form.data("init2")]();
                        });
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        $.unblock();
                        swal({ title: "Error!", text: thrownError, type: "error", html: true });
                    }
                });
            }
        }
        return true;
    });

    $('body').on('change', 'select.nav-select-pills', function (event) {
        $("a[href='" + $(this).val() + "']").click().tab("show");
    });

    $("div.tab-pane").on("click", "a.ajax-refresh", function (event) {
        event.preventDefault();
        var d = $(this).closest("div.tab-pane");
        $.formAjaxClick($(this), d.data("link"), d.data("action"));
        return false;
    });
    $("body").on("click", "form.ajax a.ajax-refresh", function (event) {
        event.preventDefault();
        $.formAjaxClick($(this));
        return false;
    });

    $('body').on('click', 'form.ajax a.submit', function (event) {
        event.preventDefault();
        var t = $(this);
        if (t.data("confirm"))
            swal({
                title: t.data("confirm"),
                text: t.data("confirm-text"),
                type: t.data("confirm-type"),
                showCancelButton: true,
                confirmButtonClass: t.data("confirm-btn-class"),
                confirmButtonText: t.data("confirm-btn-text"),
                closeOnConfirm: true
            },
            function () {
                $.formAjaxSubmit(t);
            });
        else
            $.formAjaxSubmit(t);
        return false;
    });

    $.formAjaxSubmit = function (a) {
        var $form = a.closest("form.ajax");
        $form.attr("action", a[0].href);
        if (!a.hasClass("validate") || $form.valid()) {
            $form.submit();
        }
    };

    $('body').on('click', 'form.ajax input.ajax', function () {
        $.formAjaxClick($(this));
    });

    $('body').on('change', 'form.ajax #Size', function (event) {
        event.preventDefault();
        var t = $(this);
        $(t).attr('data-size', $(t).val());
        $.formAjaxClick(t);
        return false;
    });

    $('body').on('change', 'form.ajax .org-types-filter', function (event) {
        event.preventDefault();
        var t = $(this);
        $.formAjaxClick(t);
        return false;
    });

    $('body').on('click', 'form.ajax a.ajax', function (event) {
        event.preventDefault();
        var t = $(this);
        if (t.data("confirm"))
            swal({
                title: t.data("confirm"),
                text: t.data("confirm-text"),
                type: t.data("confirm-type"),
                showCancelButton: true,
                confirmButtonClass: t.data("confirm-btn-class"),
                confirmButtonText: t.data("confirm-btn-text"),
                closeOnConfirm: true
            },
            function () {
                $.formAjaxClick(t);
            });
        else
            $.formAjaxClick(t);
        return false;
    });

    $.formAjaxClick = function (a, link, action) {
        var $form = a.closest("form.ajax");
        var $load = $form;
        var $tabinit = $form.closest("div.tab-pane[data-init]");
        var $tablink = $form.closest("div.tab-pane");
        if (a.data("loadele")) {
            $load = $(a.data("loadele"));
//            $tabinit = {};
//            $tablink = {};
        }
        var $modalbody = a.closest("div.modal-body");
        var ahref = a.attr("href");
        if (ahref === '#')
            ahref = null;
        var url = link
            || a.data("link")
            || ahref
            || $tablink.data("link")
            || $modalbody.data("target")
            || $form[0].action
            || '#';

        var type = action
            || 'POST';

        if (a.data("size"))
            $("input[name='PageSize']", $form).val(a.data("size"));
        if (a.data("page"))
            $("input[name='Page']", $form).val(a.data("page"));
        if (a.data("sortby"))
            $("input[name='Sort']", $form).val(a.data("sortby"));
        if (a.data("dir"))
            $("input[name='Direction']", $form).val(a.data("dir"));

        var data = $form.serialize();
        if (data.length === 0 || a.data("data") === "none")
            data = {};
        if (!a.hasClass("validate") || $form.valid()) {
            var isModal = $load.hasClass("modal-form");
            $.ajax({
                type: type,
                url: url,
                data: data,
                beforeSend: function () {
                    if (isModal === false)
                        $.block();
                },
                complete: function () {
                    $.unblock();
                },
                success: function (ret, status) {
                    if (type === 'GET') {
                        location.reload();
                        return true;
                    }
                    $.unblock();
                    if (a.data("redirect"))
                        window.location = ret;
                    else if (isModal === true) {
                        $load.html(ret).ready(function () {
                            $.resizeModalBackDrop();
                            $.AttachFormElements();
                            if (a.data("callback"))
                                $.InitFunctions[a.data("callback")]();
                        });
                    } else {
                        var results = $($load.data("results") || $load);

                        results.replaceWith(ret).ready(function () {
                            if ($(".scrollToTop").length > 0) {
                                $("html, body").animate({ scrollTop: 0 }, "slow");
                            }

                            $.AttachFormElements();

                            if ($tabinit.data && $tabinit.data("init")) {
                                var temp = $tabinit.data("init").split(",");
                                for (var i in temp) {
                                    if (temp.hasOwnProperty(i)) {
                                        $.InitFunctions[temp[i]]();
                                    }
                                }
                            }
                            if ($form.data("init")) {
                                var t = $form.data("init").split(",");
                                for (var ii in t)
                                    if (t.hasOwnProperty(ii))
                                        $.InitFunctions[t[ii]]();
                            }
                            if ($tabinit.data && $tabinit.data("init2"))
                                $.InitFunctions[$tabinit.data("init2")]();
                            if ($form.data("init2"))
                                $.InitFunctions[$form.data("init2")]();
                            if (a.data("callback"))
                                $.InitFunctions[a.data("callback")]();
                            if (a.data("reload"))
                                $(a.data("reload")).click();
                        });
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblock();
                    swal({title: "Error!", text: thrownError, type: "error", html: true});
                }
            });
        }
        return false;
    };

    $.validator.addMethod("unallowedcode", function (value, element, params) {
        return value !== params.code;
    }, "required, select item");

    $.validator.addMethod("dateandtimevalid", function (value, element) {
        var extraSmallDevice = $('.device-xs').is(':visible');
        var smallDevice = $('.device-sm').is(':visible');
        var validDateTimeIso = /\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d/.test(value);
        if (extraSmallDevice || smallDevice) {
            return this.optional(element) || (validDateTimeIso);
        } else {
            var stamp = value.split(" ");
            var validDate = !/Invalid|NaN/.test(moment(stamp[0], $.cultureDateFormat));
            var validTime = /^(([0-1]?[0-9])|([2][0-3])):([0-5]?[0-9])(:([0-5]?[0-9]))?$/i.test(stamp[1]);
            return this.optional(element) || (validDate && validTime);
        }
    }, "Please enter a valid date and time.");

    $('body').on('click', 'a.dialog-options', function (ev) {
        ev.preventDefault();
        var $a = $(this);
        // data-target is the dialog  and a.href is the report
        // or a.href is the dialog and form.action is the report
        var dialog = $a.data("target") || this.href;
        $.dialogOptions(dialog, $a);
    });

    $.dialogOptions = function (dialog, $a) {
        $("<div id='dialog-options' />").load(dialog, function () {
            var div = $(this);
            var dlg = div.find("div.modal-dialog");
            var f = div.find("form");

            if (!f.attr("action"))
                f.attr("action", $a[0].href); // a.href will be the report/export

            if ($a[0].title)
                div.find("h3.modal-title").text($a[0].title);

            $('#empty-dialog').html(dlg);
            $('#empty-dialog').modal("show");

            f.on('hidden', function () {
                div.remove();
                dlg.remove();
            });

            $.AttachFormElements();

            f.validate({
                submitHandler: function (form) {
                    if (form.method.toUpperCase() === 'GET')
                        form.submit();
                    else if ($(form).hasClass("ajax")) {
                        var q = f.serialize();
                        $.post(form.action, q, function (ret) {
                            if (ret) {
                                if (!$(form).hasClass("ignoreResult")) {
                                    swal(ret);
                                }
                            }

                            if ($a.data("callback")) {
                                $.InitFunctions[$a.data("callback")]();
                            }
                        });
                    } else {
                        if ($a.data("confirm")) {
                            swal({
                                title: t.data("confirm"),
                                text: t.data("confirm-text"),
                                type: t.data("confirm-type"),
                                showCancelButton: true,
                                confirmButtonClass: t.data("confirm-btn-class"),
                                confirmButtonText: t.data("confirm-btn-text"),
                                closeOnConfirm: true
                            },
                            function () {
                                form.submit();
                            });
                        }
                        else
                            form.submit();
                        if ($a.data("callback")) {
                            var q = f.serialize();
                            $.InitFunctions[$a.data("callback")]($a, q);
                        }
                    }
                    $('#empty-dialog').modal("hide");
                },
                highlight: function (element) {
                    $(element).closest(".form-group").addClass("error");
                },
                unhighlight: function (element) {
                    $(element).closest(".form-group").removeClass("error");
                }
            });
        });
        return false;
    };

    $('body').on('click', 'a.longrunop', function (ev) {
        ev.preventDefault();
        var data = {};
        if ($(this).data("post"))
            data = $(this).closest("form").serializeArray();
        $('<form class="modal-form ajax validate" />').load(this.href, data, function () {
            var f = $(this);
            var callback = $("#callback", f).val();

            $('#empty-dialog').html(f);
            $('#empty-dialog').modal("show");

            var tm = 250; // initial timeout
            $('#empty-dialog').on('hidden.bs.modal', function (e) {
                tm = 0;
                f.remove();
                if (callback) {
                    $.InitFunctions[callback]();
                }
            });

            f.on("click", "a.ajaxreloader", function (event) {
                event.preventDefault();
                var href = this.href;
                var postdata = f.serialize() || {};
                var myloop = function () {
                    $.post(href, postdata, function (ret) {
                        f.html(ret);
                        if ($("#finished", f).val())
                            tm = 0;
                        if (tm > 0) {
                            tm += 500;
                            if (tm > 3000)
                                tm = 3000;
                            setTimeout(myloop, tm);
                        }
                        postdata = f.serialize();
                    });
                }

                var t = $(this);
                if (t.data("confirm"))
                    swal({
                        title: t.data("confirm"),
                        text: t.data("confirm-text"),
                        type: t.data("confirm-type"),
                        showCancelButton: true,
                        confirmButtonClass: t.data("confirm-btn-class"),
                        confirmButtonText: t.data("confirm-btn-text"),
                        closeOnConfirm: true
                    },
                    function () {
                        setTimeout(myloop, tm);
                    });
                else
                    setTimeout(myloop, tm);
                return false;
            });
        });
        return true;
    });

    if (!$.InitFunctions)
        $.InitFunctions = {};

    $.resizeModalBackDrop = function () {
        // resize modal backdrop height.
        var dialog = $('.modal-dialog');
        var backdrop = $('.modal-backdrop');
        var height = dialog.innerHeight();

        $(backdrop).css({
            height: height + 60,
            minHeight: '100%',
            margin: 'auto'
        });
    };
});
