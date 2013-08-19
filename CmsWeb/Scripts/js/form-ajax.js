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
        $("form.ajax .date").datepicker();
        $("form.ajax select").chosen();
    };
    $("div.modal form.ajax").live("submit", function (event) {
        event.preventDefault();
        var $form = $(this);
        var $target = $form.closest("div.modal");
        $.ajax({
            type: 'POST',
            url: $form.attr('action'),
            data: $form.serialize(),
            success: function (data, status) {
                //$target.removeClass("fade");
                $target.html(data).ready(function () {
                    var top = ($(window).height() - $target.height()) / 2;
                    if (top < 10)
                        top = 10;
                    $target.css({ 'margin-top': top, 'top': '0' });
                    $.AttachFormElements();
                });
            }
        });
        return false;
    });
    $("ul.nav-tabs a.ajax").live("click", function(event) {
        var state = $(this).attr("href");
        var d = $(state);
        if(!d.hasClass("loaded"))
            $.ajax({
                type: 'POST',
                url: d.data("link"),
                data: {},
                success: function (data, status) {
                    d.html(data);
                    d.addClass("loaded");
                }
            });
        return true;
    });
    $("form.ajax a.ajax").live("click", function (event) {
        event.preventDefault();
        var $this = $(this);
        var $form = $this.closest("form.ajax");
        var $modal = $form.closest("div.modal");
        var url = $this.data("link");
        if (typeof url === 'undefined')
            url = $this[0].href;
        var data = $form.serialize();
        $.ajax({
            type: 'POST',
            url: url,
            data: data,
            success: function (data, status) {
                if ($modal.length > 0) {
                    //$modal.removeClass("fade");
                    $modal.html(data).ready(function () {
                        var top = ($(window).height() - $modal.height()) / 2;
                        if (top < 10)
                            top = 10;
                        $modal.css({ 'margin-top': top, 'top': '0' });
                        $.AttachFormElements();
                    });
                } else {
                    $form.html(data).ready(function () {
                        $.AttachFormElements();
                    });
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
        return false;
    });
    $.ajaxSetup({
        beforeSend: function () {
            $("#loading-indicator").css({
                'position': 'absolute',
                'left': $(window).width() / 2,
                'top': $(window).height() / 2,
                'z-index': 2000
            }).show();
        },
        complete: function () {
            $("#loading-indicator").hide();
        }
    });
});
