$(function () {

    $.AttachFormElements = function () {
        //$("form.ajax input.ajax-typeahead").typeahead({
        //    minLength: 3,
        //    remote: {
        //        url: "test",
        //        beforeSend: function (jqXhr, settings) {
        //            $.SetLoadingIndicator();
        //        },
        //        replace: function (url, uriEncodedQuery) {
        //            return $("input:focus").data("link") + "?query=" + uriEncodedQuery;
        //        }
        //    }
        //});
        //$.DatePickersAndChosen();
        $.InitializeDateElements();
    };

    //$.DatePickersAndChosen = function () {
    //    $("form.ajax .date").datepicker({
    //        autoclose: true,
    //        orientation: "auto",
    //        forceParse: false,
    //        format: $.dtoptions.format
    //    });
    //    $('form.ajax select:not([plain])').chosen();
    //    $('form.ajax a.editable').editable();
    //};

    $('body').on('click', 'ul.nav-tabs a.ajax,a.ajax.ui-tabs-anchor', function (event) {
        var $this = $(this);
        var alreadyClicked = $this.data('clicked');
        if (alreadyClicked) {
            return false;
        }
        $this.data('clicked', true);
        var state = $this.attr("href") || $this.data("target");
        var d = $(state);
        var url = d.data("link");
        if (!d.hasClass("loaded"))
            $.ajax({
                type: 'POST',
                url: url,
                data: {},
                beforeSend: function () {
                    $.block();
                },
                complete: function () {
                    $.unblock();
                },
                success: function (data, status) {
                    d.addClass("loaded");
                    d.html(data).ready(function () {
                        var $form = d.find("form.ajax");
                        if ($form.data("init")) {
                            $.InitFunctions[$form.data("init")]();
                        }
                        if ($form.data("init2")) {
                            $.InitFunctions[$form.data("init2")]();
                        }
                    });
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblock();
                    swal("Error!", thrownError, "error");
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

    $('body').on('click', 'form.ajax a.submit', function (event) {
        event.preventDefault();
        var t = $(this);
        if (t.data("confirm"))
            swal({
                title: "Are you sure?",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-danger",
                confirmButtonText: "Yes, delete it!",
                closeOnConfirm: false
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
        $form.submit();
    };

    $('body').on('change', 'form.ajax select.ajax', function (event) {
        event.preventDefault();
        var t = $(this);
        var link = t.data("link");
        $.formAjaxClick(t, link);
        return false;
    });

    $('body').on('click', 'form.ajax a.ajax', function (event) {
        event.preventDefault();
        var t = $(this);
        if (t.data("confirm"))
            swal({
                title: "Are you sure?",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-danger",
                confirmButtonText: "Yes, delete it!",
                closeOnConfirm: false
            },
            function () {
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
            var isModal = $form.hasClass("modal-form");

            $.ajax({
                type: 'POST',
                url: url,
                data: data,
                beforeSend: function () {
                    if (isModal == false)
                        $.block();
                },
                complete: function () {
                    $.unblock();
                },
                success: function (ret, status) {
                    $.unblock();
                    if (a.data("redirect"))
                        window.location = ret;
                    else if (isModal == true) {
                        $form.html(ret).ready(function () {
                            $.resizeModalBackDrop();
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
                            if ($form.data("init2")) {
                                $.InitFunctions[$form.data("init2")]();
                            }
                            if (a.data("callback"))
                                $.InitFunctions[a.data("callback")]();
                        });
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblock();
                    swal("Error!", thrownError, "error");
                }
            });
        }
        return false;
    };

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

    $.validator.addMethod("unallowedcode", function (value, element, params) {
        return value !== params.code;
    }, "required, select item");

    if (!$.InitFunctions)
        $.InitFunctions = {};
});
