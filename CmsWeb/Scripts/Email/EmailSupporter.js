$(function () {
    $('#supportsearch').typeahead({
        name: 'supsearch',
        valueKey: "line1",
        limit: 25,
        beforeSend: function (jqXhr, settings) {
            $.SetLoadingIndicator();
        },
        remote: '/MissionTripEmail/Search/{0}?q=%QUERY'.format($("#PeopleId").val()),
        //        minLength: 0,
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
                $("#recipients li.newsupporter").effect("highlight", {}, 2000);
            });
        });
    });
    $("#recipients > li > a").live("click", function (ev) {
        ev.preventDefault();
        var href = this.href;
        $.post(href, {}, function (ret) {
            $("#recipients").html(ret);
        });
        return false;
    });
    $("#editsupporters").click(function (ev) {
        ev.preventDefault();
        var href = this.href;
        $(this).hide();
        $("#done-editing").show();
        $.post(href, {}, function (ret) {
            $("#recipients").html(ret);
        });
        return false;
    });
    $("#done-editing").click(function (ev) {
        ev.preventDefault();
        var href = this.href;
        $(this).hide();
        $("#editsupporters").show();
        $.post(href, {}, function (ret) {
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
        $('#body').val($("#tempateBody").html());

        var a = $.map($("#recipients li").not(".notselected"), function(e, i) {
            return "Recipient=" + $(e).attr("rid");
        });
        var q = $("#SendEmail").serialize() + "&" + a.join("&");

        $.post('/MissionTripEmail/Send', q, function (ret) {
            if (ret.startsWith("http"))
                window.location = ret;
            else
                $(".send").notify(ret, "error");
        });
    });

    $(".testsend").click(function () {
        $.clearTemplateClass();
        $("#body").val($("#tempateBody").html());
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
