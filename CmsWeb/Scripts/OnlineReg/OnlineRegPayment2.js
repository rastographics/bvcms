function noBack() { window.history.forward(); }
$(function () {
    noBack();
    $("div.date input").datepicker({
        autoclose: true,
        orientation: "auto",
        forceParse: false,
        format: $.dtoptions.format
    });
    $("#applydonation").click(function (ev) {
        ev.preventDefault();
        return false;
    });
    $("#formerror").hide();
    $("a.submitbutton, a.submitlink").click(function (ev) {
        ev.preventDefault();
        if (!agreeterms) {
            alert("must agree to terms");
            return false;
        }
        var f = $(this).closest('form');
        var href = this.href;
        var q = f.serialize();
        $.post(href, q, function (ret) {
            if (ret.error) {
                $("#formerror").show();
                $('#errormessage').text(ret.error);
            } else if (ret.amt && ret.amt > 0) {
                $("#formerror").hide();
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
        $("#findid").show();
        return false;
    });
    $('#findacctclick').click(function (ev) {
        ev.preventDefault();
        $("#findacct").show();
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
        if ($("form").valid()) {
            $("#submitit").attr("disabled", "disabled");
            return true;
        }
        return false;
    });

    if ($('#IAgree').attr("id")) {
        $(".showform").hide();
        agreeterms = false;
    }
    $("#IAgree").click(function () {
        var checked_status = this.checked;
        if (checked_status == true) {
            $(".showform").show();
            $("#Terms").hide();
            agreeterms = true;
        } else {
            $(".showform").hide();
            $("#Terms").show();
            agreeterms = false;
        }
    });
    $.ShowPaymentInfo = function (v) {
        $(".Card").hide();
        $(".Bank").hide();
        if (v === 'C')
            $(".Card").show();
        else if (v === 'B')
            $(".Bank").show();
    };
    $.ShowPeriodInfo = function (v) {
        $(".everyPeriod").hide();
        $(".twiceMonthly").hide();
        if (v === 'S')
            $(".twiceMonthly").show();
        else if (v === 'E')
            $(".everyPeriod").show();
    };
    $("body").on("change", 'input[name=Type]', function () {
        var v = $("input[name=Type]:checked").val();
        $.ShowPaymentInfo(v);
    });
    $("body").on("change", 'input[name=SemiEvery]', function () {
        var v = $("input[name=SemiEvery]:checked").val();
        $("#SemiEvery").val(v);
        $.ShowPeriodInfo(v);
    });
    if ($("#allowcc").val()) {
        $.ShowPaymentInfo($("input[name=Type]:checked").val());
    }
    $.ShowPeriodInfo($("input[name=SemiEvery]:checked").val());
    $.validator.setDefaults({
        highlight: function (input) {
            $(input).addClass("input-validation-error");
        },
        unhighlight: function (input) {
            $(input).removeClass("input-validation-error");
        }
    });
    // validate signup form on keyup and submit
    $("form").validate({
        rules: {
            "First": { required: true, maxlength: 50 },
            "MiddleInitial": { maxlength: 1},
            "Last": { required: true, maxlength: 50 },
            "Suffix": { maxlength: 10 },
            "Address": { required: true, maxlength: 50 },
            "Zip": { required: true, maxlength: 15 },
            "Phone": { maxlength: 50 }
        },
        errorPlacement: function(error, element) {
            if (element.hasClass("clearField")) {
                $("#errorName").append(error);
            }
            else {
                error.insertAfter(element);
            }
        },
        errorClass: "field-validation-error"
    });

});

