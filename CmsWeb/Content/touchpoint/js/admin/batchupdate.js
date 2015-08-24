$(function () {
    $('#begin-update').click(function (ev) {
        ev.preventDefault();
        var f = $(this).closest("form");
        var q = f.serialize();
        $.block();
        $.post(f[0].action, q, function (ret) {
            $('div.alert-success').html(ret).show();
            $('#text').val('');
        })
            .fail(function (xhr, textStatus, errorThrown) {
                $('div.alert-danger').html(xhr.responseText).show();
            })
            .always(function () {
                $.unblock();
            });
        return false;
    });
});