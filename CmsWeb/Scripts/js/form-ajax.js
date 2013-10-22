$(function () {
    $.AttachFormElements = function () {
        $("form.ajax input.ajax-typeahead").typeahead({
            minLength: 3,
            source: function (query, process) {
                return $.ajax({
                    url: $(this.$element[0]).data("link"),
                    type: 'post',
                    data: { query: query },
                    dataType: 'json',
                    success: function (jsonResult) {
                        return typeof jsonResult == 'undefined' ? false : process(jsonResult);
                    }
                });
            }
        });
        $.DatePickersAndChosen();
    };
    $.DatePickersAndChosen = function () {
        $("form.ajax .date:not(.noparse)").datepicker({
            autoclose: true,
            orientation: "auto"
        });
        $("form.ajax .date.noparse").datepicker({
            autoclose: true,
            orientation: "auto",
            forceParse: false
        });
        $('form.ajax select:not([plain])').chosen();
    };
    $("ul.nav-tabs a.ajax").live("click", function (event) {
        var state = $(this).attr("href") || $(this).data("target");
        var d = $(state);
        var url = d.data("link");
        if (!d.hasClass("loaded"))
            $.ajax({
                type: 'POST',
                url: url,
                data: {},
                success: function (data, status) {
                    d.addClass("loaded");
                    d.html(data).ready(function () {
                        var $form = d.find("form.ajax");
                        if ($form.data("init")) {
                            $.InitFunctions[$form.data("init")]();
                        }
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
        var $form = $(this).closest("form.ajax");
        $form.attr("action", this.href);
        $form.submit();
        return false;
    });
    $("form.ajax a.ajax").live("click", function (event) {
        event.preventDefault();
        var t = $(this);
        if (t.data("confirm"))
            bootbox.confirm(t.data("confirm"), function(ret) {
                if (ret == true)
                    $.formAjaxClick(t);
            });
        else
            $.formAjaxClick(t);
        return false;
    });
    $.formAjaxClick = function (a, link) {
        var $form = a.closest("form.ajax");
        var url = link || a.data("link");
        if (typeof url === 'undefined')
            url = a[0].href;
        var data = $form.serialize();
        if (data.length === 0)
            data = {};
        if (!a.hasClass("validate") || $form.valid()) {
            $.ajax({
                type: 'POST',
                url: url,
                data: data,
                success: function (ret, status) {
                    if(a.data("redirect"))
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
                            if ($form.data("init"))
                                $.InitFunctions[$form.data("init")]();
                            if (a.data("callback"))
                                $.InitFunctions[a.data("callback")]();
                        });
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
        return false;
    };

    $.validator.addMethod("unallowedcode", function (value, element, params) {
        return value !== params.code;
    }, "required, select item");

    var $loadingcount = 0;
    $.ajaxSetup({
        beforeSend: function () {
            $("#loading-indicator").css({
                'position': 'absolute',
                'left': $(window).width() / 2,
                'top': $(window).height() / 2,
                'z-index': 2000
            }).show();
            $loadingcount++;
        },
        complete: function () {
            $loadingcount--;
            if ($loadingcount === 0)
                $("#loading-indicator").hide();
        }
    });
    $.InitFunctions = {};
});
