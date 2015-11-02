$(function () {


    CKEDITOR.env.isCompatible = true;

    CKEDITOR.replace('body', {
        height: 400,
        allowedContent: true,
        autoParagraph: false,
        fullPage: !$("#snippet").prop("checked"),
        customConfig: "/scripts/js/ckeditorconfig.js"
    });

    $("#snippet").change(function () {
        var checked = this.checked;
        swal({
            title: "Reload page?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-warning",
            confirmButtonText: "Yes!",
            closeOnConfirm: false
        },
        function () {
            if (checked)
                window.location.href = window.location.pathname + '?snippet=true';
            else
                window.location.href = window.location.pathname;
        });

    });
    CKEDITOR.on( 'instanceReady', function( ev )
    {
    	var writer = ev.editor.dataProcessor.writer; 	
     	var dtd = CKEDITOR.dtd;	
    	for ( var e in CKEDITOR.tools.extend( {}, dtd.$block, dtd.$inline ) )
    	{
    		ev.editor.dataProcessor.writer.setRules( e, {					
    			breakBeforeOpen : true,		
    			breakAfterOpen : true,
    			breakAfterClose : false,
    			breakBeforeClose : true
    		});
    	}
    });
});
