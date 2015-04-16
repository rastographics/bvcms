$(function () {

    var lastChecked = null;
    $(document).on("click", "input.check", null, function (e) {
        if (e.shiftKey && lastChecked !== null) {
            var start = $('input.check').index(this);
            var end = $('input.check').index(lastChecked);
            $('input.check').slice(Math.min(start, end), Math.max(start, end) + 1).prop("checked", true);
        }
        lastChecked = this;
        $.UpdateTotals();
    });

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

    $.RefreshPage = function () {
        var q = $('#form').serialize();
        $.navigate("/Promotion/Reload", q);
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

        var pboys = (boys / total * 100).toFixed(0);
        if (isNaN(pboys)) {
            $('#pboys').text('0%');
        } else {
            $('#pboys').text(pboys + '%');
        }

        var pgirls = (girls / total * 100).toFixed(0);
        if (isNaN(pgirls)) {
            $('#pgirls').text('0%');
        } else {
            $('#pgirls').text(pgirls + '%');
        }

        var phigh = (high / total * 100).toFixed(0);
        if (isNaN(phigh)) {
            $('#phigh').text('0%');
        } else {
            $('#phigh').text(phigh + '%');
        }
        
        var pmed = (med / total * 100).toFixed(0);
        if (isNaN(pmed)) {
            $('#pmed').text('0%');
        } else {
            $('#pmed').text(pmed + '%');
        }

        var plow = (low / total * 100).toFixed(0);
        if (isNaN(plow)) {
            $('#plow').text('0%');
        } else {
            $('#plow').text(plow + '%');
        }
    }
    $.UpdateTotals();

    $('#PromotionId').change($.RefreshPage);
    $('#ScheduleId').change($.RefreshPage);
    $('#FilterUnassigned').click($.RefreshPage);
    $('#NormalMembersOnly').click($.RefreshPage);
    $('body').on('click', 'input.check', $.UpdateTotals);
});

