$(function () {

    function initializePopovers() {
        $('[data-toggle="popover"]').popover({ html: true });
        $('[data-toggle="popover"]').click(function (ev) {
            ev.preventDefault();
        });
    }

    $("#run").click(function (ev) {
        ev.preventDefault();
	    if (!$.DateValid($("#Dt1").val(), true))
	        return;
	    if (!$.DateValid($("#Dt2").val(), true))
	        return;

        $.block();
        // set hidden print form elements
	    $('#campusName').text($("#CampusId option:selected").text());
	    $('#fromDate').text($("#Dt1").val());
	    $('#toDate').text($("#Dt2").val());
	    $('#taxStatus').text($("#TaxDedNonTax option:selected").text());
	    $('#sourceName').text($("#Online option:selected").text());
	    $('#includeUnClosedBundles').text($("#IncUnclosedBundles").prop('checked'));
	    $('#includeBundleType').text($("#IncludeBundleType").prop('checked'));
        
	    var f = $(this).closest('form');
		var q = f.serialize();
		$.post("/FinanceReports/TotalsByFundResults", q, function (ret) {
		    $.unblock();
		    $("#results").html(ret);
		    initializePopovers();
		});
    });

    $("a.CustomReport").click(function (ev) {
        ev.preventDefault();
	    if (!$.DateValid($("#Dt1").val(), true))
	        return;
	    if (!$.DateValid($("#Dt2").val(), true))
            return;
        var href = this.href;

        $.block();
        // set hidden print form elements
	    $('#campusName').text($("#CampusId option:selected").text());
	    $('#fromDate').text($("#Dt1").val());
	    $('#toDate').text($("#Dt2").val());
	    $('#taxStatus').text($("#TaxDedNonTax option:selected").text());
	    $('#sourceName').text($("#Online option:selected").text());
	    $('#includeUnClosedBundles').text($("#IncUnclosedBundles").prop('checked'));
	    $('#includeBundleType').text($("#IncludeBundleType").prop('checked'));
        
	    var f = $(this).closest('form');
		var q = f.serialize();
		$.post(href, q, function (ret) {
		    $.unblock();
		    $("#results").html(ret);
		    initializePopovers();
		});
    });
    $("#results").on('click', 'a.CustomExport', function (ev) {
        ev.preventDefault();
	    if (!$.DateValid($("#Dt1").val(), true))
	        return;
	    if (!$.DateValid($("#Dt2").val(), true))
            return;

        var href = this.href;
        // set hidden print form elements
	    $('#campusName').text($("#CampusId option:selected").text());
	    $('#fromDate').text($("#Dt1").val());
	    $('#toDate').text($("#Dt2").val());
	    $('#taxStatus').text($("#TaxDedNonTax option:selected").text());
	    $('#sourceName').text($("#Online option:selected").text());
	    $('#includeUnClosedBundles').text($("#IncUnclosedBundles").prop('checked'));
	    $('#includeBundleType').text($("#IncludeBundleType").prop('checked'));

        var f = $("#totals");
        f.attr("action", href);
        f.submit();
        return false;
    });

    $("#reconcile").click(function (ev) {
        ev.preventDefault();
	    if (!$.DateValid($("#Dt1").val(), true))
	        return;
	    if (!$.DateValid($("#Dt2").val(), true))
	        return;

        $.block();
        // set hidden print form elements
	    $('#campusName').text($("#CampusId option:selected").text());
	    $('#fromDate').text($("#Dt1").val());
	    $('#toDate').text($("#Dt2").val());
	    $('#taxStatus').text($("#TaxDedNonTax option:selected").text());
	    $('#sourceName').text($("#Online option:selected").text());
	    $('#includeUnClosedBundles').text($("#IncUnclosedBundles").prop('checked'));
	    $('#includeBundleType').text($("#IncludeBundleType").prop('checked'));
        
	    var f = $(this).closest('form');
		var q = f.serialize();
		$.post("/FinanceReports/TotalsByFundExport", q, function (ret) {
		    $.unblock();
		    $("#results").html(ret);
		    initializePopovers();
		});
    });

	$("#ledgerincome").click(function (ev) {
		ev.preventDefault();
		var f = $(this).closest('form');
		f.attr("action", "/Export2/Contributions/ledgerincome");
	    f.submit();
	});

	$("#exportdonordetails").click(function (ev) {
		ev.preventDefault();
		var f = $(this).closest('form');
		f.attr("action", "/Export2/Contributions/donordetails");
	    f.submit();
	});

	$("#exportdonorfundtotals").click(function (ev) {
		ev.preventDefault();
		var f = $(this).closest('form');
		f.attr("action", "/Export2/Contributions/donorfundtotals");
	    f.submit();
	});

	$("#exportdonortotals").click(function (ev) {
		ev.preventDefault();
		var f = $(this).closest('form');
		f.attr("action", "/Export2/Contributions/donortotals");
	    f.submit();
	});

	$("#glextract").click(function (ev) {
		ev.preventDefault();
		var f = $(this).closest('form');
		f.attr("action", "/Export2/GLExport");
	    f.submit();
	});

	$(document).keydown(function (e) {
	    if (e.keyCode == 17) {
	        $('#gl-divider').toggle();
	        $('#gl-extract').toggle();
        }
    });

	initializePopovers();
});
