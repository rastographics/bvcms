$(function () {
    $(".datepicker").datepicker();
	$("#run").click(function (ev) {
	    ev.preventDefault();
	    if (!$.DateValid($("#Dt1").val(), true))
	        return;
	    if (!$.DateValid($("#Dt2").val(), true))
	        return;
	    var f = $(this).closest('form');
		var q = f.serialize();
		$.post("/FinanceReports/TotalsByFundResults", q, function (ret) {
		    $("#results").html(ret);
		});
	});
	$("#exportdonordetails").click(function (ev) {
		ev.preventDefault();
		var f = $(this).closest('form');
		f.attr("action", "/Export/Contributions/donordetails");
	    f.submit();
	});
	$("#exportdonorfundtotals").click(function (ev) {
		ev.preventDefault();
		var f = $(this).closest('form');
		f.attr("action", "/Export/Contributions/donorfundtotals");
	    f.submit();
	});
	$("#exportdonortotals").click(function (ev) {
		ev.preventDefault();
		var f = $(this).closest('form');
		f.attr("action", "/Export/Contributions/donortotals");
	    f.submit();
	});
	$("#glextract").click(function (ev) {
		ev.preventDefault();
		var f = $(this).closest('form');
		f.attr("action", "/Export/GLExport");
	    f.submit();
	});
    $(document).keydown(function(e) {
        if (e.keyCode == 17) {
            $("#extractlist li.hideAlt").toggle();
        }
    });
	$("#toquickbooks").click(function (ev) {
		ev.preventDefault();

		$.blockUI({
			theme: true,
			title: 'QuickBooks Export',
			message: '<p>Pushing data to QuickBooks, please wait...</p>'
		});

		var f = $(this).closest('form');
		var q = f.serialize();
		$.post("/FinanceReports/ToQuickBooks", q, function (ret) { $.unblockUI(); });
	});
	$("#IncUnclosedBundles").click(function (ev) {
	    if (this.checked)
	        $("#toquickbooks").css( "display", "none" );
	    else
	        $("#toquickbooks").css( "display", "inline" );
	});
	$(".bt").button();
});
