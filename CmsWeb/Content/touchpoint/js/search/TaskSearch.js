$(function() {
    $.InitFunctions.UpdateTotal = function() {
        $('#totalcount').text($('#totcnt').val());
    };

    $("form")
        .on("keypress",
            'input',
            function(e) {
                if ((e.which && e.which === 13) || (e.keyCode && e.keyCode === 13)) {
                    $('#filter').click();
                    return false;
                }
                return true;
            });
    $("#form-task-search").on("click",
        "div.cell a[data-filter]",
        function(e) {
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
        function() {
            var autocomplete = $(this).data("autocomplete"),
                menu = autocomplete.menu;
            if (!autocomplete.options.selectFirst) {
                return;
            }
            menu.activate($.Event({ type: "mouseenter" }), menu.element.children().first());
        });

    $("input.person").autocomplete({
        source: function(request, response) {
            $.extend(request, { options: $("#Options").val() });
            var w = this.element[0].id.split("_")[1];
            $.post("/TaskSearch/" + w + "Names",
                request,
                function(ret) { response(ret.slice(0, 10)); },
                "json");
        },
        select: function(event, ui) {
            $("input.person").val('');
            var w = event.target.id.split("_")[1];
            $("#Search_" + w).val(ui.item.value);
            $("#filter").click();
            return false;
        }
    });
    $("#archive").click(function(ev) {
        ev.preventDefault();
        var url = $(this)[0].href;
        return archivetasks(url, "archive");
    });
    $("#unarchive").click(function(ev) {
        ev.preventDefault();
        var url = $(this)[0].href;
        return archivetasks(url, "unarchive");
    });
    $("#delete").click(function(ev) {
        ev.preventDefault();
        var url = $(this)[0].href;
        return archivetasks(url, "delete");
    });
    $("#complete").click(function(ev) {
        ev.preventDefault();
        var url = $(this)[0].href;
        return archivetasks(url, "complete");
    });
    function archivetasks (url, dowhat) {
        var cnt = $("#results").find('input[name="SelectedItem"]:checked').length;
        if (cnt === 0)
            swal("Sorry...", "You must check some boxes in the list", "error");
        else
            swal({
                title: "Are you sure you want to " + dowhat + " " + cnt + " tasks?",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-warning",
                confirmButtonText: "Yes, " + dowhat + " them!",
                closeOnConfirm: false
            },
            function() {
                var f = $("#form-task-search");
                f.attr("action", url);
                f.submit();
            });
    }
    $("#delegate").click(function(ev) {
        ev.preventDefault();
        var cnt = $("#results").find('input[name="SelectedItem"]:checked').length;
        if (cnt === 0)
            swal("Sorry...", "You must check some boxes in the list", "error");
        else
            swal({
                title: "Are you sure you want to delegate " + cnt + " tasks?",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-warning",
                confirmButtonText: "Yes, delegate them!",
                closeOnConfirm: true
            },
            function () {
                $("#delegateall").click();
            });
        return false;
    });
    var lastChecked = null;
    $("#results-table").on("click", "input[name='SelectedItem']", null, function(e) {
        if (e.shiftKey && lastChecked !== null) {
            var start = $("#results-table input[name='SelectedItem']").index(this);
            var end = $("#results-table input[name='SelectedItem']").index(lastChecked);
            $("#results-table input[name='SelectedItem']")
                .slice(Math.min(start, end), Math.max(start, end) + 1)
                .prop("checked", true);
        }
        lastChecked = this;
    });

    function initializePopovers() {
        $('[data-toggle="popover"]').popover({ html: true });
        $('[data-toggle="popover"]').click(function(ev) {
            ev.preventDefault();
        });
    }

    initializePopovers();
});
function AddSelected(ret) {
    var f = $("#form-task-search");
    f.attr("action", "/TaskSearch/Delegate/" + ret.pid);
    f.submit();
}
