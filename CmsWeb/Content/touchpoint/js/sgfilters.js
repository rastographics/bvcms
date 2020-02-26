$(function () {
    initializeHooks();

    function initializeHooks() {
        $('form').on('reset', function (e) {
            window.setTimeout(function () {
                submitForm();
            });
        });

        $('.update-control').change(function () {
            submitForm();
        });
    }

    function submitForm() {
        var data = $('#search-form').serialize();
        var urlParams = new URLSearchParams(window.location.search);

        var sgfId = 'main';
        if (urlParams.has('id')) {
            sgfId = urlParams.get('id');
        }


        $.ajax({
            type: 'POST',
            url: '/SmallGroupFinder/GetMapContent?id=' + sgfId,
            data: data,
            success: function (ret) {
                $('.map-content').replaceWith(ret);
                if (typeof initializeMaps !== "undefined")
                    initializeMaps();
            }
        });
    }
});
