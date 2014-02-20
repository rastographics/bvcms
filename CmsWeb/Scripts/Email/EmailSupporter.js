$(function () {
    $('#supportsearch').typeahead({
        name: 'supsearch',
        valueKey: "line1",
        limit: 25,
        beforeSend: function (jqXhr, settings) {
            $.SetLoadingIndicator();
        },
        remote: '/MissionTripEmail/Search/{0}?q=%QUERY'.format($("#PeopleId").val()),
        minLength: 3,
        template: 'dummy string',
        engine: {
            compile: function (t) {
                return {
                    render: function (context) {
                        var r = "<div{2}>{0}{1}</div>".format(
                            context.line1,
                            context.line2
                            ? "<br>" + context.line2
                            : "",
                            context.addmargin
                            ? " style='margin-bottom:.7em'"
                            : "");
                        return r;
                    }
                };
            }
        }
    });
    $('#supportsearch').bind('typeahead:selected', function (obj, datum, name) {
        $.post(datum.url, {}, function(ret) {
            $("#recipients").html(ret).ready(function() {
                $("#supportsearch").val("");
                $("#recipients .newsupporter").effect("highlight", {}, 2000);
            });
        });
    });
    $("#recipients a.remove").live("click", function (ev) {
        ev.preventDefault();
        var href = this.href;
        $.post(href, {}, function (ret) {
            $("#recipients").html(ret);
        });
        return false;
    });
    $("#edit-supporters").click(function (ev) {
        ev.preventDefault();
        var href = this.href;
        $("#edit-supporters").hide();
        $("#editing").show();
        $("#edit-help").hide();
        $("#done-help").show();
        $.post(href, {}, function (ret) {
            $("#recipients").html(ret);
        });
        return false;
    });
    $("#cancel-editing").click(function (ev) {
        ev.preventDefault();
        var href = this.href;
        $("#editing").hide();
        $("#edit-supporters").show();
        $("#edit-help").show();
        $("#done-help").hide();
        $.post(href, {}, function (ret) {
            $("#recipients").html(ret);
        });
        return false;
    });
    $("#done-editing").click(function (ev) {
        ev.preventDefault();
        var href = this.href;
        $("#editing").hide();
        $("#edit-supporters").show();
        $("#edit-help").show();
        $("#done-help").hide();

        var q = $("#SendEmail,#recipients").serialize();

        $.post(href, q, function (ret) {
            $("#recipients").html(ret);
        });
        return false;
    });


    var currentDiv = null;

    CKEDITOR.replace('htmleditor', {
        height: 400,
        autoParagraph: false,
        fullPage: false,
        allowedContent: true,
        filebrowserUploadUrl: '/Account/CKEditorUpload/',
        filebrowserImageUploadUrl: '/Account/CKEditorUpload/'
    });

    $("a.save").click(function () {
        var h = CKEDITOR.instances['htmleditor'].getData();
        $(currentDiv).html(h);
        $('#popupeditor').hide();
        dimOff();
    });
    $("a.cancel").click(function () {
        $('#popupeditor').hide();
        dimOff();
    });
    $.hClick = function (e) {
        currentDiv = this;
        $.removeButtons();
        CKEDITOR.instances['htmleditor'].setData($(this).html());
        dimOn();
        $("#popupeditor").show().center();
    };
    $('div[bvedit]').bind('click', $.hClick).addClass("ti");

    $(".send").click(function () {
        $('#Body').val($("#tempateBody").html());

//        var a = $.map($("#recipients li").not(".notselected"), function(e, i) {
//            return "Recipient=" + $(e).attr("rid");
//        });
//        var q = $("#SendEmail").serialize() + "&" + a.join("&");
        var q = $("#SendEmail").serialize();

        $.post('/MissionTripEmail/Send', q, function (ret) {
            if (ret.startsWith("/MissionTripEmail"))
                window.location = ret;
            else
                $(".send").notify(ret, "error");
        });
    });

    $(".testsend").click(function () {
        $.clearTemplateClass();
        $("#Body").val($("#tempateBody").html());
        $.addTemplateClass();
        var q = $("#SendEmail").serialize();
        $.post('/MissionTripEmail/TestSend', q, function (ret) {
            if (ret.error) {
                $(".testsend").notify(ret.message, "error");
            }
            else {
                $(".testsend").notify(ret.message, "success");
            }
        }, 'json');
    });

    $.removeButtons = function () {
        $("#controlButtons").remove();
    };

    $.hHoverIn = function (ev) {
        currentHover = this;
        $(this).css("border", "solid 1px #ff0000");
        $(this).append("<div id='controlButtons' class='tiAdd'><input id='addButton' type='button' value='Copy Section' /></div>");
        $("#controlButtons").css("top", $(this).offset().top + 5).css("left", $(this).offset().left + 5);
        $("#addButton").bind("click", $.hClickAdd);
        ev.stopPropagation();
    };
    $.clearTemplateClass = function () {
        $.removeButtons();
        $("div[bvedit]").removeClass();
    };

    $.addTemplateClass = function () {
        $("div[bvedit]").addClass("ti");
    };

    $.hHoverOut = function (ev) {
        currentHover = null;
        $(this).css("border", "");
        $.removeButtons();
        ev.stopPropagation();
    };

    $.hAddHoverIn = function (ev) {
        currentHover = this;
        $(this).css("border", "solid 1px #ff0000");
        $(this).append("<div id='controlButtons' class='tiAdd'><input id='removeButton' type='button' value='Remove' /></div>");
        $("#controlButtons").css("top", $(this).offset().top + 5).css("left", $(this).offset().left + 5);
        $("#removeButton").bind('click', $.removeSection);
    };

    $.updateDiv = function () {
        var h = CKEDITOR.instances['htmleditor'].getData();
        $(currentDiv).html(h);
        $('#popupeditor').hide("close");
    };

    $.removeSection = function (ev) {
        $(currentHover).parent().remove();
        ev.stopPropagation();
    };
});
