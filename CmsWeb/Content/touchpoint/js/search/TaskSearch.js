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
//    $("#Search_Owner").autocomplete({
//        source: function (request, response) {
//            $.extend(request, { options: $("#Options").val() });
//            $.post("/TaskSearch/OwnerNames/", request,
//                function (ret) { response(ret.slice(0, 10)); }, "json");
//        },
//        select: function (event, ui) {
//            $("[id^=Search_]").val('');
//            $("#Search_Owner").val(ui.item.value);
//            $("#filter").click();
//            return false;
//        }
//    });
//    $("#Search_Originator").autocomplete({
//        source: function (request, response) {
//            $.extend(request, { options: $("#Options").val() });
//            $.post("/TaskSearch/OrginatorNames", request,
//                function (ret) { response(ret.slice(0, 10)); }, "json");
//        },
//        select: function (event, ui) {
//            $("[id^=Search_]").val('');
//            $("#Search_Orginator").val(ui.item.value);
//            $("#filter").click();
//            return false;
//        }
//    });
//    $("#Search_About").autocomplete({
//        source: function (request, response) {
//            $.extend(request, { options: $("#Options").val() });
//            $.post("/TaskSearch/AboutNames", request,
//                function (ret) { response(ret.slice(0, 10)); }, "json");
//        },
//        select: function (event, ui) {
//            $("[id^=Search_]").val('');
//            $("#Search_About").val(ui.item.value);
//            $("#filter").click();
//            return false;
//        }
//    });
});