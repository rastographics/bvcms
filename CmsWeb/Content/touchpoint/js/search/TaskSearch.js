$(function () {
    $.InitFunctions.UpdateTotal = function () {
        $('#totalcount').text($('#totcnt').val());
    };

    $("form")
        .on("keypress",
            'input',
            function (e) {
                if ((e.which && e.which === 13) || (e.keyCode && e.keyCode === 13)) {
                    $('#filter').click();
                    return false;
                }
                return true;
            });
    $("#form-task-search").on("click",
        "div.cell a[data-filter]",
        function (e) {
            e.preventDefault();
            var a = $(this).data("filter").split("-", 3);
            var box = $("#Search_" + a[0]);
            // ReSharper disable once CoercedEqualsUsing
            var same = box.val() == a[1] || box.val() === a[2];
            $("input.person").val('');
            if (!same)
                box.val(a[1]);
            $("#filter").click();
            return false;
        });
    $(".ui-autocomplete-input").on("autocompleteopen",
        function () {
            var autocomplete = $(this).data("autocomplete"),
                menu = autocomplete.menu;
            if (!autocomplete.options.selectFirst) {
                return;
            }
            menu.activate($.Event({ type: "mouseenter" }), menu.element.children().first());
        });

    $("input.person").autocomplete({
        source: function (request, response) {
            $.extend(request, { options: $("#Options").val() });
            var w = this.element[0].id.split("_")[1];
            $.post("/TaskSearch/" + w + "Names", request,
                function (ret) { response(ret.slice(0, 10)); }, "json");
        },
        select: function (event, ui) {
            $("input.person").val('');
            var w = event.target.id.split("_")[1];
            $("#Search_" + w).val(ui.item.value);
            $("#filter").click();
            return false;
        }
    });
    $("#archive").click(function (ev) {
        ev.preventDefault();
        var url = $(this)[0].href;
        var cnt = 0;
        var request = $("#form-task-search").serialize();
        $.post("/TaskSearch/ArchiveCount", request, function (data) {
            cnt = parseInt(data);
            swal({
                title: "Are you sure you want to archive " + cnt + " tasks?",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-warning",
                confirmButtonText: "Yes, archive them!",
                closeOnConfirm: false
            },
            function () {
                var f = $("#form-task-search");
                f.attr("action", url);
                f.submit();
            });
        });
        return false;
    });
    function initializePopovers() {
        $('[data-toggle="popover"]').popover({ html: true });
        $('[data-toggle="popover"]').click(function (ev) {
            ev.preventDefault();
        });
    }
    initializePopovers();
});