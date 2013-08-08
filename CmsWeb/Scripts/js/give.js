$(function() {
	fieldsetDisplay();
	$("input[name=gift_type]").on("change", function() {
		fieldsetDisplay(this);		
	});
});

function fieldsetDisplay(gt) {
	$("#give_form fieldset").not("#gift_type").hide();
	if (gt) $("#give_form fieldset."+$(gt).val()).show();
	if ($("#give_form fieldset:visible").size() > 1) $("#give_form .form-actions").show();
	else $("#give_form .form-actions").hide();
}