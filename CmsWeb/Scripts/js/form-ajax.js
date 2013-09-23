$(function() {
    $.AttachFormElements = function() {
        $("form.ajax input.ajax-typeahead").typeahead({
            minLength: 3,
            source: function(query, process) {
                return $.ajax({
                    url: $(this.$element[0]).data("link"),
                    type: 'post',
                    data: { query: query },
                    dataType: 'json',
                    success: function(jsonResult) {
                        return typeof jsonResult == 'undefined' ? false : process(jsonResult);
                    }
                });
            }
        });
        $.DatePickersAndChosen();
    };
    $.DatePickersAndChosen = function() {
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
        var state = $(this).attr("href");
        var d = $(state);
        var url = d.data("link");
        if (!d.hasClass("loaded"))
            $.ajax({
                type: 'POST',
                url: url,
                data: {},
                success: function (data, status) {
                    d.html(data);
                    d.addClass("loaded");
                }
            });
        return true;
    });
    $("div.tab-pane").on("click", "a.ajax-refresh", function (event) {
        var d = $(this).closest("div.tab-pane");
        $.ajax({
            type: 'POST',
            url: d.data("link"),
            data: {},
            success: function (data, status) {
                d.html(data);
                d.addClass("loaded");
            }
        });
        return false;
    });
    $("form.ajax a.submit").live("click", function(event) {
        event.preventDefault();
        var $form = $(this).closest("form.ajax");
        $form.attr("action", this.href);
        $form.submit();
        return false;
    });
    $("form.ajax a.ajax").live("click", function (event) {
        event.preventDefault();
        var $this = $(this);
        var $form = $this.closest("form.ajax");
        var url = $this.data("link");
        if (typeof url === 'undefined')
            url = $this[0].href;
        var data = $form.serialize();
        if (data.length === 0)
            data = {};
        if (!$this.hasClass("validate") || $form.valid()) {
            $.ajax({
                type: 'POST',
                url: url,
                data: data,
                success: function (ret, status) {
                    if ($form.hasClass("modal")) {
                        $form.html(ret).ready(function () {
                            $form.removeClass("hide");
                            var top = ($(window).height() - $form.height()) / 2;
                            if (top < 10)
                                top = 10;
                            $form.css({ 'margin-top': top, 'top': '0' });
                            $.AttachFormElements();
                        });
                    } else {
                        var results = $($form.data("results") || $form);
                        results.html(ret).ready(function () {
                            $.AttachFormElements();
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
    });

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
            if($loadingcount === 0)
                $("#loading-indicator").hide();
        }
    });
});
