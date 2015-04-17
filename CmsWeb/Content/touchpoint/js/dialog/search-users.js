(function ($) {
    $.fn.SearchUsers = function (options) {
        var opts = $.extend({}, {}, options);
        return this.each(function () {
            var $this = $(this);
            $this.click(function (ev) {
               ev.preventDefault();
                var url = $(this).attr('href');

                $("<form id='search-add' class='modal-form validate ajax' />").load(url, {}, function () {
                    var f = $(this);
                    $('#empty-dialog').html(f);
                    $('#empty-dialog').modal("show");
            
                    $(f).off("click", ".UpdateSelected");
                    $(f).on("click", ".UpdateSelected", function (ev2) {
                        ev2.preventDefault();
                        var topid = $("table.results tbody tr:first ", f).find("input[type=checkbox]").attr("value");
                        var topid0 = $("#topid0").val();
                        if (opts.UpdateShared)
                            opts.UpdateShared(topid, topid0, $this);
                        $('#empty-dialog').modal("hide");
                        return false;
                    });
                    $(f).off("keypress", "#searchname");
                    $(f).on("keypress", "#searchname", function (e) {
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
                        $('#empty-dialog').remove();
                    });
                });


                return false;
            });
        });
    };
})(jQuery);
