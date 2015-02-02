$(function () {
    $.AttachFormElements = function () {
        $("form.ajax input.ajax-typeahead").typeahead({
            minLength: 3,
            remote: {
                url: "test",
                beforeSend: function (jqXhr, settings) {
                    $.SetLoadingIndicator();
                },
                replace: function (url, uriEncodedQuery) {
                    return $("input:focus").data("link") + "?query=" + uriEncodedQuery;
                }
            }
        });
        $.DatePickersAndChosen();
    };
    $.DatePickers = function() {
        $("form .dateonly").datepicker({
            autoclose: true,
            orientation: "auto",
            minView: 2,
            forceParse: false,
            format: $.dtoptions.format
        });
        $("form .datetime").datetimepicker({
            autoclose: true,
            showMeridian: true,
            orientation: "auto",
            forceParse: false,
            format: $.dtoptions.formatTime
        });
    };
    $.DatePickersAndChosen = function () {
        $.DatePickers();
        $('form.ajax select:not([plain])').select2({dropdownAutoWidth: true});
        $('form.ajax a.editable').editable();
    };
    $("ul.nav-tabs a.ajax,a.ajax.ui-tabs-anchor").live("click", function (event) {
        var $this = $(this);
        var alreadyClicked = $this.data('clicked');
        if (alreadyClicked) {
            return false;
        }
        $this.data('clicked', true);
        var state = $this.attr("href") || $this.data("target");
        var d = $(state);
        var $form = d.find("form.ajax");
        var postdata = $form.serialize();
        var url = d.data("link");
        if (!d.hasClass("loaded"))
            $.ajax({
                type: 'POST',
                url: url,
                data: postdata,
                success: function (data, status) {
                    d.addClass("loaded");
                    d.html(data).ready(function () {
                        if (d.data("init"))
                            $.InitFunctions[d.data("init")]();
                        if (d.data("init2"))
                            $.InitFunctions[d.data("init2")]();
                        if ($form.length === 0)
                            $form = d.find("form.ajax");
                        if ($form.data("init"))
                            $.InitFunctions[$form.data("init")]();
                        if ($form.data("init2"))
                            $.InitFunctions[$form.data("init2")]();
                    });
                },
                error: function (data, status) {
                    d.html(data.responseText).ready(function () {

                    });
                }
            });
        return true;
    });
    $("div.tab-pane").on("click", "a.ajax-refresh", function (event) {
        event.preventDefault();
        var d = $(this).closest("div.tab-pane");
        $.formAjaxClick($(this), d.data("link"));
        return false;
    });
    $("form.ajax a.submit").live("click", function (event) {
        event.preventDefault();
        var t = $(this);
        if (t.data("confirm"))
            bootbox.confirm(t.data("confirm"), function (ret) {
                if (ret == true)
                    $.formAjaxSubmit(t);
            });
        else
            $.formAjaxSubmit(t);
        return false;
    });
    $.formAjaxSubmit = function (a) {
        var $form = a.closest("form.ajax");
        $form.attr("action", a[0].href);
        $form.submit();
    };
    $("form.ajax input.ajax").live('click', function() {
        $.formAjaxClick($(this));
    });
    $("form.ajax a.ajax").live("click", function (event) {
        event.preventDefault();
        var t = $(this);
        if (t.data("confirm"))
            bootbox.confirm(t.data("confirm"), function (ret) {
                if (ret == true)
                    $.formAjaxClick(t);
            });
        else
            $.formAjaxClick(t);
        return false;
    });
    $.formAjaxClick = function (a, link) {
        var $form = a.closest("form.ajax");
        var $tablink = $form.closest("div.tab-pane");
        var $modalbody = a.closest("div.modal-body");
        var ahref = a.attr("href");
        if (ahref === '#')
            ahref = null;
        var url = link
            || a.data("link")
            || ahref
            || $form[0].action
            || $tablink.data("link")
            || $modalbody.data("target") 
            || '#';

        if (a.data("size"))
            $("input[name='PageSize']", $form).val(a.data("size"));
        if (a.data("page"))
            $("input[name='Page']", $form).val(a.data("page"));
        if (a.data("sortby"))
            $("input[name='Sort']", $form).val(a.data("sortby"));
        if (a.data("dir"))
            $("input[name='Direction']", $form).val(a.data("dir"));
        var $tabinit = $form.closest("div.tab-pane[data-init]");

        var data = $form.serialize();
        if (data.length === 0)
            data = {};
        if (!a.hasClass("validate") || $form.valid()) {
            $.ajax({
                type: 'POST',
                url: url,
                data: data,
                success: function (ret, status) {
                    if (a.data("redirect"))
                        window.location = ret;
                    else if ($form.hasClass("modal")) {
                        $form.html(ret).ready(function () {
                            $form.removeClass("hide");
                            var top = ($(window).height() - $form.height()) / 2;
                            if (top < 10)
                                top = 10;
                            $form.css({ 'margin-top': top, 'top': '0' });
                            $.AttachFormElements();
                            if (a.data("callback"))
                                $.InitFunctions[a.data("callback")]();
                        });
                    } else {
                        var results = $($form.data("results") || $form);
                        results.replaceWith(ret).ready(function () {
                            $.AttachFormElements();
                            if ($tabinit.data("init"))
                                $.InitFunctions[$tabinit.data("init")]();
                            if ($tabinit.data("init2")) 
                                $.InitFunctions[$tabinit.data("init2")]();
                            if ($form.data("init"))
                                $.InitFunctions[$form.data("init")]();
                            if ($form.data("init2"))
                                $.InitFunctions[$form.data("init2")]();
                            if (a.data("callback"))
                                $.InitFunctions[a.data("callback")]();
                        });
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $form.html(xhr.responseText);
                }
            });
        }
        return false;
    };

    $.validator.addMethod("unallowedcode", function (value, element, params) {
        return value !== params.code;
    }, "required, select item");
    $.validator.addMethod("dateandtimevalid", function (value, element) {
        var stamp = value.split(" ");
        var validDate = !/Invalid|NaN/.test(new Date(stamp[0]).toString());
        var validTime = /^(([0-1]?[0-9])|([2][0-3])):([0-5]?[0-9])(:([0-5]?[0-9]))?$/i.test(stamp[1]);
        return this.optional(element) || (validDate && validTime);
    }, "Please enter a valid date and time.");

    $("a.dialog-options").live("click", function(ev) {
        ev.preventDefault();
        var $a = $(this);
        // data-target is the dialog  and a.href is the report
        // or a.href is the dialog and form.action is the report
        var dialog = $a.data("target") || this.href;
        $.dialogOptions(dialog, $a);
    });
    $.dialogOptions = function(dialog, $a) {
        $("<div id='dialog-options' />").load(dialog, {}, function () {
            var d = $(this);
            var f = d.find("form");
            if ($a[0].title)
                f.find("h3.title").text($a[0].title);
            f.modal("show");
            if (!f.attr("action"))
                f.attr("action", $a[0].href); // a.href will be the report/export
            f.on('hidden', function () {
                d.remove();
                f.remove();
            });
            $.DatePickers();
            f.validate({
                submitHandler: function (form) {
                    if (form.method.toUpperCase() === 'GET')
                        form.submit();
                    else if ($(form).hasClass("ajax")) {
                        var q = f.serialize();
                        $.post(form.action, q, function(ret) {
                            if (ret)
                                $.growlUI("", ret);
                            if ($a.data("callback")) {
                                $.InitFunctions[$a.data("callback")]($a);
                            }
                        });
                    } else {
                        if ($a.data("confirm"))
                            bootbox.confirm($a.data("confirm"), function(ret) {
                                if (!ret)
                                    form.submit();
                            });
                        else
                            form.submit();
                        if ($a.data("callback")) {
                            var q = f.serialize();
                            $.InitFunctions[$a.data("callback")]($a, q);
                        }
                    }
                    f.modal("hide");
                },
                highlight: function (element) {
                    $(element).closest(".control-group").addClass("error");
                },
                unhighlight: function (element) {
                    $(element).closest(".control-group").removeClass("error");
                }
            });
        });
        return false;
    };

    $("a.longrunop").live("click", function(ev) {
        ev.preventDefault();
        var data = {};
        if ($(this).data("post"))
            data = $(this).closest("form").serializeArray();
        $('<form class="modal form-horizontal ajax validate fade hide" />').load(this.href, data, function() {
            var f = $(this);
            var callback = $("#callback", f).val();
            f.modal("show");
            var tm = 250; // initial timeout
            f.on('hidden', function () {
                tm = 0;
                f.remove();
                if(callback)
                    $.InitFunctions[callback]();
            });
            f.on("click", "a.ajaxreloader", function(event) {
                event.preventDefault();
                var href = this.href;
                var postdata = f.serialize() || {};
                var myloop = function() {
                    $.post(href, postdata , function (ret) {
                        postdata = f.serialize();
                        f.html(ret);
                        if ($("#finished", f).val())
                            tm = 0;
                        if (tm > 0) {
                            tm += 500;
                            if (tm > 3000)
                                tm = 3000;
                            setTimeout(myloop, tm);
                        }
                    });
                }
                setTimeout(myloop, tm);
                return false;
            });
        });
        return false;
    });
    var $loadingcount = 0;
    $.ajaxSetup({
        beforeSend: function () {
            $.SetLoadingIndicator();
        },
        complete: function () {
            $loadingcount--;
            if ($loadingcount === 0)
                $("#loading-indicator").hide();
        }
    });
    $.SetLoadingIndicator = function () {
        $("#loading-indicator").css({
            'position': 'absolute',
            'left': $(window).width() / 2,
            'top': $(window).height() / 2,
            'z-index': 2000
        }).show();
        $loadingcount++;
    };
    if (!$.InitFunctions)
        $.InitFunctions = {};
});
