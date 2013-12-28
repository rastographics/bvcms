$(document).ready(function () {
	// override the getTable function in pager.js
	$.getTable = function () {
		var f = $("#form").submit();
		return false;
	};
	$('#filter').click(function (ev) {
		ev.preventDefault();
		$.getTable();
	});
	$("#clear").click(function (ev) {
		ev.preventDefault();
		$("input:text").val("");
		$('input:checkbox').removeAttr('checked');
		$('#peopleid').val(0);
		$("#activity").val(0);
		$("#accesstype").val(0);
		$.getTable();
	});
	$(".bt").button();
	$(".datepicker").datepicker();
});
