(function ($) {
    $.fn.SearchUsers = function (options) {
        debug(this);
        var opts = $.extend({}, {}, options);
        return this.each(function () {
            var $this = $(this);
            $this.click(function (ev) {
                ev.preventDefault();
                var url = $(this).attr('href');
                
                $("<div />").load(url, {}, function () {
                    var d = $(this);
                    var f = d.find("form");
                    f.modal("show");

                    $(".UpdateSelected", f).click(function (ev2) {
                        ev2.preventDefault();
                        var topid = $("table.results tbody tr:first ", f).find("input[type=checkbox]").attr("value");
                        var topid0 = $("#topid0").val();
                        if (opts.UpdateShared)
                            opts.UpdateShared(topid, topid0, $this);
                        f.modal("hide");
                        return false;
                    });
                    $("#searchname").live("keypress", function (e) {
                        if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
                            e.preventDefault();
                            $('a.search').click();
                            return false;
                        }
                        return true;
                    });
                    
                    $(f).off("click", "input[type='checkbox']");
                    $(f).on("change", "input[type='checkbox']", function () {
                        var sp = $(this).parents('tr:eq(0)').find("span.move");
                        var ck = $(this).is(":checked");
                        var pid = $(this).attr("value");
                        var ord = $("#ordered").val();
                        $.post("/SearchUsers/TagUntag/" + pid, { ischecked: !ck, isordered: ord }, function (ret) {
                            if (ck && !ret)
                                sp.html("<a href='#' class='move' value='" + pid + "'>move to top</a>");
                            else
                                sp.empty();
                            if (ret)
                                $("#topid").val(pid);
                        });
                    });
                    $(f).off("click", 'a.move');
                    $(f).on("click", 'a.move', function (ev2) {
                        ev2.preventDefault();
                        var f1 = $(this).closest('form');
                        $("#topid").val($(this).attr("value"));
                        var q = f1.serialize();
                        $.post("/SearchUsers/MoveToTop", q, function (ret) {
                            $('table.results', f1).replaceWith(ret).ready(function () {
                                $('table.results > tbody > tr:even', f1).addClass('alt');
                            });
                        });
                    });

                    f.on('hidden', function () {
                        d.remove();
                        f.remove();
                    });
                });




                
                //$("<div class='dialog' style='margin: 5px'>Loading...</div>").dialog({
                //    closeOnEscape: true,
                //    title: opts.title || "Select Users",
                //    width: '550px'
                //}).bind("dialogclose", function () {
                //    $(this).dialog("destroy");
                //}).load(url, function () {
                //    var d = $(this);
                //    d.dialog("option", "position", ["center", "center"]);
                //    d.dialog("option", "width", d.offsetWidth + 10);
                //    $('table.results > tbody > tr:even', d).addClass('alt');
                //    $(".bt").button();
                //    $(".UpdateSelected", $(this)).click(function (ev2) {
                //        ev2.preventDefault();
                //        var topid = $("table.results tbody tr:first ", d).find("input[type=checkbox]").attr("value");
                //        var topid0 = $("#topid0").val();
                //        if (opts.UpdateShared)
                //            opts.UpdateShared(topid, topid0, $this);
                //        d.dialog("close");
                //        return false;
                //    });
                //    var f = $("a.search", d).closest('form');
                //    f.submit(function () {
                //        $("a.search", d).click();
                //        return false;
                //    });
                //    $("a.newsearch", d).click(function (ev2) {
                //        ev2.preventDefault();
                //        $("#searchname").val('');
                //        var q = f.serialize();
                //        $.post($(this).attr('href'), q, function (ret) {
                //            $('table.results', f).replaceWith(ret);
                //            $('table.results > tbody > tr:even', f).addClass('alt');
                //        });
                //        return false;
                //    });
                //    $("a.search", d).click(function (ev2) {
                //        ev2.preventDefault();
                //        var q = f.serialize();
                //        $.post($(this).attr('href'), q, function (ret) {
                //            $('table.results', f).replaceWith(ret);
                //            $('table.results > tbody > tr:even', f).addClass('alt');
                //        });
                //        return false;
                //    });
                //    $("a.close", d).click(function (ev2) {
                //        ev2.preventDefault();
                //        d.dialog("close");
                //        return false;
                //    });
                //    $("#searchname").live("keypress", function (e) {
                //        if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
                //            e.preventDefault();
                //            $('a.search').click();
                //            return false;
                //        }
                //        return true;
                //    });
                //    $(d).off('click', 'input[type=checkbox]');
                //    $(d).on("change", "input[type=checkbox]", function () {
                //        var sp = $(this).parents('tr:eq(0)').find("span.move");
                //        var ck = $(this).is(":checked");
                //        var pid = $(this).attr("value");
                //        var ord = $("#ordered").val()
                //        //$("#topid").val($(this).attr("value"));
                //        $.post("/SearchUsers/TagUntag/" + pid, { ischecked: !ck, isordered: ord }, function (ret) {
                //            if (ck && !ret)
                //                sp.html("<a href='#' class='move' value='" + pid + "'>move to top</a>");
                //            else
                //                sp.empty();
                //            if (ret)
                //                $("#topid").val(pid);
                //        });
                //    });
                //    $(d).off("click", 'a.move');
                //    $(d).on("click", 'a.move', function (ev2) {
                //        ev2.preventDefault();
                //        var f1 = $(this).closest('form');
                //        $("#topid").val($(this).attr("value"));
                //        var q = f1.serialize();
                //        $.post("/SearchUsers/MoveToTop", q, function (ret) {
                //            $('table.results', f1).replaceWith(ret).ready(function () {
                //                $('table.results > tbody > tr:even', f1).addClass('alt');
                //            });
                //        });
                //    });
                //});
                return false;
            });
        });
    };
    function debug($obj) {
        if (window.console && window.console.log)
            window.console.log('SearchUsers selection count: ' + $obj.size());
    };
})(jQuery);
