$(function () {
    $.dpoptions = {
        dateFormat: $.dateFormat,
        changeMonth: true,
        changeYear: true,
        yearRange: "-95:+0",
        showOn: "button",
        buttonImage: "/Content/images/calendar.gif",
        buttonImageOnly: true,
        onSelect: function (dateText, inst) {
            var f = $(this).closest('form');
            $("#age", f).text($.dodate(dateText));
        }
    };
    //$("#dob").datepicker($.dpoptions);
    $.dodate = function (bd) {
        var re0 = /^(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])((19|20)?[0-9]{2})$/i;
        var re = /^(0?[1-9]|1[012])[\/-](0?[1-9]|[12][0-9]|3[01])[\/-]((19|20)?[0-9]{2})$/i;
        var m = re0.exec(bd);
        if (m == null)
            m = re.exec(bd);
        if (m == null)
            return;

        var y = parseInt(m[3]);
        if (y < 1000)
            if (y < 50) y = y + 2000; else y = y + 1900;
        var bday = new Date(y, m[1] - 1, m[2]);
        var tday = new Date();
        if (bday > tday)
            bday = new Date(y - 100, m[1] - 1, m[2]);

        var by = bday.getFullYear();
        var bm = bday.getMonth();
        var bd = bday.getDate();
        var age = 0;
        while (bday <= tday) {
            bday = new Date(by + age, bm, bd);
            age++;
        }
        return age - 2;
    };
    $("form.DisplayEdit").on("blur", "input.dob", function () {
        var f = $(this).closest('form');
        $("#age", f).text($.dodate($(this).val()));
    });
    $("form.DisplayEdit").on("click", "a.submitbutton, a.submitlink, input.submitbutton.ajax", function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        var q = f.serialize();
        $.refreshenabled = true;
        $.post($(this).attr('href'), q, function (ret) {
            if (ret.charAt(0) === '/') {
                $("body").html("<p>Please wait...</p>");
                window.location = ret;
                return;
            }
            $(f).html(ret).ready(function () {
                if ($("#submitit").attr("onlyoneallowed") == "true" && $(".input-validation-error", f).length === 0) {
                    $.InstructionsShow();
                    f.submit();
                }
                else {
                    $.InstructionsShow();
                    $("#dob").jqdatepicker($.dpoptions);
                }
            });
        });
        return false;
    });
    $.setButtons = function () {
        $(".submitbutton").button();
    };
    $.ShowPaymentInfo = function () {
        var v = $("input[name=Type]:checked").val();
        $("div.Card").hide();
        $("div.Bank").hide();
        $("div.accountinfo").hide();
        if (v === 'C') {
            $("div.Card").show();
            $("div.accountinfo").show();
        } else if (v === 'B') {
            $("div.Bank").show();
            $("div.accountinfo").show();
        }
    };
    $.InstructionsShow = function () {
        $("div.instructions").hide();
        if ($("#selectfamily").attr("id"))
            $("div.instructions.select").show();
        else if ($("#personedit").attr("id")) {
            $("#fillout").hide();
            $("div.instructions.find").show();
        }
        else if ($("#otheredit").attr("id"))
            $("div.instructions.options").show();
        else if ($("#specialedit").attr("id"))
            $("div.instructions.special").show();
        else if ($("#username").attr("id")) {
            $("#username").focus();
            $("div.instructions.login").show();
        }
        else if ($("#submitit").attr("id"))
            $("div.instructions.submit").show();
        else if ($("#sorry").attr("id"))
            $("div.instructions.sorry").show();
        if ($("#allowcc").val())
            $.ShowPaymentInfo();
    };
    $("form.DisplayEdit").submit(function () {
        if (!$("#submitit").val())
            return false;
        $("#submitit").attr("disabled", "true");
        return true;
    });
    $("form.DisplayEdit").on("click", "a.cancel", function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        var q = f.serialize();
        $.post($(this).attr('href'), q, function (ret) {
            if (ret == 'refresh')
                location.reload();
            $(f).html(ret).ready(function () {
                setTimeout($.setButtons, 15);
            });
        });
        return false;
    });
    $(document).on("click", "#copy", function () {
        $("input[name$='.emcontact']:last").val($("input[name$='.emcontact']:hidden:last").val());
        $("input[name$='.emphone']:last").val($("input[name$='.emphone']:hidden:last").val());
        $("input[name$='.insurance']:last").val($("input[name$='.insurance']:hidden:last").val());
        $("input[name$='.policy']:last").val($("input[name$='.policy']:hidden:last").val());
        $("input[name$='.doctor']:last").val($("input[name$='.doctor']:hidden:last").val());
        $("input[name$='.docphone']:last").val($("input[name$='.docphone']:hidden:last").val());
        $("input[name$='.mname']:last").val($("input[name$='.mname']:hidden:last").val());
        $("input[name$='.fname']:last").val($("input[name$='.fname']:hidden:last").val());
        $("input[name$='.paydeposit']:last").val($("input[name$='.paydeposit']:hidden:last").val());
        return false;
    });
    $.InstructionsShow();
    $.validator.setDefaults({
        highlight: function (input) {
            $(input).addClass("ui-state-highlight");
        },
        unhighlight: function (input) {
            $(input).removeClass("ui-state-highlight");
        }
    });
    $("form.DisplayEdit").validate({
        rules: {
            "m.donation": { number: true }
        }
    });
    $(document).on("click", ".personheader a", function (e) {
        e.preventDefault();
        $(this).closest('div').nextAll('table').slideToggle();
        return false;
    });
    $(document).on("change", "input.sum", function () {
        var sum = 0;
        $("input.sum").each(function () {
            if (!isNaN(this.value) && this.value.length != 0) {
                sum += parseFloat(this.value);
            }
        });
        $("#total").html(sum.toFixed(2));
    });
    $(document).on("change", "input[name=Type]", $.ShowPaymentInfo);

    $(document).on("keypress", "#password", function (e) {
        if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
            $('#loginbt').click();
            return false;
        }
        return true;
    });

    // if we are coming back to this page to continue a registration, 
    // check to see if we should be on our way to the next step
    if ($("#submitit").attr("onlyoneallowed") == "true" && $(".input-validation-error", $("#completeReg")).length === 0) {
        $("#completeReg").submit();
    }
});

