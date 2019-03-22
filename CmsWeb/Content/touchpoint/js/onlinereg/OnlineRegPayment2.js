function noBack() { window.history.forward(); }
$(function () {
    noBack();
    $("div.date input").datepicker({
        autoclose: true,
        orientation: "auto",
        forceParse: false,
        format: $.dtoptions.format
    }).on('changeDate', function() {
        $("#StartWhenIsNew").val('True');
    });

    $("body").on("change", 'select[name=newRepeatPattern]', function () {
        var v = $("select[name=newRepeatPattern]").val();
        if (v === 'M') { // monthly
            $('#weeklyText').hide();
            $('#twoWeeksText').hide();
            $('#twiceAMonthText').hide();
            $('#monthlyText').show();

            $('#RepeatPattern').val('M');
            $('#EveryN').val('1');
            $('#Day1').val('');
            $('#Day2').val('');
            $('#SemiEvery').val('E');
            $('#Period').val('M');
        }
        else if (v === 'S') { // twice a month
            $('#weeklyText').hide();
            $('#twoWeeksText').hide();
            $('#monthlyText').hide();
            $('#twiceAMonthText').show();

            $('#RepeatPattern').val('S');
            $('#EveryN').val('1');
            $('#Day1').val('1');
            $('#twiceAMonthDay1').editable('setValue', $('#Day1').val());
            $('#Day2').val('15');
            $('#twiceAMonthDay2').editable('setValue', $('#Day2').val());
            $('#SemiEvery').val('S');
            $('#Period').val('M');
        }
        else if (v === 'W') { // weekly
            $('#twoWeeksText').hide();
            $('#twiceAMonthText').hide();
            $('#monthlyText').hide();
            $('#weeklyText').show();

            $('#RepeatPattern').val('W');
            $('#EveryN').val('1');
            $('#Day1').val('');
            $('#Day2').val('');
            $('#SemiEvery').val('E');
            $('#Period').val('W');
        }
        else if (v == '2W') { // every 2 weeks
            $('#weeklyText').hide();
            $('#twiceAMonthText').hide();
            $('#monthlyText').hide();
            $('#twoWeeksText').show();

            $('#RepeatPattern').val('W');
            $('#EveryN').val('2');
            $('#Day1').val('');
            $('#Day2').val('');
            $('#SemiEvery').val('E');
            $('#Period').val('W');
        }
    });

    var daysOfMonth = [];
    var i;
    for (i = 1; i <= 31; i++) {
        daysOfMonth.push({ value: i, text: Humanize.ordinal(i) });
    }
    
    $(".clickSelect").editable({
        mode: 'popup',
        type: 'select',
        showbuttons: false,
        send: 'never',
        source: daysOfMonth,
        placement: 'bottom'
    });

    if ($('#Day1').length && $('#Day1').val().length > 0) {
        $('#twiceAMonthDay1').editable('setValue', $('#Day1').val());
    }

    if ($('#Day2').length && $('#Day2').val().length > 0) {
        $('#twiceAMonthDay2').editable('setValue', $('#Day2').val());
    }
   
    $('#twiceAMonthDay1').on('save', function (e, params) {
        $('#Day1').val(params.newValue);
    });

    $('#twiceAMonthDay2').on('save', function (e, params) {
        $('#Day2').val(params.newValue);
    });

    $('#startWhenDate').click(function(e) {
        e.preventDefault();
    });

    var curDate = new Date();
    $('#startWhenDate').datepicker({
        autoclose: true,
        orientation: "auto",
        forceParse: false,
        format: $.dtoptions.format,
        toggleActive: false,
    }).on('changeDate', dateChanged);

    function dateChanged(e) {
        if (e.format().length > 0) {
            $("#StartWhen").val(e.format());
            $("#StartWhenIsNew").val('True');
            $("#startWhenDate").text(e.format());
            calculateDayOfMonthText(e.format());
            curDate = e.date;
        }
    }

    function calculateDayOfMonthText(date) {
        var dt = moment(date);
        var dayText = dt.format('dddd');
        var dayNum = dt.format('D');

        var result = _.find(daysOfMonth, function (d) { return d.value == dayNum; });

        $('#weeklyTextDay').text(dayText);
        $('#twoWeeksTextDay').text(dayText);
        $('#monthlyTextDay').text(result.text);
    }

    calculateDayOfMonthText($("#StartWhen").val());

    $("#calIcon").datepicker("setDate", new Date());


    $("#applydonation").click(function (ev) {
        ev.preventDefault();
        return false;
    });
    $("#formerror").hide();
    $("#savePayArea").hide();
    $("a.submitbutton, a.submitlink").click(function (ev) {
        ev.preventDefault();
        if (!agreeterms) {
            alert("must agree to terms");
            return false;
        }
        var f = $(this).closest('form');
        var href = this.href;
        var q = f.serialize();
        var $useRecaptcha = $("#useRecaptcha", f);
        $.blockUI();
        if (this.className.includes('coupon-submit')) {
            $useRecaptcha.val('');
        }
        $.post(href, q, function (ret) {
            $.unblockUI();
            if (ret.error) {
                $("#formerror").show();
                $('#errormessage').text(ret.error);
            } else if (ret.amtdue && ret.amtdue > 0) {
                $("#formerror").hide();
                $('#amt').text(ret.amt);
                $('#AmtToPay').val(ret.tiamt);
                $('#Amtdue').val(ret.amtdue);
                $('#Coupon').val('');
                $("#form-msg").show();
                $('#coupon-msg').html(ret.msg);
                $('#ApplyCoupon').hide();
            } else {
                var form = $('#success_form');
                if (ret.formmethod == "GET") {
                    window.location = ret.confirm;
                }
                else {
                    form.attr("action", ret.confirm);
                    form.submit();
                }
            }
            $useRecaptcha.val('value');
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
    $("form.recaptcha").submit(function() {
        if (!agreeterms) {
            alert("must agree to terms");
            return false;
        }
        var submitit = $("#submitit", this); 
        if (!submitit.val()) {
            return false;
        }

        var isFormValid = $(this).valid();
        if (isFormValid) {
            submitit.attr("disabled", "disabled");
            var usecaptcha = $("#useRecaptcha", this).val();
            if (usecaptcha) {
                grecaptcha.execute();
            } else {
                return true;
            }
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
    $.SetSummaryText = function () {
        var pattern = $("#RepeatPattern").val();
        var everyN = $("#EveryN").val();
        var day1 = $("#Day1").val();
        var day2 = $("#Day2").val();
        var startOn = $("#StartWhen").val();

        var summary = "";
        if (pattern === "S") {
            if (day1.length > 0 && day2.length > 0)
                summary = "Twice a month on day " + day1 + " and day " + day2;
        } else {
            var patternText = "";
            if (pattern === "M") {
                patternText = "month";
            } else {
                patternText = "week";
            }
            if (everyN > 1) {
                summary = "Every " + everyN + " " + patternText + "s";
            } else {
                summary = "Every " + everyN + " " + patternText;
            }
        }
        if (startOn && startOn.length > 0)
            summary += " starting on or after " + startOn;
        $("#SummaryText").text(summary);
    };
   
    $.ShowPaymentInfo = function (v) {
        $(".Card").hide();
        $(".Bank").hide();
        if (v === 'C') {
            $(".Card").show();
            if ($('#CreditCard').val().startsWith('X')) {
                $("#savePayArea").hide();
                $('#SavePayInfo').prop('checked', true);
            }
            else {
                $('#savePayArea').show();
                $('#SavePayInfo').prop('checked', false);
            }
            CancelCreditCardInfoUpdate();
        }
        else if (v === 'B') {
            $(".Bank").show();
            if ($('#Routing').val().startsWith('X') && $('#Account').val().startsWith('X')) {
                $("#savePayArea").hide();
                $('#SavePayInfo').prop('checked', true);
            }
            else {
                showSavePayUncheckedBox();
            }
            CancelCreditCardInfoUpdate();
        }
    };
    $.ShowPaymentInfo2 = function (v) {
        $(".Card").hide();
        $(".Bank").hide();
        if (v === 'C') {
            $(".Card").show();
            if ($('#CreditCard').val().startsWith('X')) {
                $("#savePayArea").hide();
                $('#SavePayInfo').prop('checked', true);
            }
            else {
                $('#savePayArea').show();
                $('#SavePayInfo').prop('checked', false);
            }
        }
        else if (v === 'B') {
            $(".Bank").show();
            if ($('#Account').val().startsWith('X')) {
                $("#savePayArea").hide();
                $('#SavePayInfo').prop('checked', true);
            }
            else {
                $('#savePayArea').show();
                $('#SavePayInfo').prop('checked', false);
            }
        }
    };
    $.SetRepeatPatternText = function(v) {
        if (v === 'M') {
            $("#RepeatPatternText").text(" month(s)");
        }
        else
            $("#RepeatPatternText").text(" week(s)");
    };
    $.ShowPeriodInfo = function (v) {
        $(".everyPeriod").hide();
        $(".twiceMonthly").hide();
        if (v === 'S')
            $(".twiceMonthly").show();
        else {
            $(".everyPeriod").show();
            $.SetRepeatPatternText(v);
            $("#Period").val(v);
        }
    };
    $.SetSemiEvery = function(v) {
        if (v != 'S') {
            $("#SemiEvery").val('E');
        } else {
            $("#SemiEvery").val('S');
        }
    };
    $("body").on("change", 'input[name=Type]', function () {
        var v = $("input[name=Type]:checked").val();
        $.ShowPaymentInfo(v);
    });
    $("body").on("change", 'select[name=RepeatPattern]', function () {
        var v = $("select[name=RepeatPattern]").val();
        $.SetSemiEvery(v);
        $.ShowPeriodInfo(v);
        $.SetSummaryText();
    });
    $("body").on("change", 'select[name=EveryN]', function () {
        $.SetSummaryText();
    });
    $("body").on("change", 'input[name=Day1]', function () {
        $.SetSummaryText();
    });
    $("body").on("change", 'input[name=Day2]', function () {
        $.SetSummaryText();
    });
    $("body").on("change", 'input[name=StartWhen]', function () {
        $.SetSummaryText();
    });

    if ($('#CreditCard').length) {
        if ($('#CreditCard').val().startsWith('X')) {
            $('#CVV').parents('.form-group').hide();
            $('#CancelUpdateText').parents('.form-group').hide();
        }
        $('#CreditCard').change(function () {
            $('#CVV').parents('.form-group').show();
            if ($('#CreditCard').val().startsWith('X')) {
                $('#CreditCard').val($('#CreditCard').val().replace('X', 'Y'));                
            }
            showSavePayUncheckedBox();
        });
        $('#Expires').change(function () {
            if ($('#CreditCard').val().startsWith('X')) {
                $('#CreditCard').val($('#CreditCard').val().replace('X', 'Y'));                              
            }
            $('#CVV').parents('.form-group').show();
            showSavePayUncheckedBox();
        });
        $('#CancelUpdateText').click(function () {
            CancelCreditCardInfoUpdate();
        });
    }

    if ($('#Routing').length) {
        $('#Routing').change(function () {
            if ($('#Routing').val().startsWith('X')) {
                $('#Routing').val($('#Routing').val().replace('X', 'Y'));
            }
            showSavePayUncheckedBox();
        });
        $('#Account').change(function () {
            if ($('#Routing').val().startsWith('X')) {
                $('#Routing').val($('#Routing').val().replace('X', 'Y'));
            }
            showSavePayUncheckedBox();
        });
    }

    function showSavePayUncheckedBox() {
        $('#CancelUpdateText').parents('.form-group').show();
        $('#savePayArea').show();
        $('#SavePayInfo').prop('checked', false);
    }

    function CancelCreditCardInfoUpdate() {
        //Getting CC number and Expire date from what is saved in DB
        var sessionCConFile = $("#hdnCreditCardOnFile").data('value');
        var sessionExiresonFile = $("#hdnExpiresOnFile").data('value');
        $('#CreditCard').val(sessionCConFile);
        $('#Expires').val(sessionExiresonFile);
        //Setting CVV to empty and hiding CVV and Cancel Update btn.
        $('#CVV').val('');
        $('#CVV').parents('.form-group').hide();
        $('#CancelUpdateText').parents('.form-group').hide();
        $('#Expires').focus();
        //Removes validation summary
        $('.validation-summary-errors').empty();
        $('.validation-summary-errors').addClass('validation-summary-valid');
        $('.validation-summary-errors').removeClass('validation-summary-errors');
        //Removes validation message after input-fields
        $(".field-validation-error").empty();
        $(".input-validation-error").removeClass("input-validation-error");

        $(".state-error").removeClass("state-error");
        $(".state-success").removeClass("state-success");
        $(this).trigger('reset.unobtrusiveValidation');
    }
    
    if ($("#allowcc").val()) {
        $.ShowPaymentInfo2($("input[name=Type]:checked").val());
    }
    var repeatPattern = $("select[name=RepeatPattern]").val();
    if (!repeatPattern) {
        repeatPattern = $("input[name=RepeatPattern]").val();
    }

    $.SetSemiEvery(repeatPattern);
    $.ShowPeriodInfo(repeatPattern);
    $.SetSummaryText();
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
