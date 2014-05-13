function noBack() { window.history.forward(); }
$(function () {
    noBack();
    $("#applydonation").click(function (ev) {
        ev.preventDefault();
        return false;
    });
    $("a.submitbutton, a.submitlink, input.submitbutton.ajax").click(function (ev) {
        ev.preventDefault();
        if (!agreeterms) {
            alert("must agree to terms");
            return false;
        }
        var f = $(this).closest('form');
        var q = f.serialize();
        $.post($(this).attr('href'), q, function (ret) {
            if (ret.error) {
                $('#validatecoupon').text(ret.error);
            } else if (ret.amt && ret.amt > 0) {
                $('#validatecoupon').text('');
                $('#amt').text(ret.amt);
                $('#AmtToPay').val(ret.tiamt);
                $('#Amtdue').val(ret.amtdue);
                $('#Coupon').val('');
                $('td.coupon').html(ret.msg);
            } else {
                window.location = ret.confirm;
            }
        });
        return false;
    });
    $('.clearField').each(function () {
        if ($(this).val() == '') {
            $(this).val($(this).attr('default'));
            $(this).addClass('text-label');
        }
        $(this).focus(function () {
            if (this.value == $(this).attr('default')) {
                this.value = '';
                $(this).removeClass('text-label');
            }
        });
        $(this).blur(function () {
            if (this.value == '') {
                this.value = $(this).attr('default');
                $(this).addClass('text-label');
            }
        });
    });
    $('#Coupon').showPassword();

    $('#findidclick').click(function (ev) {
        ev.preventDefault();
        $("#findid").dialog({ width: 400 });
        return false;
    });
    $('#findacctclick').click(function (ev) {
        ev.preventDefault();
        $("#findacct").dialog({ width: 450 });
        return false;
    });
    var agreeterms = true;
    $("form").submit(function () {
        if (!agreeterms) {
            alert("must agree to terms");
            return false;
        }
        if (!$("#submitit").val())
            return false;
        if ($("form").valid())
            $("#submitit").attr("disabled", "disabled");
        return true;
    });

    $("#Terms").dialog({ autoOpen: false });
    $("#displayterms").click(function () {
        $("#Terms").dialog("open");
    });

    if ($('#IAgree').attr("id")) {
        $("#submitit").attr("disabled", "disabled");
        $("a.submitbutton").attr("disabled", "disabled");
        $("#ApplyCoupon").attr("disabled", "disabled");
        agreeterms = false;
    }
    $("#IAgree").click(function () {
        var checked_status = this.checked;
        if (checked_status == true) {
            agreeterms = true;
            $.EnableSubmit();
            $("#ApplyCoupon").removeAttr("disabled");
        } else {
            agreeterms = false;
            $("#Submit").attr("disabled", "disabled");
            $("a.submitbutton").attr("disabled", "disabled");
            $("#ApplyCoupon").attr("disabled", "disabled");
        }
    });
    $.ShowPaymentInfo = function (v) {
        $(".Card").hide();
        $(".Bank").hide();
        if (v === 'C')
            $(".Card").show();
        else if (v === 'B')
            $(".Bank").show();
        $("#submitit").attr("disabled", "true");
        $.EnableSubmit();
    };
    $.EnableSubmit = function () {
        var vv;
        if ($("input[name='Type'][type=hidden]"))
            vv = $("input[name='Type']").val();
        else
            vv = $("input[name=Type]:checked").val();
        if (vv && agreeterms) {
            $("#submitit").removeAttr("disabled");
            $("a.submitbutton").removeAttr("disabled");
        }
    };
    $("body").on("change", 'input[name=Type]', function () {
        var v = $("input[name=Type]:checked").val();
        $.ShowPaymentInfo(v);
    });
    if ($("#allowcc").val()) {
        var v = $("input[name=Type]:checked").val();
        $.ShowPaymentInfo(v); // initial setting
    }
    $.validator.setDefaults({
        highlight: function (input) {
            $(input).addClass("ui-state-highlight");
        },
        unhighlight: function (input) {
            $(input).removeClass("ui-state-highlight");
        }
    });
    // validate signup form on keyup and submit
    $("form").validate({
        rules: {
            "First": { required: true, maxlength: 50 },
            "MiddleInitial": { maxlength: 1 },
            "Last": { required: true, maxlength: 50 },
            "Suffix": { maxlength: 10 },
            "Address": { required: true, maxlength: 50 },
            "City": { required: true, maxlength: 50 },
            "State": { required: true, maxlength: 4 },
            "Zip": { required: true, maxlength: 15 },
            "Phone": { maxlength: 50 }
        },
        errorPlacement: function (error, element) {
            if (element.hasClass("clearField")) {
                $("#errorName").append(error);
            }
            else {
                error.insertAfter(element);
            }
        }
    });
});

