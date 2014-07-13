$(function () {
    $(".bt").button();
    $('#PromotionId').change($.RefreshPage);
    $('#ScheduleId').change($.RefreshPage);
    $('#FilterUnassigned').click($.RefreshPage);
    $('#NormalMembersOnly').click($.RefreshPage);
    $('input.check').live("click", $.UpdateTotals);
    $('#Promotions > thead a.sortable').click(function (ev) {
        ev.preventDefault();
        var newsort = $(this).text();
        var oldsort = $("#Sort").val();
        $("#Sort").val(newsort);
        var dir = $("#Dir").val();
        if (oldsort == newsort && dir == 'asc')
            $("#Dir").val('desc');
        else
            $("#Dir").val('asc');
        $.RefreshList();
        return false;
    });
    $(window).scroll(function () {
        $('#float_box').animate({ top: $(window).scrollTop() + 90 + "px" }, { queue: false, duration: 350 });
    });
    $('#close_float').click(function () {
        $('#float_box').animate({ top: "+=15px", opacity: 0 }, "slow");
    });
    var ttotal = $('.check').length;
    var tboys = $('.check[gender=M]').length;
    var tgirls = $('.check[gender=F]').length;
    var thigh = $('.check[attend=Hi]').length;
    var tmed = $('.check[attend=Med]').length;
    var tlow = $('.check[attend=Lo]').length;
    $('#ttotal').text(ttotal);
    $('#tboys').text(tboys);
    $('#tgirls').text(tgirls);
    $('#thigh').text(thigh);
    $('#tmed').text(tmed);
    $('#tlow').text(tlow);

    $('#tpboys').text((tboys / ttotal * 100).toFixed(0) + '%');
    $('#tpgirls').text((tgirls / ttotal * 100).toFixed(0) + '%');
    $('#tphigh').text((thigh / ttotal * 100).toFixed(0) + '%');
    $('#tpmed').text((tmed / ttotal * 100).toFixed(0) + '%');
    $('#tplow').text((tlow / ttotal * 100).toFixed(0) + '%');

    $.RefreshPage = function() {
        var q = $('#form').serialize();
        $.navigate("/Promotion", q);
    }

    $.RefreshList = function () {
        var q = $('#form').serialize();
        $.post('/Promotion/List/', q, function (ret) {
            $('#Promotions > tbody').html(ret);
        });
    }

    $.UpdateTotals = function() {
        var total = $('.check:checked').length;
        var boys = $('.check:checked[gender=M]').length;
        var girls = $('.check:checked[gender=F]').length;
        var high = $('.check:checked[attend=Hi]').length;
        var med = $('.check:checked[attend=Med]').length;
        var low = $('.check:checked[attend=Lo]').length;

        $('#total').text(total);
        $('#boys').text(boys);
        $('#girls').text(girls);
        $('#high').text(high);
        $('#med').text(med);
        $('#low').text(low);

        $('#pboys').text((boys / total * 100).toFixed(0) + '%');
        $('#pgirls').text((girls / total * 100).toFixed(0) + '%');
        $('#phigh').text((high / total * 100).toFixed(0) + '%');
        $('#pmed').text((med / total * 100).toFixed(0) + '%');
        $('#plow').text((low / total * 100).toFixed(0) + '%');
    }
    $.UpdateTotals();
});

