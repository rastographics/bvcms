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
                $('#errormessage').text(ret.error);
            } else if (ret.amt && ret.amt > 0) {
                $('#errormessage').text('');
                $('#amt').text(ret.amt);
                $('#pf_AmtToPay').val(ret.tiamt);
                $('#pf_Amtdue').val(ret.amtdue);
                $('#pf_Coupon').val('');
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
    $('#pf_Coupon').showPassword();

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
        if (!$("#Submit").val())
            return false;
        if ($("form").valid())
            $("#Submit").attr("disabled", "disabled");
        return true;
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
        $("#Submit").attr("disabled", "true");
        $.EnableSubmit();
    };
    $("body").on("change", 'input[name=Type]', function () {
        var v = $("input[name=Type]:checked").val();
        $("#pf_Type").val(v);
        $.ShowPaymentInfo(v);
    });
    if ($("#allowcc").val()) {
        $.ShowPaymentInfo($("#pf_Type").val()); // initial setting
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
            "pf.First": { required: true, maxlength: 50 },
            "pf.MiddleInitial": { maxlength: 1},
            "pf.Last": { required: true, maxlength: 50 },
            "pf.Suffix": { maxlength: 10 },
            "pf.Address": { required: true, maxlength: 50 },
            "pf.City": { required: true, maxlength: 50 },
            "pf.State": { required: true, maxlength: 4 },
            "pf.Zip": { required: true, maxlength: 15 },
            "pf.Phone": { maxlength: 50 }
        },
        errorPlacement: function(error, element) {
            if (element.hasClass("clearField")) {
                $("#errorName").append(error);
            }
            else {
                error.insertAfter(element);
            }
        }
    });

});

